using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class ListItem : MonoBehaviour, IUIView<ListItemModel>
    {
        [SerializeField] private Text _labelText;
        [SerializeField] private Button _button;

        public IDisposable Bind(ListItemModel viewModel)
        {
            CompositeDisposable disposables = new();

            _labelText.text = viewModel.Label.Value;
            viewModel.Label
                .Subscribe(a => _labelText.text = a)
                .AddTo(disposables);
            
            _button.OnClickAsObservable()
                .Subscribe(viewModel.Click)
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