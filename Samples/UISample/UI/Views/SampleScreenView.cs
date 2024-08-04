using Cysharp.Threading.Tasks;

namespace ED.UI.Samples
{
    public class SampleScreenView : IUIView
    {
        public UniTask Show(bool forced)
        {
            return UniTask.CompletedTask;
        }

        public UniTask Hide(bool forced)
        {
            return UniTask.CompletedTask;
        }
    }
}