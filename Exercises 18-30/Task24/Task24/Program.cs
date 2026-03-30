using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Task24MapBenchmark
{
    public sealed class MyHashMap<K, V>
    {
        private sealed class Entry
        {
            public K Key;
            public V Value;
            public Entry Next;

            public Entry(K key, V value, Entry next)
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

        public bool ContainsKey(K key)
        {
            return FindEntry(key) != null;
        }

        public V Get(K key)
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

        public V Remove(K key)
        {
            int index = GetBucketIndex(key, table.Length);

            Entry current = table[index];
            Entry previous = null;
            var comparer = EqualityComparer<K>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Key, key))
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

        private Entry FindEntry(K key)
        {
            int index = GetBucketIndex(key, table.Length);
            Entry current = table[index];
            var comparer = EqualityComparer<K>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Key, key))
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
            int hash = ReferenceEquals(key, null) ? 0 : (key.GetHashCode() & 0x7fffffff);
            return hash % capacity;
        }
    }

    public sealed class MyTreeMap<K, V> where K : IComparable<K>
    {
        private const bool Red = true;
        private const bool Black = false;

        private sealed class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;
            public bool Color;
            public int Count;

            public Node(K key, V value, bool color, int count)
            {
                Key = key;
                Value = value;
                Color = color;
                Count = count;
            }
        }

        private Node root;

        public int Size()
        {
            return Size(root);
        }

        public bool ContainsKey(K key)
        {
            Node x = root;
            while (x != null)
            {
                int cmp = key.CompareTo(x.Key);
                if (cmp < 0) x = x.Left;
                else if (cmp > 0) x = x.Right;
                else return true;
            }

            return false;
        }

        public V Get(K key)
        {
            Node x = root;
            while (x != null)
            {
                int cmp = key.CompareTo(x.Key);
                if (cmp < 0) x = x.Left;
                else if (cmp > 0) x = x.Right;
                else return x.Value;
            }

            return default(V);
        }

        public void Put(K key, V value)
        {
            root = Put(root, key, value);
            root.Color = Black;
        }

        public void Remove(K key)
        {
            if (!ContainsKey(key))
                return;

            if (!IsRed(root.Left) && !IsRed(root.Right))
                root.Color = Red;

            root = Remove(root, key);

            if (root != null)
                root.Color = Black;
        }

        private Node Put(Node h, K key, V value)
        {
            if (h == null)
                return new Node(key, value, Red, 1);

            int cmp = key.CompareTo(h.Key);
            if (cmp < 0) h.Left = Put(h.Left, key, value);
            else if (cmp > 0) h.Right = Put(h.Right, key, value);
            else h.Value = value;

            if (IsRed(h.Right) && !IsRed(h.Left)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColors(h);

            h.Count = 1 + Size(h.Left) + Size(h.Right);
            return h;
        }

        private Node Remove(Node h, K key)
        {
            if (key.CompareTo(h.Key) < 0)
            {
                if (h.Left != null)
                {
                    if (!IsRed(h.Left) && !IsRed(h.Left.Left))
                        h = MoveRedLeft(h);

                    h.Left = Remove(h.Left, key);
                }
            }
            else
            {
                if (IsRed(h.Left))
                    h = RotateRight(h);

                if (key.CompareTo(h.Key) == 0 && h.Right == null)
                    return null;

                if (h.Right != null)
                {
                    if (!IsRed(h.Right) && !IsRed(h.Right.Left))
                        h = MoveRedRight(h);

                    if (key.CompareTo(h.Key) == 0)
                    {
                        Node x = Min(h.Right);
                        h.Key = x.Key;
                        h.Value = x.Value;
                        h.Right = DeleteMin(h.Right);
                    }
                    else
                    {
                        h.Right = Remove(h.Right, key);
                    }
                }
            }

            return Balance(h);
        }

        private Node DeleteMin(Node h)
        {
            if (h.Left == null)
                return null;

            if (!IsRed(h.Left) && !IsRed(h.Left.Left))
                h = MoveRedLeft(h);

            h.Left = DeleteMin(h.Left);
            return Balance(h);
        }

        private Node Min(Node h)
        {
            while (h.Left != null)
                h = h.Left;

            return h;
        }

        private Node RotateLeft(Node h)
        {
            Node x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.Color = h.Color;
            h.Color = Red;
            x.Count = h.Count;
            h.Count = 1 + Size(h.Left) + Size(h.Right);
            return x;
        }

        private Node RotateRight(Node h)
        {
            Node x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.Color = h.Color;
            h.Color = Red;
            x.Count = h.Count;
            h.Count = 1 + Size(h.Left) + Size(h.Right);
            return x;
        }

        private void FlipColors(Node h)
        {
            h.Color = !h.Color;
            if (h.Left != null) h.Left.Color = !h.Left.Color;
            if (h.Right != null) h.Right.Color = !h.Right.Color;
        }

        private Node MoveRedLeft(Node h)
        {
            FlipColors(h);
            if (h.Right != null && IsRed(h.Right.Left))
            {
                h.Right = RotateRight(h.Right);
                h = RotateLeft(h);
                FlipColors(h);
            }
            return h;
        }

        private Node MoveRedRight(Node h)
        {
            FlipColors(h);
            if (h.Left != null && IsRed(h.Left.Left))
            {
                h = RotateRight(h);
                FlipColors(h);
            }
            return h;
        }

        private Node Balance(Node h)
        {
            if (IsRed(h.Right)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColors(h);
            h.Count = 1 + Size(h.Left) + Size(h.Right);
            return h;
        }

        private bool IsRed(Node x)
        {
            return x != null && x.Color == Red;
        }

        private int Size(Node x)
        {
            return x == null ? 0 : x.Count;
        }
    }

    internal enum OperationKind
    {
        Put,
        Get,
        Remove
    }

    internal sealed class BenchmarkResult
    {
        public OperationKind Operation { get; set; }
        public int Size { get; set; }
        public int Run { get; set; }
        public bool Success { get; set; }
        public double HashMapMilliseconds { get; set; }
        public double TreeMapMilliseconds { get; set; }
        public string Status { get; set; }
    }

    internal static class Program
    {
        private static readonly int[] Sizes = { 1000, 10000, 100000 };
        private const int RunsPerCase = 20;

        private static void Main()
        {
            var allResults = new List<BenchmarkResult>();

            foreach (int size in Sizes)
            {
                for (int run = 1; run <= RunsPerCase; run++)
                {
                    allResults.Add(RunBenchmark(OperationKind.Put, size, run));
                    allResults.Add(RunBenchmark(OperationKind.Get, size, run));
                    allResults.Add(RunBenchmark(OperationKind.Remove, size, run));
                }
            }

            WriteOperationCsv(allResults, OperationKind.Put, "benchmark_put.csv");
            WriteOperationCsv(allResults, OperationKind.Get, "benchmark_get.csv");
            WriteOperationCsv(allResults, OperationKind.Remove, "benchmark_remove.csv");
            WriteAverageCsv(allResults, "benchmark_average.csv");
            WriteSummary(allResults, "benchmark_summary.txt");

            Console.WriteLine("Готово.");
        }

        private static BenchmarkResult RunBenchmark(OperationKind operation, int size, int run)
        {
            try
            {
                int[] keys = CreateShuffledKeys(size, 123456789 + size * 17 + run * 31 + (int)operation * 1000003);

                double hashMs;
                double treeMs;

                switch (operation)
                {
                    case OperationKind.Put:
                        hashMs = MeasurePutHashMap(keys);
                        treeMs = MeasurePutTreeMap(keys);
                        break;
                    case OperationKind.Get:
                        hashMs = MeasureGetHashMap(keys);
                        treeMs = MeasureGetTreeMap(keys);
                        break;
                    default:
                        hashMs = MeasureRemoveHashMap(keys);
                        treeMs = MeasureRemoveTreeMap(keys);
                        break;
                }

                return new BenchmarkResult
                {
                    Operation = operation,
                    Size = size,
                    Run = run,
                    Success = true,
                    HashMapMilliseconds = hashMs,
                    TreeMapMilliseconds = treeMs,
                    Status = "OK"
                };
            }
            catch (OutOfMemoryException)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return new BenchmarkResult
                {
                    Operation = operation,
                    Size = size,
                    Run = run,
                    Success = false,
                    HashMapMilliseconds = -1,
                    TreeMapMilliseconds = -1,
                    Status = "SKIPPED_OOM"
                };
            }
        }

        private static double MeasurePutHashMap(int[] keys)
        {
            var map = new MyHashMap<int, int>(EstimateHashCapacity(keys.Length));
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasurePutTreeMap(int[] keys)
        {
            var map = new MyTreeMap<int, int>();
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureGetHashMap(int[] keys)
        {
            var map = new MyHashMap<int, int>(EstimateHashCapacity(keys.Length));
            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            long checksum = 0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                checksum += map.Get(keys[i]);

            sw.Stop();
            if (checksum == long.MinValue)
                Console.WriteLine(checksum);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureGetTreeMap(int[] keys)
        {
            var map = new MyTreeMap<int, int>();
            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            long checksum = 0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                checksum += map.Get(keys[i]);

            sw.Stop();
            if (checksum == long.MinValue)
                Console.WriteLine(checksum);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveHashMap(int[] keys)
        {
            var map = new MyHashMap<int, int>(EstimateHashCapacity(keys.Length));
            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                map.Remove(keys[i]);

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveTreeMap(int[] keys)
        {
            var map = new MyTreeMap<int, int>();
            for (int i = 0; i < keys.Length; i++)
                map.Put(keys[i], keys[i]);

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++)
                map.Remove(keys[i]);

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        private static int[] CreateShuffledKeys(int size, int seed)
        {
            var keys = new int[size];
            for (int i = 0; i < size; i++)
                keys[i] = i + 1;

            var rng = new Random(seed);
            for (int i = size - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                int tmp = keys[i];
                keys[i] = keys[j];
                keys[j] = tmp;
            }

            return keys;
        }

        private static int EstimateHashCapacity(int itemCount)
        {
            long need = (long)Math.Ceiling(itemCount / 0.75);
            int cap = 16;
            while (cap < need && cap < int.MaxValue / 2)
                cap <<= 1;
            return cap;
        }

        private static void WriteOperationCsv(List<BenchmarkResult> results, OperationKind operation, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine("Operation,Size,Run,Status,HashMapMs,TreeMapMs");

                foreach (var r in results)
                {
                    if (r.Operation != operation)
                        continue;

                    writer.WriteLine(
                        operation + "," +
                        r.Size + "," +
                        r.Run + "," +
                        r.Status + "," +
                        FormatDouble(r.HashMapMilliseconds) + "," +
                        FormatDouble(r.TreeMapMilliseconds));
                }
            }
        }

        private static void WriteAverageCsv(List<BenchmarkResult> results, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine("Operation,Size,SuccessfulRuns,HashMapAvgMs,TreeMapAvgMs,FasterStructure");

                foreach (OperationKind op in Enum.GetValues(typeof(OperationKind)))
                {
                    foreach (int size in Sizes)
                    {
                        double hashSum = 0;
                        double treeSum = 0;
                        int cnt = 0;

                        foreach (var r in results)
                        {
                            if (r.Operation == op && r.Size == size && r.Success)
                            {
                                hashSum += r.HashMapMilliseconds;
                                treeSum += r.TreeMapMilliseconds;
                                cnt++;
                            }
                        }

                        if (cnt == 0)
                        {
                            writer.WriteLine(op + "," + size + ",0,NA,NA,NA");
                        }
                        else
                        {
                            double hashAvg = hashSum / cnt;
                            double treeAvg = treeSum / cnt;
                            string faster = hashAvg < treeAvg ? "HashMap" : (treeAvg < hashAvg ? "TreeMap" : "Equal");

                            writer.WriteLine(
                                op + "," +
                                size + "," +
                                cnt + "," +
                                FormatDouble(hashAvg) + "," +
                                FormatDouble(treeAvg) + "," +
                                faster);
                        }
                    }
                }
            }
        }

        private static void WriteSummary(List<BenchmarkResult> results, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine("Сравнение MyHashMap и MyTreeMap");
                writer.WriteLine();

                foreach (OperationKind op in Enum.GetValues(typeof(OperationKind)))
                {
                    writer.WriteLine("Операция: " + op);

                    foreach (int size in Sizes)
                    {
                        double hashSum = 0;
                        double treeSum = 0;
                        int cnt = 0;
                        int skipped = 0;

                        foreach (var r in results)
                        {
                            if (r.Operation == op && r.Size == size)
                            {
                                if (r.Success)
                                {
                                    hashSum += r.HashMapMilliseconds;
                                    treeSum += r.TreeMapMilliseconds;
                                    cnt++;
                                }
                                else
                                {
                                    skipped++;
                                }
                            }
                        }

                        if (cnt == 0)
                        {
                            writer.WriteLine("  Размер " + size + ": нет успешных запусков, пропущено " + skipped);
                        }
                        else
                        {
                            double hashAvg = hashSum / cnt;
                            double treeAvg = treeSum / cnt;
                            string faster = hashAvg < treeAvg ? "MyHashMap" : (treeAvg < hashAvg ? "MyTreeMap" : "Одинаково");

                            writer.WriteLine(
                                "  Размер " + size +
                                ": успешных запусков = " + cnt +
                                ", пропущено = " + skipped +
                                ", среднее Put/Get/Remove время для HashMap = " + FormatDouble(hashAvg) +
                                " мс, для TreeMap = " + FormatDouble(treeAvg) +
                                " мс, быстрее: " + faster);
                        }
                    }

                    writer.WriteLine();
                }

                writer.WriteLine("Ожидаемый общий вывод:");
                writer.WriteLine("1. MyHashMap обычно быстрее для put/get/remove в среднем случае за счёт O(1) по хешу.");
                writer.WriteLine("2. MyTreeMap обычно медленнее из-за O(log n), но даёт упорядоченность ключей и детерминированную асимптотику.");
                writer.WriteLine("3. Для очень больших размеров структура на хеш-таблице может упираться в память из-за большого массива бакетов.");
                writer.WriteLine("4. Для очень больших размеров в C# значения 10^7 и 10^8 могут оказаться непрактичными для одного процесса.");
            }
        }

        private static string FormatDouble(double value)
        {
            return value < 0 ? "NA" : value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}