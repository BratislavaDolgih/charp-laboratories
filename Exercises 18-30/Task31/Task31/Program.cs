using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task31CrissCross
{
    struct Placement
    {
        public int WordIndex;
        public int X;
        public int Y;
        public bool Horizontal;
        public int Intersections;
    }

    struct PlacedWord
    {
        public int WordIndex;
        public string Word;
        public int X;
        public int Y;
        public bool Horizontal;
    }

    sealed class CellState
    {
        public char Ch;
        public int Count;
    }

    sealed class Solution
    {
        public List<PlacedWord> Words;
        public int Intersections;
        public int Area;
        public string Signature;
    }

    sealed class Board
    {
        private readonly Dictionary<long, CellState> cells = new Dictionary<long, CellState>();
        private readonly List<PlacedWord> placed = new List<PlacedWord>();
        private int totalIntersections;

        public bool IsEmpty
        {
            get { return cells.Count == 0; }
        }

        public int PlacedCount
        {
            get { return placed.Count; }
        }

        public int TotalIntersections
        {
            get { return totalIntersections; }
        }

        public IEnumerable<KeyValuePair<long, CellState>> Cells
        {
            get { return cells; }
        }

        public List<PlacedWord> SnapshotPlacedWords()
        {
            return new List<PlacedWord>(placed);
        }

        public static long MakeKey(int x, int y)
        {
            return ((long)x << 32) ^ (uint)y;
        }

        public static int KeyX(long key)
        {
            return (int)(key >> 32);
        }

        public static int KeyY(long key)
        {
            return (int)key;
        }

        public bool HasCell(int x, int y)
        {
            return cells.ContainsKey(MakeKey(x, y));
        }

        public bool TryGetChar(int x, int y, out char ch)
        {
            CellState state;
            if (cells.TryGetValue(MakeKey(x, y), out state))
            {
                ch = state.Ch;
                return true;
            }

            ch = '\0';
            return false;
        }

        public bool CanPlace(string word, int x, int y, bool horizontal, out int intersections)
        {
            intersections = 0;

            if (string.IsNullOrEmpty(word))
                return false;

            int len = word.Length;
            bool allOverlap = true;

            int beforeX = horizontal ? x - 1 : x;
            int beforeY = horizontal ? y : y - 1;
            int afterX = horizontal ? x + len : x;
            int afterY = horizontal ? y : y + len;

            if (HasCell(beforeX, beforeY) || HasCell(afterX, afterY))
                return false;

            for (int i = 0; i < len; i++)
            {
                int cx = horizontal ? x + i : x;
                int cy = horizontal ? y : y + i;

                char existing;
                bool occupied = TryGetChar(cx, cy, out existing);

                if (occupied)
                {
                    if (existing != word[i])
                        return false;

                    intersections++;
                }
                else
                {
                    allOverlap = false;

                    if (horizontal)
                    {
                        if (HasCell(cx, cy - 1) || HasCell(cx, cy + 1))
                            return false;
                    }
                    else
                    {
                        if (HasCell(cx - 1, cy) || HasCell(cx + 1, cy))
                            return false;
                    }
                }
            }

            if (!IsEmpty && intersections == 0)
                return false;

            if (allOverlap)
                return false;

            return true;
        }

        public Placement Place(int wordIndex, string word, int x, int y, bool horizontal, int intersections)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int cx = horizontal ? x + i : x;
                int cy = horizontal ? y : y + i;
                long key = MakeKey(cx, cy);

                CellState state;
                if (cells.TryGetValue(key, out state))
                {
                    state.Count++;
                }
                else
                {
                    cells[key] = new CellState { Ch = word[i], Count = 1 };
                }
            }

            placed.Add(new PlacedWord
            {
                WordIndex = wordIndex,
                Word = word,
                X = x,
                Y = y,
                Horizontal = horizontal
            });

            totalIntersections += intersections;

            return new Placement
            {
                WordIndex = wordIndex,
                X = x,
                Y = y,
                Horizontal = horizontal,
                Intersections = intersections
            };
        }

        public void Remove(Placement placement, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int cx = placement.Horizontal ? placement.X + i : placement.X;
                int cy = placement.Horizontal ? placement.Y : placement.Y + i;
                long key = MakeKey(cx, cy);

                CellState state = cells[key];
                state.Count--;

                if (state.Count == 0)
                    cells.Remove(key);
            }

            for (int i = placed.Count - 1; i >= 0; i--)
            {
                if (placed[i].WordIndex == placement.WordIndex)
                {
                    placed.RemoveAt(i);
                    break;
                }
            }

            totalIntersections -= placement.Intersections;
        }

        public void GetBounds(out int minX, out int maxX, out int minY, out int maxY)
        {
            if (cells.Count == 0)
            {
                minX = maxX = minY = maxY = 0;
                return;
            }

            bool first = true;
            minX = maxX = minY = maxY = 0;

            foreach (var kv in cells)
            {
                int x = KeyX(kv.Key);
                int y = KeyY(kv.Key);

                if (first)
                {
                    minX = maxX = x;
                    minY = maxY = y;
                    first = false;
                }
                else
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        public int EstimateAreaAfter(string word, int x, int y, bool horizontal)
        {
            int minX, maxX, minY, maxY;
            GetBounds(out minX, out maxX, out minY, out maxY);

            int wMinX = horizontal ? x : x;
            int wMaxX = horizontal ? x + word.Length - 1 : x;
            int wMinY = horizontal ? y : y;
            int wMaxY = horizontal ? y : y + word.Length - 1;

            if (cells.Count == 0)
                return (wMaxX - wMinX + 1) * (wMaxY - wMinY + 1);

            if (wMinX < minX) minX = wMinX;
            if (wMaxX > maxX) maxX = wMaxX;
            if (wMinY < minY) minY = wMinY;
            if (wMaxY > maxY) maxY = wMaxY;

            return (maxX - minX + 1) * (maxY - minY + 1);
        }

        public string BuildGridString()
        {
            int minX, maxX, minY, maxY;
            GetBounds(out minX, out maxX, out minY, out maxY);

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            char[,] grid = new char[height, width];

            for (int r = 0; r < height; r++)
                for (int c = 0; c < width; c++)
                    grid[r, c] = '.';

            foreach (var kv in cells)
            {
                int x = KeyX(kv.Key);
                int y = KeyY(kv.Key);
                grid[y - minY, x - minX] = kv.Value.Ch;
            }

            StringBuilder sb = new StringBuilder();

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                    sb.Append(grid[r, c]);

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string BuildSignature()
        {
            List<PlacedWord> copy = SnapshotPlacedWords();
            copy.Sort((a, b) =>
            {
                int cmp = string.CompareOrdinal(a.Word, b.Word);
                if (cmp != 0) return cmp;
                cmp = a.X.CompareTo(b.X);
                if (cmp != 0) return cmp;
                cmp = a.Y.CompareTo(b.Y);
                if (cmp != 0) return cmp;
                return a.Horizontal.CompareTo(b.Horizontal);
            });

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < copy.Count; i++)
            {
                sb.Append(copy[i].Word);
                sb.Append('|');
                sb.Append(copy[i].X);
                sb.Append('|');
                sb.Append(copy[i].Y);
                sb.Append('|');
                sb.Append(copy[i].Horizontal ? 'H' : 'V');
                sb.Append(';');
            }

            return sb.ToString();
        }
    }

    sealed class Solver
    {
        private readonly string[] words;
        private readonly bool[] used;
        private readonly Board board;
        private readonly HashSet<string> foundSignatures = new HashSet<string>();
        private Solution bestSolution;
        private int distinctSolutionCount;

        public Solver(string[] words)
        {
            this.words = words;
            used = new bool[words.Length];
            board = new Board();
        }

        public int DistinctSolutionCount
        {
            get { return distinctSolutionCount; }
        }

        public Solution BestSolution
        {
            get { return bestSolution; }
        }

        public void Solve()
        {
            if (words.Length == 0)
                return;

            Search();
        }

        private void Search()
        {
            if (distinctSolutionCount >= 2)
                return;

            if (board.PlacedCount == words.Length)
            {
                string signature = board.BuildSignature();
                if (foundSignatures.Add(signature))
                {
                    distinctSolutionCount++;

                    int minX, maxX, minY, maxY;
                    board.GetBounds(out minX, out maxX, out minY, out maxY);
                    int area = (maxX - minX + 1) * (maxY - minY + 1);

                    Solution solution = new Solution
                    {
                        Words = board.SnapshotPlacedWords(),
                        Intersections = board.TotalIntersections,
                        Area = area,
                        Signature = signature
                    };

                    if (bestSolution == null || Better(solution, bestSolution))
                        bestSolution = solution;
                }

                return;
            }

            int chosenIndex;
            List<Placement> candidates;
            if (!ChooseWordAndCandidates(out chosenIndex, out candidates))
                return;

            used[chosenIndex] = true;

            for (int i = 0; i < candidates.Count; i++)
            {
                Placement p = candidates[i];
                board.Place(p.WordIndex, words[p.WordIndex], p.X, p.Y, p.Horizontal, p.Intersections);
                Search();
                board.Remove(p, words[p.WordIndex]);

                if (distinctSolutionCount >= 2)
                    break;
            }

            used[chosenIndex] = false;
        }

        private bool ChooseWordAndCandidates(out int chosenIndex, out List<Placement> bestCandidates)
        {
            chosenIndex = -1;
            bestCandidates = null;

            for (int i = 0; i < words.Length; i++)
            {
                if (used[i])
                    continue;

                List<Placement> candidates = GenerateCandidates(i);

                if (candidates.Count == 0)
                    return false;

                if (bestCandidates == null ||
                    candidates.Count < bestCandidates.Count ||
                    (candidates.Count == bestCandidates.Count && words[i].Length > words[chosenIndex].Length))
                {
                    chosenIndex = i;
                    bestCandidates = candidates;
                }
            }

            return bestCandidates != null;
        }

        private List<Placement> GenerateCandidates(int wordIndex)
        {
            string word = words[wordIndex];
            List<Placement> result = new List<Placement>();

            if (board.IsEmpty)
            {
                int intersections;
                if (board.CanPlace(word, 0, 0, true, out intersections))
                {
                    result.Add(new Placement
                    {
                        WordIndex = wordIndex,
                        X = 0,
                        Y = 0,
                        Horizontal = true,
                        Intersections = intersections
                    });
                }

                return result;
            }

            HashSet<string> seen = new HashSet<string>();

            foreach (var kv in board.Cells)
            {
                int cellX = Board.KeyX(kv.Key);
                int cellY = Board.KeyY(kv.Key);
                char ch = kv.Value.Ch;

                for (int pos = 0; pos < word.Length; pos++)
                {
                    if (word[pos] != ch)
                        continue;

                    int x1 = cellX - pos;
                    int y1 = cellY;
                    TryAddCandidate(wordIndex, word, x1, y1, true, seen, result);

                    int x2 = cellX;
                    int y2 = cellY - pos;
                    TryAddCandidate(wordIndex, word, x2, y2, false, seen, result);
                }
            }

            result.Sort((a, b) =>
            {
                int cmp = b.Intersections.CompareTo(a.Intersections);
                if (cmp != 0) return cmp;

                int areaA = board.EstimateAreaAfter(words[a.WordIndex], a.X, a.Y, a.Horizontal);
                int areaB = board.EstimateAreaAfter(words[b.WordIndex], b.X, b.Y, b.Horizontal);
                cmp = areaA.CompareTo(areaB);
                if (cmp != 0) return cmp;

                cmp = a.Y.CompareTo(b.Y);
                if (cmp != 0) return cmp;
                cmp = a.X.CompareTo(b.X);
                if (cmp != 0) return cmp;
                return a.Horizontal.CompareTo(b.Horizontal);
            });

            return result;
        }

        private void TryAddCandidate(int wordIndex, string word, int x, int y, bool horizontal, HashSet<string> seen, List<Placement> result)
        {
            string sig = x.ToString() + "|" + y.ToString() + "|" + (horizontal ? "H" : "V");
            if (!seen.Add(sig))
                return;

            int intersections;
            if (!board.CanPlace(word, x, y, horizontal, out intersections))
                return;

            result.Add(new Placement
            {
                WordIndex = wordIndex,
                X = x,
                Y = y,
                Horizontal = horizontal,
                Intersections = intersections
            });
        }

        private bool Better(Solution a, Solution b)
        {
            if (a.Intersections != b.Intersections)
                return a.Intersections > b.Intersections;

            if (a.Area != b.Area)
                return a.Area < b.Area;

            return string.CompareOrdinal(a.Signature, b.Signature) < 0;
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            const string inputFile = "input.txt";
            const string outputFile = "output.txt";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            string text = File.ReadAllText(inputFile, Encoding.UTF8);
            MatchCollection matches = Regex.Matches(text, @"\p{L}+");

            List<string> wordsList = new List<string>();
            for (int i = 0; i < matches.Count; i++)
                wordsList.Add(matches[i].Value.ToUpperInvariant());

            using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                if (wordsList.Count == 0)
                {
                    writer.WriteLine("INPUT_ERROR");
                    writer.WriteLine("Не найдено ни одного слова.");
                    Console.WriteLine("INPUT_ERROR");
                    return;
                }

                HashSet<string> unique = new HashSet<string>(StringComparer.Ordinal);
                List<string> duplicates = new List<string>();

                for (int i = 0; i < wordsList.Count; i++)
                {
                    if (!unique.Add(wordsList[i]))
                        duplicates.Add(wordsList[i]);
                }

                if (duplicates.Count > 0)
                {
                    duplicates.Sort(StringComparer.Ordinal);
                    writer.WriteLine("AMBIGUOUS");
                    writer.WriteLine("Повторяющиеся слова во входных данных:");
                    for (int i = 0; i < duplicates.Count; i++)
                        writer.WriteLine(duplicates[i]);

                    Console.WriteLine("AMBIGUOUS");
                    return;
                }

                string[] words = unique.ToArray();
                Array.Sort(words, (a, b) =>
                {
                    int cmp = b.Length.CompareTo(a.Length);
                    if (cmp != 0) return cmp;
                    return string.CompareOrdinal(a, b);
                });

                Solver solver = new Solver(words);
                solver.Solve();

                if (solver.DistinctSolutionCount == 0 || solver.BestSolution == null)
                {
                    writer.WriteLine("IMPOSSIBLE");
                    Console.WriteLine("IMPOSSIBLE");
                    return;
                }

                if (solver.DistinctSolutionCount > 1)
                    writer.WriteLine("AMBIGUOUS");
                else
                    writer.WriteLine("OK");

                writer.WriteLine("WORDS=" + words.Length);
                writer.WriteLine("INTERSECTIONS=" + solver.BestSolution.Intersections);
                writer.WriteLine("AREA=" + solver.BestSolution.Area);
                writer.WriteLine("GRID:");
                writer.Write(solver.BestSolutionToGrid());

                writer.WriteLine("PLACEMENTS:");
                List<PlacedWord> placed = solver.BestSolution.Words;
                placed.Sort((a, b) =>
                {
                    int cmp = string.CompareOrdinal(a.Word, b.Word);
                    if (cmp != 0) return cmp;
                    cmp = a.Y.CompareTo(b.Y);
                    if (cmp != 0) return cmp;
                    cmp = a.X.CompareTo(b.X);
                    if (cmp != 0) return cmp;
                    return a.Horizontal.CompareTo(b.Horizontal);
                });

                for (int i = 0; i < placed.Count; i++)
                {
                    writer.WriteLine(
                        placed[i].Word + " " +
                        placed[i].X + " " +
                        placed[i].Y + " " +
                        (placed[i].Horizontal ? "H" : "V"));
                }

                Console.WriteLine(solver.DistinctSolutionCount > 1 ? "AMBIGUOUS" : "OK");
                Console.WriteLine("Результат записан в output.txt");
            }
        }

        private static string BestSolutionToGrid(this Solver solver)
        {
            Board board = new Board();

            for (int i = 0; i < solver.BestSolution.Words.Count; i++)
            {
                PlacedWord w = solver.BestSolution.Words[i];
                int intersections;
                board.CanPlace(w.Word, w.X, w.Y, w.Horizontal, out intersections);
                board.Place(w.WordIndex, w.Word, w.X, w.Y, w.Horizontal, intersections);
            }

            return board.BuildGridString();
        }
    }
}