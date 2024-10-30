using System.Threading;
using Cysharp.Threading.Tasks;
using Enums;
using UnityEngine;

namespace ED.UI
{
    public interface IUIService
    {
        bool IsInProgress { get; }
        
        UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            UIRootKey rootKey = null,
            UIOptions? options = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel, new()
            where TView : MonoBehaviour, IUIView<TViewModel>;
        
        UniTask<TViewModel> OpenAsync<TViewModel, TView>(
            TViewModel viewModel,
            UIRootKey rootKey = null,
            UIOptions? options = null,
            CancellationToken cancellationToken = default)
            where TViewModel : class, IUIViewModel
            where TView : MonoBehaviour, IUIView<TViewModel>;
        
        UniTask CloseAsync<TViewModel>(
            TViewModel viewModel,
            CancellationToken cancellationToken = default)
            where TViewModel : IUIViewModel;
    }
}