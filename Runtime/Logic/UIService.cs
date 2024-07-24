using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ED.Additional.Collections;
using ED.MVVM;
using UnityEngine;
using UnityEngine.Pool;

namespace ED.UI
{
    public class UIService : IUIService, IDisposable
    {
        private readonly IUIModelPreprocessor _preprocessor;
        private readonly IUIViewPool _pool;
        private readonly IUIViewRoot _root;

        private readonly Dictionary<object, GameObject> _views = new();
        private readonly Dictionary<object, Action> _disposings = new();
        private readonly Dictionary<object, UIOptions> _options = new();
        private readonly Dictionary<object, bool> _states = new();
        private readonly Dictionary<object, List<IUIModel<IUIViewModel>>> _children = new();
        private readonly HashSet<object> _models = new();
        private readonly StackList<object> _stack = new();
        
        protected UIService(IUIModelPreprocessor preprocessor, IUIViewPool pool, IUIViewRoot root)
        {
            _preprocessor = preprocessor;
            _pool = pool;
            _root = root;
        }

        public void Open<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new() => Open(new T());
        public void Open<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel> => Open(model, model.DefaultViewKey);
        public void Open<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new() => Open(new T(), viewKey);
        public void Open<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel> => OpenAsync(model, viewKey).Forget();
        public UniTask<T> OpenAsync<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new() => OpenAsync(new T());
        public UniTask<T> OpenAsync<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel> => OpenAsync(model, model.DefaultViewKey);
        public UniTask<T> OpenAsync<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new() => OpenAsync(new T(), viewKey);

        public async UniTask<T> OpenAsync<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (_stack.Contains(model)) throw new InvalidOperationException($"{typeof(T).Name} is already open!");
            if (viewKey == null) throw new ArgumentNullException(nameof(viewKey));
            
            _preprocessor.Preprocess(model);

            var prev = _stack.Count > 0 ? _stack.Peek() : null;
            var (view, dispose) = await PrepareView(model, viewKey, _root.Container);
            Register(model, view, dispose, options);
            _stack.Push(model);
            await Transite(prev, model);
            
            return model;
        }

        public void Close<T>(T model) where T : IUIModel<IUIViewModel> => CloseAsync(model).Forget();

        public async UniTask CloseAsync<T>(T model) where T : IUIModel<IUIViewModel>
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!_stack.Contains(model)) throw new InvalidOperationException($"{typeof(T).Name} is not open!");

            object next = null;
            if (object.ReferenceEquals(_stack.Peek(), model))
            {
                _stack.Pop();
                _stack.TryPeek(out next);
            }
            else
            {
                _stack.Remove(model);
            }
            
            await Transite(model, next);
            await CloseChildren(model);
            _disposings[model]?.Invoke();
            Unregister(model);
        }

        private UniTask CloseChildren<T>(T model) where T : IUIModel<IUIViewModel>
        {
            if (!_children.TryGetValue(model, out var children)) return UniTask.CompletedTask;
            using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
            foreach(var child in children) tasks.Add(CloseWidgetAsync(child));
            return UniTask.WhenAll(tasks);
        }

        public void OpenWidget<T>(IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new() => OpenWidget(new T(), parent);
        public void OpenWidget<T>(T model, IUIViewRoot parent) where T : IUIModel<IUIViewModel> => OpenWidget(model, model.DefaultViewKey, parent);
        public void OpenWidget<T>(object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new() => OpenWidget(new T(), viewKey, parent);
        public void OpenWidget<T>(T model, object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel> => OpenWidgetAsync(model, viewKey, parent).Forget();
        public UniTask<T> OpenWidgetAsync<T>(IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new() => OpenWidgetAsync(new T(), parent);
        public UniTask<T> OpenWidgetAsync<T>(T model, IUIViewRoot parent) where T : IUIModel<IUIViewModel> => OpenWidgetAsync(model, model.DefaultViewKey, parent);
        public UniTask<T> OpenWidgetAsync<T>(object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new() => OpenWidgetAsync(new T(), viewKey, parent);
        public async UniTask<T> OpenWidgetAsync<T>(T model, object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (viewKey == null) throw new ArgumentNullException(nameof(viewKey));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            
            _preprocessor.Preprocess(model);
            
            //TODO
            
            return model;
        }

        public void CloseWidget<T>(T model) where T : IUIModel<IUIViewModel> => CloseWidgetAsync(model).Forget();

        public UniTask CloseWidgetAsync<T>(T model) where T : IUIModel<IUIViewModel>
        {
            
        }

        private void Register<T>(T model, GameObject view, Action dispose, UIOptions? options) where T : IUIModel<IUIViewModel>
        {
            _views[model] = view;
            _disposings[model] = dispose;
            _options[model] = options ?? model.DefaultOptions;
            _states[model] = false;
            _models.Add(model);
        }

        private void Unregister<T>(T model) where T : IUIModel<IUIViewModel>
        {
            _views.Remove(model);
            _disposings.Remove(model);
            _options.Remove(model);
            _states.Remove(model);
            _models.Remove(model);
        }

        private async UniTask<(GameObject view, Action dispose)> PrepareView<T>(T model, object viewKey, Transform parent) where T : IUIModel<IUIViewModel>
        {
            Action dispose = default;
            var (viewHandler, view) = await _pool.Get(viewKey);
            view.transform.SetParent(parent);
            view.transform.localScale = Vector3.one;
            view.transform.SetAsLastSibling();
            using var listHandler = ListPool<IUIView>.Get(out var components);
            view.GetComponents(components);
            foreach (var component in components)
            {
                var binder = BinderFactory.CreateComposite(component, model.ViewModel);
                binder.Bind();
                dispose += binder.Unbind;
            }
            dispose += viewHandler.Dispose;
            model.OnBind();
            return (view, dispose);
        }

        private async UniTask Show<T>(object model)
        {
            var needAnimation = _options[model].HasFlag(UIOptions.ShowingAnimation);
            using var componentsHandler = ListPool<IUIView>.Get(out var components);
            using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
            _views[model].GetComponents(components);
            foreach(var component in components)
                tasks.Add(component.Show(!needAnimation));
            await UniTask.WhenAll(tasks);
            _states[model] = true;
        }

        private async UniTask Hide(object model)
        {
            var needAnimation = _options[model].HasFlag(UIOptions.HidingAnimation);
            using var componentsHandler = ListPool<IUIView>.Get(out var components);
            using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
            _views[model].GetComponents(components);
            foreach(var component in components)
                tasks.Add(component.Hide(!needAnimation));
            await UniTask.WhenAll(tasks);
            _states[model] = false;
        }

        private UniTask Transite(object toHide, object toShow)
        {
            UniTask hiding = UniTask.CompletedTask;
            UniTask showing = UniTask.CompletedTask;
            if (toHide != null && _states[toHide]) hiding = Hide(toHide);
            if (toShow != null && !_states[toShow]) showing = Show(toHide);
            return UniTask.WhenAll(hiding, showing);
        }

        public void Dispose()
        {
            
        }
    }
}