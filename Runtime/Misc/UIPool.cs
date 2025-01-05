using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ED.UI.Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace ED.UI.Misc
{
    internal class UIPool
    {
        private readonly Dictionary<Type, ObjectPool<GameObject>> _poolDict = new();
        private readonly List<Transform> _poolContainers = new();
        private readonly HashSet<Type> _typesInProgress = new();
        private readonly IUIViewLoader _loader;
        private readonly Transform _root;

        public UIPool(IUIViewLoader loader, Transform root)
        {
            _loader = loader;
            _root = root;
        }
        
        public async UniTask<(IDisposable handler, GameObject view)> GetAsync(Type viewType, Transform parent, CancellationToken cancellationToken = default)
        {
            await UniTask.WaitWhile(() => _typesInProgress.Contains(viewType), cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested) return (null, null);
            if (!_poolDict.TryGetValue(viewType, out var pool))
                _poolDict[viewType] = pool = await CreatePool(viewType, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return (null, null);
            var handler = pool.Get(out var view);
            view.transform.SetParent(parent, false);
            view.transform.SetAsLastSibling();
            return (handler, view);
        }

        private async UniTask<ObjectPool<GameObject>> CreatePool(Type viewType, CancellationToken cancellationToken = default)
        {
            _typesInProgress.Add(viewType);
            var container = CreatePoolContainer($"UI_POOL_CONTAINER <{viewType.Name}>");
            var prefab = await _loader.LoadViewAsync(viewType, cancellationToken);
            if (prefab == null) throw new InvalidOperationException($"{nameof(prefab)} is null!");
            _typesInProgress.Remove(viewType);
            return new ObjectPool<GameObject>(
                () => UnityEngine.Object.Instantiate(prefab, container),
                view => { },
                view => view.transform.SetParent(container, false),
                UnityEngine.Object.Destroy);
        }

        private Transform CreatePoolContainer(string name)
        {
            var go = new GameObject(name);
            var container = go.AddComponent<RectTransform>();
            container.SetParent(_root, false);
            container.gameObject.SetActive(false);
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = Vector2.zero;
            container.offsetMax = Vector2.zero;
            _poolContainers.Add(container);
            return container;
        }

        public void Dispose()
        {
            foreach(var pool in _poolDict.Values)
                pool.Dispose();
            _poolDict.Clear();
            
            foreach(var container in _poolContainers)
                if (container != null)
                    UnityEngine.Object.Destroy(container.gameObject);
            _poolContainers.Clear();
        }
    }
}