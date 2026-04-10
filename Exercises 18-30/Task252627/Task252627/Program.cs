using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MySetsTasks
{
    public sealed class MyHashMap<K, V>
    {
        public sealed class Entry
        {
            public K Key { get; }
            public V Value { get; internal set; }
            internal Entry Next { get; set; }

            internal Entry(K key, V value, Entry next)
            {
                Key = key;
                Value = value;
                Next = next;
            }
        }

        private Entry[] table;
        private int size;
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
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Начальная ёмкость должна быть больше 0.");

            if (float.IsNaN(loadFactor) || loadFactor <= 0f)
                throw new ArgumentOutOfRangeException(nameof(loadFactor), "Коэффициент загрузки должен быть положительным.");

            table = new Entry[initialCapacity];
            size = 0;
            this.loadFactor = loadFactor;
        }

        public int Size()
        {
            return size;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public void Clear()
        {
            table = new Entry[table.Length];
            size = 0;
        }

        public bool ContainsKey(object key)
        {
            return FindEntry(key) != null;
        }

        public V Get(object key)
        {
            Entry entry = FindEntry(key);
            return entry != null ? entry.Value : default(V);
        }

        public V Put(K key, V value)
        {
            EnsureCapacityForInsert();

            int index = GetBucketIndex(key, table.Length);
            Entry current = table[index];
            var comparer = EqualityComparer<K>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Key, key))
                {
                    V oldValue = current.Value;
                    current.Value = value;
                    return oldValue;
                }

                current = current.Next;
            }

            table[index] = new Entry(key, value, table[index]);
            size++;
            return default(V);
        }

        public V Remove(object key)
        {
            int index = GetBucketIndexFromObject(key, table.Length);

            Entry current = table[index];
            Entry previous = null;

            while (current != null)
            {
                if (KeysEqualObject(current.Key, key))
                {
                    if (previous == null)
                        table[index] = current.Next;
                    else
                        previous.Next = current.Next;

                    size--;
                    return current.Value;
                }

                previous = current;
                current = current.Next;
            }

            return default(V);
        }

        public List<Entry> EntryList()
        {
            var result = new List<Entry>(size);

            for (int i = 0; i < table.Length; i++)
            {
                Entry current = table[i];
                while (current != null)
                {
                    result.Add(current);
                    current = current.Next;
                }
            }

            return result;
        }

        private Entry FindEntry(object key)
        {
            int index = GetBucketIndexFromObject(key, table.Length);
            Entry current = table[index];

            while (current != null)
            {
                if (KeysEqualObject(current.Key, key))
                    return current;

                current = current.Next;
            }

            return null;
        }

        private void EnsureCapacityForInsert()
        {
            if (size + 1 <= table.Length * loadFactor)
                return;

            Resize(table.Length * 2);
        }

        private void Resize(int newCapacity)
        {
            Entry[] oldTable = table;
            Entry[] newTable = new Entry[newCapacity];

            for (int i = 0; i < oldTable.Length; i++)
            {
                Entry current = oldTable[i];

                while (current != null)
                {
                    Entry next = current.Next;
                    int newIndex = GetBucketIndex(current.Key, newCapacity);
                    current.Next = newTable[newIndex];
                    newTable[newIndex] = current;
                    current = next;
                }
            }

            table = newTable;
        }

        private int GetBucketIndex(K key, int capacity)
        {
            return GetNonNegativeHashCode(key) % capacity;
        }

        private int GetBucketIndexFromObject(object key, int capacity)
        {
            return GetNonNegativeHashCodeFromObject(key) % capacity;
        }

        private int GetNonNegativeHashCode(K key)
        {
            if (ReferenceEquals(key, null))
                return 0;

            return key.GetHashCode() & 0x7fffffff;
        }

        private int GetNonNegativeHashCodeFromObject(object key)
        {
            if (key == null)
                return 0;

            return key.GetHashCode() & 0x7fffffff;
        }

        private bool KeysEqualObject(K leftKey, object rightKey)
        {
            if (ReferenceEquals(leftKey, null) && rightKey == null)
                return true;

            if (ReferenceEquals(leftKey, null) || rightKey == null)
                return false;

            if (!(rightKey is K))
                return false;

            return EqualityComparer<K>.Default.Equals(leftKey, (K)rightKey);
        }
    }

    public sealed class MyHashSet<T>
    {
        private static readonly object DummyValue = new object();
        private readonly MyHashMap<T, object> map;

        public MyHashSet()
        {
            map = new MyHashMap<T, object>();
        }

        public MyHashSet(int initialCapacity)
        {
            map = new MyHashMap<T, object>(initialCapacity);
        }

        public MyHashSet(int initialCapacity, float loadFactor)
        {
            map = new MyHashMap<T, object>(initialCapacity, loadFactor);
        }

        public MyHashSet(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            int capacity = Math.Max(16, a.Length * 2);
            map = new MyHashMap<T, object>(capacity, 0.75f);
            AddAll(a);
        }

        public bool Add(T e)
        {
            if (map.ContainsKey(e))
                return false;

            map.Put(e, DummyValue);
            return true;
        }

        public bool AddAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            bool changed = false;
            for (int i = 0; i < a.Length; i++)
            {
                if (Add(a[i]))
                    changed = true;
            }

            return changed;
        }

        public void Clear()
        {
            map.Clear();
        }

        public bool Contains(object o)
        {
            return map.ContainsKey(o);
        }

        public bool ContainsAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            for (int i = 0; i < a.Length; i++)
            {
                if (!Contains(a[i]))
                    return false;
            }

            return true;
        }

        public bool IsEmpty()
        {
            return map.IsEmpty();
        }

        public bool Remove(object o)
        {
            if (!Contains(o))
                return false;

            map.Remove(o);
            return true;
        }

        public bool RemoveAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            bool changed = false;
            for (int i = 0; i < a.Length; i++)
            {
                if (Remove(a[i]))
                    changed = true;
            }

            return changed;
        }

        public bool RetainAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            var keep = new MyHashSet<T>(Math.Max(16, a.Length * 2));
            keep.AddAll(a);

            List<MyHashMap<T, object>.Entry> entries = map.EntryList();
            bool changed = false;

            for (int i = 0; i < entries.Count; i++)
            {
                T key = entries[i].Key;
                if (!keep.Contains(key))
                {
                    map.Remove(key);
                    changed = true;
                }
            }

            return changed;
        }

        public int Size()
        {
            return map.Size();
        }

        public object[] ToArray()
        {
            List<MyHashMap<T, object>.Entry> entries = map.EntryList();
            object[] result = new object[entries.Count];

            for (int i = 0; i < entries.Count; i++)
                result[i] = entries[i].Key;

            return result;
        }

        public T[] ToArray(T[] a)
        {
            List<MyHashMap<T, object>.Entry> entries = map.EntryList();
            T[] result = a;

            if (result == null || result.Length < entries.Count)
                result = new T[entries.Count];

            for (int i = 0; i < entries.Count; i++)
                result[i] = entries[i].Key;

            return result;
        }
    }

    public sealed class LineByWords : IEquatable<LineByWords>, IComparable<LineByWords>
    {
        public string Original { get; }
        private readonly int[] lengths;

        public LineByWords(string line)
        {
            Original = line ?? string.Empty;
            lengths = ExtractSortedWordLengths(Original);
        }

        public int CompareTo(LineByWords other)
        {
            if (other == null)
                return 1;

            int min = lengths.Length < other.lengths.Length ? lengths.Length : other.lengths.Length;

            for (int i = 0; i < min; i++)
            {
                if (lengths[i] != other.lengths[i])
                    return lengths[i].CompareTo(other.lengths[i]);
            }

            if (lengths.Length != other.lengths.Length)
                return lengths.Length.CompareTo(other.lengths.Length);

            return 0;
        }

        public bool Equals(LineByWords other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LineByWords);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < lengths.Length; i++)
                    hash = hash * 31 + lengths[i];
                return hash;
            }
        }

        public override string ToString()
        {
            return Original;
        }

        private static int[] ExtractSortedWordLengths(string line)
        {
            if (string.IsNullOrEmpty(line))
                return new int[0];

            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int[] arr = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                arr[i] = parts[i].Length;

            Array.Sort(arr);
            return arr;
        }
    }

    internal static class Program
    {
        private const int TaskToRun = 27;

        private static void Main()
        {
            try
            {
                if (TaskToRun == 25)
                    RunTask25();
                else if (TaskToRun == 26)
                    RunTask26();
                else if (TaskToRun == 27)
                    RunTask27();
                else
                    Console.WriteLine("Укажи TaskToRun = 25, 26 или 27.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        private static void RunTask25()
        {
            var set = new MyHashSet<int>();

            Console.WriteLine("Пустое множество? " + set.IsEmpty());
            Console.WriteLine("Размер: " + set.Size());

            set.Add(10);
            set.Add(20);
            set.Add(30);
            set.Add(20);

            Console.WriteLine("После add(10,20,30,20):");
            PrintArray(set.ToArray());
            Console.WriteLine("Размер: " + set.Size());

            set.AddAll(new[] { 40, 50, 10, 60 });
            Console.WriteLine("После AddAll:");
            PrintArray(set.ToArray());

            Console.WriteLine("Contains(30): " + set.Contains(30));
            Console.WriteLine("Contains(99): " + set.Contains(99));
            Console.WriteLine("ContainsAll(10,20): " + set.ContainsAll(new[] { 10, 20 }));
            Console.WriteLine("ContainsAll(10,99): " + set.ContainsAll(new[] { 10, 99 }));

            set.Remove(20);
            Console.WriteLine("После Remove(20):");
            PrintArray(set.ToArray());

            set.RemoveAll(new[] { 10, 60, 777 });
            Console.WriteLine("После RemoveAll(10,60,777):");
            PrintArray(set.ToArray());

            set.RetainAll(new[] { 30, 40, 500 });
            Console.WriteLine("После RetainAll(30,40,500):");
            PrintArray(set.ToArray());

            int[] typed = set.ToArray((int[])null);
            Console.WriteLine("Typed ToArray:");
            PrintTypedArray(typed);

            set.Clear();
            Console.WriteLine("После Clear:");
            Console.WriteLine("Пустое множество? " + set.IsEmpty());
            Console.WriteLine("Размер: " + set.Size());
        }

        private static void RunTask26()
        {
            const string inputFile = "input26.txt";
            const string outputFile = "output26.txt";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            string[] lines = File.ReadAllLines(inputFile, Encoding.UTF8);
            var set = new MyHashSet<LineByWords>(Math.Max(16, lines.Length * 2));

            for (int i = 0; i < lines.Length; i++)
                set.Add(new LineByWords(lines[i]));

            LineByWords[] arr = set.ToArray((LineByWords[])null);
            Array.Sort(arr);

            using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                for (int i = 0; i < arr.Length; i++)
                    writer.WriteLine(arr[i].Original);
            }

            Console.WriteLine("Задача 26 выполнена.");
            Console.WriteLine("Уникальных строк по заданному сравнению: " + arr.Length);
            Console.WriteLine("Результат записан в output.txt");
        }

        private static void RunTask27()
        {
            const string inputFile = "input27.txt";
            const string outputFile = "output27.txt";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            string text = File.ReadAllText(inputFile, Encoding.UTF8);
            MatchCollection matches = Regex.Matches(text, "[A-Za-z]+");
            var set = new MyHashSet<string>(Math.Max(16, matches.Count * 2));

            for (int i = 0; i < matches.Count; i++)
                set.Add(matches[i].Value.ToLowerInvariant());

            string[] words = set.ToArray((string[])null);
            Array.Sort(words, StringComparer.Ordinal);

            using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                for (int i = 0; i < words.Length; i++)
                    writer.WriteLine(words[i]);
            }

            Console.WriteLine("Задача 27 выполнена.");
            Console.WriteLine("Уникальных слов: " + words.Length);
            Console.WriteLine("Результат записан в output.txt");
        }

        private static void PrintArray(object[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("(empty)");
                return;
            }

            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] + (i + 1 == arr.Length ? Environment.NewLine : " "));
        }

        private static void PrintTypedArray<T>(T[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("(empty)");
                return;
            }

            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] + (i + 1 == arr.Length ? Environment.NewLine : " "));
        }
    }
}