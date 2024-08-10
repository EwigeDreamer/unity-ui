using System;
using UnityEngine.Events;

namespace ED.UI
{
    public interface IUIModel : IDisposable
    {
        object DefaultViewKey { get; }
        UIOptions DefaultOptions { get; }
        object ViewModel { get; }
        void Awake();
        void Start();
    }
}