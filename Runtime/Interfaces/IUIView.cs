using Cysharp.Threading.Tasks;

namespace ED.UI
{
    public interface IUIView
    {
        UniTask Show(bool forced);
        UniTask Hide(bool forced);
    }
}