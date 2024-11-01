using UniRx;

namespace ED.UI.Samples
{
    public class Window1Model : IUIViewModel
    {
        public readonly Subject<Unit> Close = new();
        public readonly Subject<Unit> OpenNext = new();
    }
}