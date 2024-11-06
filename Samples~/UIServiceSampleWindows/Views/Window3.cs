using System;
using ED.UI.Samples.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class Window3 : BaseUIView<Window3Model>
    {
        [SerializeField] private Button _closeButton;
        
        public override IDisposable Bind(Window3Model viewModel)
        {
            CompositeDisposable disposables = new();
            
            _closeButton.OnClickAsObservable()
                .Subscribe(viewModel.Close)
                .AddTo(disposables);

            return disposables;
        }
    }
}