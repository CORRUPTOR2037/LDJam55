using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Collections
{
    [Serializable]
    public class SerializableDictionaryItem<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializableDictionaryItem() {}
        public SerializableDictionaryItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class SerializableDictionary<TItem, TKey, TValue> : IDictionary<TKey, TValue>
        where TItem : SerializableDictionaryItem<TKey, TValue>, new()
    {
        [SerializeField]
        protected List<TItem> _list = new List<TItem>();

        public int Count => _list.Count;

        public TItem this[TKey key] 
        {
            get
            {
                foreach (var item in _list)
                    if (item.Key.Equals(key))
                        return item;
                throw new KeyNotFoundException(key.ToString());
            }
            set
            {
                for (int i = 0; i < Count; i++)
                    if (_list[i].Key.Equals(key))
                    {
                        _list[i] = value;
                        return;
                    }
                _list.Add(value);
            }
        }

        public void Add(object value)
        {
            if (value is TItem ti)
                _list.Add(ti);
            else if (value is Dictionary<string, object> di)
                Add(new TItem(){ Key = (TKey) di["Key"], Value = (TValue) di["Value"] });
            else if (value is KeyValuePair<TKey, TValue> kvi)
                Add(new TItem(){ Key = kvi.Key, Value = kvi.Value });
        }

        public TItem At(int i) => _list[i];

        public bool ContainsKey(TKey key)
        {
            foreach (var item in _list)
                if (item.Key.Equals(key))
                    return true;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public IEnumerator<TItem> GetEnumerator() => _list.GetEnumerator();

        public bool Remove(TKey key)
        {
            foreach (var item in _list)
                if (item.Key.Equals(key))
                {
                    _list.Remove(item);
                    return true;
                }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = ContainsKey(key) ? this[key].Value : default(TValue);
            return ContainsKey(key);
        }

        public void Clear() => _list.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key) && this[item.Key].Value.Equals(item.Value);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        public ICollection<TKey> Keys => _list.Select(item => item.Key).ToList();
        public List<TItem> Values => _list;
        public bool IsReadOnly => false;

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(new TItem(){ Key = key, Value = value });
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => (this as IDictionary<TKey, TValue>).Add(item.Key, item.Value);
        ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new NotImplementedException();
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => throw new NotImplementedException();
        TValue IDictionary<TKey, TValue>.this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionary<SerializableDictionaryItem<TKey, TValue>, TKey, TValue>
    {
        public new TValue this[TKey key] 
        {
            get
            {
                foreach (var item in _list)
                    if (item.Key.Equals(key))
                        return item.Value;
                throw new KeyNotFoundException(key.ToString());
            }
            set
            {
                for (int i = 0; i < Count; i++)
                    if (_list[i].Key.Equals(key))
                    {
                        _list[i].Value = value;
                        return;
                    }
                _list.Add(new SerializableDictionaryItem<TKey, TValue>(key, value));
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(SerializableDictionary<,,>), true)]
    public class SerializableDictionaryDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, property.FindPropertyRelative("_list"), label);
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return UnityEditor.EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_list"));
        }
    }
#endif
}