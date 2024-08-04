using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ED.UI
{
    public interface IUIViewPool : IDisposable
    {
        UniTask<(IDisposable handler, GameObject view)> Get(object key);
    }
}