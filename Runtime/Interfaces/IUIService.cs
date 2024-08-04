using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI
{
    public interface IUIService
    {
        public event Action<bool> OnTransitionStateChanged; 
        
        public void Open<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public void Open<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        public void Open<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public void Open<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        
        public UniTask<T> OpenAsync<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenAsync<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        public UniTask<T> OpenAsync<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenAsync<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        
        public void OpenWidget<TWidget, TRoot>(TRoot root, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel>, new() where TRoot : IUIModel<IUIViewModel>;
        public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel> where TRoot : IUIModel<IUIViewModel>;
        public void OpenWidget<TWidget, TRoot>(TRoot root, object viewKey, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel>, new() where TRoot : IUIModel<IUIViewModel>;
        public void OpenWidget<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel> where TRoot : IUIModel<IUIViewModel>;
        
        public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel>, new() where TRoot : IUIModel<IUIViewModel>;
        public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel> where TRoot : IUIModel<IUIViewModel>;
        public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TRoot root, object viewKey, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel>, new() where TRoot : IUIModel<IUIViewModel>;
        public UniTask<TWidget> OpenWidgetAsync<TWidget, TRoot>(TWidget model, TRoot root, object viewKey, IUIViewRoot parent, UIOptions? options = null) where TWidget : IUIModel<IUIViewModel> where TRoot : IUIModel<IUIViewModel>;
        
        public void Close<T>(T model) where T : IUIModel<IUIViewModel>;
        public UniTask CloseAsync<T>(T model) where T : IUIModel<IUIViewModel>;
    }
}