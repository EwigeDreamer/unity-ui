using Cysharp.Threading.Tasks;
using TMPro;
using ED.MVVM;
using UnityEngine;

namespace ED.UI.Samples
{
    public class SampleScreenView : MonoBehaviour, IUIView
    {
        [SerializeField]
        [Data("title")]
        private TMP_Text _title;
        
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