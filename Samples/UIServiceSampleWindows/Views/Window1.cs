using System;
using ED.UI.Samples.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class Window1 : BaseUIView<Window1Model>
    {
        [SerializeField] private Button _openNextButton;
        
        public override IDisposable Bind(Window1Model viewModel)
        {
            CompositeDisposable disposables = new();
            
            _openNextButton.OnClickAsObservable()
                .Subscribe(viewModel.OpenNext)
                .AddTo(disposables);

            return disposables;
        }
    }
}