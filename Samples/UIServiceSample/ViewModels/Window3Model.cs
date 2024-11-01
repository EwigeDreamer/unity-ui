using UniRx;

namespace ED.UI.Samples
{
    public class Window3Model : IUIViewModel
    {
        public readonly Subject<Unit> Close = new();
    }
}