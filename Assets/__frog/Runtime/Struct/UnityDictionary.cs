using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrogLibrary
{
    [Serializable]
    public class UnityDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        // 키와 값을 하나의 단위로 묶는 내부 구조체
        [Serializable]
        public struct Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        // 리스트 1개만 사용
        [SerializeField] private List<Entry> m_entries = new List<Entry>();

        public UnityDictionary() { }

        public UnityDictionary(int capacity)
        {
            m_entries.Capacity = capacity;
        }
        
        #region IDictionary Implementation

        public bool ContainsKey(TKey key)
        {
            return m_entries.Any(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (ContainsKey(key)) throw new ArgumentException("Key already exists: " + key);

            m_entries.Add(new Entry(key, value));
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            int index = m_entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
            if (index < 0) return false;

            m_entries.RemoveAt(index);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = m_entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
            if (index >= 0)
            {
                value = m_entries[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                int index = m_entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
                if (index < 0) throw new KeyNotFoundException($"Key doesn't exist: {key}");
                return m_entries[index].Value;
            }
            set
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                int index = m_entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
                if (index < 0) throw new KeyNotFoundException($"Key doesn't exist: {key}");
                
                // 구조체이므로 값을 직접 수정하지 않고 새로 할당
                m_entries[index] = new Entry(key, value);
            }
        }

        public ICollection<TKey> Keys => m_entries.Select(e => e.Key).ToList();
        public ICollection<TValue> Values => m_entries.Select(e => e.Value).ToList();

        #endregion

        #region ICollection Implementation

        public int Count => m_entries.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public void Clear() => m_entries.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < m_entries.Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(m_entries[i].Key, m_entries[i].Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var entry in m_entries)
            {
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

     
    }
    
    public static class UnityDictionaryExtensions
    {
        public static IReadOnlyDictionary<TK, TV> ToReadonlyDictionary<TK, TV>(
            this IDictionary<TK, TV> dictionary)
        {
            Dictionary<TK, TV> result = new Dictionary<TK, TV>();
            dictionary.ToList().ForEach(e => result.Add(e.Key, e.Value));
            return result;
        }

        public static UnityDictionary<TK, TV> ToUnityDictionary<TK, TV>(
            this IDictionary<TK, TV> dictionary)
        {
            UnityDictionary<TK, TV> result = new UnityDictionary<TK, TV>();
            dictionary.ToList().ForEach(e => result.Add(e.Key, e.Value));
            return result;
        }
    }
}