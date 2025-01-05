using System;
using ED.UI.Interfaces;
using ED.UI.Misc;
using UniRx;
using UnityEngine;

namespace ED.UI.Samples.ViewModels
{
    public class ListWindowModel : IUIViewModel, IDisposable
    {
        public readonly Subject<Unit> Close = new();
        public readonly UIPropertyProvider<Transform> Container = new();

        public void Dispose()
        {
            Close?.Dispose();
            Container?.Dispose();
        }
    }
}