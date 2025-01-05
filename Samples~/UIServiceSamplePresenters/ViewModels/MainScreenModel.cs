using System;
using ED.UI.Interfaces;
using UniRx;

namespace ED.UI.Samples.ViewModels
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