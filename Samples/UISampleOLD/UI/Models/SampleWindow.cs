// using System;
// using ED.MVVM;
// using UnityEngine;
// using Zenject;
//
// namespace ED.UI.Samples
// {
//     public class SampleWindow : IUIModel
//     {
//         [Inject] private readonly IUIService UIService;
//
//         public object DefaultViewKey => nameof(SampleWindowView);
//         public UIOptions DefaultOptions => UIOptions.Default;
//         public object ViewModel => _model;
//
//         private readonly Model _model = new();
//         
//         public void Awake()
//         {
//             _model.OnCloseClicked += () => UIService.Close(this);
//         }
//
//         public void Start()
//         {
//             
//         }
//         
//         public void Dispose()
//         {
//             
//         }
//         
//         private class Model
//         {
//             public event Action OnCloseClicked;
//
//             [Method("close")]
//             private void CloseClick() => OnCloseClicked?.Invoke();
//         }
//     }
// }