using System;
using UniRx;

namespace ED.UI.Samples
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