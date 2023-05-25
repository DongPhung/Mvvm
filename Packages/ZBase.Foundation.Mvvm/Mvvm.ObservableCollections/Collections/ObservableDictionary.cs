using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZBase.Foundation.Mvvm.ObservableCollections
{
    public class ObservableDictionary<TKey, TValue>
        : IObservableCollection<KeyValuePair<TKey, TValue>>
        , IDictionary<TKey, TValue>
        , IReadOnlyDictionary<TKey, TValue>
    {
        private readonly object _syncRoot = new();
        private readonly Dictionary<TKey, TValue> _collection;

        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangingByAdd;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangingByReplace;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangingByRemove;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangingByClear;

        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangedByAdd;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangedByReplace;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangedByRemove;
        private event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _onChangedByClear;

        public ObservableDictionary()
        {
            _collection = new();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _collection = new(dictionary);
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _collection = new(collection);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _collection = new(comparer);
        }

        public ObservableDictionary(int capacity)
        {
            _collection = new(capacity);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _collection = new(dictionary, comparer);
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            _collection = new(collection, comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _collection = new(capacity, comparer);
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                lock (_syncRoot)
                {
                    return _collection.Keys;
                }
            }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                lock (_syncRoot)
                {
                    return _collection.Values;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_syncRoot)
                {
                    return _collection[key];
                }
            }

            set
            {
                lock (_syncRoot)
                {
                    var kv = new KeyValuePair<TKey, TValue>(key, value);
                    _onChangingByReplace?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Replace(this, kv));

                    _collection[key] = value;

                    _onChangedByReplace?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Replace(this, kv));
                }
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Keys;
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values;
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Keys;
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values;
        }

        public object SyncRoot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _syncRoot;
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _collection.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            lock (SyncRoot)
            {
                var kv = new KeyValuePair<TKey, TValue>(key, value);
                _onChangingByAdd?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Add(this, kv));

                var collection = _collection;
                var count = collection.Count;

                collection.Add(key, value);

                if (count != collection.Count)
                {
                    _onChangedByAdd?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Add(this, kv));
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> kv)
        {
            lock (SyncRoot)
            {
                _onChangingByAdd?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Add(this, kv));

                var collection = _collection;
                var count = collection.Count;

                collection.Add(kv.Key, kv.Value);

                if (count != collection.Count)
                {
                    _onChangedByAdd?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Add(this, kv));
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (_syncRoot)
            {
                var collection = _collection;

                if (collection.TryGetValue(key, out var value) == false)
                {
                    return false;
                }

                var kv = new KeyValuePair<TKey, TValue>(key, value);
                _onChangingByRemove?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Remove(this, kv));

                var result = _collection.Remove(key);

                if (result)
                {
                    _onChangedByRemove?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Remove(this, kv));
                }

                return result;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_syncRoot)
            {
                var collection = _collection;

                if (collection.TryGetValue(item.Key, out var value) == false)
                {
                    return false;
                }

                if (EqualityComparer<TValue>.Default.Equals(value, item.Value) == false)
                {
                    return false;
                }

                var kv = new KeyValuePair<TKey, TValue>(item.Key, value);
                _onChangingByRemove?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Remove(this, kv));

                var result = collection.Remove(item.Key);

                if (result)
                {
                    _onChangedByRemove?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Remove(this, kv));
                }

                return result;
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _onChangingByClear?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Clear(this));

                var collection = _collection;
                var count = collection.Count;
                collection.Clear();

                if (count != collection.Count)
                {
                    _onChangedByClear?.Invoke(CollectionChangeEventArgs<KeyValuePair<TKey, TValue>>.Clear(this));
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_syncRoot)
            {
                return _collection.ContainsKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_syncRoot)
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_collection).Contains(item);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncRoot)
            {
                return _collection.TryGetValue(key, out value);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)_collection).CopyTo(array, arrayIndex);
            }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _collection.GetEnumerator();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool CollectionChanged<TInstance>(CollectionAction action, CollectionChangeEventListener<KeyValuePair<TKey, TValue>, TInstance> listener)
            where TInstance : class
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            switch (action)
            {
                case CollectionAction.Add:
                {
                    this._onChangingByAdd += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangingByAdd -= listener.OnEvent;
                    return true;
                }

                case CollectionAction.Replace:
                {
                    this._onChangingByReplace += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangingByReplace -= listener.OnEvent;
                    return true;
                }
                
                case CollectionAction.Remove:
                {
                    this._onChangingByRemove += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangingByRemove -= listener.OnEvent;
                    return true;
                }

                case CollectionAction.Clear:
                {
                    this._onChangingByClear += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangingByClear -= listener.OnEvent;
                    return true;
                }
            }

            return false;
        }

        public bool CollectionChanging<TInstance>(CollectionAction action, CollectionChangeEventListener<KeyValuePair<TKey, TValue>, TInstance> listener)
            where TInstance : class
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            switch (action)
            {
                case CollectionAction.Add:
                {
                    this._onChangedByAdd += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangedByAdd -= listener.OnEvent;
                    return true;
                }

                case CollectionAction.Replace:
                {
                    this._onChangedByReplace += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangedByReplace -= listener.OnEvent;
                    return true;
                }
                
                case CollectionAction.Remove:
                {
                    this._onChangedByRemove += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangedByRemove -= listener.OnEvent;
                    return true;
                }

                case CollectionAction.Clear:
                {
                    this._onChangedByClear += listener.OnEvent;
                    listener.OnDetachAction = (listener) => this._onChangedByClear -= listener.OnEvent;
                    return true;
                }
            }

            return false;
        }
    }
}
