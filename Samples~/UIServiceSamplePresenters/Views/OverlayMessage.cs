using System;
using ED.UI.Samples.Base;
using ED.UI.Samples.ViewModels;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class OverlayMessage : BaseUIView<OverlayMessageModel>
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _messageText;
        
        public override IDisposable Bind(OverlayMessageModel viewModel)
        {
            CompositeDisposable disposables = new();
            
            _closeButton.OnClickAsObservable()
                .Subscribe(viewModel.Close)
                .AddTo(disposables);
            
            _messageText.text = viewModel.Message.Value;
            viewModel.Message
                .Subscribe(a => _messageText.text = a)
                .AddTo(disposables);

            return disposables;
        }
    }
}