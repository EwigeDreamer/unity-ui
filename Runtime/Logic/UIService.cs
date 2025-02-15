using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ED.Additional.Collections;
using ED.Extensions.System;
using ED.UI.Components;
using ED.UI.Enums;
using ED.UI.Interfaces;
using ED.UI.Misc;
using UnityEngine;
using UnityEngine.Pool;

namespace ED.UI.Logic
{
    public class UIService : IUIService, IDisposable
    {
        private readonly UICanvas _canvas;
        private readonly UIPool _pool;
        
        private readonly HashSet<IUIViewModel> _models = new();
        private readonly Dictionary<IUIViewModel, GameObject> _objects = new();
        private readonly Dictionary<IUIViewModel, IUIAppearable> _views = new();
        private readonly Dictionary<IUIViewModel, IDisposable> _disposables = new();
        private readonly Dictionary<IUIViewModel, UIRootKey> _roots = new();
        private readonly Dictionary<IUIViewModel, UIOptions> _options = new();
        private readonly Dictionary<IUIViewModel, bool> _states = new();
        private readonly Dictionary<IUIViewModel, List<IUIViewModel>> _children = new();
        private readonly Dictionary<IUIViewModel, IUIViewModel> _parents = new();

        private readonly Dictionary<UIRootKey, StackList<IUIViewModel>> _stacks;
        
        public bool IsInProgress { get; private set; }

        public UIService(
            UICanvas canvas,
            IUIViewLoader loader)
        {
            if (canvas == null)
                throw new ArgumentNullException(nameof(canvas));
            
            _canvas = canvas;
            _pool = new UIPool(loader, canvas.transform);
            _stacks = UIRootKey.Values.ToDictionary(a => a, _ => new StackList<IUIViewModel>());
        }

        public bool Contains<TViewModel>(TViewModel viewModel) where TViewModel : IUIViewModel
        {
            return _models.Contains(viewModel);
        }

        public UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            UIRootKey rootKey = null,
            UIOptions? options = null,
            Action<TViewModel> onInitCallback = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel, new()
            where TView : MonoBehaviour, IUIView<TViewModel>
            => OpenAsync<TViewModel, TView>(new TViewModel(), rootKey, options, onInitCallback, cancellationToken);

        public async UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            TViewModel viewModel,
            UIRootKey rootKey = null,
            UIOptions? options = null,
            Action<TViewModel> onInitCallback = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel
            where TView : MonoBehaviour, IUIView<TViewModel>
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is already registered");

            using var tokenHandler = CancellationTokenUtility.Combine(false, out var combinedCancellationToken,
                cancellationToken, Application.exitCancellationToken);

            await UniTask.WaitWhile(() => IsInProgress, cancellationToken: combinedCancellationToken);
            if (combinedCancellationToken.IsCancellationRequested) return viewModel;

            try
            {
                SetInProgress(true);

                rootKey ??= UIRootKey.Main;
                options ??= UIOptions.Default;
                var stack = _stacks[rootKey];
                var prevViewModel = stack.Count > 0 ? stack.Peek() : null;
                var container = _canvas.Roots[rootKey];
                
                var (go, view, disposable) = await PrepareViewAsync<TViewModel, TView>(viewModel, container, combinedCancellationToken);
                Register(viewModel, go, view, disposable, rootKey, options.Value);
                stack.Push(viewModel);
                if (combinedCancellationToken.IsCancellationRequested) return viewModel;
                
                onInitCallback?.Invoke(viewModel);
                await TransiteAsync(prevViewModel, viewModel, combinedCancellationToken);
                if (combinedCancellationToken.IsCancellationRequested) return viewModel;

                return viewModel;
            }
            finally
            {
                SetInProgress(false);
            }
        }

        public UniTask<TViewModel> OpenWidgetAsync<TViewModel, TView>(
            IUIViewModel parent,
            Transform container,
            UIOptions? options = null,
            Action<TViewModel> onInitCallback = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel, new()
            where TView : MonoBehaviour, IUIView<TViewModel>
            => OpenWidgetAsync<TViewModel, TView>(new TViewModel(), parent, container, options, onInitCallback, cancellationToken);

        public async UniTask<TViewModel> OpenWidgetAsync<TViewModel, TView>(
            TViewModel viewModel,
            IUIViewModel parent,
            Transform container,
            UIOptions? options = null,
            Action<TViewModel> onInitCallback = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel
            where TView : MonoBehaviour, IUIView<TViewModel>
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is already registered");
            if (!_models.Contains(parent))
                throw new InvalidOperationException($"ViewModel {parent.GetType().Name} is not registered");

            options ??= UIOptions.None;

            using var tokenHandler = CancellationTokenUtility.Combine(false, out var combinedCancellationToken,
                cancellationToken, Application.exitCancellationToken);
            
            var (go, view, disposable) = await PrepareViewAsync<TViewModel, TView>(viewModel, container, combinedCancellationToken);
            RegisterWidget(viewModel, parent, go, view, disposable, options.Value);
            if (combinedCancellationToken.IsCancellationRequested) return viewModel;
            
            onInitCallback?.Invoke(viewModel);
            await ShowAsync(viewModel, combinedCancellationToken);
            if (combinedCancellationToken.IsCancellationRequested) return viewModel;

            return viewModel;
        }

        public async UniTask CloseAsync<TViewModel>(TViewModel viewModel, CancellationToken cancellationToken = default) where TViewModel : IUIViewModel
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (!_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is not registered");

            using var tokenHandler = CancellationTokenUtility.Combine(false, out var combinedCancellationToken,
                cancellationToken, Application.exitCancellationToken);

            await UniTask.WaitWhile(() => IsInProgress, cancellationToken: combinedCancellationToken);
            if (combinedCancellationToken.IsCancellationRequested) return;
            
            try
            {
                SetInProgress(true);

                var rootKey = _roots[viewModel];
                var stack = _stacks[rootKey];
                IUIViewModel nextViewModel = null;
                if (ReferenceEquals(stack.Peek(), viewModel))
                {
                    stack.Pop();
                    stack.TryPeek(out nextViewModel);
                }
                else
                {
                    stack.Remove(viewModel);
                }

                await TransiteAsync(viewModel, nextViewModel, combinedCancellationToken);
                if (combinedCancellationToken.IsCancellationRequested) return;
                await CloseChildrenAsync(viewModel, combinedCancellationToken);
                _disposables[viewModel].Dispose();
                Unregister(viewModel);
            }
            finally
            {
                SetInProgress(false);
            }
        }

        public async UniTask CloseWidgetAsync<TViewModel>(TViewModel viewModel, CancellationToken cancellationToken = default) where TViewModel : IUIViewModel
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (!_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is not registered");

            using var tokenHandler = CancellationTokenUtility.Combine(false, out var combinedCancellationToken,
                cancellationToken, Application.exitCancellationToken);
            
            await HideAsync(viewModel, combinedCancellationToken);
            _disposables[viewModel].Dispose();
            UnregisterWidget(viewModel);
        }

         private UniTask CloseChildrenAsync(IUIViewModel viewModel, CancellationToken cancellationToken)
         {
             if (!_children.TryGetValue(viewModel, out var children)) return UniTask.CompletedTask;
             using var handler1 = ListPool<UniTask>.Get(out var tasks);
             int i = children.Count;
             while (i --> 0)
             {
                 var child = children[i];
                 _options[child] &= ~UIOptions.Animation;
                 tasks.Add(CloseWidgetAsync(child, cancellationToken));
             }
             return UniTask.WhenAll(tasks).AttachExternalCancellation(cancellationToken);
         }

        private void SetInProgress(bool value)
        {
            _canvas.SetInteractable(!value);
            IsInProgress = value;
        }

        private async UniTask TransiteAsync(IUIViewModel toHide, IUIViewModel toShow, CancellationToken cancellationToken)
        {
            UniTask hiding = UniTask.CompletedTask;
            UniTask showing = UniTask.CompletedTask;
            if (toHide != null && _states[toHide]) hiding = HideAsync(toHide, cancellationToken);
            if (toShow != null && !_states[toShow]) showing = ShowAsync(toShow, cancellationToken);
            await UniTask.WhenAll(hiding, showing).AttachExternalCancellation(cancellationToken);
        }

        private async UniTask ShowAsync(IUIViewModel viewModel, CancellationToken cancellationToken)
        {
            var needAnimation = _options[viewModel].HasFlag(UIOptions.ShowAnimation);
            await _views[viewModel].ShowAsync(!needAnimation, cancellationToken);
            _states[viewModel] = true;
        }

        private async UniTask HideAsync(IUIViewModel viewModel, CancellationToken cancellationToken)
        {
            var needAnimation = _options[viewModel].HasFlag(UIOptions.HideAnimation);
            await _views[viewModel].HideAsync(!needAnimation, cancellationToken);
            _states[viewModel] = false;
        }

        private async UniTask<(GameObject go, TView view, IDisposable disposable)> PrepareViewAsync<TViewModel, TView>(
            TViewModel viewModel,
            Transform container,
            CancellationToken cancellationToken)
            where TViewModel : class, IUIViewModel
            where TView : MonoBehaviour, IUIView<TViewModel>
        {
            DisposableCollection disposables = new();
            var (viewHandler, go) = await _pool.GetAsync(typeof(TView), container, cancellationToken);
            var view = go.GetComponent<TView>();
            disposables.Add(view.Bind(viewModel));
            disposables.Add(viewHandler);
            return (go, view, disposables);
        }

        private void Register(
            IUIViewModel viewModel,
            GameObject go,
            IUIAppearable view,
            IDisposable disposable,
            UIRootKey rootKey,
            UIOptions options)
        {
            _models.Add(viewModel);
            _objects[viewModel] = go;
            _views[viewModel] = view;
            _disposables[viewModel] = disposable;
            _options[viewModel] = options;
            _roots[viewModel] = rootKey;
            _states[viewModel] = false;
        }

        private void Unregister(IUIViewModel viewModel)
        {
            _models.Remove(viewModel);
            _objects.Remove(viewModel);
            _views.Remove(viewModel);
            _disposables.Remove(viewModel);
            _options.Remove(viewModel);
            _roots.Remove(viewModel);
            _states.Remove(viewModel);
        }

        private void RegisterWidget(
            IUIViewModel viewModel,
            IUIViewModel parentModel,
            GameObject go,
            IUIAppearable view,
            IDisposable disposable,
            UIOptions options)
        {
            Register(viewModel, go, view, disposable, null, options);
             if (!_children.TryGetValue(parentModel, out var children))
                 _children[parentModel] = children = new List<IUIViewModel>();
             _parents[viewModel] = parentModel;
             children.Add(viewModel);
        }

        private void UnregisterWidget(IUIViewModel viewModel)
        {
            Unregister(viewModel);
            var parent = _parents[viewModel];
            if (_children.TryGetValue(parent, out var children))
            {
                children.Remove(viewModel);
                if (children.Count == 0)
                    _children.Remove(parent);
            }
            _parents.Remove(viewModel);
        }
        
        public void Dispose()
        {
            _pool.Dispose();
            _models.Clear();
            _objects.Clear();
            _views.Clear();
            _disposables.Clear();
            _options.Clear();
            _roots.Clear();
            _states.Clear();
            _stacks.Clear();
            _children.Clear();
            _parents.Clear();
        }
    }
}