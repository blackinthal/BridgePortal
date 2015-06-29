using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Common
{
    public sealed class ConcurrentList<T> : IList<T>
    {
        private readonly IList<T> _items = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            lock (_items)
            {
                return _items.ToList().GetEnumerator();
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get
            {
                lock (_items)
                {
                    return _items.Count;
                }
            }
        }


        public void Add(T item)
        {
            lock (_items)
            {
                _items.Add(item);
            }
        }

        public void Clear()
        {
            lock (_items)   // lock on the list
            {
                _items.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_items)   // lock on the list
            {
                return _items.Contains(item);
            }
        }
        public bool Remove(T item)
        {
            lock (_items)   // lock on the list
            {
                return _items.Remove(item);
            }
        }


        public T this[int index]
        {
            get
            {
                lock (_items)   // lock on the list
                {
                    return _items[index];
                }
            }
            set
            {
                lock (_items)   // lock on the list
                {
                    _items[index] = value;
                }
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_items)   // lock on the list
            {
                _items.CopyTo(array, arrayIndex);
            }
        }


        public bool IsReadOnly { get; private set; }
        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

    }
}
