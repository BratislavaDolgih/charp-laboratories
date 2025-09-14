using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solution_for_Lab2
{
    abstract class Area
    {
        private int timeNow = 0;
        public string AreaTitle { get; set; }
        protected int Time { get; set; }
        public Area(string title)
        {
            this.AreaTitle = title;
        }
        
        // Абстрактный метод установки времени в области.
        public abstract void SettingTimeInArea(int time);
        // Абстрактный метод получения времени.
        public abstract int GetAreaTime();

        public override string ToString()
        {
            return ($"class: Area[title:{AreaTitle}]");
        }
    }

    class City : Area 
    {
        public int Population { get; }
        public string CityNaming { get; }

        public City(string areaTitle, 
            string cityname, int popul): base(areaTitle)
        {
            this.CityNaming = cityname;
            if (popul < 1)
            {
                Console.WriteLine("Now population is equal to 0.");
                this.Population = popul;
            }
        }

        public static int operator +(City a, City b)
        {
            if (a == null || b == null)
            {
                return -1;
            }
            
            return a.Population + b.Population;
        }

        public override void SettingTimeInArea(int time)
        {
            if (time < 0)
            {
                Console.WriteLine("Time MUST BE negative!");
                return;
            }
            base.Time = time;
        }

        public override int GetAreaTime()
        {
            return base.Time;
        }

        public override string ToString()
        {
            return ($"class City[title:{this.CityNaming}]" +
                $" extends {base.ToString()}");
        }
    }

    class Metropolis : City
    {
        public int Skyscrapers { get; }

        public Metropolis(string areaTitle, string cityName, int population, 
            int skyscrapers)
            : base(areaTitle, cityName, population)
        {
            this.Skyscrapers = skyscrapers;
        }

        public override string ToString()
        {
            return $"class Metropolis[skyscrapers:{Skyscrapers}] " +
                   $"extends {base.ToString()}";
        }
    }

    class Place : City
    {
        private bool IsOpened { get; set; }
        public Place(string areaTitle, string cityName, int placePoupulation)
            : base(areaTitle, cityName, placePoupulation)
        {
            this.IsOpened = true;
        }

        public override void SettingTimeInArea(int time)
        {
            Console.WriteLine("Warning! Your time is useless " +
                "because in the place time will STOPPED.");
            base.SettingTimeInArea(0);
        }
        public override int GetAreaTime()
        {
            return base.Time;
        }

        public override string ToString()
        {
            return ($"Place[opened - {(this.IsOpened ? "true" : "false")}]");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Area[] areas = new Area[]
            {
                new City("Los Santos", "SmallCity", 1000),
                new City("Beverly Hills", "BigCity", 5000),
                new Metropolis("MegaArea", "NewYork", 10_000_000, 200),
                new Place("FunPark", "DreamLand", 430)
            };

            foreach (Area area in areas)
            {
                Console.WriteLine(area);
                area.SettingTimeInArea(15);
                Console.WriteLine($"Time in area: {area.GetAreaTime()}");
                Console.WriteLine();
            }
        }
    }
}
