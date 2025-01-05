using System;
using ED.UI.Samples.Base;
using ED.UI.Samples.ViewModels;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class ListWindow : BaseUIView<ListWindowModel>
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _container;
        
        public override IDisposable Bind(ListWindowModel viewModel)
        {
            CompositeDisposable disposables = new();
            
            _closeButton.OnClickAsObservable()
                .Subscribe(viewModel.Close)
                .AddTo(disposables);
            viewModel.Container
                .Setup(_container)
                .AddTo(disposables);

            return disposables;
        }
    }
}