using System;
using ED.UI.Interfaces;
using UniRx;

namespace ED.UI.Samples
{
    public class Window2Model : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> Close = new();
        public readonly Subject<Unit> OpenNext = new();

        public void Dispose()
        {
            Close?.Dispose();
            OpenNext?.Dispose();
        }
    }
}