using System;
using System.Collections.Generic;

// En kurs/utbildning med namn, en kapacitet (max antal platser) och en lista med studerande.
public class Course
{
    // Fält: namn, max antal platser och lista av studerande.
    public string Name { get; set; }
    public int MaxSeats { get; set; }
    public List<Student> Students { get; set; }

    // Kursen kan skapas med bara ett namn — antalet platser har då ett standardvärde.
    public Course(string name, int maxSeats = 10)
    {
        Name = name;
        MaxSeats = maxSeats;
        Students = new List<Student>();
    }

    // Metod Enroll(student) — anmäler en studerande till kursen, om det finns plats.
    public bool Enroll(Student student)
    {
        // Felhantering: samma studerande kan inte anmäla sig flera gånger till samma kurs.
        if (Students.Contains(student))
        {
            Console.WriteLine($"  [Fel] {student.FullName} är redan anmäld till {Name}.");
            return false;
        }

        // Felhantering: kursen är full.
        if (Students.Count >= MaxSeats)
        {
            Console.WriteLine($"  [Fel] {Name} är full ({MaxSeats} platser). Inga platser kvar.");
            return false;
        }

        Students.Add(student);
        student.Courses.Add(this); // håll studerandens schema uppdaterat
        return true;
    }

    // Metod Remove(student) — tar bort en studerande ur kursen.
    public void Remove(Student student)
    {
        if (Students.Remove(student))
        {
            student.Courses.Remove(this); // håll studerandens schema uppdaterat
        }
    }

    // Metod RollCall() — skriver ut alla studerande i kursen.
    public void RollCall()
    {
        Console.WriteLine($"--- {Name} ({Students.Count}/{MaxSeats} platser) ---");
        if (Students.Count == 0)
        {
            Console.WriteLine("  (inga studerande)");
            return;
        }

        foreach (var student in Students)
        {
            Console.WriteLine($"  {student.FullName} ({student.Age} år)");
        }
    }

    // En ToString() som t.ex. ger "Matematik (2/5 platser)".
    public override string ToString()
    {
        return $"{Name} ({Students.Count}/{MaxSeats} platser)";
    }
}

