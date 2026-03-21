using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HashMapTask
{
    public class MyHashMap<K, V>
    {
        private Entry[] table;
        private int size;
        private readonly float loadFactor;

        private const int DefaultInitialCapacity = 16;
        private const float DefaultLoadFactor = 0.75f;

        public MyHashMap()
            : this(DefaultInitialCapacity, DefaultLoadFactor)
        {
        }

        public MyHashMap(int initialCapacity)
            : this(initialCapacity, DefaultLoadFactor)
        {
        }

        public MyHashMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Начальная ёмкость должна быть больше 0.");

            if (float.IsNaN(loadFactor) || loadFactor <= 0f)
                throw new ArgumentOutOfRangeException(nameof(loadFactor), "Коэффициент загрузки должен быть положительным числом.");

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

        public bool ContainsValue(object value)
        {
            V targetValue = ConvertObjectToValue(value);

            var comparer = EqualityComparer<V>.Default;

            for (int i = 0; i < table.Length; i++)
            {
                Entry current = table[i];
                while (current != null)
                {
                    if (comparer.Equals(current.Value, targetValue))
                        return true;

                    current = current.Next;
                }
            }

            return false;
        }

        public HashSet<K> KeySet()
        {
            var result = new HashSet<K>(EqualityComparer<K>.Default);

            for (int i = 0; i < table.Length; i++)
            {
                Entry current = table[i];
                while (current != null)
                {
                    result.Add(current.Key);
                    current = current.Next;
                }
            }

            return result;
        }

        public HashSet<Entry> EntrySet()
        {
            var result = new HashSet<Entry>();

            for (int i = 0; i < table.Length; i++)
            {
                Entry current = table[i];
                while (current != null)
                {
                    result.Add(new Entry(current.Key, current.Value, null));
                    current = current.Next;
                }
            }

            return result;
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

            Entry newEntry = new Entry(key, value, table[index]);
            table[index] = newEntry;
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
            int hash = GetNonNegativeHashCode(key);
            return hash % capacity;
        }

        private int GetBucketIndexFromObject(object key, int capacity)
        {
            int hash = GetNonNegativeHashCodeFromObject(key);
            return hash % capacity;
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

            return EqualityComparer<K>.Default.Equals(leftKey, (K)rightKey);
        }

        private V ConvertObjectToValue(object value)
        {
            if (value == null)
                return default(V);

            if (value is V typedValue)
                return typedValue;

            throw new ArgumentException("Переданный объект нельзя преобразовать к типу значения отображения.");
        }

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

            public override bool Equals(object obj)
            {
                Entry other = obj as Entry;
                if (other == null)
                    return false;

                return EqualityComparer<K>.Default.Equals(Key, other.Key)
                       && EqualityComparer<V>.Default.Equals(Value, other.Value);
            }

            public override int GetHashCode()
            {
                int keyHash = ReferenceEquals(Key, null) ? 0 : EqualityComparer<K>.Default.GetHashCode(Key);
                int valueHash = ReferenceEquals(Value, null) ? 0 : EqualityComparer<V>.Default.GetHashCode(Value);

                unchecked
                {
                    return (keyHash * 397) ^ valueHash;
                }
            }

            public override string ToString()
            {
                return string.Format("{0} = {1}", Key, Value);
            }
        }
    }

    internal class Program
    {
        private static void Main()
        {
            const string inputFileName = "input.txt";

            if (!File.Exists(inputFileName))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            var tagCounts = new MyHashMap<string, int>();
            var regex = new Regex(@"</?[A-Za-z][A-Za-z0-9]*>", RegexOptions.Compiled);

            try
            {
                foreach (string line in File.ReadLines(inputFileName))
                {
                    MatchCollection matches = regex.Matches(line);

                    foreach (Match match in matches)
                    {
                        string normalizedTag = NormalizeTag(match.Value);

                        if (tagCounts.ContainsKey(normalizedTag))
                        {
                            int currentCount = tagCounts.Get(normalizedTag);
                            tagCounts.Put(normalizedTag, currentCount + 1);
                        }
                        else
                        {
                            tagCounts.Put(normalizedTag, 1);
                        }
                    }
                }

                Console.WriteLine("Количество вхождений тегов:");
                foreach (var entry in tagCounts.EntrySet())
                {
                    Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке файла: " + ex.Message);
            }
        }

        private static string NormalizeTag(string tag)
        {
            // Убираем < и >
            string inner = tag.Substring(1, tag.Length - 2);

            // Убираем ведущий /
            if (inner.Length > 0 && inner[0] == '/')
                inner = inner.Substring(1);

            // Приводим к нижнему регистру
            return inner.ToLowerInvariant();
        }
    }
}