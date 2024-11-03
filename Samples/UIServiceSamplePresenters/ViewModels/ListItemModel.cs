using System;
using UniRx;

namespace ED.UI.Samples
{
    public class ListItemModel : IUIViewModel, IDisposable
    {
        public readonly ReactiveProperty<string> Label = new();
        public readonly Subject<Unit> Click = new();
        
        public void Dispose()
        {
            Label?.Dispose();
            Click?.Dispose();
        }
    }
}