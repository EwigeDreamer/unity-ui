using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI.Samples
{
    public class Window1 : MonoBehaviour, IUIView<Window1Model>
    {
        public UniTask ShowAsync(bool forced, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(bool forced, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public IDisposable Bind(Window1Model viewModel)
        {
            return null;
        }
    }
}