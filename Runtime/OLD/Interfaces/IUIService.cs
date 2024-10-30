// using System;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// namespace ED.UI.OLD
// {
//     public interface IUIService
//     {
//         public event Action<bool> OnTransitionStateChanged; 
//         
//         public void Open<T>(UIOptions? options = null) where T : IUIModel, new();
//         public void Open<T>(T model, UIOptions? options = null) where T : IUIModel;
//         public void Open<T>(object viewKey, UIOptions? options = null) where T : IUIModel, new();
//         public void Open<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel;
//         
//         public UniTask<T> OpenAsync<T>(UIOptions? options = null) where T : IUIModel, new();
//         public UniTask<T> OpenAsync<T>(T model, UIOptions? options = null) where T : IUIModel;
//         public UniTask<T> OpenAsync<T>(object viewKey, UIOptions? options = null) where T : IUIModel, new();
//         public UniTask<T> OpenAsync<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel;
//         
//         public void OpenWidget<TWidget, TRoot>(TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel;
//         public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel;
//         public void OpenWidget<TWidget, TRoot>(TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel;
//         public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel;
//         
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel;
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel;
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel, new() where TRoot : IUIModel;
//         public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, Transform parent, UIOptions? options = null) where TWidget : IUIModel where TRoot : IUIModel;
//         
//         public void Close<T>(T model) where T : IUIModel;
//         public UniTask CloseAsync<T>(T model) where T : IUIModel;
//         void CloseWidget<T>(T model) where T : IUIModel;
//         UniTask CloseWidgetAsync<T>(T model) where T : IUIModel;
//     }
// }