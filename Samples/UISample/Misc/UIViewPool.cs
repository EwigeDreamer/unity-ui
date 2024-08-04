using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace ED.UI.Samples
{
    public class UIViewPool : IUIViewPool
    {
        private readonly Dictionary<object, ObjectPool<GameObject>> _poolDict = new();
        private readonly List<Transform> _poolContainers = new();
        
        public async UniTask<(IDisposable handler, GameObject view)> Get(object key)
        {
            if (!_poolDict.TryGetValue(key, out var pool))
                _poolDict[key] = pool = await CreatePool(key);
            var handler = pool.Get(out var view);
            return (handler, view);
        }

        private async UniTask<ObjectPool<GameObject>> CreatePool(object key)
        {
            if (key is not string path)
                throw new InvalidOperationException($"{nameof(key)} should be a string Resources path!");
            var container = CreatePoolContainer($"UI_POOL_CONTAINER <{path}>");
            var prefab = (GameObject) await Resources.LoadAsync<GameObject>(path).ToUniTask();
            if (prefab == null)
                throw new InvalidOperationException($"{nameof(prefab)} is null!");
            return new ObjectPool<GameObject>(
                () => UnityEngine.Object.Instantiate(prefab),
                view =>
                {
                    view.transform.SetParent(null);
                    view.SetActive(true);
                },
                view =>
                {
                    view.transform.SetParent(container);
                    view.SetActive(false);
                },
                UnityEngine.Object.Destroy);
        }

        private Transform CreatePoolContainer(string name)
        {
            var container = new GameObject(name).transform;
            UnityEngine.Object.DontDestroyOnLoad(container);
            _poolContainers.Add(container);
            return container;
        }

        public void Dispose()
        {
            foreach(var pool in _poolDict.Values)
                pool.Dispose();
            _poolDict.Clear();
            foreach(var container in _poolContainers)
                UnityEngine.Object.Destroy(container);
            _poolContainers.Clear();
        }
    }
}