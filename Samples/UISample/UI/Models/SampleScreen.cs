using System;
using ED.MVVM;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace ED.UI.Samples
{
    public class SampleScreen : IUIModel
    {
        [Inject] private readonly IUIService UIService;
        
        public object DefaultViewKey => nameof(SampleScreenView);
        public UIOptions DefaultOptions => UIOptions.Default;
        public object ViewModel => _model;

        private readonly Model _model = new();
        
        public void Awake()
        {
            _model.Title.Value = "Hello World!";
            _model.OnOpenWindowClicked += () => UIService.Open<SampleWindow>();
            _model.OnOpenRedWindowClicked += () => UIService.Open<SampleWindow>(nameof(SampleWindowViewRed));
            _model.OnOpenGreenWindowClicked += () => UIService.Open<SampleWindow>(nameof(SampleWindowViewGreen));
            _model.OnOpenBlueWindowClicked += () => UIService.Open<SampleWindow>(nameof(SampleWindowViewBlue));
        }

        public void Start()
        {
            
        }

        public void Dispose()
        {
            
        }

        private class Model : IUIViewModel
        {
            
            [Data("title")]
            public ReactiveProperty<string> Title = new ();

            [Preserve]
            [Method("open_window")]
            private void OpenWindowClick() => OnOpenWindowClicked?.Invoke();
            public event Action OnOpenWindowClicked;

            [Preserve]
            [Method("open_red_window")]
            private void OpenRedWindowClick() => OnOpenRedWindowClicked?.Invoke();
            public event Action OnOpenRedWindowClicked;

            [Preserve]
            [Method("open_green_window")]
            private void OpenGreenWindowClick() => OnOpenGreenWindowClicked?.Invoke();
            public event Action OnOpenGreenWindowClicked;

            [Preserve]
            [Method("open_blue_window")]
            private void OpenBlueWindowClick() => OnOpenBlueWindowClicked?.Invoke();
            public event Action OnOpenBlueWindowClicked;
        }
    }
}