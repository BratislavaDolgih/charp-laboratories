using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Task23VariablesMap
{
    public enum VariableType
    {
        Int,
        Float,
        Double
    }

    public sealed class VariableInfo
    {
        public VariableType Type { get; }
        public string Value { get; }

        public VariableInfo(VariableType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return GetTypeName(Type) + "(" + Value + ")";
        }

        public static string GetTypeName(VariableType type)
        {
            switch (type)
            {
                case VariableType.Int:
                    return "int";
                case VariableType.Float:
                    return "float";
                default:
                    return "double";
            }
        }
    }

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
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            if (float.IsNaN(loadFactor) || loadFactor <= 0f)
                throw new ArgumentOutOfRangeException(nameof(loadFactor));

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

            return EqualityComparer<K>.Default.Equals(leftKey, (K)rightKey);
        }
    }

    internal static class Program
    {
        private static readonly Regex StatementRegex = new Regex(@"[^;]*;", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex DefinitionRegex = new Regex(@"^\s*([A-Za-z_][A-Za-z0-9_]*)\s+([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(\d+)\s*;\s*$", RegexOptions.Singleline | RegexOptions.Compiled);

        private static void Main()
        {
            const string inputFile = "input.txt";
            const string outputFile = "output.txt";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            string text = File.ReadAllText(inputFile);
            var map = new MyHashMap<string, VariableInfo>();
            var messages = new List<string>();

            int lastEnd = 0;
            MatchCollection statements = StatementRegex.Matches(text);

            foreach (Match statement in statements)
            {
                if (!string.IsNullOrWhiteSpace(text.Substring(lastEnd, statement.Index - lastEnd)))
                {
                    string garbage = text.Substring(lastEnd, statement.Index - lastEnd).Trim();
                    messages.Add("Некорректный фрагмент: " + NormalizeWhitespace(garbage));
                }

                string raw = statement.Value;
                Match parsed = DefinitionRegex.Match(raw);

                if (!parsed.Success)
                {
                    messages.Add("Некорректное определение: " + NormalizeWhitespace(raw));
                }
                else
                {
                    string typeName = parsed.Groups[1].Value;
                    string variableName = parsed.Groups[2].Value;
                    string value = parsed.Groups[3].Value;

                    VariableType variableType;
                    if (!TryParseType(typeName, out variableType))
                    {
                        messages.Add("Некорректный тип в определении: " + NormalizeWhitespace(raw));
                    }
                    else if (map.ContainsKey(variableName))
                    {
                        messages.Add("Переопределение переменной: " + variableName);
                    }
                    else
                    {
                        map.Put(variableName, new VariableInfo(variableType, value));
                    }
                }

                lastEnd = statement.Index + statement.Length;
            }

            if (!string.IsNullOrWhiteSpace(text.Substring(lastEnd)))
            {
                messages.Add("Некорректный фрагмент: " + NormalizeWhitespace(text.Substring(lastEnd).Trim()));
            }

            List<MyHashMap<string, VariableInfo>.Entry> entries = map.EntryList();
            entries.Sort((a, b) => string.CompareOrdinal(a.Key, b.Key));

            using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                foreach (var entry in entries)
                {
                    writer.WriteLine(VariableInfo.GetTypeName(entry.Value.Type) + " => " + entry.Key + "(" + entry.Value.Value + ")");
                }
            }

            Console.WriteLine("Корректных определений сохранено: " + map.Size());
            Console.WriteLine("Результат записан в output.txt");

            if (messages.Count == 0)
            {
                Console.WriteLine("Ошибок и переопределений не обнаружено.");
            }
            else
            {
                Console.WriteLine("Сообщения:");
                foreach (string message in messages)
                    Console.WriteLine(message);
            }
        }

        private static bool TryParseType(string typeName, out VariableType type)
        {
            switch (typeName)
            {
                case "int":
                    type = VariableType.Int;
                    return true;
                case "float":
                    type = VariableType.Float;
                    return true;
                case "double":
                    type = VariableType.Double;
                    return true;
                default:
                    type = default(VariableType);
                    return false;
            }
        }

        private static string NormalizeWhitespace(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            return Regex.Replace(s, @"\s+", " ").Trim();
        }
    }
}