namespace ED.UI.Samples
{
    public class SampleScreenModel : IUIModel<SampleScreenModel.Model>
    {
        public object DefaultViewKey => nameof(SampleScreenView);
        public UIOptions DefaultOptions => UIOptions.Default;
        public Model model { get; } = new();
        
        public void OnBind()
        {
            
        }
        
        public class Model : IUIViewModel
        {
            
        }
    }
}