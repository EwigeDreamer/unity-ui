namespace ED.UI
{
    public interface IUIView<TViewModel> : IUIAppearable, IUIBindable<TViewModel> where TViewModel : IUIViewModel { }
}