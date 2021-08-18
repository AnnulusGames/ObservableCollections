﻿using ObservableCollections.Internal;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ObservableCollections
{
    // can not implements ISet<T> because set operation can not get added/removed values.
    public sealed partial class ObservableHashSet<T> : IReadOnlySet<T>, IReadOnlyCollection<T>, IObservableCollection<T>
        where T : notnull
    {
        readonly HashSet<T> set;
        public object SyncRoot { get; } = new object();

        public ObservableHashSet()
        {
            this.set = new HashSet<T>();
        }

        public ObservableHashSet(int capacity)
        {
            this.set = new HashSet<T>(capacity);
        }

        public ObservableHashSet(IEnumerable<T> collection)
        {
            this.set = new HashSet<T>(collection);
        }

        public event NotifyCollectionChangedEventHandler<T>? CollectionChanged;

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return set.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            lock (SyncRoot)
            {
                if (set.Add(item))
                {
                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Add(item, -1));
                    return true;
                }

                return false;
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (SyncRoot)
            {
                if (!items.TryGetNonEnumeratedCount(out var capacity))
                {
                    capacity = 4;
                }

                using (var list = new ResizableArray<T>(capacity))
                {
                    foreach (var item in items)
                    {
                        if (set.Add(item))
                        {
                            list.Add(item);
                        }
                    }

                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Add(list.Span, -1));
                }
            }
        }

        public void AddRange(T[] items)
        {
            AddRange(items.AsSpan());
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            lock (SyncRoot)
            {
                using (var list = new ResizableArray<T>(items.Length))
                {
                    foreach (var item in items)
                    {
                        if (set.Add(item))
                        {
                            list.Add(item);
                        }
                    }

                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Add(list.Span, -1));
                }
            }
        }

        public bool Remove(T item)
        {
            lock (SyncRoot)
            {
                if (set.Remove(item))
                {
                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Remove(item, -1));
                    return true;
                }

                return false;
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            lock (SyncRoot)
            {
                if (!items.TryGetNonEnumeratedCount(out var capacity))
                {
                    capacity = 4;
                }

                using (var list = new ResizableArray<T>(capacity))
                {
                    foreach (var item in items)
                    {
                        if (set.Remove(item))
                        {
                            list.Add(item);
                        }
                    }

                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Remove(list.Span, -1));
                }
            }
        }

        public void RemoveRange(T[] items)
        {
            RemoveRange(items.AsSpan());
        }

        public void RemoveRange(ReadOnlySpan<T> items)
        {
            lock (SyncRoot)
            {
                using (var list = new ResizableArray<T>(items.Length))
                {
                    foreach (var item in items)
                    {
                        if (set.Remove(item))
                        {
                            list.Add(item);
                        }
                    }

                    CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Remove(list.Span, -1));
                }
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                set.Clear();
                CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<T>.Reset());
            }
        }

        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            return set.TryGetValue(equalValue, out actualValue);
        }

        public bool Contains(T item)
        {
            lock (SyncRoot)
            {
                return set.Contains(item);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.IsProperSubsetOf(other);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.IsProperSupersetOf(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.IsSubsetOf(other);
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.IsSupersetOf(other);
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.Overlaps(other);
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            lock (SyncRoot)
            {
                return set.SetEquals(other);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SynchronizedEnumerator<T>(SyncRoot, set.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
