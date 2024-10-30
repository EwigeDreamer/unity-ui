using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI.Samples
{
    public class UIServiceSample : MonoBehaviour
    {
        [SerializeField] private UICanvas _canvas;

        private IUIService _uiService;
        
        private void Awake()
        {
            _uiService = new UIService(_canvas, new UIResourcesLoader());
        }

        private void Start()
        {
            _uiService.OpenAsync<Window1Model, Window1>().Forget();
        }
    }
}
