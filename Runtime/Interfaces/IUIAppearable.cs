using System.Threading;
using Cysharp.Threading.Tasks;

namespace ED.UI.Interfaces
{
    public interface IUIAppearable
    {
        UniTask ShowAsync(bool forced, CancellationToken cancellationToken = default);
        UniTask HideAsync(bool forced, CancellationToken cancellationToken = default);
    }
}