using UnityEngine;
using Zenject;

namespace ED.UI.Samples
{
    public class UISampleInstaller : MonoInstaller, IInitializable
    {
        [SerializeField] private IUIViewRoot _uiRoot;
        
        public override void InstallBindings()
        {
            BindUIService();
            BindControllers();
            
            Container.BindInterfacesTo(GetType()).FromInstance(this).AsSingle();
        }

        private void BindUIService()
        {
            var preprocessor = new UIZenjectPreprocessor(Container);
            var pool = new UIViewPool();
            var uiService = new UIService(preprocessor, pool, _uiRoot);
            Container.BindInterfacesTo(uiService.GetType()).FromInstance(uiService).AsSingle();
        }

        private void BindControllers()
        {
            Container.BindInterfacesAndSelfTo<UISampleController>().AsSingle();
        }

        public void Initialize()
        {

        }
    }
}