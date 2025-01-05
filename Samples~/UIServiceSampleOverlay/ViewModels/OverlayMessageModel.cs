using System;
using ED.UI.Interfaces;
using UniRx;

namespace ED.UI.Samples.ViewModels
{
    public class OverlayMessageModel : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> Close = new();
        public readonly ReactiveProperty<string> Message = new();

        public void Dispose()
        {
            Close?.Dispose();
            Message?.Dispose();
        }
    }
}