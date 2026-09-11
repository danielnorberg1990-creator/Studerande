using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


using System.Text;

// Course
// En kurs kan ha flera deltagare.
// Felhantering så att en studerande inte anmäler sig flera gånger till samma kurs.
//
// Student
// Måste innehålla felhantering för att kontrollera att man inte kan
// anmäla sig till samma kurs flera gånger.
//
// Programmet är interaktivt: du kan manuellt lägga till kurser och studerande,
// anmäla studerande till kurser samt spara listorna till .txt-filer.

class Program
{
    // Alla kurser och studerande hålls i globala listor så att de finns kvar under menyn.
    static List<Course> courses = new List<Course>();
    static List<Student> students = new List<Student>();

    static void Main()
    {
        bool exit = false;
        while (!exit)
        {
            ShowMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddCourse();
                    break;
                case "2":
                    AddStudent();
                    break;
                case "3":
                    EnrollStudent();
                    break;
                case "4":
                    ShowCourses();
                    break;
                case "5":
                    ShowStudents();
                    break;
                case "6":
                    SaveToFile();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Välj ett nummer mellan 0 och 6.");
                    break;
            }
        }
    }

    // Visar huvudmenyn.
    static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("===== STUDERANDEHANTERING =====");
        Console.WriteLine("1. Lägg till en kurs/utbildning");
        Console.WriteLine("2. Lägg till en studerande");
        Console.WriteLine("3. Anmäl en studerande till en kurs");
        Console.WriteLine("4. Visa alla kurser");
        Console.WriteLine("5. Visa alla studerande");
        Console.WriteLine("6. Spara listor till .txt-filer");
        Console.WriteLine("0. Avsluta");
        Console.Write("Välj: ");
    }

    // Skapar en kurs manuellt med namn (och valfritt antal platser).
    static void AddCourse()
    {
        Console.Write("Namn på utbildningen: ");
        string name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("[Fel] Namnet får inte vara tomt.");
            return;
        }

        int maxSeats = PromptSeats();
        courses.Add(new Course(name, maxSeats));
        Console.WriteLine($"Kursen \"{name}\" ({maxSeats} platser) har lagts till.");
    }

    // Frågar efter antal platser med standardvärdet 10.
    static int PromptSeats()
    {
        Console.Write("Antal platser [10]: ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int seats) || seats <= 0)
        {
            return 10; // standardvärde
        }

        return seats;
    }

    // Skapar en studerande manuellt med förnamn, efternamn och ålder.
    static void AddStudent()
    {
        Console.Write("Förnamn: ");
        string firstName = Console.ReadLine();

        Console.Write("Efternamn: ");
        string lastName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("[Fel] Förnamn och efternamn får inte vara tomt.");
            return;
        }

                Console.Write("Ålder: ");
        int age = 0;
        while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
        {
            Console.Write("Ålder (felaktigt värde, försök igen): ");
        }

        students.Add(new Student(firstName, lastName, age));
        Console.WriteLine($"Studerande {firstName} {lastName} ({age} år) har lagts till.");
    }

    // Låter användaren anmäla en studerande till en kurs.
    static void EnrollStudent()
    {
        if (students.Count == 0 || courses.Count == 0)
        {
            Console.WriteLine("[Fel] Du behöver minst en studerande och en kurs först.");
            return;
        }

        Student student = PickStudent();
        if (student == null) return;

        Course course = PickCourse();
        if (course == null) return;

        student.Join(course); // Enroll hanterar dubbelanmälan och full kurs
    }

    // Låter användaren välja en studerande ur listan.
    static Student PickStudent()
    {
        Console.WriteLine("Studerande:");
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {students[i].FullName} ({students[i].Age} år)");
        }

        return PickFromList(students, "Välj studerande: ");
    }

    // Låter användaren välja en kurs ur listan.
    static Course PickCourse()
    {
        Console.WriteLine("Kurser:");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {courses[i]}");
        }

        return PickFromList(courses, "Välj kurs: ");
    }

        // Hjälpmetod: läser in ett giltigt listval och returnerar objektet från listan.
        static T PickFromList<T>(List<T> items, string prompt)
        {
            int count = items.Count;
            int choice;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > count)
            {
                Console.Write($"Ogiltigt val. Välj 1-{count}: ");
            }
            return items[choice - 1];
        }

    // Visar alla kurser med röstlapp.
    static void ShowCourses()
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("Inga kurser har lagts till.");
            return;
        }

        foreach (var course in courses)
        {
            course.RollCall();
        }
    }

    // Visar alla studerande med deras schema.
    static void ShowStudents()
    {
        if (students.Count == 0)
        {
            Console.WriteLine("Inga studerande har lagts till.");
            return;
        }

        foreach (var student in students)
        {
            student.Schedule();
        }
    }

    // Spara listorna till .txt-filer: students.txt och courses.txt.
    static void SaveToFile()
    {
        SaveStudents();
        SaveCourses();
    }

    // Sparar studentlistan till students.txt.
    static void SaveStudents()
    {
        string path = "students.txt";
        var sb = new StringBuilder();
        sb.AppendLine("Studerande:");
        sb.AppendLine(new string('-', 40));

        foreach (var student in students)
        {
            string kursar = student.Courses.Count == 0
                ? "Ingen kurs"
                : string.Join(", ", student.Courses.Select(c => c.Name));
            sb.AppendLine($"{student.FullName} | {student.Age} år | Kurser: {kursar}");
        }

        File.WriteAllText(path, sb.ToString());
        Console.WriteLine($"Studentlista sparad till {path} ({students.Count} st).");
    }

    // Sparar kurslistan till courses.txt.
    static void SaveCourses()
    {
        string path = "courses.txt";
        var sb = new StringBuilder();
        sb.AppendLine("Kurser/utbildningar:");
        sb.AppendLine(new string('-', 40));

        foreach (var course in courses)
        {
            string deltagare = course.Students.Count == 0
                ? "Inga deltagare"
                : string.Join(", ", course.Students.Select(s => s.FullName));
            sb.AppendLine($"{course.Name} | Platser: {course.Students.Count}/{course.MaxSeats} | Deltagare: {deltagare}");
        }

        File.WriteAllText(path, sb.ToString());
        Console.WriteLine($"Kurslista sparad till {path} ({courses.Count} st).");
    }
}
