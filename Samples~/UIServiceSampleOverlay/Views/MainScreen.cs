using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ED.UI.Interfaces;
using ED.UI.Samples.ViewModels;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples.Views
{
    public class MainScreen : MonoBehaviour, IUIView<MainScreenModel>
    {
        [SerializeField] private Button _showMessageButton;
        [SerializeField] private Text _messageCounterText;
        
        public IDisposable Bind(MainScreenModel viewModel)
        {
            CompositeDisposable disposables = new();
            
            _showMessageButton.OnClickAsObservable()
                .Subscribe(viewModel.ShowMessage)
                .AddTo(disposables);

            _messageCounterText.text = $"Counter: {viewModel.MessageCounter.Value}";
            viewModel.MessageCounter
                .Subscribe(a => _messageCounterText.text = $"Counter: {a}")
                .AddTo(disposables);

            return disposables;
        }

        public UniTask ShowAsync(bool forced, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(bool forced, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }
    }
}