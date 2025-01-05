using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ED.UI.Interfaces;
using UnityEngine;

namespace ED.UI.Samples.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseUIView<TViewModel> : MonoBehaviour, IUIView<TViewModel> where TViewModel : IUIViewModel
    {
        [SerializeField] private RectTransform _content;
        
        private CanvasGroup _group;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        public UniTask ShowAsync(bool forced, CancellationToken cancellationToken = default)
        {
            var duration = 0.5f;
            return DOTween.Sequence()
                .Join(DOTween.To(
                        () => 0.75f,
                        v => _content.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, v),
                        1f,
                        duration)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject))
                .Join(DOTween.To(
                        () => 0f,
                        v => _group.alpha = v,
                        1f,
                        duration)
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject))
                .SetLink(gameObject)
                .Play()
                .WithCancellation(cancellationToken);
        }

        public UniTask HideAsync(bool forced, CancellationToken cancellationToken = default)
        {
            var duration = 0.5f;
            return DOTween.Sequence()
                .Join(DOTween.To(
                        () => 1f,
                        v => _content.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, v),
                        0.75f,
                        duration)
                    .SetEase(Ease.InBack)
                    .SetLink(gameObject))
                .Join(DOTween.To(
                        () => 1f,
                        v => _group.alpha = v,
                        0f,
                        duration)
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject))
                .SetLink(gameObject)
                .Play()
                .WithCancellation(cancellationToken);
        }

        public abstract IDisposable Bind(TViewModel viewModel);
    }
}