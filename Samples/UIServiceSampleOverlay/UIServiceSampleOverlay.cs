using Components;
using Cysharp.Threading.Tasks;
using Enums;
using UniRx;
using UnityEngine;

namespace ED.UI.Samples
{
    public class UIServiceSampleOverlay : MonoBehaviour
    {
        [SerializeField] private UICanvas _canvas;

        private IUIService _uiService;

        private readonly MainScreenModel _mainScreenModel = new();
        
        private void Awake()
        {
            _uiService = new UIService(_canvas, new UIResourcesLoader());
            
            _mainScreenModel.AddTo(this);
        }

        private void Start()
        {
            InitMainScreen();
            
            OpenMainScreen();
        }

        private void InitMainScreen()
        {
            _mainScreenModel.ShowMessage.Subscribe(Show);

            void Show(Unit _)
            {
                _mainScreenModel.MessageCounter.Value++;
                ShowMessage($"Message {_mainScreenModel.MessageCounter.Value}");
            }
        }

        private void OpenMainScreen()
        {
            var root = UIRootKey.Main;
            var options = UIOptions.None;
            _uiService.OpenAsync<MainScreenModel, MainScreen>(_mainScreenModel, root, options).Forget();
        }

        private void ShowMessage(string message)
        {
            var model = new OverlayMessageModel();
            var root = UIRootKey.Overlay;
            var options = UIOptions.Default & ~UIOptions.HidePrevious;
            model.Message.Value = message;
            model.Close.Subscribe(Close);
            Open();

            async void Open()
            {
                await _uiService.OpenAsync<OverlayMessageModel, OverlayMessage>(model, root, options);
            }

            async void Close(Unit _)
            {
                await _uiService.CloseAsync(model);
                model.Dispose();
            }
        }
    }
}
