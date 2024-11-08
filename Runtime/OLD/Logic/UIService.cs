// using System;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using ED.Additional.Collections;
// using ED.MVVM;
// using UnityEngine;
// using UnityEngine.Pool;
//
// namespace ED.UI.OLD
// {
//     public class UIService : IUIService, IDisposable
//     {
//         private readonly IUIModelPreprocessor _preprocessor;
//         private readonly IUIViewPool _pool;
//         private readonly Transform _root;
//
//         private readonly Dictionary<object, object> _keys = new();
//         private readonly Dictionary<object, GameObject> _views = new();
//         private readonly Dictionary<object, IDisposable> _disposables = new();
//         private readonly Dictionary<object, UIOptions> _options = new();
//         private readonly Dictionary<object, bool> _states = new();
//         private readonly Dictionary<object, List<IUIModel>> _childrens = new();
//         private readonly Dictionary<object, IUIModel> _roots = new();
//         private readonly HashSet<IUIModel> _models = new();
//         private readonly StackList<IUIModel> _stack = new();
//         
//         public event Action<bool> OnTransitionStateChanged;
//         
//         public UIService(IUIModelPreprocessor preprocessor, IUIViewPool pool, Transform root)
//         {
//             _preprocessor = preprocessor;
//             _pool = pool;
//             _root = root;
//         }
//
//         public void Open<T>(UIOptions? options = null) where T : IUIModel, new() => Open(new T(), options);
//         public void Open<T>(T model, UIOptions? options = null) where T : IUIModel => Open(model, model.DefaultViewKey, options);
//         public void Open<T>(object viewKey, UIOptions? options = null) where T : IUIModel, new() => Open(new T(), viewKey, options);
//         public void Open<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel => OpenAsync(model, viewKey, options).Forget();
//         public UniTask<T> OpenAsync<T>(UIOptions? options = null) where T : IUIModel, new() => OpenAsync(new T(), options);
//         public UniTask<T> OpenAsync<T>(T model, UIOptions? options = null) where T : IUIModel => OpenAsync(model, model.DefaultViewKey, options);
//         public UniTask<T> OpenAsync<T>(object viewKey, UIOptions? options = null) where T : IUIModel, new() => OpenAsync(new T(), viewKey, options);
//
//         public async UniTask<T> OpenAsync<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel
//         {
//             if (model == null) throw new ArgumentNullException(nameof(model));
//             if (_models.Contains(model)) throw new InvalidOperationException($"{typeof(T).Name} is already exist!");
//             
//             _preprocessor?.Preprocess(model);
//
//             var prev = _stack.Count > 0 ? _stack.Peek() : null;
//             var (view, disposable) = await PrepareView(model, viewKey, _root);
//             Register(model, view, viewKey, disposable, options);
//             _stack.Push(model);
//             model.Awake();
//             await Transite(prev, model);
//             model.Start();
//             
//             return model;
//         }
//
//         public void Close<T>(T model) where T : IUIModel => CloseAsync(model).Forget();
//
//         public async UniTask CloseAsync<T>(T model) where T : IUIModel
//         {
//             if (model == null) throw new ArgumentNullException(nameof(model));
//             if (!_models.Contains(model)) throw new InvalidOperationException($"{typeof(T).Name} is not found!");
//
//             IUIModel next = null;
//             if (object.ReferenceEquals(_stack.Peek(), model))
//             {
//                 _stack.Pop();
//                 _stack.TryPeek(out next);
//             }
//             else
//             {
//                 _stack.Remove(model);
//             }
//             
//             await Transite(model, next);
//             await CloseChildren(model);
//             _disposables[model].Dispose();
//             Unregister(model);
//             model.Dispose();
//         }
//
//         private UniTask CloseChildren<T>(T model) where T : IUIModel
//         {
//             if (!_childrens.TryGetValue(model, out var children)) return UniTask.CompletedTask;
//             using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
//             foreach(var child in children) tasks.Add(CloseWidgetAsync(child));
//             return UniTask.WhenAll(tasks);
//         }
//
//         public void OpenWidget<TWidget, TRoot>(TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel => OpenWidget(new TWidget(), root, parent, options);
//         public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel => OpenWidget(model, root, model.DefaultViewKey, parent, options);
//         public void OpenWidget<TWidget, TRoot>(TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel => OpenWidget(new TWidget(), root, viewKey, parent, options);
//         public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel => OpenWidgetAsync(model, root, viewKey, parent, options).Forget();
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel => OpenWidgetAsync(new TWidget(), root, parent, options);
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel => OpenWidgetAsync(model, root, model.DefaultViewKey, parent, options);
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel => OpenWidgetAsync(new TWidget(), root, viewKey, parent, options);
//         public async UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, Transform parent, UIOptions? options = null)
//             where TWidget : IUIModel
//             where TRoot : IUIModel
//         {
//             if (model == null) throw new ArgumentNullException(nameof(model));
//             if (_models.Contains(model)) throw new InvalidOperationException($"Widget {typeof(TWidget).Name} is already exist!");
//             if (root == null) throw new ArgumentNullException(nameof(root));
//             if (parent == null) throw new ArgumentNullException(nameof(parent));
//             
//             _preprocessor.Preprocess(model);
//             
//             var (view, disposable) = await PrepareView(model, viewKey, parent);
//             RegisterWidget(model, root, view, viewKey, disposable, options);
//             model.Awake();
//             await Show(model);
//             model.Start();
//             return model;
//         }
//
//         public void CloseWidget<T>(T model) where T : IUIModel => CloseWidgetAsync(model).Forget();
//
//         public async UniTask CloseWidgetAsync<T>(T model) where T : IUIModel
//         {
//             if (model == null) throw new ArgumentNullException(nameof(model));
//             if (!_models.Contains(model)) throw new InvalidOperationException($"Widget {typeof(T).Name} is not found!");
//             
//             await Hide(model);
//             _disposables[model].Dispose();
//             UnregisterWidget(model);
//             model.Dispose();
//         }
//
//         private void Register<T>(T model, GameObject view, object viewKey, IDisposable disposable, UIOptions? options) where T : IUIModel
//         {
//             _keys[model] = viewKey ?? model.DefaultViewKey;
//             _views[model] = view;
//             _disposables[model] = disposable;
//             _options[model] = options ?? model.DefaultOptions;
//             _states[model] = false;
//             _models.Add(model);
//         }
//
//         private void Unregister<T>(T model) where T : IUIModel
//         {
//             _keys.Remove(model);
//             _views.Remove(model);
//             _disposables.Remove(model);
//             _options.Remove(model);
//             _states.Remove(model);
//             _models.Remove(model);
//         }
//
//         private void RegisterWidget<TWidget, TRoot>(TWidget model, TRoot root, GameObject view, object viewKey, IDisposable disposable, UIOptions? options)
//             where TWidget : IUIModel
//             where TRoot : IUIModel
//         {
//             Register(model, view, viewKey, disposable, options);
//             if (!_childrens.TryGetValue(root, out var children))
//                 _childrens[root] = children = new List<IUIModel>();
//             _roots[model] = root;
//             children.Add(model);
//         }
//
//         private void UnregisterWidget<T>(T model) where T : IUIModel
//         {
//             Unregister(model);
//             var root = _roots[model];
//             if (_childrens.TryGetValue(root, out var children))
//             {
//                 children.Remove(model);
//                 if (children.Count == 0) _childrens.Remove(root);
//             }
//             _roots.Remove(model);
//         }
//
//         private async UniTask<(GameObject view, IDisposable disposables)> PrepareView<T>(T model, object viewKey, Transform parent) where T : IUIModel
//         {
//             viewKey ??= model.DefaultViewKey;
//             var (viewHandler, view) = await _pool.GetAsync(viewKey, parent);
//             view.transform.SetAsLastSibling();
//             using var componentsHandler = ListPool<IUIView>.Get(out var components);
//             view.GetComponents(components);
//             DisposableCollection disposables = new();
//             foreach (var component in components)
//             {
//                 var binder = BinderFactory.CreateComposite(component, model.ViewModel);
//                 disposables.Add(binder.ToDisposable());
//                 binder.Bind();
//             }
//             disposables.Add(viewHandler);
//             return (view, disposables);
//         }
//
//         private async UniTask Show<T>(T model) where T : IUIModel
//         {
//             var needAnimation = _options[model].HasFlag(UIOptions.ShowAnimation);
//             using var componentsHandler = ListPool<IUIView>.Get(out var components);
//             using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
//             _views[model].GetComponents(components);
//             foreach(var component in components)
//                 tasks.Add(component.Show(!needAnimation));
//             await UniTask.WhenAll(tasks);
//             _states[model] = true;
//         }
//
//         private async UniTask Hide<T>(T model) where T : IUIModel
//         {
//             var needAnimation = _options[model].HasFlag(UIOptions.HideAnimation);
//             using var componentsHandler = ListPool<IUIView>.Get(out var components);
//             using var tasksHandler = ListPool<UniTask>.Get(out var tasks);
//             _views[model].GetComponents(components);
//             foreach(var component in components)
//                 tasks.Add(component.Hide(!needAnimation));
//             await UniTask.WhenAll(tasks);
//             _states[model] = false;
//         }
//
//         private async UniTask Transite<T1, T2>(T1 toHide, T2 toShow) where T1 : IUIModel where T2 : IUIModel
//         {
//             OnTransitionStateChanged?.Invoke(true);
//             UniTask hiding = UniTask.CompletedTask;
//             UniTask showing = UniTask.CompletedTask;
//             if (toHide != null && _states[toHide]) hiding = Hide(toHide);
//             if (toShow != null && !_states[toShow]) showing = Show(toShow);
//             await UniTask.WhenAll(hiding, showing);
//             OnTransitionStateChanged?.Invoke(false);
//         }
//
//         public void Dispose()
//         {
//             foreach(var disposable in _disposables.Values) disposable.Dispose();
//             foreach(var childrenList in _childrens.Values) childrenList.Clear();
//             
//             _keys.Clear();
//             _views.Clear();
//             _disposables.Clear();
//             _options.Clear();
//             _states.Clear();
//             _models.Clear();
//             _childrens.Clear();
//             _roots.Clear();
//             
//             _pool.Dispose();
//         }
//     }
// }