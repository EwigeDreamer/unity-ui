using System;

namespace ED.UI
{
    public interface IUIBindable<TViewModel> where TViewModel : IUIViewModel
    {
        IDisposable Bind(TViewModel viewModel);
    }
}