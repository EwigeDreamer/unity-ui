using Cysharp.Threading.Tasks;
using ED.UI.Enums;
using ED.UI.Interfaces;
using ED.UI.Samples.ViewModels;
using UniRx;

namespace ED.UI.Samples.Misc
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
            _service.OpenAsync<OverlayMessageModel, OverlayMessage>(model, UIRootKey.Overlay).Forget();
            
            async void Close(Unit _)
            {
                await _service.CloseAsync(model);
                disposables.Dispose();
            }
        }
    }
}