using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using InstituteCore;

namespace Solution_for_Lab3
{
    class InstituteHelper
    {
        private InstituteHelper()
        {
            // Запрещаем создавать экземпляр
        }

        private static List<Institute> dataBase = new List<Institute>();
        public static void AddInstitute(Institute institute)
        {
            if (institute == null)
                throw new ArgumentNullException(nameof(institute));

            dataBase.Add(institute);
        }
        public static bool RemoveInstitute(Institute institute)
        {
            return dataBase.Remove(institute);
        }

        public static void AddStudInfoIntoTheGroup(Institute institute)
        {
            if (institute == null)
                throw new Exception("В параметрах null, перепроверьте!");

            // Выбор курса
            Console.WriteLine("> Выбери индекс курса!");
            for (int i = 0; i < institute.Courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}) Курс {institute.Courses[i].Number}");
            }

            int localCourse = int.Parse(Console.ReadLine()) - 1;
            if (localCourse < 0 || localCourse >= institute.Courses.Count)
                throw new IndexOutOfRangeException("Неверный индекс курса.");

            // Выбор группы
            var groups = institute.Courses[localCourse].GetGroups();
            Console.WriteLine("> Выбери индекс группы:");
            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {groups[i].GroupName}");
            }

            int localGroup = int.Parse(Console.ReadLine()) - 1;
            if (localGroup < 0 || localGroup >= groups.Count)
                throw new IndexOutOfRangeException("Неверный индекс группы.");

            // Ввод студента
            Console.WriteLine("\n> Ввод данных студента:");
            Console.Write("Имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Фамилия: ");
            string surName = Console.ReadLine();

            Console.Write("Отчество: ");
            string patronymic = Console.ReadLine();

            var student = new Institute.Student(firstName, surName, patronymic);
            var group = groups[localGroup];

            Console.WriteLine("Сколько предметов у студента?");
            int subs = int.Parse(Console.ReadLine());

            for (int i = 0; i < subs; ++i)
            {
                Console.Write("Введи название предмета и оценку (2-5) через пробел -> ");
                string[] tokens = Console.ReadLine().Split();
                student.AddSubject(new Institute.Subject(tokens[0], byte.Parse(tokens[1])));
            }

            group.AddStudent(student);

            Console.WriteLine($"Студент {firstName} {surName} добавлен в группу {group.GroupName}.");
        }
        public static void RemoveStudInfoFromTheGroup(Institute institute)
        {
            if (institute == null)
                throw new Exception("В параметрах null, перепроверьте!");

            // Выбор курса
            Console.WriteLine("> Выбери индекс курса!");
            for (int i = 0; i < institute.Courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}) Курс {institute.Courses[i].Number}");
            }

            int localCourse = int.Parse(Console.ReadLine()) - 1;
            if (localCourse < 0 || localCourse >= institute.Courses.Count)
                throw new IndexOutOfRangeException("Неверный индекс курса.");

            // Выбор группы
            var groups = institute.Courses[localCourse].GetGroups();
            Console.WriteLine("> Выбери индекс группы:");
            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {groups[i].GroupName}");
            }

            int localGroup = int.Parse(Console.ReadLine()) - 1;
            if (localGroup < 0 || localGroup >= groups.Count)
                throw new IndexOutOfRangeException("Неверный индекс группы.");

            var group = groups[localGroup];
            var students = group.GetStudents;

            if (students.Count == 0)
            {
                Console.WriteLine("В этой группе нет студентов!");
                return;
            }

            // Выбор студента
            Console.WriteLine("> Выбери студента для удаления:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {students[i].FirstName} {students[i].SurName} {students[i].Patronymic}");
            }

            int localStudent = int.Parse(Console.ReadLine()) - 1;
            if (localStudent < 0 || localStudent >= students.Count)
                throw new IndexOutOfRangeException("Неверный индекс студента.");

            var student = students[localStudent];

            // Удаление
            group.RemoveStudent(student);
            Console.WriteLine($"Студент {student.FirstName} {student.SurName} удалён из группы {group.GroupName}.");
        }


        public static void SaveToFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                foreach (Institute ins in dataBase)
                {
                    writer.WriteLine($"Институт: {ins.Naming}");
                    foreach (var course in ins.Courses)
                    {
                        writer.WriteLine($"  Курс {course.Number}");
                        foreach (var group in course.GetGroups())
                        {
                            writer.WriteLine($"    Группа: {group.GroupName}");
                            foreach (var student in group.GetStudents)
                            {
                                writer.WriteLine($"      Студент: {student.FirstName} {student.SurName} {student.Patronymic}");
                                foreach (var subject in student.GetSubjects)
                                {
                                    writer.WriteLine($"        Предмет: {subject.Title}, оценка ({subject.Mark})");
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void PrintByVariant(Institute itt)
        {
            if (itt == null)
            {
                Console.WriteLine("База пуста!");
                return;
            }

            
            if (itt.Courses.Count < 1)
            {
                Console.WriteLine("Нет курсов!"); return;
            }
            foreach (var crs in itt.Courses)
            {
                if (crs.GetGroups().Count < 1)
                {
                    Console.WriteLine("Нет групп!"); return;
                }
                foreach (var gr in crs.GetGroups())
                {
                    if (gr.GetStudents.Count < 1)
                    {
                        Console.WriteLine("Нет студентов!"); return;
                    }
                    foreach (var st in gr.GetStudents)
                    {
                        bool hasOnlyGoodMarks = true;
                        foreach (var subject in st.GetSubjects)
                        {
                            if (subject.Mark < 3)
                            {
                                hasOnlyGoodMarks = false;
                                break;
                            }
                        }
                        if (hasOnlyGoodMarks)
                        {
                            Console.WriteLine($"Ученик-хорошист: {st.FirstName} {st.SurName} {st.Patronymic}");
                        }
                    }
                }
            }
        }
        public static Institute ConstructInstituteInstance()
        {
            Console.Write("Введите название института: ");
            string name = Console.ReadLine();

            Institute institute = new Institute();
            institute.msgType += institute.PrintMagenta;
            institute.Naming = name;

            Console.Write("Сколько курсов в институте? ");
            int courseCount = int.Parse(Console.ReadLine());

            for (int i = 0; i < courseCount; i++)
            {
                Console.Write($"Введите номер курса {i + 1} (1-4): ");
                int courseNumber = int.Parse(Console.ReadLine());

                Institute.Course course = new Institute.Course(courseNumber, institute);
                institute.msgType("Создан новый курс!");

                course.FillGroups();
                institute.msgType += institute.PrintYellow;
                institute.msgType("Заполнены группы! (добавлена ссылка на желтый метод)");
                institute.AddCourse(course);
            }

            institute.msgType = delegate
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("ИНСТИТУТ СОЗДАН (делегат бирюзовый)");
                Console.ForegroundColor = oldColor;
            };
            return institute;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Проверка на капчу...");
            PseudoCaptcha captcha = new PseudoCaptcha();
            captcha.writeErr += (sender, msg) => Console.WriteLine("Событие: " + msg);
            bool getResult = captcha.Check();
            if (!getResult) return;

            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("-= Институтская система управления =-");

            Institute currentInstitute = null;
            char choice = ' ';

            while (choice != 'q')
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("[1] Создать новый институт");
                Console.WriteLine("[2] Добавить студента в группу");
                Console.WriteLine("[3] Удалить студента из группы");
                Console.WriteLine("[4] Сохранить данные в файл");
                Console.WriteLine("[5] Удалить институт");
                Console.WriteLine("[6] ВЫВЕСТИ ЧУВАКОВ БЕЗ ДВОЕК И ТРОЕК");
                Console.WriteLine("[q] Выйти");

                Console.Write("\nВыберите действие: ");
                choice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (choice)
                {
                    case '1':
                        currentInstitute = InstituteHelper.ConstructInstituteInstance();
                        break;

                    case '2':
                        if (currentInstitute != null)
                        {
                            InstituteHelper.AddStudInfoIntoTheGroup(currentInstitute);
                        }
                        else
                        {
                            Console.WriteLine("Сначала создайте институт!");
                        }
                        break;

                    case '3':
                        if (currentInstitute != null)
                        {
                            InstituteHelper.RemoveStudInfoFromTheGroup(currentInstitute);
                        }
                        else
                        {
                            Console.WriteLine("Сначала создайте институт!");
                        }
                        break;

                    case '4':
                        if (currentInstitute != null)
                        {
                            InstituteHelper.AddInstitute(currentInstitute);
                            InstituteHelper.SaveToFile("institute_data.txt");
                            Console.WriteLine("Данные сохранены в файл institute_data.txt");
                        }
                        else
                        {
                            Console.WriteLine("Сначала создайте институт!");
                        }
                        break;

                    case '5':
                        if (currentInstitute != null)
                        {
                            InstituteHelper.RemoveInstitute(currentInstitute);
                            currentInstitute = null;
                            Console.WriteLine("Институт удалён.");
                        }
                        else
                        {
                            Console.WriteLine("Институт ещё не создан!");
                        }
                        break;

                    case '6':
                        if (currentInstitute != null)
                        {
                            InstituteHelper.PrintByVariant(currentInstitute);
                        }
                        break;
                    case 'q':
                        Console.WriteLine("Выход из программы...");
                        break;

                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }
    }
}
