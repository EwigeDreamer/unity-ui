using System;

namespace ED.UI.Interfaces
{
    public interface IUIBindable<TViewModel> where TViewModel : IUIViewModel
    {
        IDisposable Bind(TViewModel viewModel);
    }
}