using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Capsule2026.Impl
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var set = new RBTreeSet<int>();

            Console.WriteLine("=== Заполнение множества ===");
            int[] numbers = { 50, 30, 70, 20, 40, 60, 80, 10, 25, 35, 45, 65, 75, 85 };

            foreach (int n in numbers)
            {
                set.Add(n);
                Console.WriteLine("Добавили: " + n);
            }

            Console.WriteLine();
            Console.WriteLine("Текущее множество:");
            Console.WriteLine(set);

            Console.WriteLine();
            Console.WriteLine("=== Базовые операции ===");
            Console.WriteLine("Первый элемент: " + set.First());
            Console.WriteLine("Последний элемент: " + set.Last());
            Console.WriteLine("Содержит 40? " + set.Contains(40));
            Console.WriteLine("Содержит 100? " + set.Contains(100));
            Console.WriteLine("Размер множества: " + set.Size());

            Console.WriteLine();
            Console.WriteLine("=== Навигационные методы ===");
            Console.WriteLine("Ceiling(42): " + set.Ceiling(42)); // ближайший >= 42
            Console.WriteLine("Floor(42): " + set.Floor(42));     // ближайший <= 42
            Console.WriteLine("Higher(40): " + set.Higher(40));   // строго больше 40
            Console.WriteLine("Lower(40): " + set.Lower(40));     // строго меньше 40

            Console.WriteLine();
            Console.WriteLine("=== Удаление ===");
            Console.WriteLine("Удаляем 20: " + set.Remove(20));
            Console.WriteLine("Удаляем 70: " + set.Remove(70));
            Console.WriteLine("Удаляем 999: " + set.Remove(999));

            Console.WriteLine("Множество после удаления:");
            Console.WriteLine(set);

            Console.WriteLine();
            Console.WriteLine("=== Подмножество [30, 75) ===");
            var subset = set.SubSet(30, 75);
            Console.WriteLine(subset);

            Console.WriteLine();
            Console.WriteLine("=== HeadSet(< 50) ===");
            var head = set.HeadSet(50);
            Console.WriteLine(head);

            Console.WriteLine();
            Console.WriteLine("=== TailSet(40 >=) ===");
            var tail = set.TailSet(40, true);
            Console.WriteLine(tail);

            Console.WriteLine();
            Console.WriteLine("=== Обратный порядок ===");
            var desc = set.DescendingSet();
            Console.WriteLine(desc);

            Console.WriteLine();
            Console.WriteLine("=== PollFirst / PollLast ===");
            Console.WriteLine("PollFirst(): " + set.PollFirst());
            Console.WriteLine("PollLast(): " + set.PollLast());
            Console.WriteLine("После удаления первого и последнего:");
            Console.WriteLine(set);

            Console.WriteLine();
            Console.WriteLine("=== Итоговый обход foreach ===");
            foreach (int x in set)
            {
                Console.Write(x + " ");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Готово.");
            Console.ReadKey();
        }
    }

    public sealed class MyTreeMap<K, V>
    {
        private const bool Red = false;
        private const bool Black = true;

        private readonly IComparer<K> _comparer;
        private Entry _root;
        private int _size;

        internal sealed class Entry
        {
            public K Key;
            public V Value;
            public Entry Left;
            public Entry Right;
            public Entry Parent;
            public bool Color = Black;

            public Entry(K key, V value, Entry parent, bool color)
            {
                Key = key;
                Value = value;
                Parent = parent;
                Color = color;
            }
        }

        public MyTreeMap()
            : this(null)
        {
        }

        public MyTreeMap(IComparer<K> comparer)
        {
            _comparer = comparer ?? Comparer<K>.Default;
        }

        internal int CompareKeys(K a, K b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a), "Null keys are not supported");
            if (b == null) throw new ArgumentNullException(nameof(b), "Null keys are not supported");
            return _comparer.Compare(a, b);
        }

        private static bool ColorOf(Entry p)
        {
            return p == null ? Black : p.Color;
        }

        private static Entry ParentOf(Entry p)
        {
            return p == null ? null : p.Parent;
        }

        private static Entry LeftOf(Entry p)
        {
            return p == null ? null : p.Left;
        }

        private static Entry RightOf(Entry p)
        {
            return p == null ? null : p.Right;
        }

        private static void SetColor(Entry p, bool c)
        {
            if (p != null) p.Color = c;
        }

        public int Size()
        {
            return _size;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public void Clear()
        {
            _root = null;
            _size = 0;
        }

        public bool ContainsKey(K key)
        {
            return GetEntry(key) != null;
        }

        public V Get(K key)
        {
            var p = GetEntry(key);
            return p == null ? default(V) : p.Value;
        }

        public V Put(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            if (_root == null)
            {
                _root = new Entry(key, value, null, Black);
                _size = 1;
                return default(V);
            }

            Entry parent;
            Entry t = _root;
            int cmp = 0;

            do
            {
                parent = t;
                cmp = CompareKeys(key, t.Key);
                if (cmp < 0)
                {
                    t = t.Left;
                }
                else if (cmp > 0)
                {
                    t = t.Right;
                }
                else
                {
                    V old = t.Value;
                    t.Value = value;
                    return old;
                }
            } while (t != null);

            Entry e = new Entry(key, value, parent, Red);
            if (cmp < 0)
                parent.Left = e;
            else
                parent.Right = e;

            FixAfterInsertion(e);
            _size++;
            return default(V);
        }

        public V Remove(K key)
        {
            Entry p = GetEntry(key);
            if (p == null) return default(V);

            V oldValue = p.Value;
            DeleteEntry(p);
            return oldValue;
        }

        public K FirstKey()
        {
            Entry p = FirstEntry();
            if (p == null) throw new InvalidOperationException("Map is empty");
            return p.Key;
        }

        public K LastKey()
        {
            Entry p = LastEntry();
            if (p == null) throw new InvalidOperationException("Map is empty");
            return p.Key;
        }

        public K CeilingKey(K key)
        {
            Entry p = GetCeilingEntry(key);
            return p == null ? default(K) : p.Key;
        }

        public K FloorKey(K key)
        {
            Entry p = GetFloorEntry(key);
            return p == null ? default(K) : p.Key;
        }

        public K HigherKey(K key)
        {
            Entry p = GetHigherEntry(key);
            return p == null ? default(K) : p.Key;
        }

        public K LowerKey(K key)
        {
            Entry p = GetLowerEntry(key);
            return p == null ? default(K) : p.Key;
        }

        internal Entry FirstEntry()
        {
            Entry p = _root;
            if (p != null)
            {
                while (p.Left != null) p = p.Left;
            }
            return p;
        }

        internal Entry LastEntry()
        {
            Entry p = _root;
            if (p != null)
            {
                while (p.Right != null) p = p.Right;
            }
            return p;
        }

        private Entry Successor(Entry t)
        {
            if (t == null) return null;

            if (t.Right != null)
            {
                Entry p = t.Right;
                while (p.Left != null) p = p.Left;
                return p;
            }

            Entry parent = t.Parent;
            Entry ch = t;
            while (parent != null && ch == parent.Right)
            {
                ch = parent;
                parent = parent.Parent;
            }

            return parent;
        }

        private Entry GetEntry(K key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            Entry p = _root;
            while (p != null)
            {
                int cmp = CompareKeys(key, p.Key);
                if (cmp < 0)
                    p = p.Left;
                else if (cmp > 0)
                    p = p.Right;
                else
                    return p;
            }

            return null;
        }

        internal Entry GetCeilingEntry(K key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            Entry p = _root;
            Entry candidate = null;

            while (p != null)
            {
                int cmp = CompareKeys(key, p.Key);
                if (cmp == 0) return p;

                if (cmp < 0)
                {
                    candidate = p;
                    p = p.Left;
                }
                else
                {
                    p = p.Right;
                }
            }

            return candidate;
        }

        internal Entry GetFloorEntry(K key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            Entry p = _root;
            Entry candidate = null;

            while (p != null)
            {
                int cmp = CompareKeys(key, p.Key);
                if (cmp == 0) return p;

                if (cmp < 0)
                {
                    p = p.Left;
                }
                else
                {
                    candidate = p;
                    p = p.Right;
                }
            }

            return candidate;
        }

        internal Entry GetHigherEntry(K key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            Entry p = _root;
            Entry candidate = null;

            while (p != null)
            {
                int cmp = CompareKeys(key, p.Key);
                if (cmp < 0)
                {
                    candidate = p;
                    p = p.Left;
                }
                else
                {
                    p = p.Right;
                }
            }

            return candidate;
        }

        internal Entry GetLowerEntry(K key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Null keys are not supported");

            Entry p = _root;
            Entry candidate = null;

            while (p != null)
            {
                int cmp = CompareKeys(key, p.Key);
                if (cmp <= 0)
                {
                    p = p.Left;
                }
                else
                {
                    candidate = p;
                    p = p.Right;
                }
            }

            return candidate;
        }

        private void RotateLeft(Entry p)
        {
            if (p == null) return;

            Entry r = p.Right;

            p.Right = r.Left;
            if (r.Left != null) r.Left.Parent = p;

            r.Parent = p.Parent;
            if (p.Parent == null)
            {
                _root = r;
            }
            else if (p.Parent.Left == p)
            {
                p.Parent.Left = r;
            }
            else
            {
                p.Parent.Right = r;
            }

            r.Left = p;
            p.Parent = r;
        }

        private void RotateRight(Entry p)
        {
            if (p == null) return;

            Entry l = p.Left;
            p.Left = l.Right;
            if (l.Right != null) l.Right.Parent = p;

            l.Parent = p.Parent;
            if (p.Parent == null)
            {
                _root = l;
            }
            else if (p.Parent.Right == p)
            {
                p.Parent.Right = l;
            }
            else
            {
                p.Parent.Left = l;
            }

            l.Right = p;
            p.Parent = l;
        }

        private void FixAfterInsertion(Entry x)
        {
            x.Color = Red;

            while (x != null && x != _root && ColorOf(ParentOf(x)) == Red)
            {
                if (ParentOf(x) == LeftOf(ParentOf(ParentOf(x))))
                {
                    Entry y = RightOf(ParentOf(ParentOf(x))); // uncle
                    if (ColorOf(y) == Red)
                    {
                        SetColor(ParentOf(x), Black);
                        SetColor(y, Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == RightOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateLeft(x);
                        }

                        SetColor(ParentOf(x), Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        RotateRight(ParentOf(ParentOf(x)));
                    }
                }
                else
                {
                    Entry y = LeftOf(ParentOf(ParentOf(x))); // uncle
                    if (ColorOf(y) == Red)
                    {
                        SetColor(ParentOf(x), Black);
                        SetColor(y, Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == LeftOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateRight(x);
                        }

                        SetColor(ParentOf(x), Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        RotateLeft(ParentOf(ParentOf(x)));
                    }
                }
            }

            _root.Color = Black;
        }

        private void DeleteEntry(Entry p)
        {
            _size--;

            if (p.Left != null && p.Right != null)
            {
                Entry s = Successor(p);
                p.Key = s.Key;
                p.Value = s.Value;
                p = s;
            }

            Entry replacement = p.Left != null ? p.Left : p.Right;

            if (replacement != null)
            {
                replacement.Parent = p.Parent;

                if (p.Parent == null)
                {
                    _root = replacement;
                }
                else if (p == p.Parent.Left)
                {
                    p.Parent.Left = replacement;
                }
                else
                {
                    p.Parent.Right = replacement;
                }

                p.Left = null;
                p.Right = null;
                p.Parent = null;

                if (p.Color == Black)
                {
                    FixAfterDeletion(replacement);
                }
            }
            else if (p.Parent == null)
            {
                _root = null;
            }
            else
            {
                if (p.Color == Black)
                {
                    FixAfterDeletion(p);
                }

                if (p.Parent != null)
                {
                    if (p == p.Parent.Left)
                        p.Parent.Left = null;
                    else if (p == p.Parent.Right)
                        p.Parent.Right = null;

                    p.Parent = null;
                }
            }
        }

        private void FixAfterDeletion(Entry x)
        {
            while (x != _root && ColorOf(x) == Black)
            {
                if (x == LeftOf(ParentOf(x)))
                {
                    Entry sib = RightOf(ParentOf(x));

                    if (ColorOf(sib) == Red)
                    {
                        SetColor(sib, Black);
                        SetColor(ParentOf(x), Red);
                        RotateLeft(ParentOf(x));
                        sib = RightOf(ParentOf(x));
                    }

                    if (ColorOf(LeftOf(sib)) == Black && ColorOf(RightOf(sib)) == Black)
                    {
                        SetColor(sib, Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(RightOf(sib)) == Black)
                        {
                            SetColor(LeftOf(sib), Black);
                            SetColor(sib, Red);
                            RotateRight(sib);
                            sib = RightOf(ParentOf(x));
                        }

                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), Black);
                        SetColor(RightOf(sib), Black);
                        RotateLeft(ParentOf(x));
                        x = _root;
                    }
                }
                else
                {
                    Entry sib = LeftOf(ParentOf(x));

                    if (ColorOf(sib) == Red)
                    {
                        SetColor(sib, Black);
                        SetColor(ParentOf(x), Red);
                        RotateRight(ParentOf(x));
                        sib = LeftOf(ParentOf(x));
                    }

                    if (ColorOf(RightOf(sib)) == Black && ColorOf(LeftOf(sib)) == Black)
                    {
                        SetColor(sib, Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(LeftOf(sib)) == Black)
                        {
                            SetColor(RightOf(sib), Black);
                            SetColor(sib, Red);
                            RotateLeft(sib);
                            sib = LeftOf(ParentOf(x));
                        }

                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), Black);
                        SetColor(LeftOf(sib), Black);
                        RotateRight(ParentOf(x));
                        x = _root;
                    }
                }
            }

            SetColor(x, Black);
        }
    }

    public sealed class RBTreeSet<E> : IEnumerable<E>
    {
        private static readonly object Present = new object();

        private readonly MyTreeMap<E, object> _map;

        private readonly bool _hasLowerBound;
        private readonly E _lowerBound;
        private readonly bool _lowerInclusive;

        private readonly bool _hasUpperBound;
        private readonly E _upperBound;
        private readonly bool _upperInclusive;

        private readonly bool _descending;

        public RBTreeSet()
        {
            _map = new MyTreeMap<E, object>();
            _hasLowerBound = false;
            _lowerBound = default(E);
            _lowerInclusive = true;
            _hasUpperBound = false;
            _upperBound = default(E);
            _upperInclusive = false;
            _descending = false;
        }

        public RBTreeSet(MyTreeMap<E, object> map)
            : this(map, false, default(E), true, false, default(E), false, false)
        {
        }

        public RBTreeSet(IComparer<E> comparer)
        {
            _map = new MyTreeMap<E, object>(comparer);
            _hasLowerBound = false;
            _lowerBound = default(E);
            _lowerInclusive = true;
            _hasUpperBound = false;
            _upperBound = default(E);
            _upperInclusive = false;
            _descending = false;
        }

        public RBTreeSet(E[] array)
            : this()
        {
            AddAll(array);
        }

        private RBTreeSet(
            MyTreeMap<E, object> map,
            bool hasLowerBound,
            E lowerBound,
            bool lowerInclusive,
            bool hasUpperBound,
            E upperBound,
            bool upperInclusive,
            bool descending)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _hasLowerBound = hasLowerBound;
            _lowerBound = lowerBound;
            _lowerInclusive = lowerInclusive;
            _hasUpperBound = hasUpperBound;
            _upperBound = upperBound;
            _upperInclusive = upperInclusive;
            _descending = descending;
        }

        private int Compare(E a, E b)
        {
            return _map.CompareKeys(a, b);
        }

        private bool TooLow(E e)
        {
            if (!_hasLowerBound) return false;
            int c = Compare(e, _lowerBound);
            return c < 0 || (c == 0 && !_lowerInclusive);
        }

        private bool TooHigh(E e)
        {
            if (!_hasUpperBound) return false;
            int c = Compare(e, _upperBound);
            return c > 0 || (c == 0 && !_upperInclusive);
        }

        private bool InRange(E e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e), "Null elements are not supported");
            return !TooLow(e) && !TooHigh(e);
        }

        private MyTreeMap<E, object>.Entry FirstAscendingEntryInRange()
        {
            MyTreeMap<E, object>.Entry candidate;

            if (!_hasLowerBound)
            {
                candidate = _map.FirstEntry();
            }
            else
            {
                candidate = _lowerInclusive
                    ? _map.GetCeilingEntry(_lowerBound)
                    : _map.GetHigherEntry(_lowerBound);
            }

            if (candidate == null || TooHigh(candidate.Key))
                return null;

            return candidate;
        }

        private MyTreeMap<E, object>.Entry LastAscendingEntryInRange()
        {
            MyTreeMap<E, object>.Entry candidate;

            if (!_hasUpperBound)
            {
                candidate = _map.LastEntry();
            }
            else
            {
                candidate = _upperInclusive
                    ? _map.GetFloorEntry(_upperBound)
                    : _map.GetLowerEntry(_upperBound);
            }

            if (candidate == null || TooLow(candidate.Key))
                return null;

            return candidate;
        }

        private MyTreeMap<E, object>.Entry NextAscendingEntry(E e)
        {
            var candidate = _map.GetHigherEntry(e);
            if (candidate == null || TooHigh(candidate.Key))
                return null;
            return candidate;
        }

        private MyTreeMap<E, object>.Entry NextDescendingEntry(E e)
        {
            var candidate = _map.GetLowerEntry(e);
            if (candidate == null || TooLow(candidate.Key))
                return null;
            return candidate;
        }

        private E FirstAscendingInRange()
        {
            var entry = FirstAscendingEntryInRange();
            return entry == null ? default(E) : entry.Key;
        }

        private E LastAscendingInRange()
        {
            var entry = LastAscendingEntryInRange();
            return entry == null ? default(E) : entry.Key;
        }

        private bool TryGetFirstByViewOrder(out E value)
        {
            var entry = _descending ? LastAscendingEntryInRange() : FirstAscendingEntryInRange();
            if (entry == null)
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        private bool TryGetLastByViewOrder(out E value)
        {
            var entry = _descending ? FirstAscendingEntryInRange() : LastAscendingEntryInRange();
            if (entry == null)
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        private bool TryGetCeiling(E obj, out E value)
        {
            MyTreeMap<E, object>.Entry entry = _descending
                ? _map.GetFloorEntry(obj)
                : _map.GetCeilingEntry(obj);

            if (entry == null || !InRange(entry.Key))
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        private bool TryGetFloor(E obj, out E value)
        {
            MyTreeMap<E, object>.Entry entry = _descending
                ? _map.GetCeilingEntry(obj)
                : _map.GetFloorEntry(obj);

            if (entry == null || !InRange(entry.Key))
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        private bool TryGetHigher(E obj, out E value)
        {
            MyTreeMap<E, object>.Entry entry = _descending
                ? _map.GetLowerEntry(obj)
                : _map.GetHigherEntry(obj);

            if (entry == null || !InRange(entry.Key))
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        private bool TryGetLower(E obj, out E value)
        {
            MyTreeMap<E, object>.Entry entry = _descending
                ? _map.GetHigherEntry(obj)
                : _map.GetLowerEntry(obj);

            if (entry == null || !InRange(entry.Key))
            {
                value = default(E);
                return false;
            }

            value = entry.Key;
            return true;
        }

        public bool Add(E e)
        {
            if (!InRange(e))
                throw new ArgumentException("Element is out of view range: " + e);

            return Equals(_map.Put(e, Present), null);
        }

        public bool AddAll(E[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "Array must not be null");

            bool changed = false;
            foreach (E e in array)
            {
                changed |= Add(e);
            }

            return changed;
        }

        public void Clear()
        {
            if (IsWholeMapView())
            {
                _map.Clear();
                return;
            }

            List<E> keys = new List<E>();
            foreach (E e in this)
                keys.Add(e);

            foreach (E e in keys)
                _map.Remove(e);
        }

        public bool Contains(object obj)
        {
            try
            {
                E e = (E)obj;
                return e != null && InRange(e) && _map.ContainsKey(e);
            }
            catch
            {
                return false;
            }
        }

        public bool ContainsAll(E[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "Array must not be null");

            foreach (E e in array)
            {
                if (!Contains(e)) return false;
            }

            return true;
        }

        public bool IsEmpty()
        {
            E value;
            return !TryGetFirstByViewOrder(out value);
        }

        public bool Remove(object obj)
        {
            try
            {
                E e = (E)obj;
                return e != null && InRange(e) && !Equals(_map.Remove(e), null);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveAll(E[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "Array must not be null");

            bool changed = false;
            foreach (E e in array)
            {
                changed |= Remove(e);
            }

            return changed;
        }

        public bool RetainAll(E[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "Array must not be null");

            HashSet<E> keep = new HashSet<E>(array);
            List<E> toDelete = new List<E>();

            foreach (E e in this)
            {
                if (!keep.Contains(e))
                    toDelete.Add(e);
            }

            foreach (E e in toDelete)
            {
                _map.Remove(e);
            }

            return toDelete.Count > 0;
        }

        public int Size()
        {
            if (IsWholeMapView()) return _map.Size();

            int count = 0;
            foreach (var _ in this) count++;
            return count;
        }

        public object[] ToArray()
        {
            List<E> list = new List<E>();
            foreach (E e in this) list.Add(e);
            return list.Cast<object>().ToArray();
        }

        public T[] ToArray<T>(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            List<E> list = new List<E>();
            foreach (E e in this) list.Add(e);

            if (array.Length < list.Count)
            {
                T[] result = new T[list.Count];
                for (int i = 0; i < list.Count; i++)
                    result[i] = (T)(object)list[i];
                return result;
            }

            for (int i = 0; i < list.Count; i++)
                array[i] = (T)(object)list[i];

            if (array.Length > list.Count)
                array[list.Count] = default(T);

            return array;
        }

        public E First()
        {
            E result;
            if (!TryGetFirstByViewOrder(out result))
                throw new InvalidOperationException("Set is empty");
            return result;
        }

        public E Last()
        {
            E result;
            if (!TryGetLastByViewOrder(out result))
                throw new InvalidOperationException("Set is empty");
            return result;
        }

        public RBTreeSet<E> SubSet(E fromElement, E toElement)
        {
            return SubSet(fromElement, true, toElement, false);
        }

        public RBTreeSet<E> HeadSet(E toElement)
        {
            return HeadSet(toElement, false);
        }

        public RBTreeSet<E> TailSet(E fromElement)
        {
            return TailSet(fromElement, true);
        }

        public E Ceiling(E obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Null elements are not supported");
            E value;
            return TryGetCeiling(obj, out value) ? value : default(E);
        }

        public E Floor(E obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Null elements are not supported");
            E value;
            return TryGetFloor(obj, out value) ? value : default(E);
        }

        public E Higher(E obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Null elements are not supported");
            E value;
            return TryGetHigher(obj, out value) ? value : default(E);
        }

        public E Lower(E obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Null elements are not supported");
            E value;
            return TryGetLower(obj, out value) ? value : default(E);
        }

        public RBTreeSet<E> HeadSet(E upperBound, bool inclusive)
        {
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));
            return Intersect(false, default(E), true, true, upperBound, inclusive, _descending);
        }

        public RBTreeSet<E> SubSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
        {
            if (lowerBound == null) throw new ArgumentNullException(nameof(lowerBound));
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));

            int cmp = Compare(lowerBound, upperBound);
            if (cmp > 0)
                throw new ArgumentException("fromElement > toElement");

            return Intersect(true, lowerBound, lowIncl, true, upperBound, highIncl, _descending);
        }

        public RBTreeSet<E> TailSet(E fromElement, bool inclusive)
        {
            if (fromElement == null) throw new ArgumentNullException(nameof(fromElement));
            return Intersect(true, fromElement, inclusive, false, default(E), true, _descending);
        }

        public E PollLast()
        {
            E e;
            if (TryGetLastByViewOrder(out e))
            {
                _map.Remove(e);
                return e;
            }

            return default(E);
        }

        public E PollFirst()
        {
            E e;
            if (TryGetFirstByViewOrder(out e))
            {
                _map.Remove(e);
                return e;
            }

            return default(E);
        }

        public IEnumerable<E> DescendingIterator()
        {
            return Iterate(!_descending);
        }

        public RBTreeSet<E> DescendingSet()
        {
            return new RBTreeSet<E>(
                _map,
                _hasLowerBound, _lowerBound, _lowerInclusive,
                _hasUpperBound, _upperBound, _upperInclusive,
                !_descending
            );
        }

        public IEnumerator<E> GetEnumerator()
        {
            return Iterate(_descending).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<E> Iterate(bool iterateDescending)
        {
            MyTreeMap<E, object>.Entry next =
                iterateDescending ? LastAscendingEntryInRange() : FirstAscendingEntryInRange();

            while (next != null)
            {
                E result = next.Key;
                next = iterateDescending ? NextDescendingEntry(result) : NextAscendingEntry(result);
                yield return result;
            }
        }

        private bool IsWholeMapView()
        {
            return !_hasLowerBound && !_hasUpperBound;
        }

        private RBTreeSet<E> Intersect(
            bool addLower, E newLower, bool newLowerIncl,
            bool addUpper, E newUpper, bool newUpperIncl,
            bool newDescending)
        {
            bool resHasLower = _hasLowerBound;
            E resLower = _lowerBound;
            bool resLowerIncl = _lowerInclusive;

            bool resHasUpper = _hasUpperBound;
            E resUpper = _upperBound;
            bool resUpperIncl = _upperInclusive;

            if (addLower)
            {
                if (!resHasLower)
                {
                    resHasLower = true;
                    resLower = newLower;
                    resLowerIncl = newLowerIncl;
                }
                else
                {
                    int c = Compare(newLower, resLower);
                    if (c > 0)
                    {
                        resLower = newLower;
                        resLowerIncl = newLowerIncl;
                    }
                    else if (c == 0)
                    {
                        resLowerIncl = resLowerIncl && newLowerIncl;
                    }
                }
            }

            if (addUpper)
            {
                if (!resHasUpper)
                {
                    resHasUpper = true;
                    resUpper = newUpper;
                    resUpperIncl = newUpperIncl;
                }
                else
                {
                    int c = Compare(newUpper, resUpper);
                    if (c < 0)
                    {
                        resUpper = newUpper;
                        resUpperIncl = newUpperIncl;
                    }
                    else if (c == 0)
                    {
                        resUpperIncl = resUpperIncl && newUpperIncl;
                    }
                }
            }

            if (resHasLower && resHasUpper)
            {
                int c = Compare(resLower, resUpper);
                if (c > 0)
                    throw new ArgumentException("Empty/inverted range after intersection");
            }

            return new RBTreeSet<E>(
                _map,
                resHasLower, resLower, resLowerIncl,
                resHasUpper, resUpper, resUpperIncl,
                newDescending
            );
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", this) + "]";
        }
    }
}