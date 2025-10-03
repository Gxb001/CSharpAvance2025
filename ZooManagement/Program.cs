
namespace ZooManagement
{
    public abstract class Animal // classe abstraite
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        // constructeur de la classe mere(abstraite)
        protected Animal(string name)
        {
            _name = name;
        }
        public abstract string MakeSound(); // polymorphisme
    }
    
    public class Lion : Animal // classe Lio hérite de Animal
    {
        public Lion(string name) : base(name) { }

        public override string MakeSound()
        {
            return "Roar";
        }
    }

    // pareil pour éléphant
    public class Elephant : Animal
    {
        public Elephant(string name) : base(name) { }

        public override string MakeSound()
        {
            return "Trump Trump, pas Donald";
        }
    }

    
    public class Zoo
    {
        private List<Animal> _animals = new List<Animal>(); //encapsulation 
        
        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
        }

        // faire parler tous les animaux
        public void MakeAllAnimalsSpeak()
        {
            foreach (var animal in _animals)
            {
                Console.WriteLine($"{animal.Name} dit: {animal.MakeSound()}");
            }
        }
    }

    // démo
    class Program
    {
        static void Main(string[] args)
        {
            Zoo zoo = new Zoo();
            zoo.AddAnimal(new Lion("Simba"));
            zoo.AddAnimal(new Elephant("Dumbo"));
            zoo.MakeAllAnimalsSpeak();
        }
    }
}