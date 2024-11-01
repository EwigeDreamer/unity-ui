using UniRx;

namespace ED.UI.Samples
{
    public class Window2Model : IUIViewModel
    {
        public readonly Subject<Unit> Close = new();
        public readonly Subject<Unit> OpenNext = new();
    }
}