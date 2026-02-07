using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTask2
{
    internal class Program
    {
        static void Main()
        {
            string[] nm = Console.ReadLine().Split();
            int n = int.Parse(nm[0]);
            int m = int.Parse(nm[1]);
            int[,] matrix = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                string[] row = Console.ReadLine().Split();
                for (int j = 0; j < m; j++)
                    matrix[i, j] = int.Parse(row[j]);
            }
            string[] xyz = Console.ReadLine().Split();
            int x = int.Parse(xyz[0]); // строка
            int y = int.Parse(xyz[1]); // столбец
            int z = int.Parse(xyz[2]); // новый цвет
            int oldColor = matrix[x, y];
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((x, y));
            // направления
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            while (stack.Count > 0)
            {
                var (cx, cy) = stack.Pop();
                if (cx < 0 || cy < 0 || cx >= n || cy >= m)
                    continue;
                if (matrix[cx, cy] != oldColor)
                    continue;
                matrix[cx, cy] = z;
                for (int dir = 0; dir < 8; dir++)
                {
                    int nx = cx + dx[dir];
                    int ny = cy + dy[dir];
                    if (nx >= 0 && ny >= 0 && nx < n && ny < m && matrix[nx, ny] == oldColor)
                        stack.Push((nx, ny));
                }
            }
            PrintMatrix(matrix, n, m);
        }

        static void PrintMatrix(int[,] matrix, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(matrix[i, j]);
                    if (j < m - 1) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
