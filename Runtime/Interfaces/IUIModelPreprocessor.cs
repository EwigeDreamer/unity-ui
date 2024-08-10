namespace ED.UI
{
    /// <summary>
    /// This interface provides the way for your implementation of Dependency Injection (DI)
    /// </summary>
    public interface IUIModelPreprocessor
    {
        void Preprocess<T>(T model) where T : IUIModel;
    }
}