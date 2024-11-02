using System;
using UniRx;

namespace ED.UI.Samples
{
    public class Window1Model : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> OpenNext = new();

        public void Dispose()
        {
            OpenNext?.Dispose();
        }
    }
}