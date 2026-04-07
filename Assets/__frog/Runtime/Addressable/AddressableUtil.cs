using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace FrogLibrary
{
    public static class AddressableUtil
    {
        private static readonly Type k_componentType = typeof(Component);

        private static readonly Dictionary<string, AsyncOperationHandle> m_handleDict = new();

        public static T Load<T>(string address)
        {
            AsyncOperationHandle handle = GetHandle<T>(address);

            object obj = handle.WaitForCompletion();

            return GetResult<T>(obj);
        }

        public static async UniTask<T> LoadAsync<T>(string address)
        {
            AsyncOperationHandle handle = GetHandle<T>(address);

            await handle.ToUniTask();

            object obj = handle.Result;

            return GetResult<T>(obj);
        }

        #region # Private

        private static AsyncOperationHandle GetHandle<T>(string address)
        {
            var isComponent = k_componentType.IsAssignableFrom(typeof(T));

            if (m_handleDict.TryGetValue(address, out AsyncOperationHandle handle))
            {

            }
            else
            {
                handle = isComponent
                    ? Addressables.LoadAssetAsync<GameObject>(address)
                    : Addressables.LoadAssetAsync<T>(address);

                m_handleDict.Add(address, handle);
            }

            return handle;
        }

        private static T GetResult<T>(object obj)
        {
            var isComponent = k_componentType.IsAssignableFrom(typeof(T));

            T result = default;

            if (isComponent)
            {
                GameObject go = obj as GameObject;
                if (go)
                    go.TryGetComponent(out result);
            }
            else
            {
                result = (T)obj;
            }

            return result;
        }

        #endregion

        public static void Unload(string address)
        {
            if (!m_handleDict.TryGetValue(address, out AsyncOperationHandle value))
            {
                return;
            }
            
            Addressables.Release(value);

            m_handleDict.Remove(address);
        }
        
        public static T Instantiate<T>(string address, Transform parent = null) where T : Object
        {
            T mem = Load<T>(address);

            return Object.Instantiate(mem, parent);
        }

        public static async UniTask<T> InstantiateAsync<T>(string address, Transform parent = null)
            where T : Object
        {
            T mem = await LoadAsync<T>(address);
            
            return Object.Instantiate(mem, parent);
        }
    }
}
