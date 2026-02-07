using System;
using System.Collections.Generic;
using System.Linq;
namespace PersonalTask
{ 
    class Program
    {
        struct Vector
        {
            public int Str, Def, Int;
            public Vector(int s, int d, int i) { Str = s; Def = d; Int = i; }
            public int Sum() => Str + Def + Int;
            public override string ToString() => $"({Str}, {Def}, {Int})";
        }
        struct Bonus
        {
            public Vector V;
            public int Q;
            public Bonus(Vector v, int q) { V = v; Q = q; }
        }
        static void Main()
        {
            int k = 1, n = 10;
            int levels = n - k;
            // бонусы
            var bonuses = new List<Bonus>
            {
                new Bonus(new Vector(7, 2, 0), 2),
                new Bonus(new Vector(2, 0, 6), 1),
                new Bonus(new Vector(-1, -3, 1), 3),
                new Bonus(new Vector(-1, 3, 1), 2)
            };
            // случайные бонусы
            Random rnd = new Random();
            var baseStats = new Vector[levels];

            for (int i = 0; i < levels; i++) baseStats[i] = new Vector(rnd.Next(-3, 4), rnd.Next(-3, 4), rnd.Next(-3, 4));
            Console.WriteLine("Приросты за уровни:");
            for (int i = 0; i < levels; i++)
                Console.WriteLine($"[{k + i} {k + i + 1}]: {baseStats[i]}");
            var sorted = bonuses.OrderByDescending(b => b.V.Sum()).ToList();
            var chosenBonuses = new List<Vector>();
            int currentLevel = 0;
            foreach (var b in sorted)
            {
                for (int i = 0; i < b.Q && currentLevel < levels; i++)
                {
                    chosenBonuses.Add(b.V);
                    currentLevel++;
                }
                if (currentLevel >= levels)
                    break;
            }
            // если уровней больше, чем суммарное q продолжаем по кругу
            while (chosenBonuses.Count < levels)
            {
                foreach (var b in sorted)
                {
                    for (int i = 0; i < b.Q && chosenBonuses.Count < levels; i++)
                        chosenBonuses.Add(b.V);
                    if (chosenBonuses.Count >= levels)
                        break;
                }
            }
            // итоговые статы
            var total = new Vector(0, 0, 0);
            for (int i = 0; i < levels; i++)
                total = new Vector(
                    total.Str + baseStats[i].Str + chosenBonuses[i].Str,
                    total.Def + baseStats[i].Def + chosenBonuses[i].Def,
                    total.Int + baseStats[i].Int + chosenBonuses[i].Int
                );
            Console.WriteLine("\nТраектория бонусов:");
            for (int i = 0; i < chosenBonuses.Count; i++)
                Console.WriteLine($"Уровень {k + i + 1}: бонус {chosenBonuses[i]}");
            Console.WriteLine($"Ответ с максимальной суммой: {total.Sum()}");
        }
    }
}