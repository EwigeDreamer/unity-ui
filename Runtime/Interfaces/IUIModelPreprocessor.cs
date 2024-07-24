namespace ED.UI
{
    public interface IUIModelPreprocessor
    {
        void Preprocess<T>(T model) where T : IUIModel<IUIViewModel>;
    }
}