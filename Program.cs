using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


class Program
{
    // Alla kurser och studerande hålls i globala listor så att de finns kvar under menyn.
    static List<Course> courses = new List<Course>();
    static List<Student> students = new List<Student>();

    static void Main()
    {
        LoadFromFiles(); // Hämtar upp kurslista och studentlista ur courses.txt och students.txt om filerna är skapade.

        bool exit = false;
        while (!exit)
        {
            //Menyval som finns och är lagt i en switch för att förenkla kodhanteringen och gränssnittet.
            ShowMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddCourse(); // lägg till kurs
                    break;
                case "2":
                    AddStudent(); // lägg till studerande
                    break;
                case "3":
                    EnrollStudent(); // anmäla studerande till kurs (via kursens Enroll)
                    break;
                case "4":
                    ShowCourses(); // visa alla kurser
                    break;
                case "5":
                    ShowStudents(); // visa alla studerande
                    break;
                case "6":
                    SaveToFile(); // Sparar till filerna courses.txt och students.txt
                    break;
                case "7":
                    RemoveStudentFromCourse(); // ta bort studerande från kurs (via kursens Remove)
                    break;
                case "0": // Avslutar programmet
                    Console.WriteLine("Programmet avslutas.");
                    exit = true;
                    break;
                default:
                    //Felhantering om annan siffra utanför switchens menyval anges.
                    Console.WriteLine("Ogiltigt val. Välj ett nummer mellan 0 och 7.");
                    break;
            }
        }
    }

    // Visar huvudmenyn med valen nedan.
    static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("===== STUDERANDEHANTERING =====");
        Console.WriteLine("1. Lägg till en kurs/utbildning");
        Console.WriteLine("2. Lägg till en studerande");
        Console.WriteLine("3. Anmäl en studerande till en kurs (via kursens Enroll)");
        Console.WriteLine("4. Visa alla kurser");
        Console.WriteLine("5. Visa alla studerande");
        Console.WriteLine("6. Spara listor till .txt-filer");
        Console.WriteLine("7. Ta bort en studerande från en kurs (via kursens Remove)");
        Console.WriteLine("0. Avsluta");
        Console.Write("Välj: ");
    }

    // Skapa en kurs manuellt med namn och valfritt antal platser som kursen ska innehålla.
    static void AddCourse()
    {
        //frågar användaren efter namn på kursen som ska skapas.
        Console.Write("Namn på utbildningen: ");
        string name = Console.ReadLine();

        // felhantering om namnet är tomt eller null.
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("[Fel] Namnet får inte vara tomt.");
            return;
        }

        int maxSeats = PromptSeats(); // frågar användaren efter antal platser, om inget värde anges så blir standardvärdet 10.

        courses.Add(new Course(name, maxSeats)); // skapar en ny kurs med namnet och antal platser.
        Console.WriteLine($"Kursen \"{name}\" ({maxSeats} platser) har lagts till."); //bekräftelse att kursen med antalet platser lagts till.
    }

    // Frågar efter antal platser, om inget värde anges så blir standardvärdet 10.
    static int PromptSeats()
    {
        Console.Write("Antal platser [10]: ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int seats) || seats <= 0) // om användaren inte anger något eller om det är ett negativt tal eller en sträng, så blir standardvärdet 10
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

        //Felhantering ifall värdet är null eller tomt.
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("[Fel] Förnamn eller efternamn får inte vara tomt.");
            return;
        }

        Console.Write("Ålder: "); // ålder på den studerande
        int age = 0;

        //Felhantering för att hantera att man inte kan skriva en negativ ålder.
        while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
        {
            Console.Write("Ålder (felaktigt värde, försök igen): ");
        }

        students.Add(new Student(firstName, lastName, age));
        Console.WriteLine($"Studerande {firstName} {lastName} ({age} år) har lagts till."); //Bekräftelse att den studerande lagts till.
    }

    // Låter användaren anmäla en studerande till en kurs från kursens håll,
    // dvs. genom att anropa kursens egen metod Enroll.
    // Listan visar både de ej anmälda och de redan anmälda till kursen,
    // så att regeln för dubbelanmälan går att testa.
    // Om kursen redan är fullsatt visas ett meddelande och man återgår till huvudmenyn.
    static void EnrollStudent()
    {
        // Felhantering om ingen studerande eller kurs valts.
        if (students.Count == 0 || courses.Count == 0)
        {
            Console.WriteLine("[Fel] Du behöver minst en studerande och en kurs först.");
            return;
        }

        // Avbryter anmälan och återgår till huvudmenyn ifall inget val gjorts.
        Course course = PickCourse();
        if (course == null) return;

        // Felhantering: om kursen är fullsatt kan ingen till anmälas – avbryt och återgå till huvudmenyn.
        if (course.Students.Count >= course.MaxSeats)
        {
            Console.WriteLine("Kursen är fullsatt, ta bort elever om du vill anmäla fler.");
            return;
        }

        Student student = PickStudentForEnrollment(course); // Visar både de ej anmälda och de redan anmälda till kursen.

        bool enrolled = course.Enroll(student); // Kursens egen metod – hanterar dubbelanmälan och full kurs.
        if (enrolled)
        {
            Console.WriteLine($"{student.FullName} har anmälts till {course.Name}."); //Bekräftelse på anmälan.
        }
    }

    // Lister alla studerande i två grupper – de ej anmälda till kursen först och
    // de redan anmälda efteråt – så att båda grupperna syns och går att välja.
    // Returnerar den valda studeranden, eller null om användaren avbryter valet.
    static Student PickStudentForEnrollment(Course course)
    {
        // Dela upp studerandena i de som inte är anmälda och de som redan är anmälda till kursen.
        List<Student> notEnrolled = new List<Student>();
        List<Student> alreadyEnrolled = new List<Student>();
        for (int i = 0; i < students.Count; i++)
        {
            if (course.Students.Contains(students[i]))
            {
                alreadyEnrolled.Add(students[i]);
            }
            else
            {
                notEnrolled.Add(students[i]);
            }
        }

        // Gemensam lista där de ej anmälda kommer först – numreringen följer denna ordning.
        List<Student> all = new List<Student>();
        all.AddRange(notEnrolled);
        all.AddRange(alreadyEnrolled);

        Console.WriteLine($"Ej anmälda till {course.Name}:");
        for (int i = 0; i < notEnrolled.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {notEnrolled[i].FullName} ({notEnrolled[i].Age} år)");
        }
        if (notEnrolled.Count == 0)
        {
            Console.WriteLine("  (inga)");
        }

        Console.WriteLine($"Redan anmälda till {course.Name}:");
        for (int i = 0; i < alreadyEnrolled.Count; i++)
        {
            Console.WriteLine($"  {notEnrolled.Count + i + 1}. {alreadyEnrolled[i].FullName} ({alreadyEnrolled[i].Age} år)");
        }
        if (alreadyEnrolled.Count == 0)
        {
            Console.WriteLine("  (inga)");
        }

        return PickFromList(all, "Välj studerande: ");
    }

    // Låter användaren ta bort en studerande från en kurs från kursens håll,
    // dvs. genom att anropa kursens egen metod Remove.
    // Endast studerande som är anmälda till kursen visas och kan väljas.
    static void RemoveStudentFromCourse()
    {
        // Felhantering om ingen studerande eller kurs valts.
        if (students.Count == 0 || courses.Count == 0)
        {
            Console.WriteLine("[Fel] Du behöver minst en studerande och en kurs först.");
            return;
        }

        // Återgå till menyn ifall felaktigt val gjorts. Vilken kurs som helst kan väljas, även utan studerande.
        Course course = PickCourse();
        if (course == null) return;

        // Om ingen studerande är anmäld till kursen finns det ingen att ta bort.
        if (course.Students.Count == 0)
        {
            Console.WriteLine($"Inga studerande är anmälda till {course.Name}.");
            return;
        }

        ShowEnrolledIn(course); // Visar de anmälda numrerat så användaren vet vilken siffra som hör till vilket namn.
        Student student = PickFromList(course.Students, "Välj studerande: "); // Endast de anmälda till kursen kan väljas.

        bool removed = course.Remove(student); // Kursens egen metod – avgör om hen var anmäld.
        if (removed)
        {
            Console.WriteLine($"{student.FullName} har tagits bort från {course.Name}."); //Bekräftelse på borttag.
        }
    }

    // Skriver ut vilka studerande som är anmälda till en kurs, som information.
    static void ShowEnrolledIn(Course course)
    {
        if (course.Students.Count == 0)
        {
            Console.WriteLine($"Inga studerande är anmälda till {course.Name}.");
            return;
        }

        // Skriver ut namnen på de anmälda så användaren vet vilken siffra som hör till vilket namn.
        Console.WriteLine($"Studerande anmälda till {course.Name}:");
        for (int i = 0; i < course.Students.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {course.Students[i].FullName} ({course.Students[i].Age} år)");
        }
    }

    // Låter användaren välja en studerande ur listan.
    static Student PickStudent()
    {
        //Räknar upp alla studerande med namn och ålder.
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
        //Räknar upp alla kurser med namn och antal platser.
        Console.WriteLine("Kurser:");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {courses[i]}");
        }

        return PickFromList(courses, "Välj kurs: ");
    }

    // Hjälpmetod: läser in ett giltigt listval och returnerar objektet från listan.
    // Variabeln T är generisk och kan därför innehålla både studerande och courses i detta fall.
    static T PickFromList<T>(List<T> items, string prompt)
    {
        int count = items.Count; // sparar hu många objekt listan innehåller.
        int choice;  // deklarerar variabeln choice
        Console.Write(prompt);

        // loop som fortsätter tills användaren gjort ett giltigt val i programmet.
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > count)
        {
            Console.Write($"Ogiltigt val. Välj 1-{count}: ");
        }
        return items[choice - 1];
    }

    // Visar alla kurser som är registrerade.
    static void ShowCourses()
    {
        //Felhantering för att kontrollera om det finns kurser.
        if (courses.Count == 0)
        {
            Console.WriteLine("Inga kurser har lagts till.");
            return;
        }

        // funktion som kallas för att gå igenom alla kurser en efter en tills listan är slut.
        foreach (var course in courses)
        {
            course.RollCall();
        }
    }

    // Visar alla studerande med deras schema.
    static void ShowStudents()
    {
        //Felhantering för att kontrollera om det finns studerande.
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

    // Spara listorna till .txt-filer: students.txt och courses.txt i detta fallet för att kunna återställa listorna när programmet körs igen.
    static void SaveToFile()
    {
        SaveStudents();
        SaveCourses();
    }

    // Sparar studentlistan till students.txt.
    static void SaveStudents()
    {
        string path = "students.txt"; //filnamnet som ska sparas
        var sb = new StringBuilder(); //StringBuilder används för att skriva ut studentlistan till fil.
        sb.AppendLine("Studerande:"); // Första raden i txt filen, anges för att förenkla vad listan innehåller.
        sb.AppendLine(new string('-', 40)); //Upprepar tecknet "-" 40 gånger för radbrytning.

        foreach (var student in students) //Loopar igenom listan av registrerade studenter och skapar en rad i txt filen per studerande.
        {
            string kursar = student.Courses.Count == 0
                ? "Ingen kurs"
                : string.Join(", ", student.Courses.Select(c => c.Name)); //Skapar en sträng med kursnamnen för studerande.
            sb.AppendLine($"{student.FullName} | {student.Age} år | Kurser: {kursar}");
        }

        File.WriteAllText(path, sb.ToString()); //Sparar listan till filen.
        Console.WriteLine($"Studentlista sparad till {path} ({students.Count} st)."); //Bekräftelse att listan sparades korrekt till filen.

    }

    // Sparar kurslistan till courses.txt.
    static void SaveCourses()
    {
        string path = "courses.txt"; //filnamnet på filen som ska sparas.
        var sb = new StringBuilder(); // StringBuilder används för att skriva ut courses listan till fil.
        sb.AppendLine("Kurser/utbildningar:"); //Skapar en sträng med kursnamnen för studerande.
        sb.AppendLine(new string('-', 40)); // upprepar tecknet "-" 40 gånger för radbrytning.

        foreach (var course in courses)
        {
            string deltagare = course.Students.Count == 0 // om kursen inte har någon deltagare skrivs "Inga deltagare" ut, annars skrivs fulla namnet på studenten, kursens namn, hur många platser som är tagna samt vilka deltagare i kursen.
                ? "Inga deltagare"
                : string.Join(", ", course.Students.Select(s => s.FullName));
            sb.AppendLine($"{course.Name} | Platser: {course.Students.Count}/{course.MaxSeats} | Deltagare: {deltagare}");
        }

        File.WriteAllText(path, sb.ToString()); // Skriver till fil.
        Console.WriteLine($"Kurslista sparad till {path} ({courses.Count} st)."); //Bekräftelse att filen är sparad.
    }

    // Laddar upp kurslista och studentlista ur .txt-filerna när programmet startar.
    static void LoadFromFiles()
    {
        LoadCourses();
        LoadStudents();

        if (courses.Count == 0 && students.Count == 0)
        {
            //Felhantering ifall det inte finns några .txt filer. Startar då programmet utan fil.
            Console.WriteLine("Inga filer hittades – programmet startar tomt.");
        }
        else
        {
            Console.WriteLine($"Laddade {courses.Count} kurser och {students.Count} studerande från filerna."); //Bekräftelse att filerna har laddats in i programmet.
        }
    }

    // Läser kurslistan från courses.txt.
    // Radformat: "Kursnamn | Platser: antal/max | Deltagare: ..."
    static void LoadCourses()
    {
        string path = "courses.txt"; //filens namn.
        if (!File.Exists(path)) return; // om filen existerar återgå till meny.

        string[] lines = File.ReadAllLines(path); //läser in filen och sparar den som en array med strängar.
        foreach (string line in lines) // loopar igenom varje rad i arrayen
        {
            //Felhantering: om raden är tom eller börjar med "----" så skippas den.
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("---")) continue;

            //
            string[] parts = line.Split('|'); //Delar upp raden.
            if (parts.Length < 2) continue;

            string name = parts[0].Trim(); // Hämta kursnamnet från "Kursnamn: ..."

            // Hämta max antal platser ur "Platser: x/y"
            string info = parts[1].Trim(); // Hämta info om kursplatsen.
            if (!info.Contains("Platser:")) continue; // Om info inte innehåller "Platser:" så skippas den.

            string[] seatParts = info.Split(new[] { "Platser:", "/" }, StringSplitOptions.RemoveEmptyEntries); // Delar upp info om kursplatsen.
            if (seatParts.Length < 2 || !int.TryParse(seatParts[1].Trim(), out int maxSeats)) continue; // Om info inte innehåller "Platser:" så skippas den.


            courses.Add(new Course(name, maxSeats)); // Skapar en ny kurs med namnet och max antal platser.

        }
    }

    // Läser studentlistan från students.txt och återställer deras anmälningar.
    // Radformat: "Förnamn Efternamn | ålder år | Kurser: kurs1, kurs2"
    static void LoadStudents()
    {
        string path = "students.txt"; // Filnamnet.
        if (!File.Exists(path)) return; // Om filen inte finns, avbryter metoden.

        string[] lines = File.ReadAllLines(path); // Läser in alla rader från filen.
        foreach (string line in lines) // Loopar igenom varje rad.
        {
            //Felhantering för null och tomma strängar.
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("---")) continue;

            string[] parts = line.Split('|'); // Delar upp raden i delar.
            if (parts.Length < 2) continue; // Om det inte finns minst två delar, avbryter metoden.

            // Namnet kan innehålla mellanslag, så dela vid sista mellanslaget.
            string name = parts[0].Trim(); // Namnet kan innehålla mellanslag, delas vid sista mellanslaget.
            int lastSpace = name.LastIndexOf(' '); // Hittar sista mellanslaget inför delningen.
            if (lastSpace <= 0) continue; // Om det inte finns något mellanslag, avbryter metoden.

            string firstName = name.Substring(0, lastSpace).Trim(); // Namnet kan innehålla mellanslag, delas vid sista mellanslaget.
            string lastName = name.Substring(lastSpace + 1).Trim(); // Namnet kan innehålla mellanslag, delas vid sista mellanslaget.

            // Hämta åldern ur "xx år"
            if (!int.TryParse(parts[1].Trim().Replace("år", "").Trim(), out int age)) continue; // Om åldern inte kan konverteras till ett heltal, avbryter metoden.


            Student student = new Student(firstName, lastName, age); // Skapa en ny studerande med namn, ålder och en tom lista över kurs.

            students.Add(student);

            // Återställ anmälningarna om fältet "Kurser: ..." finns.
            if (parts.Length >= 3 && parts[2].Contains("Kurser:")) // Kontrollerar så raden har delats på 3.
            {
                string kursarText = parts[2].Replace("Kurser:", "").Trim(); // Plockar ut kursnamnet från den tredje delen.

                if (kursarText != "Ingen kurs" && kursarText.Length > 0) // Om det finns kurser, läses dem in.

                {
                    foreach (string kursNamn in kursarText.Split(',')) // splittar kursnamnen i kursarText med komma.
                    {
                        string kurs = kursNamn.Trim();
                        Course course = courses.FirstOrDefault(c => c.Name == kurs); // hittar kursen med namnet kursNamn.
                        if (course != null)
                        {
                            student.Join(course);
                        }
                    }
                }
            }
        }
    }
}
