using System;
using System.Collections.Generic;

// Course
// En kurs kan ha flera deltagare.
// Felhantering så att en studerande inte anmäler sig flera gånger till samma kurs.
//
// Student
// Måste innehålla felhantering för att kontrollera att man inte kan
// anmäla sig till samma kurs flera gånger.

class Program
{
    static void Main()
    {
        // Tillgängliga utbildningar med respektive antal platser (MaxSeats).
        Course server = new Course("Server och virtualiseringsspecialist", 10);
        Course mjukvara = new Course("Mjukvaruutvecklare", 25);
        Course webbdesign = new Course("Webbdesign", 15);

        List<Course> availableCourses = new List<Course>
        {
            server,
            mjukvara,
            webbdesign
        };

        // Skapa studerande.
        Student alice = new Student("Alice");
        Student bob = new Student("Bob");
        Student carol = new Student("Carol");

        // Anmäl studerande till kurser.
        alice.Join(server);
        alice.Join(webbdesign);
        bob.Join(server);

        // Felhantering: Alice försöker anmäla sig till Server ... igen.
        Console.WriteLine("\n== Försök på dubbelanmälan ==");
        alice.Join(server);

        // Visa scheman och röstlappar.
        Console.WriteLine("\n== Scheman ==");
        alice.Schedule();
        bob.Schedule();
        carol.Schedule();

        Console.WriteLine("\n== Röstlapp på alla kurser ==");
        foreach (var course in availableCourses)
        {
            course.RollCall();
        }

        // Carol lämnar en kurs hon aldrig varit med i (kontroll av felhantering).
        Console.WriteLine("\n== Carol lämnar Server och virtualiseringsspecialist ==");
        carol.Leave(server);

        Console.WriteLine("\nKurslista via ToString():");
        foreach (var course in availableCourses)
        {
            Console.WriteLine($"  {course}");
        }
    }
}
