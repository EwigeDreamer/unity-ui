using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ED.UI.Interfaces;
using UnityEngine;

namespace ED.UI.Samples
{
    public class UIResourcesLoader : IUIViewLoader
    {
        public async UniTask<GameObject> LoadViewAsync(Type viewType, CancellationToken cancellationToken = default)
        {
            var path = $"UIServiceSampleWindows/{viewType.Name}";
            return (GameObject) await Resources.LoadAsync<GameObject>(path).ToUniTask(cancellationToken: cancellationToken);
        }
    }
}