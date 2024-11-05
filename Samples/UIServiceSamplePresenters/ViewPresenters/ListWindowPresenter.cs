using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace ED.UI.Samples
{
    public class ListWindowPresenter : IDisposable
    {
        private readonly IUIService _service;
        private readonly ListWindowModel _model = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly List<ListItemModel> _items = new();
        
        public ListWindowPresenter(IUIService service, OverlayMessagePresenter overlayMessagePresenter)
        {
            _service = service;
            _model.Close
                .Subscribe(_ => Close())
                .AddTo(_disposables);

            int count = 10;
            for (int i = 0; i < count; ++i)
            {
                var number = i + 1;
                var item = new ListItemModel();
                item.Label.Value = $"Item number {number}";
                item.Click
                    .Subscribe(_ => overlayMessagePresenter.Open($"This is item number {number}"))
                    .AddTo(_disposables);
                _items.Add(item);
            }
        }

        public void Open()
        {
            _service.OpenAsync<ListWindowModel, ListWindow>(_model, onInitCallback: Init).Forget();

            async void Init(ListWindowModel model)
            {
                if (model.Container.TryGetValue(out var container))
                {
                    foreach (var item in _items)
                    {
                        await _service.OpenWidgetAsync<ListItemModel, ListItem>(item, model, container);
                    }
                }
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