using Zenject;

namespace ED.UI.Samples
{
    public class UIZenjectPreprocessor : IUIModelPreprocessor
    {
        private readonly DiContainer Container;

        public UIZenjectPreprocessor(DiContainer container)
        {
            Container = container;
        }
        
        public void Preprocess<T>(T model) where T : IUIModel<IUIViewModel>
        {
            Container.Inject(model);
        }
    }
}