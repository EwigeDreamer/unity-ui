namespace ED.UI.Interfaces
{
    public interface IUIView<TViewModel> : IUIAppearable, IUIBindable<TViewModel> where TViewModel : IUIViewModel { }
}