using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI
{
    public interface IUIViewPool
    {
        UniTask<(IDisposable handler, GameObject view)> Get(object key);
    }
}