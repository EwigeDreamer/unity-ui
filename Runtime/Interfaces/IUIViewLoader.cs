using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI
{
    public interface IUIViewLoader
    {
        UniTask<GameObject> LoadViewAsync(Type viewType, CancellationToken cancellationToken = default);
    }
}