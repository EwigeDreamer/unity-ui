using System;
using UniRx;

namespace ED.UI.Samples
{
    public class Window3Model : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> Close = new();

        public void Dispose()
        {
            Close?.Dispose();
        }
    }
}