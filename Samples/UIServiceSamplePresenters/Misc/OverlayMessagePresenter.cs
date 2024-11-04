using Cysharp.Threading.Tasks;
using UniRx;

namespace ED.UI.Samples
{
    public class OverlayMessagePresenter
    {
        private readonly IUIService _service;

        public OverlayMessagePresenter(IUIService service)
        {
            _service = service;
        }

        public void Open(string message)
        {
            CompositeDisposable disposables = new();
            OverlayMessageModel model = new();
            model.Message.Value = message;
            model.Close.Subscribe(Close).AddTo(disposables);
            model.AddTo(disposables);
            _service.OpenAsync<OverlayMessageModel, OverlayMessage>(model).Forget();
            
            async void Close(Unit _)
            {
                await _service.CloseAsync(model);
                disposables.Dispose();
            }
        }
    }
}