using System;
using System.Collections.Generic;

namespace Task28Iterators
{
    public class MyIteratorException : Exception
    {
        public MyIteratorException(string message) : base(message) { }
    }

    public class MyNoSuchElementException : MyIteratorException
    {
        public MyNoSuchElementException() : base("Следующий или предыдущий элемент отсутствует.") { }
    }

    public class MyIllegalStateException : MyIteratorException
    {
        public MyIllegalStateException() : base("Операция невозможна в текущем состоянии итератора.") { }
    }

    public class MyIndexOutOfBoundsException : MyIteratorException
    {
        public MyIndexOutOfBoundsException() : base("Индекс выходит за границы коллекции.") { }
    }

    public interface MyIterator<T>
    {
        bool HasNext();
        T Next();
        void Remove();
    }

    public interface MyListIterator<T> : MyIterator<T>
    {
        bool HasPrevious();
        T Previous();
        int NextIndex();
        int PreviousIndex();
        void Set(T element);
        void Add(T element);
    }

    public class MyArrayList<T>
    {
        private T[] items;
        private int size;

        public MyArrayList()
        {
            items = new T[4];
        }

        public int Size()
        {
            return size;
        }

        public void Add(T item)
        {
            EnsureCapacity(size + 1);
            items[size++] = item;
        }

        public void Add(int index, T item)
        {
            CheckIndexForInsert(index);
            EnsureCapacity(size + 1);
            Array.Copy(items, index, items, index + 1, size - index);
            items[index] = item;
            size++;
        }

        public T Get(int index)
        {
            CheckIndex(index);
            return items[index];
        }

        public T SetAt(int index, T item)
        {
            CheckIndex(index);
            T old = items[index];
            items[index] = item;
            return old;
        }

        public T RemoveAt(int index)
        {
            CheckIndex(index);
            T old = items[index];
            Array.Copy(items, index + 1, items, index, size - index - 1);
            items[--size] = default(T);
            return old;
        }

        public MyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public MyListIterator<T> ListIterator(int index)
        {
            CheckIndexForInsert(index);
            return new MyItr(this, index);
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= items.Length)
                return;

            int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
            while (newCapacity < capacity)
                newCapacity *= 2;

            T[] arr = new T[newCapacity];
            Array.Copy(items, arr, size);
            items = arr;
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= size)
                throw new MyIndexOutOfBoundsException();
        }

        private void CheckIndexForInsert(int index)
        {
            if (index < 0 || index > size)
                throw new MyIndexOutOfBoundsException();
        }

        private sealed class MyItr : MyListIterator<T>
        {
            private readonly MyArrayList<T> list;
            private int cursor;
            private int lastRet;

            public MyItr(MyArrayList<T> list, int index)
            {
                this.list = list;
                cursor = index;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < list.size;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return list.items[cursor++];
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (!HasPrevious())
                    throw new MyNoSuchElementException();

                cursor--;
                lastRet = cursor;
                return list.items[cursor];
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                list.RemoveAt(lastRet);
                if (lastRet < cursor)
                    cursor--;

                lastRet = -1;
            }

            public void Set(T element)
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                list.SetAt(lastRet, element);
            }

            public void Add(T element)
            {
                list.Add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyVector<T>
    {
        private T[] items;
        private int size;

        public MyVector()
        {
            items = new T[4];
        }

        public int Size()
        {
            return size;
        }

        public void Add(T item)
        {
            EnsureCapacity(size + 1);
            items[size++] = item;
        }

        public void Add(int index, T item)
        {
            CheckIndexForInsert(index);
            EnsureCapacity(size + 1);
            Array.Copy(items, index, items, index + 1, size - index);
            items[index] = item;
            size++;
        }

        public T Get(int index)
        {
            CheckIndex(index);
            return items[index];
        }

        public T SetAt(int index, T item)
        {
            CheckIndex(index);
            T old = items[index];
            items[index] = item;
            return old;
        }

        public T RemoveAt(int index)
        {
            CheckIndex(index);
            T old = items[index];
            Array.Copy(items, index + 1, items, index, size - index - 1);
            items[--size] = default(T);
            return old;
        }

        public MyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public MyListIterator<T> ListIterator(int index)
        {
            CheckIndexForInsert(index);
            return new MyItr(this, index);
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= items.Length)
                return;

            int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
            while (newCapacity < capacity)
                newCapacity *= 2;

            T[] arr = new T[newCapacity];
            Array.Copy(items, arr, size);
            items = arr;
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= size)
                throw new MyIndexOutOfBoundsException();
        }

        private void CheckIndexForInsert(int index)
        {
            if (index < 0 || index > size)
                throw new MyIndexOutOfBoundsException();
        }

        private sealed class MyItr : MyListIterator<T>
        {
            private readonly MyVector<T> vector;
            private int cursor;
            private int lastRet;

            public MyItr(MyVector<T> vector, int index)
            {
                this.vector = vector;
                cursor = index;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < vector.size;
            }

            public T Next()
            {
                if (cursor >= vector.size)
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return vector.items[cursor++];
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (cursor <= 0)
                    throw new MyNoSuchElementException();

                cursor--;
                lastRet = cursor;
                return vector.items[cursor];
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                vector.RemoveAt(lastRet);
                if (lastRet < cursor)
                    cursor--;

                lastRet = -1;
            }

            public void Set(T element)
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                vector.SetAt(lastRet, element);
            }

            public void Add(T element)
            {
                vector.Add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyLinkedList<T>
    {
        private sealed class Node
        {
            public T Value;
            public Node Prev;
            public Node Next;

            public Node(T value)
            {
                Value = value;
            }
        }

        private Node head;
        private Node tail;
        private int size;

        public int Size()
        {
            return size;
        }

        public void Add(T item)
        {
            Node node = new Node(item);

            if (tail == null)
            {
                head = tail = node;
            }
            else
            {
                tail.Next = node;
                node.Prev = tail;
                tail = node;
            }

            size++;
        }

        public void Add(int index, T item)
        {
            if (index < 0 || index > size)
                throw new MyIndexOutOfBoundsException();

            if (index == size)
            {
                Add(item);
                return;
            }

            Node next = GetNode(index);
            Node node = new Node(item);
            Node prev = next.Prev;

            node.Next = next;
            node.Prev = prev;
            next.Prev = node;

            if (prev != null)
                prev.Next = node;
            else
                head = node;

            size++;
        }

        public T Get(int index)
        {
            return GetNode(index).Value;
        }

        public T SetAt(int index, T item)
        {
            Node node = GetNode(index);
            T old = node.Value;
            node.Value = item;
            return old;
        }

        public T RemoveAt(int index)
        {
            Node node = GetNode(index);
            RemoveNode(node);
            return node.Value;
        }

        public MyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public MyListIterator<T> ListIterator(int index)
        {
            if (index < 0 || index > size)
                throw new MyIndexOutOfBoundsException();

            return new MyItr(this, index);
        }

        private Node GetNode(int index)
        {
            if (index < 0 || index >= size)
                throw new MyIndexOutOfBoundsException();

            Node current;
            int i;

            if (index < size / 2)
            {
                current = head;
                i = 0;
                while (i < index)
                {
                    current = current.Next;
                    i++;
                }
            }
            else
            {
                current = tail;
                i = size - 1;
                while (i > index)
                {
                    current = current.Prev;
                    i--;
                }
            }

            return current;
        }

        private void RemoveNode(Node node)
        {
            Node prev = node.Prev;
            Node next = node.Next;

            if (prev != null)
                prev.Next = next;
            else
                head = next;

            if (next != null)
                next.Prev = prev;
            else
                tail = prev;

            size--;
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

            public bool HasNext()
            {
                return cursor < list.size;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                T value = list.Get(cursor);
                cursor++;
                return value;
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (!HasPrevious())
                    throw new MyNoSuchElementException();

                cursor--;
                lastRet = cursor;
                return list.Get(cursor);
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                list.RemoveAt(lastRet);
                if (lastRet < cursor)
                    cursor--;

                lastRet = -1;
            }

            public void Set(T element)
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                list.SetAt(lastRet, element);
            }

            public void Add(T element)
            {
                list.Add(cursor, element);
                cursor++;
                lastRet = -1;
            }
        }
    }

    public class MyPriorityQueue<T> where T : IComparable<T>
    {
        private T[] heap;
        private int size;

        public MyPriorityQueue()
        {
            heap = new T[8];
        }

        public int Size()
        {
            return size;
        }

        public void Offer(T item)
        {
            EnsureCapacity(size + 1);
            heap[size] = item;
            SiftUp(size);
            size++;
        }

        public T Peek()
        {
            if (size == 0)
                throw new MyNoSuchElementException();

            return heap[0];
        }

        public T Poll()
        {
            if (size == 0)
                throw new MyNoSuchElementException();

            T result = heap[0];
            size--;
            heap[0] = heap[size];
            heap[size] = default(T);
            if (size > 0)
                SiftDown(0);

            return result;
        }

        public bool RemoveValue(T value)
        {
            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < size; i++)
            {
                if (comparer.Equals(heap[i], value))
                {
                    size--;
                    heap[i] = heap[size];
                    heap[size] = default(T);

                    if (i < size)
                    {
                        SiftDown(i);
                        SiftUp(i);
                    }

                    return true;
                }
            }

            return false;
        }

        public MyIterator<T> Iterator()
        {
            return new MyItr(this);
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= heap.Length)
                return;

            int newCapacity = heap.Length * 2;
            while (newCapacity < capacity)
                newCapacity *= 2;

            T[] arr = new T[newCapacity];
            Array.Copy(heap, arr, size);
            heap = arr;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[parent].CompareTo(heap[index]) <= 0)
                    break;

                Swap(parent, index);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = left + 1;
                int smallest = index;

                if (left < size && heap[left].CompareTo(heap[smallest]) < 0)
                    smallest = left;

                if (right < size && heap[right].CompareTo(heap[smallest]) < 0)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            T tmp = heap[i];
            heap[i] = heap[j];
            heap[j] = tmp;
        }

        private sealed class MyItr : MyIterator<T>
        {
            private readonly MyPriorityQueue<T> queue;
            private int cursor;
            private int lastRet;
            private T[] snapshot;

            public MyItr(MyPriorityQueue<T> queue)
            {
                this.queue = queue;
                snapshot = new T[queue.size];
                Array.Copy(queue.heap, snapshot, queue.size);
                cursor = 0;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < snapshot.Length;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                queue.RemoveValue(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    public class MyArrayDeque<T>
    {
        private T[] items;
        private int head;
        private int size;

        public MyArrayDeque()
        {
            items = new T[8];
        }

        public int Size()
        {
            return size;
        }

        public void AddFirst(T item)
        {
            EnsureCapacity(size + 1);
            head = (head - 1 + items.Length) % items.Length;
            items[head] = item;
            size++;
        }

        public void AddLast(T item)
        {
            EnsureCapacity(size + 1);
            items[(head + size) % items.Length] = item;
            size++;
        }

        public T RemoveFirst()
        {
            if (size == 0)
                throw new MyNoSuchElementException();

            T value = items[head];
            items[head] = default(T);
            head = (head + 1) % items.Length;
            size--;
            return value;
        }

        public T RemoveLast()
        {
            if (size == 0)
                throw new MyNoSuchElementException();

            int index = PhysicalIndex(size - 1);
            T value = items[index];
            items[index] = default(T);
            size--;
            return value;
        }

        public T GetAt(int index)
        {
            CheckIndex(index);
            return items[PhysicalIndex(index)];
        }

        public T RemoveAt(int index)
        {
            CheckIndex(index);
            T value = GetAt(index);

            if (index < size / 2)
            {
                for (int i = index; i > 0; i--)
                    items[PhysicalIndex(i)] = items[PhysicalIndex(i - 1)];

                items[head] = default(T);
                head = (head + 1) % items.Length;
            }
            else
            {
                for (int i = index; i < size - 1; i++)
                    items[PhysicalIndex(i)] = items[PhysicalIndex(i + 1)];

                items[PhysicalIndex(size - 1)] = default(T);
            }

            size--;
            return value;
        }

        public MyIterator<T> Iterator()
        {
            return new MyItr(this);
        }

        private int PhysicalIndex(int logicalIndex)
        {
            return (head + logicalIndex) % items.Length;
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= size)
                throw new MyIndexOutOfBoundsException();
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= items.Length)
                return;

            int newCapacity = items.Length * 2;
            while (newCapacity < capacity)
                newCapacity *= 2;

            T[] arr = new T[newCapacity];
            for (int i = 0; i < size; i++)
                arr[i] = GetAt(i);

            items = arr;
            head = 0;
        }

        private sealed class MyItr : MyIterator<T>
        {
            private readonly MyArrayDeque<T> deque;
            private int cursor;
            private int lastRet;

            public MyItr(MyArrayDeque<T> deque)
            {
                this.deque = deque;
                cursor = 0;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < deque.size;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return deque.GetAt(cursor++);
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                deque.RemoveAt(lastRet);
                if (lastRet < cursor)
                    cursor--;

                lastRet = -1;
            }
        }
    }

    public class MyHashSet<T>
    {
        private T[] items;
        private bool[] used;
        private int count;
        private const float LoadFactor = 0.75f;

        public MyHashSet()
        {
            items = new T[16];
            used = new bool[16];
        }

        public int Size()
        {
            return count;
        }

        public bool Add(T item)
        {
            EnsureCapacity();
            return Insert(item, items, used);
        }

        public bool Contains(T item)
        {
            int cap = items.Length;
            int index = Hash(item) % cap;
            int start = index;
            var comparer = EqualityComparer<T>.Default;

            while (used[index])
            {
                if (comparer.Equals(items[index], item))
                    return true;

                index = (index + 1) % cap;
                if (index == start)
                    break;
            }

            return false;
        }

        public bool Remove(T item)
        {
            int cap = items.Length;
            int index = Hash(item) % cap;
            int start = index;
            var comparer = EqualityComparer<T>.Default;

            while (used[index])
            {
                if (comparer.Equals(items[index], item))
                {
                    used[index] = false;
                    items[index] = default(T);
                    count--;
                    RehashCluster((index + 1) % cap);
                    return true;
                }

                index = (index + 1) % cap;
                if (index == start)
                    break;
            }

            return false;
        }

        public T[] ToArray()
        {
            T[] arr = new T[count];
            int p = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if (used[i])
                    arr[p++] = items[i];
            }

            return arr;
        }

        public MyIterator<T> Iterator()
        {
            return new MyItr(this);
        }

        private void RehashCluster(int start)
        {
            int cap = items.Length;
            int i = start;

            while (used[i])
            {
                T value = items[i];
                used[i] = false;
                items[i] = default(T);
                count--;
                Insert(value, items, used);
                i = (i + 1) % cap;
            }
        }

        private void EnsureCapacity()
        {
            if ((count + 1f) / items.Length < LoadFactor)
                return;

            T[] oldItems = items;
            bool[] oldUsed = used;

            items = new T[oldItems.Length * 2];
            used = new bool[oldUsed.Length * 2];
            count = 0;

            for (int i = 0; i < oldItems.Length; i++)
            {
                if (oldUsed[i])
                    Insert(oldItems[i], items, used);
            }
        }

        private bool Insert(T item, T[] arr, bool[] arrUsed)
        {
            int cap = arr.Length;
            int index = Hash(item) % cap;
            var comparer = EqualityComparer<T>.Default;

            while (arrUsed[index])
            {
                if (comparer.Equals(arr[index], item))
                    return false;

                index = (index + 1) % cap;
            }

            arr[index] = item;
            arrUsed[index] = true;
            count++;
            return true;
        }

        private int Hash(T item)
        {
            return (item == null ? 0 : item.GetHashCode() & 0x7fffffff);
        }

        private sealed class MyItr : MyIterator<T>
        {
            private readonly MyHashSet<T> set;
            private readonly T[] snapshot;
            private int cursor;
            private int lastRet;

            public MyItr(MyHashSet<T> set)
            {
                this.set = set;
                snapshot = set.ToArray();
                cursor = 0;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < snapshot.Length;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                set.Remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    public class MyTreeSet<T> where T : IComparable<T>
    {
        private sealed class Node
        {
            public T Value;
            public Node Left;
            public Node Right;

            public Node(T value)
            {
                Value = value;
            }
        }

        private Node root;
        private int count;

        public int Size()
        {
            return count;
        }

        public bool Add(T item)
        {
            bool added = false;
            root = Add(root, item, ref added);
            if (added)
                count++;
            return added;
        }

        public bool Contains(T item)
        {
            Node cur = root;

            while (cur != null)
            {
                int cmp = item.CompareTo(cur.Value);
                if (cmp == 0)
                    return true;

                cur = cmp < 0 ? cur.Left : cur.Right;
            }

            return false;
        }

        public bool Remove(T item)
        {
            bool removed = false;
            root = Remove(root, item, ref removed);
            if (removed)
                count--;
            return removed;
        }

        public T[] ToArray()
        {
            T[] arr = new T[count];
            int index = 0;
            Fill(root, arr, ref index);
            return arr;
        }

        public MyIterator<T> Iterator()
        {
            return new MyItr(this);
        }

        private Node Add(Node node, T item, ref bool added)
        {
            if (node == null)
            {
                added = true;
                return new Node(item);
            }

            int cmp = item.CompareTo(node.Value);
            if (cmp < 0)
                node.Left = Add(node.Left, item, ref added);
            else if (cmp > 0)
                node.Right = Add(node.Right, item, ref added);

            return node;
        }

        private Node Remove(Node node, T item, ref bool removed)
        {
            if (node == null)
                return null;

            int cmp = item.CompareTo(node.Value);

            if (cmp < 0)
            {
                node.Left = Remove(node.Left, item, ref removed);
            }
            else if (cmp > 0)
            {
                node.Right = Remove(node.Right, item, ref removed);
            }
            else
            {
                removed = true;

                if (node.Left == null)
                    return node.Right;

                if (node.Right == null)
                    return node.Left;

                Node min = node.Right;
                while (min.Left != null)
                    min = min.Left;

                node.Value = min.Value;
                bool dummy = false;
                node.Right = Remove(node.Right, min.Value, ref dummy);
            }

            return node;
        }

        private void Fill(Node node, T[] arr, ref int index)
        {
            if (node == null)
                return;

            Fill(node.Left, arr, ref index);
            arr[index++] = node.Value;
            Fill(node.Right, arr, ref index);
        }

        private sealed class MyItr : MyIterator<T>
        {
            private readonly MyTreeSet<T> set;
            private readonly T[] snapshot;
            private int cursor;
            private int lastRet;

            public MyItr(MyTreeSet<T> set)
            {
                this.set = set;
                snapshot = set.ToArray();
                cursor = 0;
                lastRet = -1;
            }

            public bool HasNext()
            {
                return cursor < snapshot.Length;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new MyNoSuchElementException();

                lastRet = cursor;
                return snapshot[cursor++];
            }

            public void Remove()
            {
                if (lastRet < 0)
                    throw new MyIllegalStateException();

                set.Remove(snapshot[lastRet]);
                lastRet = -1;
            }
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            try
            {
                DemoArrayList();
                DemoVector();
                DemoLinkedList();
                DemoPriorityQueue();
                DemoArrayDeque();
                DemoHashSet();
                DemoTreeSet();
            }
            catch (MyIteratorException ex)
            {
                Console.WriteLine("Ошибка итератора: " + ex.Message);
            }
        }

        private static void DemoArrayList()
        {
            Console.WriteLine("MyArrayList");
            var list = new MyArrayList<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            var it = list.ListIterator();
            Console.WriteLine(it.Next());
            it.Set(11);
            it.Add(15);
            Console.WriteLine(it.Next());
            it.Remove();

            var it2 = list.ListIterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoVector()
        {
            Console.WriteLine("MyVector");
            var vector = new MyVector<string>();
            vector.Add("A");
            vector.Add("B");
            vector.Add("C");

            var it = vector.ListIterator(1);
            Console.WriteLine(it.Next());
            it.Set("BB");
            Console.WriteLine(it.Previous());
            it.Add("AA");

            var it2 = vector.ListIterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoLinkedList()
        {
            Console.WriteLine("MyLinkedList");
            var list = new MyLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            var it = list.ListIterator();
            Console.WriteLine(it.Next());
            Console.WriteLine(it.Next());
            Console.WriteLine(it.Previous());
            it.Remove();
            it.Add(99);

            var it2 = list.ListIterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoPriorityQueue()
        {
            Console.WriteLine("MyPriorityQueue");
            var pq = new MyPriorityQueue<int>();
            pq.Offer(5);
            pq.Offer(1);
            pq.Offer(7);
            pq.Offer(3);

            var it = pq.Iterator();
            while (it.HasNext())
            {
                int x = it.Next();
                Console.Write(x + " ");
                if (x == 7)
                    it.Remove();
            }
            Console.WriteLine();

            while (pq.Size() > 0)
                Console.Write(pq.Poll() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoArrayDeque()
        {
            Console.WriteLine("MyArrayDeque");
            var deque = new MyArrayDeque<string>();
            deque.AddLast("B");
            deque.AddFirst("A");
            deque.AddLast("C");
            deque.AddLast("D");

            var it = deque.Iterator();
            while (it.HasNext())
            {
                string s = it.Next();
                Console.Write(s + " ");
                if (s == "C")
                    it.Remove();
            }
            Console.WriteLine();

            var it2 = deque.Iterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoHashSet()
        {
            Console.WriteLine("MyHashSet");
            var set = new MyHashSet<int>();
            set.Add(10);
            set.Add(20);
            set.Add(30);
            set.Add(20);

            var it = set.Iterator();
            while (it.HasNext())
            {
                int x = it.Next();
                Console.Write(x + " ");
                if (x == 20)
                    it.Remove();
            }
            Console.WriteLine();

            var it2 = set.Iterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DemoTreeSet()
        {
            Console.WriteLine("MyTreeSet");
            var set = new MyTreeSet<int>();
            set.Add(5);
            set.Add(1);
            set.Add(9);
            set.Add(3);

            var it = set.Iterator();
            while (it.HasNext())
            {
                int x = it.Next();
                Console.Write(x + " ");
                if (x == 5)
                    it.Remove();
            }
            Console.WriteLine();

            var it2 = set.Iterator();
            while (it2.HasNext())
                Console.Write(it2.Next() + " ");
            Console.WriteLine();
        }
    }
}