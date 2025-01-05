using System;

namespace ED.UI.Misc
{
    public class UIPropertyProvider<TValue> : IDisposable
    {
        private PropertyHandler _handler;
        
        public IDisposable Setup(TValue value)
        {
            _handler?.Dispose();
            _handler = new PropertyHandler(value);
            return _handler;
        }

        public bool TryGetValue(out TValue value)
        {
            value = default;
            if (_handler == null) return false;
            if (_handler.IsDisposed) return false;
            value = _handler.Value;
            return true;
        }
        
        public void Dispose()
        {
            _handler?.Dispose();
            _handler = null;
        }

        private class PropertyHandler : IDisposable
        {
            public TValue Value { get; private set; }
            public bool IsDisposed { get; private set; }

            public PropertyHandler(TValue value)
            {
                Value = value;
            }
            
            public void Dispose()
            {
                Value = default;
                IsDisposed = true;
            }
        }
    }
}