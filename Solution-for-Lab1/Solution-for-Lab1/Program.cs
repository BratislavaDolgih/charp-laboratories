using System;

namespace Solution_for_Lab1
{
    // Создание класса «Область».
    class CountryArea
    {
        private string areaName;      
        private int areaPopulation;
        private double mortalityRate;

        // Геттеры / Сеттеры
        public double MortalityValidate
        {
            set
            {
                if (!double.IsNaN(value) && !double.IsInfinity(value)
                    && value <= 100 && value >= 0)
                {
                    this.mortalityRate = value;
                }
                else
                {
                    Console.WriteLine("Incorrect percentage!");
                }
            }
        }
        public double Mortality { get; }
        public string AreaName { get; set; }
        public int Population { get; set; }

        // Конструкторы.
        public CountryArea()
        {
            this.areaName = "N/D";
            this.areaPopulation = 0;
            this.mortalityRate = 0;
        }
        public CountryArea(string areaName, int areaPopulation, double mortalityRate)
        {
            this.areaName = areaName;
            this.areaPopulation = areaPopulation;
            this.mortalityRate = mortalityRate;
        }
        public void Information()
        {
            Console.WriteLine($"Class 'CountryArea', info:\n" +
                $"Name of area: {this.areaName}\n" +
                $"Population: {this.areaPopulation}\n" +
                $"Mortality Rate: {this.mortalityRate}");
        }
    }

    // Унаследованный класс «Город» от «Области».
    class City : CountryArea
    {
        private string cityName;
        private int tournamentRate;

        public string CityName { set; get; }
        public string TournamentName { set; get; }

        public City(): base()
        {
            this.cityName = "N/D";
            this.tournamentRate = 0;
        }

        public City(string cityName, int tournamentRate): base()
        {
            this.cityName = cityName;
            this.tournamentRate = tournamentRate;
        }

        public City(string areaName, int population, double percetageOfDeaths,
            string cityName, int tournamentRate): base(areaName, population, percetageOfDeaths)
        {
            this.cityName = cityName;
            this.tournamentRate = tournamentRate;
        }

        public void Information()
        {
            base.Information();
            Console.WriteLine($"Class 'City', info:\n" +
                $"City: {this.cityName}" +
                $"Rating: {this.tournamentRate}");
        }
    }

    // Унаследованный класс «Мегаполис» от «Города».
    class Metropolis : City
    {
        private string metropolisName;
        private double cost;

        public string MetropolisName { set; get; }
        public double Cost 
        { 
            private set
            {
                if (value < 0)
                {
                    Console.WriteLine("Cost won't be negative!");
                }
            }
            get
            {
                return cost;
            } 
        }

        public Metropolis() : base()
        {
            this.MetropolisName = "N/D";
            this.cost = 0.0;
        }

        public Metropolis(string metropolis, double cost, string cityName, int tournamentRate) 
            : base(cityName, tournamentRate)
        {
            this.metropolisName = metropolis;
            Cost = cost;
        }

        public Metropolis(string metropolis, double cost, 
            string cityName, int tournamentRate,
            string areaName, int population, double percetageOfDeaths): 
            base(areaName, population, percetageOfDeaths, cityName, tournamentRate) 
        {
            this.metropolisName = metropolis;
            this.cost = cost;
        }

        public void Information()
        {
            base.Information();
            Console.WriteLine($"Class 'Metropolis', info:\n" +
                $"Metropolis: {this.metropolisName}" +
                $"Cost: {this.cost}");
        }
    }

    // Унаследованный класс «Место» от «Города».
    // Класс «Место в городе» — например, парк, станция, здание
    class CityPlace : City
    {
        public string PlaceName { get; set; }
        public string Category { get; set; }
        public bool IsPopular { get; set; }
        public CityPlace() : base()
        {
            PlaceName = "N/D";
            Category = "Разное";
            IsPopular = false;
        }
        public CityPlace(string placeName, string category, bool isPopular,
                         string cityName, int tournamentRating)
            : base(cityName, tournamentRating)
        {
            PlaceName = placeName;
            Category = category;
            IsPopular = isPopular;
        }
        public void Information()
        {
            base.Information();
            Console.WriteLine($"Класс 'CityPlace':\n" +
                $"Place: {PlaceName}\n" +
                $"Category: {Category}\n" +
                $"This place is popular: {(IsPopular ? "yes!" : "no :((")}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Полиморфный массив объектов ===\n");
            CountryArea[] places = new CountryArea[5];

            // Заполняем разными объектами
            places[0] = new CountryArea("Свердловская область", 4_300_000, 12.1);
            places[1] = new City("Свердловская область", 1_500_000, 11.8, "Екатеринбург", 17);
            places[2] = new Metropolis("Москва-Сити", 85000, "Москва", 95, "Московская область", 13_000_000, 11.3);
            places[3] = new CityPlace("Красная площадь", "Достопримечательность", true, "Москва", 100);
            places[4] = new City("Казань", 85);

            // Выводим информацию о каждом объекте
            for (int i = 0; i < places.Length; i++)
            {
                Console.WriteLine($"--- Element [{i}] ---");
                places[i].Information();
                Console.WriteLine();
            }
        }
    }
}
