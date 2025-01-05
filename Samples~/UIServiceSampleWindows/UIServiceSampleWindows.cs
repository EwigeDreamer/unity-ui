using Cysharp.Threading.Tasks;
using ED.UI.Components;
using ED.UI.Interfaces;
using ED.UI.Logic;
using UniRx;
using UnityEngine;

namespace ED.UI.Samples
{
    public class UIServiceSampleWindows : MonoBehaviour
    {
        [SerializeField] private UICanvas _canvas;

        private IUIService _uiService;

        private readonly Window1Model _window1Model = new();
        private readonly Window2Model _window2Model = new();
        private readonly Window3Model _window3Model = new();
        
        private void Awake()
        {
            _uiService = new UIService(_canvas, new UIResourcesLoader());
            
            _window1Model.AddTo(this);
            _window2Model.AddTo(this);
            _window3Model.AddTo(this);
        }

        private void Start()
        {
            InitWindow1();
            InitWindow2();
            InitWindow3();
            
            OpenWindow1();
        }

        private void InitWindow1()
        {
            _window1Model.OpenNext.Subscribe(_ => OpenWindow2());
        }
        private void InitWindow2()
        {
            _window2Model.Close.Subscribe(_ => _uiService.CloseAsync(_window2Model).Forget());
            _window2Model.OpenNext.Subscribe(_ => OpenWindow3());
        }
        private void InitWindow3()
        {
            _window3Model.Close.Subscribe(_ => _uiService.CloseAsync(_window3Model).Forget());
        }
        
        private void OpenWindow1() => _uiService.OpenAsync<Window1Model, Window1>(_window1Model).Forget();
        private void OpenWindow2() => _uiService.OpenAsync<Window2Model, Window2>(_window2Model).Forget();
        private void OpenWindow3() => _uiService.OpenAsync<Window3Model, Window3>(_window3Model).Forget();
    }
}
