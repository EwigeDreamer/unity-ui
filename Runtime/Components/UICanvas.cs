using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvas : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasGroup _group;
        
        public IReadOnlyDictionary<UIRootKey, Transform> Roots { get; private set; }

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _group = GetComponent<CanvasGroup>();
            
            var roots = new Dictionary<UIRootKey, Transform>();
            foreach (var key in UIRootKey.Values.OrderBy(a => (int)a))
            {
                var go = new GameObject(key);
                var root = go.AddComponent<RectTransform>();
                root.SetParent(transform, false);
                root.anchorMin = Vector2.zero;
                root.anchorMax = Vector2.one;
                root.offsetMin = Vector2.zero;
                root.offsetMax = Vector2.zero;
                roots.Add(key, root);
            }
            Roots = roots;
        }

        public void SetInteractable(bool value)
        {
            _group.interactable = value;
        }
    }
}