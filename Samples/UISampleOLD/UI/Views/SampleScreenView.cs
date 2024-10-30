// using Cysharp.Threading.Tasks;
// using TMPro;
// using ED.MVVM;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace ED.UI.Samples
// {
//     public class SampleScreenView : MonoBehaviour, IUIView
//     {
//         [SerializeField]
//         [Data("title")]
//         private TMP_Text _title;
//
//         [SerializeField]
//         [Data("open_window")]
//         private Button _openWindowButton;
//
//         [SerializeField]
//         [Data("open_red_window")]
//         private Button _openRedWindowButton;
//
//         [SerializeField]
//         [Data("open_green_window")]
//         private Button _openGreenWindowButton;
//
//         [SerializeField]
//         [Data("open_blue_window")]
//         private Button _openBlueWindowButton;
//         
//         public UniTask Show(bool forced)
//         {
//             return UniTask.CompletedTask;
//         }
//
//         public UniTask Hide(bool forced)
//         {
//             return UniTask.CompletedTask;
//         }
//     }
// }