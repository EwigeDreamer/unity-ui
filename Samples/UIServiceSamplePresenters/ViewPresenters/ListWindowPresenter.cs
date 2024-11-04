using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace ED.UI.Samples
{
    public class ListWindowPresenter : IDisposable
    {
        private readonly IUIService _service;
        private readonly ListWindowModel _model;
        private readonly CompositeDisposable _disposables = new();
        
        private readonly OverlayMessagePresenter _overlayMessagePresenter;
        
        public ListWindowPresenter(IUIService service, OverlayMessagePresenter overlayMessagePresenter)
        {
            _service = service;
            _model = new ListWindowModel();
            _model.Close
                .Subscribe(_ => Close())
                .AddTo(_disposables);
            _overlayMessagePresenter = overlayMessagePresenter;
        }

        public void Open()
        {
            _service.OpenAsync<ListWindowModel, ListWindow>(_model, onInitCallback: Init).Forget();

            void Init(ListWindowModel model)
            {
                //TODO
            }
        }

        public void Close()
        {
            _service.CloseAsync(_model).Forget();
        }
        
        public void Dispose()
        {
            if (_service.Contains(_model))
                _service.CloseAsync(_model).Forget();
            _model.Dispose();
            _disposables.Dispose();
        }
    }
}