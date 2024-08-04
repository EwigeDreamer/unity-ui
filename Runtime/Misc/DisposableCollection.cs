using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace ED.UI
{
    internal class DisposableCollection : IDisposable
    {
        private List<IDisposable> _list;
        private bool _isDisposed;

        public DisposableCollection()
        {
            _list = ListPool<IDisposable>.Get();
        }

        public void Add(IDisposable item)
        {
            if (item == null) return;
            if (_isDisposed) item.Dispose();
            else _list.Add(item);
        }
        
        public void Dispose()
        {
            if (_isDisposed) return;
            foreach(var item in _list) item.Dispose();
            ListPool<IDisposable>.Release(_list);
            _list = null;
            _isDisposed = true;
        }
    }
}