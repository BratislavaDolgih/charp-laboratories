
using System;
using System.Collections.Generic;

namespace Task29Interfaces
{
    public class MyException : Exception
    {
        public MyException(string message) : base(message) { }
    }

    public class MyNullArgumentException : MyException
    {
        public MyNullArgumentException(string message) : base(message) { }
    }

    public class MyIndexOutOfBoundsException : MyException
    {
        public MyIndexOutOfBoundsException(string message) : base(message) { }
    }

    public class MyNoSuchElementException : MyException
    {
        public MyNoSuchElementException(string message) : base(message) { }
    }

    public class MyIllegalStateException : MyException
    {
        public MyIllegalStateException(string message) : base(message) { }
    }

    public class MyUnsupportedOperationException : MyException
    {
        public MyUnsupportedOperationException(string message) : base(message) { }
    }

    public delegate int MyComparator<T>(T a, T b);

    public interface MyIterator<T>
    {
        bool hasNext();
        T next();
        void remove();
    }

    public interface MyListIterator<T> : MyIterator<T>
    {
        bool hasPrevious();
        T previous();
        int nextIndex();
        int previousIndex();
        void set(T element);
        void add(T element);
    }

    public interface MyCollection<T>
    {
        bool add(T e);
        bool addAll(MyCollection<T> c);
        void clear();
        bool contains(object o);
        bool containsAll(MyCollection<T> c);
        bool isEmpty();
        bool remove(object o);
        bool removeAll(MyCollection<T> c);
        bool retainAll(MyCollection<T> c);
        int size();
        object[] toArray();
        T[] toArray(T[] a);
    }

    public interface MyList<T> : MyCollection<T>
    {
        void add(int index, T e);
        bool addAll(int index, MyCollection<T> c);
        T get(int index);
        int indexOf(object o);
        int lastIndexOf(object o);
        MyListIterator<T> listIterator();
        MyListIterator<T> listIterator(int index);
        T remove(int index);
        T set(int index, T e);
        MyList<T> subList(int fromIndex, int toIndex);
    }

    public interface MyQueue<T> : MyCollection<T>
    {
        T element();
        bool offer(T obj);
        T peek();
        T poll();
    }

    public interface MyDeque<T> : MyCollection<T>
    {
        void addFirst(T obj);
        void addLast(T obj);
        T getFirst();
        T getLast();
        bool offerFirst(T obj);
        bool offerLast(T obj);
        T pop();
        void push(T obj);
        T peekFirst();
        T peekLast();
        T pollFirst();
        T pollLast();
        T removeLast();
        T removeFirst();
        bool removeLastOccurrence(object obj);
        bool removeFirstOccurrence(object obj);
    }

    public interface MySet<T> : MyCollection<T>
    {
        T first();
        T last();
        MySet<T> subSet(T fromElement, T toElement);
        MySet<T> headSet(T toElement);
        MySet<T> tailSet(T fromElement);
    }

    public interface MySortedSet<T> : MySet<T>
    {
    }

    public interface MyNavigableSet<T> : MySortedSet<T>
    {
        T lower(T obj);
        T floor(T obj);
        T higher(T obj);
        T ceiling(T obj);
        MySet<T> headSet(T upperBound, bool incl);
        MySet<T> subSet(T lowerBound, bool lowIncl, T upperBound, bool highIncl);
        MySet<T> tailSet(T fromElement, bool inclusive);
        T pollLast();
        T pollFirst();
        MyIterator<T> descendingIterator();
        MyNavigableSet<T> descendingSet();
    }

    public sealed class MyMapEntry<K, V>
    {
        public K Key { get; private set; }
        public V Value { get; set; }

        public MyMapEntry(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }

    public interface MyMap<K, V>
    {
        void clear();
        bool containsKey(object key);
        bool containsValue(object value);
        MyCollection<MyMapEntry<K, V>> entrySet();
        V get(object key);
        bool isEmpty();
        MySet<K> keySet();
        V put(K key, V value);
        void putAll(MyMap<K, V> m);
        V remove(object key);
        int size();
        MyCollection<V> values();
    }

    public interface MySortedMap<K, V> : MyMap<K, V>
    {
        K firstKey();
        K lastKey();
        MySortedMap<K, V> headMap(K end);
        MySortedMap<K, V> subMap(K start, K end);
        MySortedMap<K, V> tailMap(K start);
    }

    public interface MyNavigableMap<K, V> : MySortedMap<K, V>
    {
        MyMapEntry<K, V> lowerEntry(K key);
        MyMapEntry<K, V> floorEntry(K key);
        MyMapEntry<K, V> higherEntry(K key);
        MyMapEntry<K, V> ceilingEntry(K key);
        K lowerKey(K key);
        K floorKey(K key);
        K higherKey(K key);
        K ceilingKey(K key);
        MyMapEntry<K, V> pollFirstEntry();
        MyMapEntry<K, V> pollLastEntry();
        MyMapEntry<K, V> firstEntry();
        MyMapEntry<K, V> lastEntry();
    }

    public class MyArrayList<T> : MyList<T>
    {
        protected T[] items;
        protected int count;

        public MyArrayList()
        {
            items = new T[10];
            count = 0;
        }

        public MyArrayList(MyCollection<T> c)
            : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        protected void ensureCapacity(int capacity)
        {
            if (capacity <= items.Length) return;
            int newCapacity = items.Length == 0 ? 10 : items.Length * 2;
            while (newCapacity < capacity) newCapacity *= 2;
            T[] arr = new T[newCapacity];
            Array.Copy(items, arr, count);
            items = arr;
        }

        protected void checkIndex(int index)
        {
            if (index < 0 || index >= count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
        }

        protected void checkIndexForAdd(int index)
        {
            if (index < 0 || index > count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
        }

        public virtual bool add(T e)
        {
            ensureCapacity(count + 1);
            items[count++] = e;
            return true;
        }

        public virtual void add(int index, T e)
        {
            checkIndexForAdd(index);
            ensureCapacity(count + 1);
            Array.Copy(items, index, items, index + 1, count - index);
            items[index] = e;
            count++;
        }

        public virtual bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            ensureCapacity(count + arr.Length);
            Array.Copy(arr, 0, items, count, arr.Length);
            count += arr.Length;
            return true;
        }

        public virtual bool addAll(int index, MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            checkIndexForAdd(index);
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            ensureCapacity(count + arr.Length);
            Array.Copy(items, index, items, index + arr.Length, count - index);
            Array.Copy(arr, 0, items, index, arr.Length);
            count += arr.Length;
            return true;
        }

        public virtual void clear()
        {
            for (int i = 0; i < count; i++) items[i] = default(T);
            count = 0;
        }

        public virtual bool contains(object o)
        {
            return indexOf(o) >= 0;
        }

        public virtual bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                if (!contains(arr[i]))
                    return false;
            return true;
        }

        public virtual T get(int index)
        {
            checkIndex(index);
            return items[index];
        }

        public virtual int indexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
                if (comparer.Equals(items[i], (o is T) ? (T)o : default(T)) && (o is T || o == null))
                    return i;
            return -1;
        }

        public virtual bool isEmpty()
        {
            return count == 0;
        }

        public virtual int lastIndexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = count - 1; i >= 0; i--)
                if (comparer.Equals(items[i], (o is T) ? (T)o : default(T)) && (o is T || o == null))
                    return i;
            return -1;
        }

        public virtual MyListIterator<T> listIterator()
        {
            return new MyItr(this, 0);
        }

        public virtual MyListIterator<T> listIterator(int index)
        {
            checkIndexForAdd(index);
            return new MyItr(this, index);
        }

        public virtual bool remove(object o)
        {
            int idx = indexOf(o);
            if (idx < 0) return false;
            remove(idx);
            return true;
        }

        public virtual T remove(int index)
        {
            checkIndex(index);
            T old = items[index];
            Array.Copy(items, index + 1, items, index, count - index - 1);
            items[--count] = default(T);
            return old;
        }

        public virtual bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                while (remove(arr[i]))
                    changed = true;
            return changed;
        }

        public virtual bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            int write = 0;
            for (int i = 0; i < count; i++)
            {
                if (c.contains(items[i]))
                {
                    items[write++] = items[i];
                }
                else
                {
                    changed = true;
                }
            }
            for (int i = write; i < count; i++) items[i] = default(T);
            count = write;
            return changed;
        }

        public virtual T set(int index, T e)
        {
            checkIndex(index);
            T old = items[index];
            items[index] = e;
            return old;
        }

        public virtual int size()
        {
            return count;
        }

        public virtual MyList<T> subList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex < fromIndex || toIndex > count)
                throw new MyIndexOutOfBoundsException("Index out of bounds.");
            MyArrayList<T> result = new MyArrayList<T>();
            for (int i = fromIndex; i < toIndex; i++)
                result.add(items[i]);
            return result;
        }

        public virtual object[] toArray()
        {
            object[] arr = new object[count];
            for (int i = 0; i < count; i++) arr[i] = items[i];
            return arr;
        }

        public virtual T[] toArray(T[] a)
        {
            if (a == null || a.Length < count)
                a = new T[count];
            Array.Copy(items, a, count);
            return a;
        }

        protected class MyItr : MyListIterator<T>
        {
            private readonly MyArrayList<T> list;
            protected int cursor;
            protected int lastRet;

            public MyItr(MyArrayList<T> list, int index)
            {
                this.list = list;
                cursor = index;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < list.count;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return list.items[cursor++];
            }

            public bool hasPrevious()
            {
                return cursor > 0;
            }

            public T previous()
            {
                if (!hasPrevious()) throw new MyNoSuchElementException("No previous element.");
                lastRet = --cursor;
                return list.items[cursor];
            }

            public int nextIndex()
            {
                return cursor;
            }

            public int previousIndex()
            {
                return cursor - 1;
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                list.remove(lastRet);
                if (lastRet < cursor) cursor--;
                lastRet = -1;
            }

            public void set(T element)
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                list.set(lastRet, element);
            }

            public void add(T element)
            {
                list.add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyVector<T> : MyArrayList<T>
    {
        private readonly object syncRoot = new object();

        public MyVector() : base() { }

        public MyVector(MyCollection<T> c) : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        public override bool add(T e)
        {
            lock (syncRoot) return base.add(e);
        }

        public override void add(int index, T e)
        {
            lock (syncRoot) base.add(index, e);
        }

        public override bool addAll(MyCollection<T> c)
        {
            lock (syncRoot) return base.addAll(c);
        }

        public override bool addAll(int index, MyCollection<T> c)
        {
            lock (syncRoot) return base.addAll(index, c);
        }

        public override void clear()
        {
            lock (syncRoot) base.clear();
        }

        public override bool contains(object o)
        {
            lock (syncRoot) return base.contains(o);
        }

        public override bool containsAll(MyCollection<T> c)
        {
            lock (syncRoot) return base.containsAll(c);
        }

        public override T get(int index)
        {
            lock (syncRoot) return base.get(index);
        }

        public override int indexOf(object o)
        {
            lock (syncRoot) return base.indexOf(o);
        }

        public override bool isEmpty()
        {
            lock (syncRoot) return base.isEmpty();
        }

        public override int lastIndexOf(object o)
        {
            lock (syncRoot) return base.lastIndexOf(o);
        }

        public override bool remove(object o)
        {
            lock (syncRoot) return base.remove(o);
        }

        public override T remove(int index)
        {
            lock (syncRoot) return base.remove(index);
        }

        public override bool removeAll(MyCollection<T> c)
        {
            lock (syncRoot) return base.removeAll(c);
        }

        public override bool retainAll(MyCollection<T> c)
        {
            lock (syncRoot) return base.retainAll(c);
        }

        public override T set(int index, T e)
        {
            lock (syncRoot) return base.set(index, e);
        }

        public override int size()
        {
            lock (syncRoot) return base.size();
        }

        public override MyList<T> subList(int fromIndex, int toIndex)
        {
            lock (syncRoot) return base.subList(fromIndex, toIndex);
        }

        public override object[] toArray()
        {
            lock (syncRoot) return base.toArray();
        }

        public override T[] toArray(T[] a)
        {
            lock (syncRoot) return base.toArray(a);
        }

        public override MyListIterator<T> listIterator()
        {
            lock (syncRoot) return new VectorItr(this, 0);
        }

        public override MyListIterator<T> listIterator(int index)
        {
            lock (syncRoot)
            {
                checkIndexForAdd(index);
                return new VectorItr(this, index);
            }
        }

        private sealed class VectorItr : MyItr
        {
            private readonly MyVector<T> vector;

            public VectorItr(MyVector<T> vector, int index) : base(vector, index)
            {
                this.vector = vector;
            }

            public new bool hasNext()
            {
                lock (vector.syncRoot) return base.hasNext();
            }

            public new T next()
            {
                lock (vector.syncRoot) return base.next();
            }

            public new bool hasPrevious()
            {
                lock (vector.syncRoot) return base.hasPrevious();
            }

            public new T previous()
            {
                lock (vector.syncRoot) return base.previous();
            }

            public new int nextIndex()
            {
                lock (vector.syncRoot) return base.nextIndex();
            }

            public new int previousIndex()
            {
                lock (vector.syncRoot) return base.previousIndex();
            }

            public new void remove()
            {
                lock (vector.syncRoot) base.remove();
            }

            public new void set(T element)
            {
                lock (vector.syncRoot) base.set(element);
            }

            public new void add(T element)
            {
                lock (vector.syncRoot) base.add(element);
            }
        }
    }

    public class MyLinkedList<T> : MyList<T>
    {
        private sealed class Node
        {
            public T Value;
            public Node Prev;
            public Node Next;
            public Node(T value) { Value = value; }
        }

        private Node head;
        private Node tail;
        private int count;

        public MyLinkedList() { }

        public MyLinkedList(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        private Node getNode(int index)
        {
            if (index < 0 || index >= count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
            Node cur;
            if (index < count / 2)
            {
                cur = head;
                for (int i = 0; i < index; i++) cur = cur.Next;
            }
            else
            {
                cur = tail;
                for (int i = count - 1; i > index; i--) cur = cur.Prev;
            }
            return cur;
        }

        private void insertBefore(Node node, Node newNode)
        {
            if (node == null)
            {
                if (tail == null)
                {
                    head = tail = newNode;
                }
                else
                {
                    tail.Next = newNode;
                    newNode.Prev = tail;
                    tail = newNode;
                }
            }
            else
            {
                newNode.Next = node;
                newNode.Prev = node.Prev;
                if (node.Prev != null) node.Prev.Next = newNode; else head = newNode;
                node.Prev = newNode;
            }
            count++;
        }

        public bool add(T e)
        {
            insertBefore(null, new Node(e));
            return true;
        }

        public void add(int index, T e)
        {
            if (index < 0 || index > count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
            if (index == count) insertBefore(null, new Node(e));
            else insertBefore(getNode(index), new Node(e));
        }

        public bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            for (int i = 0; i < arr.Length; i++) add(arr[i]);
            return true;
        }

        public bool addAll(int index, MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            if (index < 0 || index > count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            for (int i = 0; i < arr.Length; i++) add(index + i, arr[i]);
            return true;
        }

        public void clear()
        {
            head = tail = null;
            count = 0;
        }

        public bool contains(object o)
        {
            return indexOf(o) >= 0;
        }

        public bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (!contains(arr[i])) return false;
            return true;
        }

        public T get(int index)
        {
            return getNode(index).Value;
        }

        public int indexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            Node cur = head;
            int idx = 0;
            while (cur != null)
            {
                if ((o is T || o == null) && comparer.Equals(cur.Value, (o is T) ? (T)o : default(T)))
                    return idx;
                cur = cur.Next;
                idx++;
            }
            return -1;
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public int lastIndexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            Node cur = tail;
            int idx = count - 1;
            while (cur != null)
            {
                if ((o is T || o == null) && comparer.Equals(cur.Value, (o is T) ? (T)o : default(T)))
                    return idx;
                cur = cur.Prev;
                idx--;
            }
            return -1;
        }

        public MyListIterator<T> listIterator()
        {
            return new MyItr(this, 0);
        }

        public MyListIterator<T> listIterator(int index)
        {
            if (index < 0 || index > count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
            return new MyItr(this, index);
        }

        public bool remove(object o)
        {
            int idx = indexOf(o);
            if (idx < 0) return false;
            remove(idx);
            return true;
        }

        public T remove(int index)
        {
            Node node = getNode(index);
            if (node.Prev != null) node.Prev.Next = node.Next; else head = node.Next;
            if (node.Next != null) node.Next.Prev = node.Prev; else tail = node.Prev;
            count--;
            return node.Value;
        }

        public bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                while (remove(arr[i]))
                    changed = true;
            return changed;
        }

        public bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            Node cur = head;
            while (cur != null)
            {
                Node next = cur.Next;
                if (!c.contains(cur.Value))
                {
                    if (cur.Prev != null) cur.Prev.Next = cur.Next; else head = cur.Next;
                    if (cur.Next != null) cur.Next.Prev = cur.Prev; else tail = cur.Prev;
                    count--;
                    changed = true;
                }
                cur = next;
            }
            return changed;
        }

        public T set(int index, T e)
        {
            Node node = getNode(index);
            T old = node.Value;
            node.Value = e;
            return old;
        }

        public int size()
        {
            return count;
        }

        public MyList<T> subList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex < fromIndex || toIndex > count)
                throw new MyIndexOutOfBoundsException("Index out of bounds.");
            MyLinkedList<T> result = new MyLinkedList<T>();
            Node cur = getNode(fromIndex);
            for (int i = fromIndex; i < toIndex; i++)
            {
                result.add(cur.Value);
                cur = cur.Next;
            }
            return result;
        }

        public object[] toArray()
        {
            object[] arr = new object[count];
            Node cur = head;
            int i = 0;
            while (cur != null)
            {
                arr[i++] = cur.Value;
                cur = cur.Next;
            }
            return arr;
        }

        public T[] toArray(T[] a)
        {
            if (a == null || a.Length < count) a = new T[count];
            Node cur = head;
            int i = 0;
            while (cur != null)
            {
                a[i++] = cur.Value;
                cur = cur.Next;
            }
            return a;
        }

        private sealed class MyItr : MyListIterator<T>
        {
            private readonly MyLinkedList<T> list;
            private int cursor;
            private int lastRet;

            public MyItr(MyLinkedList<T> list, int index)
            {
                this.list = list;
                cursor = index;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < list.count;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return list.get(cursor++);
            }

            public bool hasPrevious()
            {
                return cursor > 0;
            }

            public T previous()
            {
                if (!hasPrevious()) throw new MyNoSuchElementException("No previous element.");
                lastRet = --cursor;
                return list.get(cursor);
            }

            public int nextIndex()
            {
                return cursor;
            }

            public int previousIndex()
            {
                return cursor - 1;
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                list.remove(lastRet);
                if (lastRet < cursor) cursor--;
                lastRet = -1;
            }

            public void set(T element)
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                list.set(lastRet, element);
            }

            public void add(T element)
            {
                list.add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyPriorityQueue<T> : MyQueue<T>
    {
        private T[] heap;
        private int count;
        private readonly Comparer<T> comparer;

        public MyPriorityQueue()
        {
            heap = new T[10];
            count = 0;
            comparer = Comparer<T>.Default;
        }

        public MyPriorityQueue(MyCollection<T> c) : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        private void ensureCapacity(int capacity)
        {
            if (capacity <= heap.Length) return;
            int newCap = heap.Length * 2;
            while (newCap < capacity) newCap *= 2;
            T[] arr = new T[newCap];
            Array.Copy(heap, arr, count);
            heap = arr;
        }

        private void swap(int i, int j)
        {
            T t = heap[i];
            heap[i] = heap[j];
            heap[j] = t;
        }

        private void siftUp(int index)
        {
            while (index > 0)
            {
                int p = (index - 1) / 2;
                if (comparer.Compare(heap[p], heap[index]) <= 0) break;
                swap(p, index);
                index = p;
            }
        }

        private void siftDown(int index)
        {
            while (true)
            {
                int l = index * 2 + 1;
                int r = l + 1;
                int s = index;
                if (l < count && comparer.Compare(heap[l], heap[s]) < 0) s = l;
                if (r < count && comparer.Compare(heap[r], heap[s]) < 0) s = r;
                if (s == index) break;
                swap(index, s);
                index = s;
            }
        }

        public bool add(T e)
        {
            return offer(e);
        }

        public bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            for (int i = 0; i < arr.Length; i++) offer(arr[i]);
            return true;
        }

        public void clear()
        {
            for (int i = 0; i < count; i++) heap[i] = default(T);
            count = 0;
        }

        public bool contains(object o)
        {
            var eq = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
                if ((o is T || o == null) && eq.Equals(heap[i], (o is T) ? (T)o : default(T)))
                    return true;
            return false;
        }

        public bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (!contains(arr[i])) return false;
            return true;
        }

        public T element()
        {
            if (count == 0) throw new MyNoSuchElementException("Queue is empty.");
            return heap[0];
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public bool offer(T obj)
        {
            ensureCapacity(count + 1);
            heap[count] = obj;
            siftUp(count);
            count++;
            return true;
        }

        public T peek()
        {
            return count == 0 ? default(T) : heap[0];
        }

        public T poll()
        {
            if (count == 0) return default(T);
            T res = heap[0];
            count--;
            heap[0] = heap[count];
            heap[count] = default(T);
            if (count > 0) siftDown(0);
            return res;
        }

        public bool remove(object o)
        {
            var eq = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
            {
                if ((o is T || o == null) && eq.Equals(heap[i], (o is T) ? (T)o : default(T)))
                {
                    count--;
                    heap[i] = heap[count];
                    heap[count] = default(T);
                    if (i < count)
                    {
                        siftDown(i);
                        siftUp(i);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                while (remove(arr[i]))
                    changed = true;
            return changed;
        }

        public bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            for (int i = 0; i < count;)
            {
                if (!c.contains(heap[i]))
                {
                    count--;
                    heap[i] = heap[count];
                    heap[count] = default(T);
                    if (i < count)
                    {
                        siftDown(i);
                        siftUp(i);
                    }
                    changed = true;
                }
                else
                {
                    i++;
                }
            }
            return changed;
        }

        public int size()
        {
            return count;
        }

        public object[] toArray()
        {
            object[] arr = new object[count];
            for (int i = 0; i < count; i++) arr[i] = heap[i];
            return arr;
        }

        public T[] toArray(T[] a)
        {
            if (a == null || a.Length < count) a = new T[count];
            Array.Copy(heap, a, count);
            return a;
        }

        public MyIterator<T> iterator()
        {
            return new PQItr(this);
        }

        private sealed class PQItr : MyIterator<T>
        {
            private readonly MyPriorityQueue<T> queue;
            private readonly T[] snapshot;
            private int cursor;
            private int lastRet;

            public PQItr(MyPriorityQueue<T> queue)
            {
                this.queue = queue;
                snapshot = queue.toArray((T[])null);
                cursor = 0;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < snapshot.Length;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                queue.remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    public class MyArrayDeque<T> : MyList<T>, MyDeque<T>
    {
        private T[] items;
        private int head;
        private int count;

        public MyArrayDeque()
        {
            items = new T[10];
            head = 0;
            count = 0;
        }

        public MyArrayDeque(MyCollection<T> c) : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        private void ensureCapacity(int capacity)
        {
            if (capacity <= items.Length) return;
            int newCap = items.Length * 2;
            while (newCap < capacity) newCap *= 2;
            T[] arr = new T[newCap];
            for (int i = 0; i < count; i++) arr[i] = get(i);
            items = arr;
            head = 0;
        }

        private int physicalIndex(int logicalIndex)
        {
            return (head + logicalIndex) % items.Length;
        }

        private void checkIndex(int index)
        {
            if (index < 0 || index >= count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
        }

        private void checkIndexForAdd(int index)
        {
            if (index < 0 || index > count) throw new MyIndexOutOfBoundsException("Index out of bounds.");
        }

        public bool add(T e)
        {
            addLast(e);
            return true;
        }

        public void add(int index, T e)
        {
            checkIndexForAdd(index);
            if (index == 0)
            {
                addFirst(e);
                return;
            }
            if (index == count)
            {
                addLast(e);
                return;
            }

            ensureCapacity(count + 1);
            if (index < count / 2)
            {
                head = (head - 1 + items.Length) % items.Length;
                for (int i = 0; i < index; i++)
                    items[physicalIndex(i)] = items[physicalIndex(i + 1)];
                items[physicalIndex(index)] = e;
            }
            else
            {
                for (int i = count; i > index; i--)
                    items[physicalIndex(i)] = items[physicalIndex(i - 1)];
                items[physicalIndex(index)] = e;
            }
            count++;
        }

        public bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            for (int i = 0; i < arr.Length; i++) addLast(arr[i]);
            return true;
        }

        public bool addAll(int index, MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            checkIndexForAdd(index);
            T[] arr = c.toArray((T[])null);
            if (arr.Length == 0) return false;
            for (int i = 0; i < arr.Length; i++) add(index + i, arr[i]);
            return true;
        }

        public void addFirst(T obj)
        {
            ensureCapacity(count + 1);
            head = (head - 1 + items.Length) % items.Length;
            items[head] = obj;
            count++;
        }

        public void addLast(T obj)
        {
            ensureCapacity(count + 1);
            items[physicalIndex(count)] = obj;
            count++;
        }

        public void clear()
        {
            for (int i = 0; i < count; i++) items[physicalIndex(i)] = default(T);
            head = 0;
            count = 0;
        }

        public bool contains(object o)
        {
            return indexOf(o) >= 0;
        }

        public bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (!contains(arr[i])) return false;
            return true;
        }

        public T get(int index)
        {
            checkIndex(index);
            return items[physicalIndex(index)];
        }

        public T getFirst()
        {
            if (count == 0) throw new MyNoSuchElementException("Deque is empty.");
            return items[head];
        }

        public T getLast()
        {
            if (count == 0) throw new MyNoSuchElementException("Deque is empty.");
            return items[physicalIndex(count - 1)];
        }

        public int indexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
                if ((o is T || o == null) && comparer.Equals(get(i), (o is T) ? (T)o : default(T)))
                    return i;
            return -1;
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public int lastIndexOf(object o)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = count - 1; i >= 0; i--)
                if ((o is T || o == null) && comparer.Equals(get(i), (o is T) ? (T)o : default(T)))
                    return i;
            return -1;
        }

        public MyListIterator<T> listIterator()
        {
            return new DequeItr(this, 0);
        }

        public MyListIterator<T> listIterator(int index)
        {
            checkIndexForAdd(index);
            return new DequeItr(this, index);
        }

        public bool offerFirst(T obj)
        {
            addFirst(obj);
            return true;
        }

        public bool offerLast(T obj)
        {
            addLast(obj);
            return true;
        }

        public T peekFirst()
        {
            return count == 0 ? default(T) : items[head];
        }

        public T peekLast()
        {
            return count == 0 ? default(T) : items[physicalIndex(count - 1)];
        }

        public T pollFirst()
        {
            if (count == 0) return default(T);
            return removeFirst();
        }

        public T pollLast()
        {
            if (count == 0) return default(T);
            return removeLast();
        }

        public T pop()
        {
            return removeFirst();
        }

        public void push(T obj)
        {
            addFirst(obj);
        }

        public bool remove(object o)
        {
            int idx = indexOf(o);
            if (idx < 0) return false;
            remove(idx);
            return true;
        }

        public T remove(int index)
        {
            checkIndex(index);
            T old = get(index);
            if (index < count / 2)
            {
                for (int i = index; i > 0; i--)
                    items[physicalIndex(i)] = items[physicalIndex(i - 1)];
                items[head] = default(T);
                head = (head + 1) % items.Length;
            }
            else
            {
                for (int i = index; i < count - 1; i++)
                    items[physicalIndex(i)] = items[physicalIndex(i + 1)];
                items[physicalIndex(count - 1)] = default(T);
            }
            count--;
            return old;
        }

        public T removeFirst()
        {
            if (count == 0) throw new MyNoSuchElementException("Deque is empty.");
            T old = items[head];
            items[head] = default(T);
            head = (head + 1) % items.Length;
            count--;
            return old;
        }

        public bool removeFirstOccurrence(object obj)
        {
            return remove(obj);
        }

        public T removeLast()
        {
            if (count == 0) throw new MyNoSuchElementException("Deque is empty.");
            int idx = physicalIndex(count - 1);
            T old = items[idx];
            items[idx] = default(T);
            count--;
            return old;
        }

        public bool removeLastOccurrence(object obj)
        {
            int idx = lastIndexOf(obj);
            if (idx < 0) return false;
            remove(idx);
            return true;
        }

        public bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                while (remove(arr[i]))
                    changed = true;
            return changed;
        }

        public bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            for (int i = 0; i < count;)
            {
                if (!c.contains(get(i)))
                {
                    remove(i);
                    changed = true;
                }
                else
                {
                    i++;
                }
            }
            return changed;
        }

        public T set(int index, T e)
        {
            checkIndex(index);
            int p = physicalIndex(index);
            T old = items[p];
            items[p] = e;
            return old;
        }

        public int size()
        {
            return count;
        }

        public MyList<T> subList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex < fromIndex || toIndex > count)
                throw new MyIndexOutOfBoundsException("Index out of bounds.");
            MyArrayDeque<T> result = new MyArrayDeque<T>();
            for (int i = fromIndex; i < toIndex; i++) result.addLast(get(i));
            return result;
        }

        public object[] toArray()
        {
            object[] arr = new object[count];
            for (int i = 0; i < count; i++) arr[i] = get(i);
            return arr;
        }

        public T[] toArray(T[] a)
        {
            if (a == null || a.Length < count) a = new T[count];
            for (int i = 0; i < count; i++) a[i] = get(i);
            return a;
        }

        private sealed class DequeItr : MyListIterator<T>
        {
            private readonly MyArrayDeque<T> deque;
            private int cursor;
            private int lastRet;

            public DequeItr(MyArrayDeque<T> deque, int index)
            {
                this.deque = deque;
                cursor = index;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < deque.count;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return deque.get(cursor++);
            }

            public bool hasPrevious()
            {
                return cursor > 0;
            }

            public T previous()
            {
                if (!hasPrevious()) throw new MyNoSuchElementException("No previous element.");
                lastRet = --cursor;
                return deque.get(cursor);
            }

            public int nextIndex()
            {
                return cursor;
            }

            public int previousIndex()
            {
                return cursor - 1;
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                deque.remove(lastRet);
                if (lastRet < cursor) cursor--;
                lastRet = -1;
            }

            public void set(T element)
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                deque.set(lastRet, element);
            }

            public void add(T element)
            {
                deque.add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyHashMap<K, V> : MyMap<K, V>
    {
        private sealed class Entry
        {
            public K Key;
            public V Value;
            public Entry Next;
            public Entry(K key, V value, Entry next) { Key = key; Value = value; Next = next; }
        }

        private Entry[] table;
        private int count;
        private readonly float loadFactor;

        public MyHashMap()
            : this(16, 0.75f)
        {
        }

        public MyHashMap(int initialCapacity)
            : this(initialCapacity, 0.75f)
        {
        }

        public MyHashMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity <= 0) throw new MyException("Initial capacity must be positive.");
            if (loadFactor <= 0f || float.IsNaN(loadFactor)) throw new MyException("Invalid load factor.");
            table = new Entry[initialCapacity];
            count = 0;
            this.loadFactor = loadFactor;
        }

        private int hash(object key)
        {
            return ((key == null ? 0 : key.GetHashCode()) & 0x7fffffff);
        }

        private int bucket(object key, int length)
        {
            return hash(key) % length;
        }

        private void resize(int newCapacity)
        {
            Entry[] old = table;
            table = new Entry[newCapacity];
            for (int i = 0; i < old.Length; i++)
            {
                Entry cur = old[i];
                while (cur != null)
                {
                    Entry next = cur.Next;
                    int idx = bucket(cur.Key, table.Length);
                    cur.Next = table[idx];
                    table[idx] = cur;
                    cur = next;
                }
            }
        }

        private void ensureCapacity()
        {
            if ((count + 1f) / table.Length > loadFactor)
                resize(table.Length * 2);
        }

        public void clear()
        {
            table = new Entry[table.Length];
            count = 0;
        }

        public bool containsKey(object key)
        {
            int idx = bucket(key, table.Length);
            Entry cur = table[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                    return true;
                cur = cur.Next;
            }
            return false;
        }

        public bool containsValue(object value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                Entry cur = table[i];
                while (cur != null)
                {
                    if (Equals(cur.Value, value))
                        return true;
                    cur = cur.Next;
                }
            }
            return false;
        }

        public MyCollection<MyMapEntry<K, V>> entrySet()
        {
            MyArrayList<MyMapEntry<K, V>> list = new MyArrayList<MyMapEntry<K, V>>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry cur = table[i];
                while (cur != null)
                {
                    list.add(new MyMapEntry<K, V>(cur.Key, cur.Value));
                    cur = cur.Next;
                }
            }
            return list;
        }

        public V get(object key)
        {
            int idx = bucket(key, table.Length);
            Entry cur = table[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                    return cur.Value;
                cur = cur.Next;
            }
            return default(V);
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public MySet<K> keySet()
        {
            MyHashSet<K> set = new MyHashSet<K>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry cur = table[i];
                while (cur != null)
                {
                    set.add(cur.Key);
                    cur = cur.Next;
                }
            }
            return set;
        }

        public V put(K key, V value)
        {
            ensureCapacity();
            int idx = bucket(key, table.Length);
            Entry cur = table[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                {
                    V old = cur.Value;
                    cur.Value = value;
                    return old;
                }
                cur = cur.Next;
            }
            table[idx] = new Entry(key, value, table[idx]);
            count++;
            return default(V);
        }

        public void putAll(MyMap<K, V> m)
        {
            if (m == null) throw new MyNullArgumentException("Map is null.");
            MyCollection<MyMapEntry<K, V>> entries = m.entrySet();
            MyMapEntry<K, V>[] arr = entries.toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++)
                put(arr[i].Key, arr[i].Value);
        }

        public V remove(object key)
        {
            int idx = bucket(key, table.Length);
            Entry cur = table[idx];
            Entry prev = null;
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                {
                    if (prev == null) table[idx] = cur.Next;
                    else prev.Next = cur.Next;
                    count--;
                    return cur.Value;
                }
                prev = cur;
                cur = cur.Next;
            }
            return default(V);
        }

        public int size()
        {
            return count;
        }

        public MyCollection<V> values()
        {
            MyArrayList<V> list = new MyArrayList<V>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry cur = table[i];
                while (cur != null)
                {
                    list.add(cur.Value);
                    cur = cur.Next;
                }
            }
            return list;
        }
    }

    public class MyTreeMap<K, V> : MyNavigableMap<K, V>
    {
        private sealed class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;
            public Node Parent;
            public Node(K key, V value, Node parent)
            {
                Key = key;
                Value = value;
                Parent = parent;
            }
        }

        private readonly MyComparator<K> comparator;
        private Node root;
        private int count;

        public MyTreeMap()
        {
            comparator = null;
        }

        public MyTreeMap(MyComparator<K> comp)
        {
            comparator = comp;
        }

        public MyTreeMap(MyMap<K, V> m) : this()
        {
            if (m == null) throw new MyNullArgumentException("Map is null.");
            putAll(m);
        }

        public MyTreeMap(MySortedMap<K, V> sm) : this()
        {
            if (sm == null) throw new MyNullArgumentException("Map is null.");
            putAll(sm);
        }

        private int compare(K a, K b)
        {
            if (comparator != null) return comparator(a, b);
            return Comparer<K>.Default.Compare(a, b);
        }

        private Node findNode(object keyObj)
        {
            if (!(keyObj is K) && keyObj != null) return null;
            K key = (K)keyObj;
            Node cur = root;
            while (cur != null)
            {
                int cmp = compare(key, cur.Key);
                if (cmp == 0) return cur;
                cur = cmp < 0 ? cur.Left : cur.Right;
            }
            return null;
        }

        private Node minimum(Node node)
        {
            if (node == null) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }

        private Node maximum(Node node)
        {
            if (node == null) return null;
            while (node.Right != null) node = node.Right;
            return node;
        }

        private void transplant(Node u, Node v)
        {
            if (u.Parent == null) root = v;
            else if (u == u.Parent.Left) u.Parent.Left = v;
            else u.Parent.Right = v;
            if (v != null) v.Parent = u.Parent;
        }

        private void deleteNode(Node z)
        {
            if (z.Left == null) transplant(z, z.Right);
            else if (z.Right == null) transplant(z, z.Left);
            else
            {
                Node y = minimum(z.Right);
                if (y.Parent != z)
                {
                    transplant(y, y.Right);
                    y.Right = z.Right;
                    y.Right.Parent = y;
                }
                transplant(z, y);
                y.Left = z.Left;
                y.Left.Parent = y;
            }
            count--;
        }

        public void clear()
        {
            root = null;
            count = 0;
        }

        public bool containsKey(object key)
        {
            return findNode(key) != null;
        }

        public bool containsValue(object value)
        {
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++)
                if (Equals(arr[i].Value, value))
                    return true;
            return false;
        }

        public MyCollection<MyMapEntry<K, V>> entrySet()
        {
            MyArrayList<MyMapEntry<K, V>> list = new MyArrayList<MyMapEntry<K, V>>();
            inOrderEntries(root, list);
            return list;
        }

        private void inOrderEntries(Node node, MyArrayList<MyMapEntry<K, V>> list)
        {
            if (node == null) return;
            inOrderEntries(node.Left, list);
            list.add(new MyMapEntry<K, V>(node.Key, node.Value));
            inOrderEntries(node.Right, list);
        }

        public V get(object key)
        {
            Node node = findNode(key);
            return node == null ? default(V) : node.Value;
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public MySet<K> keySet()
        {
            MyTreeSet<K> set = new MyTreeSet<K>(comparator);
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++) set.add(arr[i].Key);
            return set;
        }

        public V put(K key, V value)
        {
            if (root == null)
            {
                root = new Node(key, value, null);
                count = 1;
                return default(V);
            }

            Node cur = root;
            Node parent = null;
            int cmp = 0;

            while (cur != null)
            {
                parent = cur;
                cmp = compare(key, cur.Key);
                if (cmp == 0)
                {
                    V old = cur.Value;
                    cur.Value = value;
                    return old;
                }
                cur = cmp < 0 ? cur.Left : cur.Right;
            }

            Node node = new Node(key, value, parent);
            if (cmp < 0) parent.Left = node; else parent.Right = node;
            count++;
            return default(V);
        }

        public void putAll(MyMap<K, V> m)
        {
            if (m == null) throw new MyNullArgumentException("Map is null.");
            MyMapEntry<K, V>[] arr = m.entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++) put(arr[i].Key, arr[i].Value);
        }

        public V remove(object key)
        {
            Node node = findNode(key);
            if (node == null) return default(V);
            V old = node.Value;
            deleteNode(node);
            return old;
        }

        public int size()
        {
            return count;
        }

        public MyCollection<V> values()
        {
            MyArrayList<V> list = new MyArrayList<V>();
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++) list.add(arr[i].Value);
            return list;
        }

        public K firstKey()
        {
            Node node = minimum(root);
            if (node == null) throw new MyNoSuchElementException("Map is empty.");
            return node.Key;
        }

        public K lastKey()
        {
            Node node = maximum(root);
            if (node == null) throw new MyNoSuchElementException("Map is empty.");
            return node.Key;
        }

        public MySortedMap<K, V> headMap(K end)
        {
            MyTreeMap<K, V> map = new MyTreeMap<K, V>(comparator);
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++)
                if (compare(arr[i].Key, end) < 0)
                    map.put(arr[i].Key, arr[i].Value);
            return map;
        }

        public MySortedMap<K, V> subMap(K start, K end)
        {
            MyTreeMap<K, V> map = new MyTreeMap<K, V>(comparator);
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++)
                if (compare(arr[i].Key, start) >= 0 && compare(arr[i].Key, end) < 0)
                    map.put(arr[i].Key, arr[i].Value);
            return map;
        }

        public MySortedMap<K, V> tailMap(K start)
        {
            MyTreeMap<K, V> map = new MyTreeMap<K, V>(comparator);
            MyMapEntry<K, V>[] arr = entrySet().toArray((MyMapEntry<K, V>[])null);
            for (int i = 0; i < arr.Length; i++)
                if (compare(arr[i].Key, start) > 0)
                    map.put(arr[i].Key, arr[i].Value);
            return map;
        }

        public MyMapEntry<K, V> lowerEntry(K key)
        {
            Node cur = root;
            Node best = null;
            while (cur != null)
            {
                if (compare(cur.Key, key) < 0)
                {
                    best = cur;
                    cur = cur.Right;
                }
                else cur = cur.Left;
            }
            return best == null ? null : new MyMapEntry<K, V>(best.Key, best.Value);
        }

        public MyMapEntry<K, V> floorEntry(K key)
        {
            Node cur = root;
            Node best = null;
            while (cur != null)
            {
                int cmp = compare(cur.Key, key);
                if (cmp == 0) return new MyMapEntry<K, V>(cur.Key, cur.Value);
                if (cmp < 0)
                {
                    best = cur;
                    cur = cur.Right;
                }
                else cur = cur.Left;
            }
            return best == null ? null : new MyMapEntry<K, V>(best.Key, best.Value);
        }

        public MyMapEntry<K, V> higherEntry(K key)
        {
            Node cur = root;
            Node best = null;
            while (cur != null)
            {
                if (compare(cur.Key, key) > 0)
                {
                    best = cur;
                    cur = cur.Left;
                }
                else cur = cur.Right;
            }
            return best == null ? null : new MyMapEntry<K, V>(best.Key, best.Value);
        }

        public MyMapEntry<K, V> ceilingEntry(K key)
        {
            Node cur = root;
            Node best = null;
            while (cur != null)
            {
                int cmp = compare(cur.Key, key);
                if (cmp == 0) return new MyMapEntry<K, V>(cur.Key, cur.Value);
                if (cmp > 0)
                {
                    best = cur;
                    cur = cur.Left;
                }
                else cur = cur.Right;
            }
            return best == null ? null : new MyMapEntry<K, V>(best.Key, best.Value);
        }

        public K lowerKey(K key)
        {
            MyMapEntry<K, V> e = lowerEntry(key);
            return e == null ? default(K) : e.Key;
        }

        public K floorKey(K key)
        {
            MyMapEntry<K, V> e = floorEntry(key);
            return e == null ? default(K) : e.Key;
        }

        public K higherKey(K key)
        {
            MyMapEntry<K, V> e = higherEntry(key);
            return e == null ? default(K) : e.Key;
        }

        public K ceilingKey(K key)
        {
            MyMapEntry<K, V> e = ceilingEntry(key);
            return e == null ? default(K) : e.Key;
        }

        public MyMapEntry<K, V> pollFirstEntry()
        {
            Node node = minimum(root);
            if (node == null) return null;
            MyMapEntry<K, V> e = new MyMapEntry<K, V>(node.Key, node.Value);
            deleteNode(node);
            return e;
        }

        public MyMapEntry<K, V> pollLastEntry()
        {
            Node node = maximum(root);
            if (node == null) return null;
            MyMapEntry<K, V> e = new MyMapEntry<K, V>(node.Key, node.Value);
            deleteNode(node);
            return e;
        }

        public MyMapEntry<K, V> firstEntry()
        {
            Node node = minimum(root);
            return node == null ? null : new MyMapEntry<K, V>(node.Key, node.Value);
        }

        public MyMapEntry<K, V> lastEntry()
        {
            Node node = maximum(root);
            return node == null ? null : new MyMapEntry<K, V>(node.Key, node.Value);
        }
    }

    public class MyHashSet<T> : MySet<T>
    {
        private static readonly object PRESENT = new object();
        private readonly MyHashMap<T, object> map;

        public MyHashSet()
        {
            map = new MyHashMap<T, object>();
        }

        public MyHashSet(MyCollection<T> c) : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        private int compare(T a, T b)
        {
            return Comparer<T>.Default.Compare(a, b);
        }

        private T[] orderedArray()
        {
            T[] arr = map.keySet().toArray((T[])null);
            Array.Sort(arr, Comparer<T>.Default);
            return arr;
        }

        public bool add(T e)
        {
            if (map.containsKey(e)) return false;
            map.put(e, PRESENT);
            return true;
        }

        public bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (add(arr[i])) changed = true;
            return changed;
        }

        public void clear()
        {
            map.clear();
        }

        public bool contains(object o)
        {
            return map.containsKey(o);
        }

        public bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (!contains(arr[i])) return false;
            return true;
        }

        public T first()
        {
            if (isEmpty()) throw new MyNoSuchElementException("Set is empty.");
            return orderedArray()[0];
        }

        public MySet<T> headSet(T toElement)
        {
            MyHashSet<T> set = new MyHashSet<T>();
            T[] arr = orderedArray();
            for (int i = 0; i < arr.Length; i++) if (compare(arr[i], toElement) < 0) set.add(arr[i]);
            return set;
        }

        public bool isEmpty()
        {
            return map.isEmpty();
        }

        public T last()
        {
            if (isEmpty()) throw new MyNoSuchElementException("Set is empty.");
            T[] arr = orderedArray();
            return arr[arr.Length - 1];
        }

        public bool remove(object o)
        {
            if (!contains(o)) return false;
            map.remove(o);
            return true;
        }

        public bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (remove(arr[i])) changed = true;
            return changed;
        }

        public bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                if (!c.contains(arr[i]))
                {
                    remove(arr[i]);
                    changed = true;
                }
            return changed;
        }

        public int size()
        {
            return map.size();
        }

        public MySet<T> subSet(T fromElement, T toElement)
        {
            MyHashSet<T> set = new MyHashSet<T>();
            T[] arr = orderedArray();
            for (int i = 0; i < arr.Length; i++)
                if (compare(arr[i], fromElement) >= 0 && compare(arr[i], toElement) < 0)
                    set.add(arr[i]);
            return set;
        }

        public MySet<T> tailSet(T fromElement)
        {
            MyHashSet<T> set = new MyHashSet<T>();
            T[] arr = orderedArray();
            for (int i = 0; i < arr.Length; i++) if (compare(arr[i], fromElement) >= 0) set.add(arr[i]);
            return set;
        }

        public object[] toArray()
        {
            return map.keySet().toArray();
        }

        public T[] toArray(T[] a)
        {
            return map.keySet().toArray(a);
        }

        public MyIterator<T> iterator()
        {
            return new SetItr(this.toArray((T[])null), this);
        }

        protected class SetItr : MyIterator<T>
        {
            protected readonly T[] snapshot;
            protected readonly MyHashSet<T> set;
            protected int cursor;
            protected int lastRet;

            public SetItr(T[] snapshot, MyHashSet<T> set)
            {
                this.snapshot = snapshot;
                this.set = set;
                cursor = 0;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < snapshot.Length;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                set.remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    public class MyTreeSet<T> : MyNavigableSet<T>
    {
        private static readonly object PRESENT = new object();
        private readonly MyTreeMap<T, object> m;
        private readonly MyComparator<T> comparator;

        public MyTreeSet()
        {
            m = new MyTreeMap<T, object>();
            comparator = null;
        }

        public MyTreeSet(MyTreeMap<T, object> map)
        {
            if (map == null) throw new MyNullArgumentException("Map is null.");
            m = map;
            comparator = null;
        }

        public MyTreeSet(MyComparator<T> comp)
        {
            comparator = comp;
            m = new MyTreeMap<T, object>(comp);
        }

        public MyTreeSet(MyCollection<T> c) : this()
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            addAll(c);
        }

        public bool add(T e)
        {
            bool exists = m.containsKey(e);
            m.put(e, PRESENT);
            return !exists;
        }

        public bool addAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (add(arr[i])) changed = true;
            return changed;
        }

        public void clear()
        {
            m.clear();
        }

        public bool contains(object o)
        {
            return m.containsKey(o);
        }

        public bool containsAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (!contains(arr[i])) return false;
            return true;
        }

        public T first()
        {
            return m.firstKey();
        }

        public T last()
        {
            return m.lastKey();
        }

        public bool isEmpty()
        {
            return m.isEmpty();
        }

        public bool remove(object o)
        {
            if (!m.containsKey(o)) return false;
            m.remove(o);
            return true;
        }

        public bool removeAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = c.toArray((T[])null);
            for (int i = 0; i < arr.Length; i++) if (remove(arr[i])) changed = true;
            return changed;
        }

        public bool retainAll(MyCollection<T> c)
        {
            if (c == null) throw new MyNullArgumentException("Collection is null.");
            bool changed = false;
            T[] arr = toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                if (!c.contains(arr[i]))
                {
                    remove(arr[i]);
                    changed = true;
                }
            return changed;
        }

        public int size()
        {
            return m.size();
        }

        public object[] toArray()
        {
            return m.keySet().toArray();
        }

        public T[] toArray(T[] a)
        {
            return m.keySet().toArray(a);
        }

        public MySet<T> subSet(T fromElement, T toElement)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            MySet<T> keys = m.subMap(fromElement, toElement).keySet();
            set.addAll(keys);
            return set;
        }

        public MySet<T> headSet(T toElement)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            set.addAll(m.headMap(toElement).keySet());
            return set;
        }

        public MySet<T> tailSet(T fromElement)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            MyMapEntry<T, object>[] arr = m.entrySet().toArray((MyMapEntry<T, object>[])null);
            for (int i = 0; i < arr.Length; i++)
                if (compare(arr[i].Key, fromElement) >= 0)
                    set.add(arr[i].Key);
            return set;
        }

        private int compare(T a, T b)
        {
            if (comparator != null) return comparator(a, b);
            return Comparer<T>.Default.Compare(a, b);
        }

        public T lower(T obj)
        {
            MyMapEntry<T, object> e = m.lowerEntry(obj);
            return e == null ? default(T) : e.Key;
        }

        public T floor(T obj)
        {
            MyMapEntry<T, object> e = m.floorEntry(obj);
            return e == null ? default(T) : e.Key;
        }

        public T higher(T obj)
        {
            MyMapEntry<T, object> e = m.higherEntry(obj);
            return e == null ? default(T) : e.Key;
        }

        public T ceiling(T obj)
        {
            MyMapEntry<T, object> e = m.ceilingEntry(obj);
            return e == null ? default(T) : e.Key;
        }

        public MySet<T> headSet(T upperBound, bool incl)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            T[] arr = toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                if (incl ? compare(arr[i], upperBound) <= 0 : compare(arr[i], upperBound) < 0)
                    set.add(arr[i]);
            return set;
        }

        public MySet<T> subSet(T lowerBound, bool lowIncl, T upperBound, bool highIncl)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            T[] arr = toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
            {
                bool lowOk = lowIncl ? compare(arr[i], lowerBound) >= 0 : compare(arr[i], lowerBound) > 0;
                bool highOk = highIncl ? compare(arr[i], upperBound) <= 0 : compare(arr[i], upperBound) < 0;
                if (lowOk && highOk) set.add(arr[i]);
            }
            return set;
        }

        public MySet<T> tailSet(T fromElement, bool inclusive)
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            T[] arr = toArray((T[])null);
            for (int i = 0; i < arr.Length; i++)
                if (inclusive ? compare(arr[i], fromElement) >= 0 : compare(arr[i], fromElement) > 0)
                    set.add(arr[i]);
            return set;
        }

        public T pollLast()
        {
            MyMapEntry<T, object> e = m.pollLastEntry();
            return e == null ? default(T) : e.Key;
        }

        public T pollFirst()
        {
            MyMapEntry<T, object> e = m.pollFirstEntry();
            return e == null ? default(T) : e.Key;
        }

        public MyIterator<T> descendingIterator()
        {
            T[] arr = toArray((T[])null);
            Array.Reverse(arr);
            return new DescItr(arr, this);
        }

        public MyNavigableSet<T> descendingSet()
        {
            MyTreeSet<T> set = new MyTreeSet<T>(comparator);
            T[] arr = toArray((T[])null);
            for (int i = arr.Length - 1; i >= 0; i--) set.add(arr[i]);
            return set;
        }

        public MyIterator<T> iterator()
        {
            return new AscItr(toArray((T[])null), this);
        }

        private sealed class AscItr : MyIterator<T>
        {
            private readonly T[] snapshot;
            private readonly MyTreeSet<T> set;
            private int cursor;
            private int lastRet;

            public AscItr(T[] snapshot, MyTreeSet<T> set)
            {
                this.snapshot = snapshot;
                this.set = set;
                cursor = 0;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < snapshot.Length;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                set.remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }

        private sealed class DescItr : MyIterator<T>
        {
            private readonly T[] snapshot;
            private readonly MyTreeSet<T> set;
            private int cursor;
            private int lastRet;

            public DescItr(T[] snapshot, MyTreeSet<T> set)
            {
                this.snapshot = snapshot;
                this.set = set;
                cursor = 0;
                lastRet = -1;
            }

            public bool hasNext()
            {
                return cursor < snapshot.Length;
            }

            public T next()
            {
                if (!hasNext()) throw new MyNoSuchElementException("No next element.");
                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void remove()
            {
                if (lastRet < 0) throw new MyIllegalStateException("Illegal iterator state.");
                set.remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            MyArrayList<int> arrayList = new MyArrayList<int>();
            arrayList.add(1);
            arrayList.add(2);
            arrayList.add(3);

            MyVector<string> vector = new MyVector<string>();
            vector.add("A");
            vector.add("B");
            vector.add("C");

            MyLinkedList<int> linkedList = new MyLinkedList<int>();
            linkedList.add(10);
            linkedList.add(20);
            linkedList.add(30);

            MyPriorityQueue<int> priorityQueue = new MyPriorityQueue<int>();
            priorityQueue.offer(5);
            priorityQueue.offer(1);
            priorityQueue.offer(3);

            MyArrayDeque<int> deque = new MyArrayDeque<int>();
            deque.addLast(100);
            deque.addFirst(50);
            deque.addLast(150);

            MyHashSet<int> hashSet = new MyHashSet<int>();
            hashSet.add(7);
            hashSet.add(2);
            hashSet.add(9);

            MyTreeSet<int> treeSet = new MyTreeSet<int>();
            treeSet.add(7);
            treeSet.add(2);
            treeSet.add(9);

            MyHashMap<string, int> hashMap = new MyHashMap<string, int>();
            hashMap.put("one", 1);
            hashMap.put("two", 2);

            MyTreeMap<int, string> treeMap = new MyTreeMap<int, string>();
            treeMap.put(2, "B");
            treeMap.put(1, "A");
            treeMap.put(3, "C");

            Console.WriteLine("ArrayList size = " + arrayList.size());
            Console.WriteLine("Vector get(1) = " + vector.get(1));
            Console.WriteLine("LinkedList lastIndexOf(20) = " + linkedList.lastIndexOf(20));
            Console.WriteLine("PriorityQueue poll = " + priorityQueue.poll());
            Console.WriteLine("Deque first = " + deque.getFirst());
            Console.WriteLine("HashSet first = " + hashSet.first());
            Console.WriteLine("TreeSet ceiling(8) = " + treeSet.ceiling(8));
            Console.WriteLine("HashMap containsKey('two') = " + hashMap.containsKey("two"));
            Console.WriteLine("TreeMap higherKey(2) = " + treeMap.higherKey(2));

            MyIterator<int> descIt = treeSet.descendingIterator();
            Console.Write("TreeSet descending: ");
            while (descIt.hasNext())
                Console.Write(descIt.next() + " ");
            Console.WriteLine();
        }
    }
}
