using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ED.UI.Samples
{
    public class MainScreen : MonoBehaviour, IUIView<MainScreenModel>
    {
        [SerializeField] private Button _OpenListWindowButton;
        
        public IDisposable Bind(MainScreenModel viewModel)
        {
            CompositeDisposable disposables = new();
            
            _OpenListWindowButton.OnClickAsObservable()
                .Subscribe(viewModel.OpenListWindow)
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