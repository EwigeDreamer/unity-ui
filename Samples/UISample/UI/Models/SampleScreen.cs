using ED.MVVM;
using UniRx;

namespace ED.UI.Samples
{
    public class SampleScreen : IUIModel
    {
        public object DefaultViewKey => nameof(SampleScreenView);
        public UIOptions DefaultOptions => UIOptions.Default;
        public object ViewModel => _model;

        private readonly Model _model = new();
        
        public void Awake()
        {
            _model.Title.Value = "Hello World!";
        }

        public void Start()
        {
            
        }

        public void Dispose()
        {
            
        }

        public class Model : IUIViewModel
        {
            [Data("title")]
            public ReactiveProperty<string> Title = new ();
        }
    }
}