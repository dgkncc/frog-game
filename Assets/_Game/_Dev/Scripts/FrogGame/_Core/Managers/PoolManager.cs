using AYellowpaper.SerializedCollections;
using FrogGame._Core.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrogGame._Core.Managers
{
    public class PoolManager : SingleMonoBehaviour<PoolManager>
    {
        public SerializedDictionary<PoolMonoBehaviour, int> PoolPrefabs;

        private Dictionary<Type, (PoolMonoBehaviour Prefab, Stack<PoolMonoBehaviour> Stack)> objectPool;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            objectPool = new Dictionary<Type, (PoolMonoBehaviour Prefab, Stack<PoolMonoBehaviour> Stack)>();

            foreach (var poolObject in PoolPrefabs)
            {
                var type = poolObject.Key.GetType();

                if (!objectPool.ContainsKey(type))
                {
                    var stack = new Stack<PoolMonoBehaviour>();

                    for (int i = 0; i < poolObject.Value; i++)
                    {
                        var newGo = Instantiate(poolObject.Key);
                        newGo.gameObject.SetActive(false);
                        newGo.transform.SetParent(transform);
                        newGo.transform.position = transform.position;
                        stack.Push(newGo);
                        newGo.OnInitialized();
                    }

                    objectPool.Add(type, (poolObject.Key, stack));
                }
            }
        }

        public void ReturnObjectToPool(PoolMonoBehaviour go)
        {
            if (objectPool[go.GetType()].Stack.Contains(go))//Expensive safety feature
                return;                                     //<-

            go.transform.position = transform.position;
            go.gameObject.SetActive(false);
            go.transform.SetParent(transform);
            go.OnReturnedToPool();
            objectPool[go.GetType()].Stack.Push(go);
        }

        public T GetFromPool<T>(Transform parent = null) where T : PoolMonoBehaviour
        {
            var type = typeof(T);

            if (objectPool.TryGetValue(type, out var pool) && pool.Stack.Count > 0)
            {
                return ActivateObject(pool.Stack.Pop(), parent) as T;
            }

            return InstantiateNewObject<T>(parent);
        }

        public PoolMonoBehaviour GetFromPool(PoolMonoBehaviour prefab, Transform parent = null)
        {
            var type = prefab.GetType();

            if (objectPool.TryGetValue(type, out var pool) && pool.Stack.Count > 0)
            {
                return ActivateObject(pool.Stack.Pop(), parent);
            }

            return InstantiateNewObject(prefab, parent);
        }

        private PoolMonoBehaviour ActivateObject(PoolMonoBehaviour go, Transform parent)
        {
            if (go == null) return null;

            go.gameObject.SetActive(true);
            go.transform.SetParent(parent ?? transform);
            go.OnGotFromPool();
            return go;
        }

        private T InstantiateNewObject<T>(Transform parent) where T : PoolMonoBehaviour
        {
            var type = typeof(T);
            var newGo = Instantiate(objectPool[type].Prefab) as T;

            if (newGo != null)
            {
                newGo.transform.SetParent(parent ?? transform);
                newGo.transform.position = transform.position;
                newGo.OnGotFromPool();
            }

            return newGo;
        }

        private PoolMonoBehaviour InstantiateNewObject(PoolMonoBehaviour prefab, Transform parent)
        {
            var newGo = Instantiate(prefab);
            newGo.transform.SetParent(parent ?? transform);
            newGo.OnInitialized();
            newGo.OnGotFromPool();
            return newGo;
        }
    }

}
