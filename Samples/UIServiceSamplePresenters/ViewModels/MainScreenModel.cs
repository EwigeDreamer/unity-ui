using System;
using UniRx;

namespace ED.UI.Samples
{
    public class MainScreenModel : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> OpenListWindow = new();

        public void Dispose()
        {
            OpenListWindow?.Dispose();
        }
    }
}