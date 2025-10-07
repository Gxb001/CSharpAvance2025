namespace ZooManagement;

public abstract class Animal // classe abstraite
{
    // constructeur de la classe mere(abstraite)
    protected Animal(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public abstract string MakeSound(); // polymorphisme
}

public class Lion : Animal // classe Lio hérite de Animal
{
    public Lion(string name) : base(name)
    {
    }

    public override string MakeSound()
    {
        return "Roar";
    }
}

// pareil pour éléphant
public class Elephant : Animal
{
    public Elephant(string name) : base(name)
    {
    }

    public override string MakeSound()
    {
        return "Trump Trump, pas Donald";
    }
}

public class Zoo
{
    private readonly List<Animal> _animals = new(); //encapsulation 

    public void AddAnimal(Animal animal)
    {
        _animals.Add(animal);
    }

    // faire parler tous les animaux
    public void MakeAllAnimalsSpeak()
    {
        foreach (var animal in _animals) Console.WriteLine($"{animal.Name} dit: {animal.MakeSound()}");
    }
}

// démo
internal class Program
{
    private static void Main(string[] args)
    {
        var zoo = new Zoo();
        zoo.AddAnimal(new Lion("Simba"));
        zoo.AddAnimal(new Elephant("Dumbo"));
        zoo.MakeAllAnimalsSpeak();
    }
}