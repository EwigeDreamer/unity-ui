using ED.UI.Components;
using ED.UI.Interfaces;
using ED.UI.Logic;
using ED.UI.Samples.Misc;
using ED.UI.Samples.ViewPresenters;
using UnityEngine;
using UniRx;

namespace ED.UI.Samples
{
    public class UIServiceSamplePresenters : MonoBehaviour
    {
        [SerializeField] private UICanvas _canvas;

        private IUIService _uiService;
        
        private MainScreenPresenter _mainScreenPresenter;
        private ListWindowPresenter _listWindowPresenter;
        private OverlayMessagePresenter _overlayMessagePresenter;

        private void Awake()
        {
            _uiService = new UIService(_canvas, new UIResourcesLoader());
            _overlayMessagePresenter = new OverlayMessagePresenter(_uiService);
            _mainScreenPresenter = new MainScreenPresenter(_uiService, _overlayMessagePresenter);
            _listWindowPresenter = new ListWindowPresenter(_uiService, _overlayMessagePresenter);
            
            _mainScreenPresenter.OpenListWindowClicked
                .Subscribe(_ => _listWindowPresenter.Open())
                .AddTo(this);
        }

        private void Start()
        {
            _mainScreenPresenter.Open();
        }
    }
}
