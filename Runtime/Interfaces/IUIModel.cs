using System;
using UnityEngine.Events;

namespace ED.UI
{
    public interface IUIModel<out TViewModel> where TViewModel : IUIViewModel
    {
        object DefaultViewKey { get; }
        UIOptions DefaultOptions { get; }
        TViewModel model { get; }
        void OnBind();
    }
}