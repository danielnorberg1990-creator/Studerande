using System;
using System.Collections.Generic;

// En studerande med ett namn och en lista med kurser/utbildningar hen går på.
public class Student
{
    // Fält: namn och lista av kurser.
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<Course> Courses { get; set; }

    public Student(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Name = $"{firstName} {lastName}";
        Courses = new List<Course>();
    }

    // Metod Join(course) — går med i en kurs.
    public void Join(Course course)
    {
        course.Enroll(this);
    }

    // Metod Leave(course) — lämnar en kurs.
    public void Leave(Course course)
    {
        course.Remove(this);
    }

    // Metod Schedule() — skriver ut vilka kurser den studerande går.
    public void Schedule()
    {
        Console.WriteLine($"--- Schema för {Name} ---");
        if (Courses.Count == 0)
        {
            Console.WriteLine("  (ingen kurs anmäld)");
            return;
        }

        foreach (var course in Courses)
        {
            Console.WriteLine($"  {course.Name}");
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

