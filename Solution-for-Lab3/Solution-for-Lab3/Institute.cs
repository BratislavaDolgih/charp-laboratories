using Solution_for_Lab3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static InstituteCore.Institute;

namespace InstituteCore
{
    public class Institute : InstituteHelpAPI
    {
        public delegate void ViewOf(string name);
        public delegate void MsgType(string msgSuccessOrError);
        public ViewOf view;
        public MsgType msgType;

        private List<Course> courses = new List<Course>();
        public List<Course> Courses
        {
            get
            {
                return new List<Course>(this.courses);
            }
        }
        public string Naming { get; set; }
        public void AddCourse(Course course)
        {
            view = WhichMethodWasInvoked;
            courses.Add(course);
            view.Invoke("AddCourse");
        }
        public bool RemoveCourse(Course course)
        {
            if (course == null)
            {
                msgType += delegate
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ВСЁ ЗАРАЖЕНО КРАСНЫМ ЦВЕТОМ. ВЫХОДА НЕТ");
                };
                view("Курсов и так нет, ошибочка");
                return false;
            }
            courses.Remove(course);

            msgType += PrintMagenta;
            foreach (var del in msgType.GetInvocationList())
            {
                if (del.Method.Name == nameof(PrintYellow))
                {
                    msgType -= (MsgType)del;
                }
            }
            msgType("Все ЖЁЛТЫЕ сообщения удалены + удалён курс!");
            return true;
        }

        public class Course
        {
            private List<Group> groups = new List<Group>();
            public List<Group> GetGroups() => new List<Group>(groups);
            public Institute parent;
            
            private int number;
            public int Number
            {
                get
                {
                    return this.number;
                }
                set
                {
                    if (value < 1 || value > 4)
                    {
                        Console.WriteLine("Ошибка записи номера курса!\n" +
                            "Курс должен быть в диапазоне [1;4]");
                        throw new InvalidDataException("Range error.");
                    }
                    number = value;
                }
            }
            public Course(int numberOfCourse, Institute parent)
            {
                if (numberOfCourse < 1 || numberOfCourse > 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(numberOfCourse));
                }

                Number = numberOfCourse;
                this.parent = parent;
            }

            public void FillGroups()
            {
                Console.WriteLine($"Сколько групп на {Number}-м курсе?");
                int count = int.Parse(Console.ReadLine());
                if (count < 1)
                    throw new Exception("Некорректное количество групп");

                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"\nВведите название для группы #{i + 1}: ");
                    string groupName = Console.ReadLine();
                    groups.Add(new Group(groupName, this.parent));
                }
            }

            public void AddGroup(Group group) => groups.Add(group);
            public bool RemoveGroup(Group group) => groups.Remove(group);
        }
        public class Group
        {
            private List<Student> students = new List<Student>();
            public Institute parent;
            private string groupName;
            public string GroupName { get; set; }
            public List<Student> GetStudents 
            { 
                get
                {
                    return new List<Student>(this.students);
                }
            }
            public Group(string groupName, Institute parent)
            {
                GroupName = groupName;
                this.parent = parent;
            }

            public void FillStudents()
            {
                Console.WriteLine($"Сколько студентов в группе {GroupName}?");
                int count = int.Parse( Console.ReadLine() );
                if (count < 1)
                    throw new Exception("Некорректное количество студентов");

                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"\nСтудент #{i + 1}:");
                    Console.Write("Имя: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Фамилия: ");
                    string surName = Console.ReadLine();

                    Console.Write("Отчество: ");
                    string patronymic = Console.ReadLine();

                    students.Add(new Student(firstName, surName, patronymic));
                }
            }
            public void AddStudent(Student s) => students.Add(s);
            public bool RemoveStudent(Student student)
            {
                if (student == null)
                {
                    parent.msgType += PrintError;
                    parent.msgType("Студентов и так нет, ошибочка");
                    throw new ArgumentNullException(nameof(student));
                }

                parent.msgType += PrintSuccess;
                foreach (var del in parent.msgType.GetInvocationList())
                {
                    if (del.Method.Name == nameof(PrintError))
                    {
                        parent.msgType -= (MsgType)del;
                    }
                }
                parent.msgType("Все КРАСНЫЕ сообщения удалены + удалён ученик!");
                return students.Remove(student);
            }

            private void PrintSuccess(string goodMsg)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(goodMsg);
                Console.ForegroundColor = oldColor;
            }
            private void PrintError(string badMsg)
            {
                var oldColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(badMsg);
                Console.ForegroundColor = oldColour;
            }
        }
        public class Student
        {
            private List<Subject> subjects = new List<Subject>();
            public List<Subject> GetSubjects 
            { 
                get
                {
                    return new List<Subject>(this.subjects);
                }
            }

            private string firstName;
            private string surName;
            private string patronymic;

            public string FirstName { get; set; }
            public string SurName { get; set; }
            public string Patronymic { get; set; }

            public Student(string firstName, string surName, string patronymic)
            {
                FirstName = firstName;
                SurName = surName;
                Patronymic = patronymic;
            }
            public void AddSubject (Subject subject) => subjects.Add(subject);
        }

        // Самый нижний класс в иерархии.
        public class Subject
        {
            private string title = "N/D";
            private byte mark;
            public byte Mark 
            { 
                get
                {
                    return mark;
                }
                set
                {
                    if (value < 2)
                    {
                        this.mark = 2;
                    }
                    else if (value > 5)
                    {
                        this.mark = 5;
                    } 
                    else
                    {
                        this.mark = value;
                    }
                }
            }
            public string Title {
                get
                {
                    return title;
                }
                set
                {
                    title = value;
                } 
            }
            public Subject(string name, byte mark)
            {
                Title = name;
                Mark = mark;
            }
        }

        private void WhichMethodWasInvoked(string methodName)
        {
            Console.WriteLine("Делегат передаёт привет из метода " + methodName);
        }

        public void PrintYellow(string goodMsg)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(goodMsg);
            Console.ForegroundColor = oldColor;
        }
        public void PrintMagenta(string badMsg)
        {
            var oldColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(badMsg);
            Console.ForegroundColor = oldColour;
        }
    }

    class PseudoCaptcha
    {
        public delegate void ErrorHandler(object sender, string e);
        public event ErrorHandler writeErr;
        public bool Check()
        {
            Console.WriteLine($"На какое число надо поделить 24, чтобы получить 2?");
            int input = int.Parse(Console.ReadLine());
            try
            {
                int getTwo = 24 / input;
                if (getTwo == 2) { return true; }

                int[] array = new int[12];
                Console.WriteLine("Проверка провалена. Вторая попытка.");
                Console.WriteLine("Дан массив из 12 элементов. Обратитесь к любому, написав индекс:");
                int ind = int.Parse(Console.ReadLine());
                try
                {
                    int getEl = array[ind];
                    return true;
                }
                catch (IndexOutOfRangeException ex)
                {
                    writeErr?.Invoke(this, $"Прога упала, т.к.: {ex.Message}");
                    Console.WriteLine("ВЫ РОБОТ.");
                    return false;
                }
            }
            catch (DivideByZeroException e)
            {
                writeErr?.Invoke(this, $"Программа упала, так как: {e.Message}");
                return false;
            }
        }
    }
    class ErrorEventArgs : EventArgs
    {
        public string Response { get; set; }
        public ErrorEventArgs(object sender, string resp) => Response = resp;
    }
}
