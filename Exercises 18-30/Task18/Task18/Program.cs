using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeMapImplementation
{
    // Делегат для компаратора
    public delegate int TreeMapComparator<K>(K a, K b);

    // Класс для представления пары ключ-значение
    public class Entry<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }

        public Entry(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Key}={Value}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Entry<K, V> other)
            {
                return EqualityComparer<K>.Default.Equals(Key, other.Key) &&
                       EqualityComparer<V>.Default.Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                int keyHash = 0;
                if (Key != null)
                {
                    keyHash = Key.GetHashCode();
                }

                int valueHash = 0;
                if (Value != null)
                {
                    valueHash = Value.GetHashCode();
                }

                hash = hash * 23 + keyHash;
                hash = hash * 23 + valueHash;

                return hash;
            }
        }

    }

    // Узел обычного двоичного дерева поиска
    internal class TreeNode<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public TreeNode<K, V> Left { get; set; }
        public TreeNode<K, V> Right { get; set; }
        public TreeNode<K, V> Parent { get; set; }

        public TreeNode(K key, V value, TreeNode<K, V> parent = null)
        {
            Key = key;
            Value = value;
            Parent = parent;
        }
    }

    // Основной класс MyTreeMap (на основе обычного BST)
    public class MyTreeMap<K, V>
    {
        private TreeMapComparator<K> comparator;
        private TreeNode<K, V> root;
        private int size;

        // 1. Конструктор с естественным порядком
        public MyTreeMap()
        {
            if (!typeof(IComparable<K>).IsAssignableFrom(typeof(K)) &&
                !typeof(IComparable).IsAssignableFrom(typeof(K)))
            {
                throw new InvalidOperationException(
                    $"Тип {typeof(K).Name} не реализует IComparable. Используйте конструктор с компаратором.");
            }

            this.comparator = (a, b) =>
            {
                if (a is IComparable<K> compGeneric)
                    return compGeneric.CompareTo(b);
                if (a is IComparable comp)
                    return comp.CompareTo(b);
                throw new InvalidOperationException("Невозможно сравнить элементы");
            };
            this.root = null;
            this.size = 0;
        }

        // 2. Конструктор с пользовательским компаратором
        public MyTreeMap(TreeMapComparator<K> comp)
        {
            this.comparator = comp ?? throw new ArgumentNullException(nameof(comp));
            this.root = null;
            this.size = 0;
        }

        // 3. Clear - удаление всех элементов
        public void Clear()
        {
            root = null;
            size = 0;
        }

        // 4. ContainsKey - проверка наличия ключа
        public bool ContainsKey(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return GetNode(key) != null;
        }

        // 5. ContainsValue - проверка наличия значения
        public bool ContainsValue(V value)
        {
            return ContainsValueRecursive(root, value);
        }

        private bool ContainsValueRecursive(TreeNode<K, V> node, V value)
        {
            if (node == null) return false;

            if (EqualityComparer<V>.Default.Equals(node.Value, value))
                return true;

            return ContainsValueRecursive(node.Left, value) ||
                   ContainsValueRecursive(node.Right, value);
        }

        // 6. EntrySet - получение множества всех пар
        public HashSet<Entry<K, V>> EntrySet()
        {
            var set = new HashSet<Entry<K, V>>();
            InOrderTraversal(root, node => set.Add(new Entry<K, V>(node.Key, node.Value)));
            return set;
        }

        // 7. Get - получение значения по ключу
        public V Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetNode(key);
            return node != null ? node.Value : default(V);
        }

        // 8. IsEmpty - проверка на пустоту
        public bool IsEmpty()
        {
            return size == 0;
        }

        // 9. KeySet - получение множества ключей
        public HashSet<K> KeySet()
        {
            var set = new HashSet<K>();
            InOrderTraversal(root, node => set.Add(node.Key));
            return set;
        }

        // 10. Put - добавление пары ключ-значение
        public V Put(K key, V value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (root == null)
            {
                root = new TreeNode<K, V>(key, value);
                size = 1;
                return default(V);
            }

            TreeNode<K, V> parent = null;
            TreeNode<K, V> current = root;
            int cmp = 0;

            // Поиск места для вставки
            while (current != null)
            {
                parent = current;
                cmp = comparator(key, current.Key);

                if (cmp < 0)
                    current = current.Left;
                else if (cmp > 0)
                    current = current.Right;
                else
                {
                    // Ключ уже существует - обновляем значение
                    V oldValue = current.Value;
                    current.Value = value;
                    return oldValue;
                }
            }

            // Создание нового узла
            var newNode = new TreeNode<K, V>(key, value, parent);
            if (cmp < 0)
                parent.Left = newNode;
            else
                parent.Right = newNode;

            size++;
            return default(V);
        }

        // 11. Remove - удаление по ключу
        public V Remove(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetNode(key);
            if (node == null)
                return default(V);

            V oldValue = node.Value;
            DeleteNode(node);
            return oldValue;
        }

        // 12. Size - получение количества элементов
        public int Size()
        {
            return size;
        }

        // 13. FirstKey - первый ключ
        public K FirstKey()
        {
            if (root == null)
                throw new InvalidOperationException("Отображение пусто");
            return GetMin(root).Key;
        }

        // 14. LastKey - последний ключ
        public K LastKey()
        {
            if (root == null)
                throw new InvalidOperationException("Отображение пусто");
            return GetMax(root).Key;
        }

        // 15. HeadMap - все элементы с ключом < end
        public MyTreeMap<K, V> HeadMap(K end)
        {
            if (end == null)
                throw new ArgumentNullException(nameof(end));

            var result = new MyTreeMap<K, V>(comparator);
            InOrderTraversal(root, node =>
            {
                if (comparator(node.Key, end) < 0)
                    result.Put(node.Key, node.Value);
            });
            return result;
        }

        // 16. SubMap - элементы с start <= ключ < end
        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));
            if (end == null)
                throw new ArgumentNullException(nameof(end));

            var result = new MyTreeMap<K, V>(comparator);
            InOrderTraversal(root, node =>
            {
                if (comparator(node.Key, start) >= 0 && comparator(node.Key, end) < 0)
                    result.Put(node.Key, node.Value);
            });
            return result;
        }

        // 17. TailMap - все элементы с ключом > start
        public MyTreeMap<K, V> TailMap(K start)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));

            var result = new MyTreeMap<K, V>(comparator);
            InOrderTraversal(root, node =>
            {
                if (comparator(node.Key, start) > 0)
                    result.Put(node.Key, node.Value);
            });
            return result;
        }

        // 18. LowerEntry - пара с ключом < key
        public Entry<K, V> LowerEntry(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetLowerNode(key);
            return node != null ? new Entry<K, V>(node.Key, node.Value) : null;
        }

        // 19. FloorEntry - пара с ключом <= key
        public Entry<K, V> FloorEntry(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetFloorNode(key);
            return node != null ? new Entry<K, V>(node.Key, node.Value) : null;
        }

        // 20. HigherEntry - пара с ключом > key
        public Entry<K, V> HigherEntry(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetHigherNode(key);
            return node != null ? new Entry<K, V>(node.Key, node.Value) : null;
        }

        // 21. CeilingEntry - пара с ключом >= key
        public Entry<K, V> CeilingEntry(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetCeilingNode(key);
            return node != null ? new Entry<K, V>(node.Key, node.Value) : null;
        }

        // 22-25. Методы для получения ключей
        public K LowerKey(K key) => LowerEntry(key).Key;
        public K FloorKey(K key) => FloorEntry(key).Key;
        public K HigherKey(K key) => HigherEntry(key).Key;
        public K CeilingKey(K key) => CeilingEntry(key).Key;

        // 26. PollFirstEntry - удаление и возврат первого элемента
        public Entry<K, V> PollFirstEntry()
        {
            if (root == null)
                return null;

            var min = GetMin(root);
            var entry = new Entry<K, V>(min.Key, min.Value);
            DeleteNode(min);
            return entry;
        }

        // 27. PollLastEntry - удаление и возврат последнего элемента
        public Entry<K, V> PollLastEntry()
        {
            if (root == null)
                return null;

            var max = GetMax(root);
            var entry = new Entry<K, V>(max.Key, max.Value);
            DeleteNode(max);
            return entry;
        }

        // 28. FirstEntry - первый элемент без удаления
        public Entry<K, V> FirstEntry()
        {
            if (root == null)
                return null;
            var min = GetMin(root);
            return new Entry<K, V>(min.Key, min.Value);
        }

        // 29. LastEntry - последний элемент без удаления
        public Entry<K, V> LastEntry()
        {
            if (root == null)
                return null;
            var max = GetMax(root);
            return new Entry<K, V>(max.Key, max.Value);
        }

        // ============ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ============

        private TreeNode<K, V> GetNode(K key)
        {
            var current = root;
            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp < 0)
                    current = current.Left;
                else if (cmp > 0)
                    current = current.Right;
                else
                    return current;
            }
            return null;
        }

        private TreeNode<K, V> GetMin(TreeNode<K, V> node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private TreeNode<K, V> GetMax(TreeNode<K, V> node)
        {
            while (node.Right != null)
                node = node.Right;
            return node;
        }

        private TreeNode<K, V> GetLowerNode(K key)
        {
            TreeNode<K, V> current = root;
            TreeNode<K, V> result = null;

            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp <= 0)
                    current = current.Left;
                else
                {
                    result = current;
                    current = current.Right;
                }
            }
            return result;
        }

        private TreeNode<K, V> GetFloorNode(K key)
        {
            TreeNode<K, V> current = root;
            TreeNode<K, V> result = null;

            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp < 0)
                    current = current.Left;
                else if (cmp > 0)
                {
                    result = current;
                    current = current.Right;
                }
                else
                    return current;
            }
            return result;
        }

        private TreeNode<K, V> GetHigherNode(K key)
        {
            TreeNode<K, V> current = root;
            TreeNode<K, V> result = null;

            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp >= 0)
                    current = current.Right;
                else
                {
                    result = current;
                    current = current.Left;
                }
            }
            return result;
        }

        private TreeNode<K, V> GetCeilingNode(K key)
        {
            TreeNode<K, V> current = root;
            TreeNode<K, V> result = null;

            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp < 0)
                {
                    result = current;
                    current = current.Left;
                }
                else if (cmp > 0)
                    current = current.Right;
                else
                    return current;
            }
            return result;
        }

        private void InOrderTraversal(TreeNode<K, V> node, Action<TreeNode<K, V>> action)
        {
            if (node == null) return;
            InOrderTraversal(node.Left, action);
            action(node);
            InOrderTraversal(node.Right, action);
        }

        // Удаление узла из BST (стандартный алгоритм)
        private void DeleteNode(TreeNode<K, V> node)
        {
            size--;

            // Случай 1: Узел - лист (нет детей)
            if (node.Left == null && node.Right == null)
            {
                if (node.Parent == null)
                {
                    root = null;
                }
                else if (node.Parent.Left == node)
                {
                    node.Parent.Left = null;
                }
                else
                {
                    node.Parent.Right = null;
                }
            }
            // Случай 2: Узел имеет только правого ребёнка
            else if (node.Left == null)
            {
                if (node.Parent == null)
                {
                    root = node.Right;
                    root.Parent = null;
                }
                else if (node.Parent.Left == node)
                {
                    node.Parent.Left = node.Right;
                    node.Right.Parent = node.Parent;
                }
                else
                {
                    node.Parent.Right = node.Right;
                    node.Right.Parent = node.Parent;
                }
            }
            // Случай 3: Узел имеет только левого ребёнка
            else if (node.Right == null)
            {
                if (node.Parent == null)
                {
                    root = node.Left;
                    root.Parent = null;
                }
                else if (node.Parent.Left == node)
                {
                    node.Parent.Left = node.Left;
                    node.Left.Parent = node.Parent;
                }
                else
                {
                    node.Parent.Right = node.Left;
                    node.Left.Parent = node.Parent;
                }
            }
            // Случай 4: Узел имеет двух детей
            else
            {
                // Находим минимальный узел в правом поддереве (преемник)
                var successor = GetMin(node.Right);

                // Копируем данные преемника в удаляемый узел
                node.Key = successor.Key;
                node.Value = successor.Value;

                size++; 
                DeleteNode(successor);
            }
        }

        public void PrintTree()
        {
            if (root == null)
            {
                Console.WriteLine("(пустое дерево)");
                return;
            }
            PrintTreeRecursive(root, "", true);
        }

        private void PrintTreeRecursive(TreeNode<K, V> node, string indent, bool last)
        {
            if (node != null)
            {
                Console.Write(indent);
                if (last)
                {
                    Console.Write("└─");
                    indent += "  ";
                }
                else
                {
                    Console.Write("├─");
                    indent += "│ ";
                }
                Console.WriteLine($"{node.Key}={node.Value}");

                PrintTreeRecursive(node.Left, indent, false);
                PrintTreeRecursive(node.Right, indent, true);
            }
        }
    }

    // ============ ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ ============
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ДЕМОНСТРАЦИЯ MyTreeMap (BST) ===\n");

            Example1_BasicOperations();

            Example2_CustomComparator();

            Example3_NavigationMethods();

            Example4_RangeMaps();

            Example5_ErrorHandling();

            Example6_TreeVisualization();

            Console.WriteLine("\n=== ПРОГРАММА ЗАВЕРШЕНА ===");
        }

        static void Example1_BasicOperations()
        {
            Console.WriteLine("--- Пример 1: Основные операции ---");
            var map = new MyTreeMap<int, string>();

            map.Put(5, "Пять");
            map.Put(2, "Два");
            map.Put(8, "Восемь");
            map.Put(1, "Один");
            map.Put(3, "Три");
            map.Put(7, "Семь");
            map.Put(9, "Девять");

            Console.WriteLine($"Размер: {map.Size()}");
            Console.WriteLine($"Пусто: {map.IsEmpty()}");
            Console.WriteLine($"Содержит ключ 3: {map.ContainsKey(3)}");
            Console.WriteLine($"Содержит значение 'Два': {map.ContainsValue("Два")}");
            Console.WriteLine($"Получить значение по ключу 5: {map.Get(5)}");

            Console.WriteLine("\nВсе ключи (отсортированные):");
            foreach (var key in map.KeySet().OrderBy(x => x))
                Console.WriteLine($"  {key}");

            Console.WriteLine("\nВсе пары (отсортированные):");
            foreach (var entry in map.EntrySet().OrderBy(e => e.Key))
                Console.WriteLine($"  {entry}");

            // Обновление значения
            var oldValue = map.Put(3, "Три (обновлено)");
            Console.WriteLine($"\nОбновление ключа 3: старое значение = '{oldValue}'");
            Console.WriteLine($"Новое значение: {map.Get(3)}");

            // Удаление
            var removed = map.Remove(2);
            Console.WriteLine($"\nУдалён ключ 2, значение: '{removed}'");
            Console.WriteLine($"Размер после удаления: {map.Size()}");

            Console.WriteLine();
        }

        static void Example2_CustomComparator()
        {
            Console.WriteLine("--- Пример 2: Пользовательский компаратор ---");

            // Компаратор для обратного порядка
            TreeMapComparator<int> reverseComparator = (a, b) => b.CompareTo(a);
            var map = new MyTreeMap<int, string>(reverseComparator);

            map.Put(10, "Десять");
            map.Put(5, "Пять");
            map.Put(15, "Пятнадцать");
            map.Put(3, "Три");
            map.Put(20, "Двадцать");

            Console.WriteLine("Ключи в обратном порядке:");
            foreach (var key in map.KeySet().OrderByDescending(x => x))
                Console.WriteLine($"  {key} -> {map.Get(key)}");

            Console.WriteLine($"\nПервый ключ (max): {map.FirstKey()}");
            Console.WriteLine($"Последний ключ (min): {map.LastKey()}");

            Console.WriteLine();
        }

        static void Example3_NavigationMethods()
        {
            Console.WriteLine("--- Пример 3: Навигационные методы ---");
            var map = new MyTreeMap<int, string>();

            int[] keys = { 10, 20, 30, 40, 50, 60, 70 };
            foreach (var k in keys)
                map.Put(k, $"Значение_{k}");

            int searchKey = 35;
            Console.WriteLine($"Поиск относительно ключа {searchKey}:");
            Console.WriteLine($"  Lower (<{searchKey}): {map.LowerEntry(searchKey)}");
            Console.WriteLine($"  Floor (<={searchKey}): {map.FloorEntry(searchKey)}");
            Console.WriteLine($"  Ceiling (>={searchKey}): {map.CeilingEntry(searchKey)}");
            Console.WriteLine($"  Higher (>{searchKey}): {map.HigherEntry(searchKey)}");

            searchKey = 40;
            Console.WriteLine($"\nПоиск относительно существующего ключа {searchKey}:");
            Console.WriteLine($"  Lower (<{searchKey}): {map.LowerKey(searchKey)}");
            Console.WriteLine($"  Floor (≤{searchKey}): {map.FloorKey(searchKey)}");
            Console.WriteLine($"  Ceiling (≥{searchKey}): {map.CeilingKey(searchKey)}");
            Console.WriteLine($"  Higher (>{searchKey}): {map.HigherKey(searchKey)}");

            Console.WriteLine($"\nПервый элемент: {map.FirstEntry()}");
            Console.WriteLine($"Последний элемент: {map.LastEntry()}");

            var first = map.PollFirstEntry();
            Console.WriteLine($"\nУдалён первый: {first}");
            Console.WriteLine($"Новый первый: {map.FirstEntry()}");

            var last = map.PollLastEntry();
            Console.WriteLine($"Удалён последний: {last}");
            Console.WriteLine($"Новый последний: {map.LastEntry()}");

            Console.WriteLine();
        }

        static void Example4_RangeMaps()
        {
            Console.WriteLine("--- Пример 4: HeadMap, TailMap, SubMap ---");
            var map = new MyTreeMap<int, string>();

            for (int i = 10; i <= 100; i += 10)
                map.Put(i, $"Val_{i}");

            Console.WriteLine("Исходная карта:");
            foreach (var entry in map.EntrySet().OrderBy(e => e.Key))
                Console.WriteLine($"  {entry}");

            var headMap = map.HeadMap(50);
            Console.WriteLine($"\nHeadMap (< 50): размер = {headMap.Size()}");
            foreach (var entry in headMap.EntrySet().OrderBy(e => e.Key))
                Console.WriteLine($"  {entry}");

            var tailMap = map.TailMap(50);
            Console.WriteLine($"\nTailMap (> 50): размер = {tailMap.Size()}");
            foreach (var entry in tailMap.EntrySet().OrderBy(e => e.Key))
                Console.WriteLine($"  {entry}");

            var subMap = map.SubMap(30, 70);
            Console.WriteLine($"\nSubMap [30, 70): размер = {subMap.Size()}");
            foreach (var entry in subMap.EntrySet().OrderBy(e => e.Key))
                Console.WriteLine($"  {entry}");

            Console.WriteLine();
        }

        static void Example5_ErrorHandling()
        {
            Console.WriteLine("--- Пример 5: Обработка ошибок ---");

            try
            {
                var map = new MyTreeMap<int, string>();
                map.Put(0, null); // null-значение - OK
                Console.WriteLine(" Можно добавить null-значение");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }

            try
            {
                var map = new MyTreeMap<string, int>();
                map.Put(null, 42); // null-ключ - ОШИБКА
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($" Поймана ожидаемая ошибка: {ex.ParamName} не может быть null");
            }

            try
            {
                var map = new MyTreeMap<int, string>();
                var key = map.FirstKey(); // Пустая карта - ОШИБКА
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($" Поймана ожидаемая ошибка: {ex.Message}");
            }

            try
            {
                // Тип без IComparable - ОШИБКА
                var map = new MyTreeMap<object, string>();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($" Поймана ошибка для несравниваемого типа");
            }

            // Безопасное удаление несуществующего ключа
            var safeMap = new MyTreeMap<int, string>();
            safeMap.Put(1, "Один");
            var result = safeMap.Remove(999);
            Console.WriteLine($" Удаление несуществующего ключа вернуло: {result ?? "null"}");

            // PollFirstEntry на пустой карте
            var emptyMap = new MyTreeMap<int, string>();
            var polled = emptyMap.PollFirstEntry();
            result = polled != null ? "NOT EMPTY" : "null";
            Console.WriteLine($" PollFirstEntry на пустой карте вернул: {result}");

            Console.WriteLine();
        }

        static void Example6_TreeVisualization()
        {
            Console.WriteLine("--- Пример 6: Визуализация дерева ---");
            var map = new MyTreeMap<int, string>();

            Console.WriteLine("Добавление элементов в порядке: 50, 30, 70, 20, 40, 60, 80");
            int[] values = { 50, 30, 70, 20, 40, 60, 80 };
            foreach (var v in values)
                map.Put(v, $"v{v}");

            Console.WriteLine("\nСтруктура BST:");
            map.PrintTree();

            Console.WriteLine("\nУдаление узла 30 (имеет двух детей):");
            map.Remove(30);
            map.PrintTree();

            Console.WriteLine("\n\nДобавление несбалансированной последовательности: 1, 2, 3, 4, 5");
            var unbalanced = new MyTreeMap<int, string>();
            for (int i = 1; i <= 5; i++)
                unbalanced.Put(i, $"n{i}");

            Console.WriteLine("\nСтруктура вырожденного дерева (связный список):");
            unbalanced.PrintTree();

            Console.WriteLine();
        }
    }
}