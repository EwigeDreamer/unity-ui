using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI
{
    public interface IUIService
    {
        public void Open<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public void Open<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        public void Open<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public void Open<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        
        public UniTask<T> OpenAsync<T>(UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenAsync<T>(T model, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        public UniTask<T> OpenAsync<T>(object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenAsync<T>(T model, object viewKey, UIOptions? options = null) where T : IUIModel<IUIViewModel>;
        
        public void OpenWidget<T>(IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new();
        public void OpenWidget<T>(T model, IUIViewRoot parent) where T : IUIModel<IUIViewModel>;
        public void OpenWidget<T>(object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new();
        public void OpenWidget<T>(T model, object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>;
        
        public UniTask<T> OpenWidgetAsync<T>(IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenWidgetAsync<T>(T model, IUIViewRoot parent) where T : IUIModel<IUIViewModel>;
        public UniTask<T> OpenWidgetAsync<T>(object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>, new();
        public UniTask<T> OpenWidgetAsync<T>(T model, object viewKey, IUIViewRoot parent) where T : IUIModel<IUIViewModel>;
        
        public void Close<T>(T model) where T : IUIModel<IUIViewModel>;
        public UniTask CloseAsync<T>(T model) where T : IUIModel<IUIViewModel>;
    }
}