using System;
using ED.UI.Interfaces;
using UniRx;

namespace ED.UI.Samples.ViewModels
{
    public class MainScreenModel : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> ShowMessage = new();
        public readonly ReactiveProperty<int> MessageCounter = new();

        public void Dispose()
        {
            ShowMessage?.Dispose();
            MessageCounter?.Dispose();
        }
    }
}