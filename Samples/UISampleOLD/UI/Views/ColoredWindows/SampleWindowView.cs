// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using ED.MVVM;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace ED.UI.Samples
// {
//     public class SampleWindowView : MonoBehaviour, IUIView
//     {
//         [SerializeField] private Transform _window;
//         [SerializeField] private CanvasGroup _group;
//         private const float Duration = 0.25f;
//
//         [SerializeField]
//         [Data("close")]
//         private Button _closeButton;
//         
//         public UniTask Show(bool forced)
//         {
//             if (forced)
//             {
//                 transform.localScale = Vector3.one;
//                 return UniTask.CompletedTask;
//             }
//
//             return DOTween.Sequence()
//                 .AppendCallback(() => _group.interactable = false)
//                 .Append(DOTween.To(
//                         () => 0f,
//                         v => _group.alpha = v,
//                         1f,
//                         Duration)
//                     .SetEase(Ease.Linear))
//                 .Join(DOTween.To(
//                         () => Vector3.zero,
//                         v => _window.localScale = v,
//                         Vector3.one,
//                         Duration)
//                     .SetEase(Ease.OutBack))
//                 .AppendCallback(() => _group.interactable = true)
//                 .Play()
//                 .ToUniTask();
//         }
//
//         public UniTask Hide(bool forced)
//         {
//             if (forced)
//             {
//                 transform.localScale = Vector3.zero;
//                 return UniTask.CompletedTask;
//             }
//
//             return DOTween.Sequence()
//                 .AppendCallback(() => _group.interactable = false)
//                 .Append(DOTween.To(
//                         () => 1f,
//                         v => _group.alpha = v,
//                         0f,
//                         Duration)
//                     .SetEase(Ease.Linear))
//                 .Join(DOTween.To(
//                         () => Vector3.one,
//                         v => _window.localScale = v,
//                         Vector3.zero,
//                         Duration)
//                     .SetEase(Ease.InBack))
//                 .AppendCallback(() => _group.interactable = true)
//                 .Play()
//                 .ToUniTask();
//         }
//     }
// }