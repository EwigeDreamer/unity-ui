using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Components;
using Cysharp.Threading.Tasks;
using ED.Additional.Collections;
using ED.Extensions.Unity;
using Enums;
using UnityEngine;

namespace ED.UI
{
    public class UIService : IUIService, IDisposable
    {
        private readonly UICanvas _canvas;
        private readonly UIPool _pool;
        
        private readonly HashSet<IUIViewModel> _models = new();
        private readonly Dictionary<object, GameObject> _objects = new();
        private readonly Dictionary<object, IUIAppearable> _views = new();
        private readonly Dictionary<object, IDisposable> _disposables = new();
        private readonly Dictionary<object, UIRootKey> _roots = new();
        private readonly Dictionary<object, UIOptions> _options = new();
        private readonly Dictionary<object, bool> _states = new();
        private readonly IReadOnlyDictionary<UIRootKey, StackList<IUIViewModel>> _stacks;
        
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
        
        public UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            UIRootKey rootKey = null,
            UIOptions? options = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel, new()
            where TView : MonoBehaviour, IUIView<TViewModel>
            => OpenAsync<TViewModel, TView>(new TViewModel(), rootKey, options, cancellationToken);

        public async UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            TViewModel viewModel,
            UIRootKey rootKey = null,
            UIOptions? options = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel
            where TView : MonoBehaviour, IUIView<TViewModel>
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is already registered");


            try
            {
                IsInProgress = true;
                _canvas.SetInteractable(false);

                rootKey ??= UIRootKey.Main;
                options ??= UIOptions.Default;
                var stack = _stacks[rootKey];
                var prevViewModel = stack.Count > 0 ? stack.Peek() : null;
                var container = _canvas.Roots[rootKey];
                
                var (go, view, disposable) = await PrepareViewAsync<TViewModel, TView>(viewModel, container, cancellationToken);
                Register(viewModel, go, view, disposable, rootKey, options.Value);
                stack.Push(viewModel);
                if (cancellationToken.IsCancellationRequested) return viewModel;
                
                await TransiteAsync(prevViewModel, viewModel, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return viewModel;

                return viewModel;
            }
            finally
            {
                _canvas.ToActual()?.SetInteractable(true);
                IsInProgress = false;
            }
        }

        public async UniTask CloseAsync<TViewModel>(TViewModel viewModel, CancellationToken cancellationToken = default) where TViewModel : IUIViewModel
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (!_models.Contains(viewModel))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is not registered");
            
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
            
            await TransiteAsync(viewModel, nextViewModel, cancellationToken);
            _disposables[viewModel].Dispose();
            Unregister(viewModel);
        }

        private async UniTask TransiteAsync(IUIViewModel toHide, IUIViewModel toShow, CancellationToken cancellationToken = default)
        {
            UniTask hiding = UniTask.CompletedTask;
            UniTask showing = UniTask.CompletedTask;
            if (toHide != null && _states[toHide]) hiding = HideAsync(toHide, cancellationToken);
            if (toShow != null && !_states[toShow]) showing = ShowAsync(toShow, cancellationToken);
            await UniTask.WhenAll(hiding, showing).AttachExternalCancellation(cancellationToken);
        }

        private async UniTask ShowAsync(IUIViewModel model, CancellationToken cancellationToken = default)
        {
            var needAnimation = _options[model].HasFlag(UIOptions.ShowAnimation);
            await _views[model].ShowAsync(!needAnimation, cancellationToken);
            _states[model] = true;
        }

        private async UniTask HideAsync(IUIViewModel model, CancellationToken cancellationToken = default)
        {
            var needAnimation = _options[model].HasFlag(UIOptions.HideAnimation);
            await _views[model].HideAsync(!needAnimation, cancellationToken);
            _states[model] = false;
        }

        private async UniTask<(GameObject go, TView view, IDisposable disposable)> PrepareViewAsync<TViewModel, TView>(
            TViewModel viewModel,
            Transform container,
            CancellationToken cancellationToken = default)
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
            IUIViewModel model,
            GameObject go,
            IUIAppearable view,
            IDisposable disposable,
            UIRootKey rootKey,
            UIOptions options)
        {
            _models.Add(model);
            _objects[model] = go;
            _views[model] = view;
            _disposables[model] = disposable;
            _options[model] = options;
            _roots[model] = rootKey;
            _states[model] = false;
        }

        private void Unregister(IUIViewModel model)
        {
            _models.Remove(model);
            _objects.Remove(model);
            _views.Remove(model);
            _disposables.Remove(model);
            _options.Remove(model);
            _roots.Remove(model);
            _states.Remove(model);
        }
        
        public void Dispose()
        {
            _pool.Dispose();
        }
    }
}