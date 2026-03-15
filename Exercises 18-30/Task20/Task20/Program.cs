using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphTasksVariant8
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Задача 8. Минимальный остовный лес. Алгоритм Прима ===");
            DemoPrim();

            Console.WriteLine();
            Console.WriteLine("=== Задача 11. Максимальный поток. Алгоритм Диница ===");
            DemoDinic();

            Console.WriteLine();
            Console.WriteLine("=== Задача 14. Максимальная клика. Эвристический алгоритм слияния клик ===");
            DemoCliqueMerge();
        }

        private static void DemoPrim()
        {
            int n = 7;
            var graph = new WeightedUndirectedGraph(n);

            graph.AddEdge(0, 1, 4);
            graph.AddEdge(0, 2, 3);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(1, 3, 2);
            graph.AddEdge(2, 3, 4);

            graph.AddEdge(4, 5, 2);
            graph.AddEdge(5, 6, 1);
            graph.AddEdge(4, 6, 3);

            var prim = new PrimMinimumSpanningForest();
            var result = prim.Build(graph);

            Console.WriteLine("Рёбра минимального остовного леса:");
            foreach (var edge in result.Edges)
            {
                Console.WriteLine("{0} - {1}, вес = {2}", edge.From, edge.To, edge.Weight);
            }

            Console.WriteLine("Суммарный вес леса: " + result.TotalWeight);
        }

        private static void DemoDinic()
        {
            int n = 6;
            var dinic = new DinicMaxFlow(n);

            dinic.AddEdge(0, 1, 16);
            dinic.AddEdge(0, 2, 13);
            dinic.AddEdge(1, 2, 10);
            dinic.AddEdge(2, 1, 4);
            dinic.AddEdge(1, 3, 12);
            dinic.AddEdge(3, 2, 9);
            dinic.AddEdge(2, 4, 14);
            dinic.AddEdge(4, 3, 7);
            dinic.AddEdge(3, 5, 20);
            dinic.AddEdge(4, 5, 4);

            long maxFlow = dinic.GetMaxFlow(0, 5);

            Console.WriteLine("Максимальный поток из 0 в 5: " + maxFlow);
        }

        private static void DemoCliqueMerge()
        {
            int n = 8;
            var graph = new UndirectedGraph(n);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 3);

            graph.AddEdge(2, 4);
            graph.AddEdge(3, 4);

            graph.AddEdge(4, 5);
            graph.AddEdge(4, 6);
            graph.AddEdge(5, 6);

            graph.AddEdge(6, 7);

            var solver = new CliqueMergeHeuristic();
            var clique = solver.FindMaximumCliqueHeuristic(graph);

            Console.WriteLine("Найденная клика:");
            Console.WriteLine(string.Join(" ", clique));
            Console.WriteLine("Размер клики: " + clique.Count);
        }
    }

    public class WeightedEdge
    {
        public int To { get; }
        public int Weight { get; }

        public WeightedEdge(int to, int weight)
        {
            To = to;
            Weight = weight;
        }
    }

    public class WeightedUndirectedGraph
    {
        private readonly List<WeightedEdge>[] _adj;

        public int VertexCount { get; }

        public WeightedUndirectedGraph(int vertexCount)
        {
            VertexCount = vertexCount;
            _adj = new List<WeightedEdge>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                _adj[i] = new List<WeightedEdge>();
            }
        }

        public void AddEdge(int u, int v, int weight)
        {
            _adj[u].Add(new WeightedEdge(v, weight));
            _adj[v].Add(new WeightedEdge(u, weight));
        }

        public List<WeightedEdge> GetNeighbors(int v)
        {
            return _adj[v];
        }
    }

    public class ForestEdge
    {
        public int From { get; }
        public int To { get; }
        public int Weight { get; }

        public ForestEdge(int from, int to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }

    public class PrimResult
    {
        public List<ForestEdge> Edges { get; }
        public long TotalWeight { get; }

        public PrimResult(List<ForestEdge> edges, long totalWeight)
        {
            Edges = edges;
            TotalWeight = totalWeight;
        }
    }

    public class PrimMinimumSpanningForest
    {
        public PrimResult Build(WeightedUndirectedGraph graph)
        {
            int n = graph.VertexCount;
            var used = new bool[n];
            var resultEdges = new List<ForestEdge>();
            long totalWeight = 0;

            for (int start = 0; start < n; start++)
            {
                if (used[start])
                {
                    continue;
                }

                var pq = new SortedSet<PrimState>(new PrimStateComparer());
                pq.Add(new PrimState(0, start, -1));

                while (pq.Count > 0)
                {
                    var current = pq.Min;
                    pq.Remove(current);

                    if (used[current.Vertex])
                    {
                        continue;
                    }

                    used[current.Vertex] = true;

                    if (current.Parent != -1)
                    {
                        resultEdges.Add(new ForestEdge(current.Parent, current.Vertex, current.Weight));
                        totalWeight += current.Weight;
                    }

                    foreach (var edge in graph.GetNeighbors(current.Vertex))
                    {
                        if (!used[edge.To])
                        {
                            pq.Add(new PrimState(edge.Weight, edge.To, current.Vertex));
                        }
                    }
                }
            }

            return new PrimResult(resultEdges, totalWeight);
        }

        private class PrimState
        {
            private static int _idGenerator = 0;

            public int Weight { get; }
            public int Vertex { get; }
            public int Parent { get; }
            public int Id { get; }

            public PrimState(int weight, int vertex, int parent)
            {
                Weight = weight;
                Vertex = vertex;
                Parent = parent;
                Id = _idGenerator++;
            }
        }

        private class PrimStateComparer : IComparer<PrimState>
        {
            public int Compare(PrimState x, PrimState y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int cmp = x.Weight.CompareTo(y.Weight);
                if (cmp != 0) return cmp;

                cmp = x.Vertex.CompareTo(y.Vertex);
                if (cmp != 0) return cmp;

                cmp = x.Parent.CompareTo(y.Parent);
                if (cmp != 0) return cmp;

                return x.Id.CompareTo(y.Id);
            }
        }
    }

    public class DinicMaxFlow
    {
        private class Edge
        {
            public int To;
            public long Capacity;
            public int ReverseIndex;

            public Edge(int to, long capacity, int reverseIndex)
            {
                To = to;
                Capacity = capacity;
                ReverseIndex = reverseIndex;
            }
        }

        private readonly List<Edge>[] _graph;
        private readonly int[] _level;
        private readonly int[] _ptr;

        public DinicMaxFlow(int vertexCount)
        {
            _graph = new List<Edge>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                _graph[i] = new List<Edge>();
            }

            _level = new int[vertexCount];
            _ptr = new int[vertexCount];
        }

        public void AddEdge(int from, int to, long capacity)
        {
            var forward = new Edge(to, capacity, _graph[to].Count);
            var backward = new Edge(from, 0, _graph[from].Count);

            _graph[from].Add(forward);
            _graph[to].Add(backward);
        }

        public long GetMaxFlow(int source, int sink)
        {
            long flow = 0;

            while (Bfs(source, sink))
            {
                Array.Clear(_ptr, 0, _ptr.Length);

                long pushed;
                do
                {
                    pushed = Dfs(source, sink, long.MaxValue);
                    flow += pushed;
                }
                while (pushed > 0);
            }

            return flow;
        }

        private bool Bfs(int source, int sink)
        {
            for (int i = 0; i < _level.Length; i++)
            {
                _level[i] = -1;
            }

            var queue = new Queue<int>();
            queue.Enqueue(source);
            _level[source] = 0;

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();

                foreach (var edge in _graph[v])
                {
                    if (edge.Capacity > 0 && _level[edge.To] == -1)
                    {
                        _level[edge.To] = _level[v] + 1;
                        queue.Enqueue(edge.To);
                    }
                }
            }

            return _level[sink] != -1;
        }

        private long Dfs(int v, int sink, long pushed)
        {
            if (pushed == 0)
            {
                return 0;
            }

            if (v == sink)
            {
                return pushed;
            }

            while (_ptr[v] < _graph[v].Count)
            {
                int edgeIndex = _ptr[v];
                var edge = _graph[v][edgeIndex];

                if (_level[edge.To] == _level[v] + 1 && edge.Capacity > 0)
                {
                    long tr = Dfs(edge.To, sink, Math.Min(pushed, edge.Capacity));
                    if (tr > 0)
                    {
                        edge.Capacity -= tr;
                        _graph[edge.To][edge.ReverseIndex].Capacity += tr;
                        return tr;
                    }
                }

                _ptr[v]++;
            }

            return 0;
        }
    }

    public class UndirectedGraph
    {
        private readonly HashSet<int>[] _adj;

        public int VertexCount { get; }

        public UndirectedGraph(int vertexCount)
        {
            VertexCount = vertexCount;
            _adj = new HashSet<int>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                _adj[i] = new HashSet<int>();
            }
        }

        public void AddEdge(int u, int v)
        {
            if (u == v) return;
            _adj[u].Add(v);
            _adj[v].Add(u);
        }

        public bool HasEdge(int u, int v)
        {
            return _adj[u].Contains(v);
        }

        public IEnumerable<int> GetNeighbors(int v)
        {
            return _adj[v];
        }

        public int Degree(int v)
        {
            return _adj[v].Count;
        }
    }

    public class CliqueMergeHeuristic
    {
        public List<int> FindMaximumCliqueHeuristic(UndirectedGraph graph)
        {
            var cliques = BuildInitialCliques(graph);

            bool merged;
            do
            {
                merged = false;

                for (int i = 0; i < cliques.Count && !merged; i++)
                {
                    for (int j = i + 1; j < cliques.Count && !merged; j++)
                    {
                        var mergedClique = TryMergeCliques(cliques[i], cliques[j], graph);
                        if (mergedClique != null)
                        {
                            cliques[i] = mergedClique;
                            cliques.RemoveAt(j);
                            merged = true;
                        }
                    }
                }
            }
            while (merged);

            var best = cliques
                .OrderByDescending(c => c.Count)
                .ThenBy(c => string.Join(",", c))
                .FirstOrDefault();

            return best ?? new List<int>();
        }

        private List<List<int>> BuildInitialCliques(UndirectedGraph graph)
        {
            int n = graph.VertexCount;
            var order = Enumerable.Range(0, n)
                .OrderByDescending(graph.Degree)
                .ThenBy(v => v)
                .ToList();

            var cliques = new List<List<int>>();

            foreach (int v in order)
            {
                bool added = false;

                for (int i = 0; i < cliques.Count; i++)
                {
                    if (CanAddToClique(v, cliques[i], graph))
                    {
                        cliques[i].Add(v);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    cliques.Add(new List<int> { v });
                }
            }

            for (int i = 0; i < cliques.Count; i++)
            {
                cliques[i].Sort();
            }

            return cliques;
        }

        private bool CanAddToClique(int vertex, List<int> clique, UndirectedGraph graph)
        {
            for (int i = 0; i < clique.Count; i++)
            {
                if (!graph.HasEdge(vertex, clique[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private List<int> TryMergeCliques(List<int> first, List<int> second, UndirectedGraph graph)
        {
            var union = first.Concat(second).Distinct().OrderBy(x => x).ToList();

            if (IsClique(union, graph))
            {
                return union;
            }

            var candidate = BuildCliqueFromVertices(union, graph);
            if (candidate.Count > Math.Max(first.Count, second.Count))
            {
                return candidate;
            }

            return null;
        }

        private bool IsClique(List<int> vertices, UndirectedGraph graph)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    if (!graph.HasEdge(vertices[i], vertices[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private List<int> BuildCliqueFromVertices(List<int> vertices, UndirectedGraph graph)
        {
            var ordered = vertices
                .OrderByDescending(graph.Degree)
                .ThenBy(v => v)
                .ToList();

            var clique = new List<int>();

            foreach (int v in ordered)
            {
                bool ok = true;
                for (int i = 0; i < clique.Count; i++)
                {
                    if (!graph.HasEdge(v, clique[i]))
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                {
                    clique.Add(v);
                }
            }

            clique.Sort();
            return clique;
        }
    }
}