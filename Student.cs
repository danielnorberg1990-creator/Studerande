using System;
using System.Collections.Generic;

// En studerande med namn samt en lista med kurser/utbildningar personen går på.
public class Student
{
    // Fält: namn, ålder och lista av kurser.
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<Course> Courses { get; set; }

    public Student(string firstName, string lastName, int age) // konstruktor för studerande
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Name = $"{firstName} {lastName}";
        Courses = new List<Course>();
    }

    // Metod Join(course) — gå med i en kurs. Returnerar true om studeranden anmäldes till kursen.
    public bool Join(Course course)
    {
        return course.Enroll(this);
    }

    // Metod Leave(course) — lämna en kurs. Returnerar true om studeranden togs bort ur kursen.
    public bool Leave(Course course)
    {
        return course.Remove(this);
    }

    // Metod Schedule() — skriver ut vilka kurser den studerande går i schema format.
    public void Schedule()
    {
        Console.WriteLine($"--- Schema för {Name} ---");
        if (Courses.Count == 0)
        //Felhantering ifall den studerande inte har någon anmäld kurs.
        {
            Console.WriteLine("  (ingen kurs anmäld)");
            return;
        }

        foreach (var course in Courses)
        {
            Console.WriteLine($"  {course.Name}"); //Listar namnet på kurserna, en efter en.
        }
    }

    // En ToString() med den studerandes namn.
    public override string ToString()
    {
        return Name;
    }

    // Fullständigt namn (förnamn + efternamn).
    public string FullName => $"{FirstName} {LastName}";
}

