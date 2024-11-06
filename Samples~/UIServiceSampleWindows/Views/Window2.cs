using System;
using ED.UI.Samples.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class Window2 : BaseUIView<Window2Model>
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openNextButton;
        
        public override IDisposable Bind(Window2Model viewModel)
        {
            CompositeDisposable disposables = new();
            
            _closeButton.OnClickAsObservable()
                .Subscribe(viewModel.Close)
                .AddTo(disposables);
            
            _openNextButton.OnClickAsObservable()
                .Subscribe(viewModel.OpenNext)
                .AddTo(disposables);

            return disposables;
        }
    }
}