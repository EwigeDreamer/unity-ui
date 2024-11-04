using System;
using Cysharp.Threading.Tasks;
using Enums;
using UniRx;

namespace ED.UI.Samples
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly IUIService _service;
        private readonly MainScreenModel _model;
        private readonly CompositeDisposable _disposables = new();
        
        private readonly OverlayMessagePresenter _overlayMessagePresenter;
        
        private Subject<Unit> _openListWindowClicked = new();
        public IObservable<Unit> OpenListWindowClicked => _openListWindowClicked;

        public MainScreenPresenter(IUIService service, OverlayMessagePresenter overlayMessagePresenter)
        {
            _service = service;
            _model = new MainScreenModel();
            _model.OpenListWindow
                .Subscribe(_openListWindowClicked)
                .AddTo(_disposables);
            _overlayMessagePresenter = overlayMessagePresenter;
        }

        public void Open()
        {
            var root = UIRootKey.Main;
            var options = UIOptions.None;
            _service.OpenAsync<MainScreenModel, MainScreen>(_model, root, options).Forget();
        }
        
        public void Dispose()
        {
            if (_service.Contains(_model))
                _service.CloseAsync(_model).Forget();
            _model.Dispose();
            _openListWindowClicked.Dispose();
            _disposables.Dispose();
        }
    }
}