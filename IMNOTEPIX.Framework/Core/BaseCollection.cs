using System;
using System.Collections;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fCore
{
    /// <summary>
    /// Direct 1:1 port of RAGENativeUI's BaseCollection&lt;T&gt; - no RPH/native dependencies
    /// existed in the original, so nothing needed to change.
    /// </summary>
    public class BaseCollection<T> : ICollection<T>
    {
        public delegate void CollectionItemAddedEventHandler(BaseCollection<T> sender, T addedItem);
        public delegate void CollectionItemRemovedEventHandler(BaseCollection<T> sender, T removedItem);
        public delegate void CollectionItemModifiedEventHandler(BaseCollection<T> sender, T oldItem, T newItem);
        public delegate void CollectionClearedEventHandler(BaseCollection<T> sender);

        protected List<T> InternalList { get; set; }

        public virtual event CollectionItemAddedEventHandler ItemAdded;
        public virtual event CollectionItemRemovedEventHandler ItemRemoved;
        public virtual event CollectionItemModifiedEventHandler ItemModified;
        public virtual event CollectionClearedEventHandler Cleared;

        public virtual int Count { get { return InternalList.Count; } }

        public virtual bool IsReadOnly { get { return false; } }

        public virtual T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return InternalList[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                T old = InternalList[index];
                if (!EqualityComparer<T>.Default.Equals(old, value))
                {
                    InternalList[index] = value;
                    ItemModified?.Invoke(this, old, value);
                }
            }
        }

        public BaseCollection()
        {
            InternalList = new List<T>();
        }

        public BaseCollection(IEnumerable<T> collection)
        {
            InternalList = new List<T>(collection);
        }

        public virtual void Add(T item)
        {
            InternalList.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        public virtual void Add(params T[] items) => AddRange((IEnumerable<T>)items);

        public virtual void AddRange(IEnumerable<T> items)
        {
            InternalList.AddRange(items);
            if (ItemAdded != null)
            {
                foreach (T i in items)
                {
                    ItemAdded.Invoke(this, i);
                }
            }
        }

        public virtual void RemoveAt(int index)
        {
            T i = InternalList[index];
            InternalList.RemoveAt(index);
            ItemRemoved?.Invoke(this, i);
        }

        public virtual bool Remove(T item)
        {
            bool result = InternalList.Remove(item);
            if (result)
                ItemRemoved?.Invoke(this, item);
            return result;
        }

        public virtual void Clear()
        {
            InternalList.Clear();
            Cleared?.Invoke(this);
        }

        public virtual bool Contains(T item)
        {
            return InternalList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        public virtual int IndexOf(T item)
        {
            return InternalList.IndexOf(item);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}