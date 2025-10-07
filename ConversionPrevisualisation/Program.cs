using System.Text.Json;
using System.Xml.Linq;

internal class Person // Definition de la classe Person cohérente à la structure du fichier Peoples.json
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            // Etape 1 : Importer le JSON depuis le fichier Data/Peoples.json
            var jsonFilePath = "../../../Data/Peoples.json"; //chemin du dossier Data
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("Erreur : Le fichier Data/Peoples.json n'existe pas.");
                return;
            }

            var json = File.ReadAllText(jsonFilePath); // charge le dossier json
            var people =
                JsonSerializer
                    .Deserialize<
                        List<Person>>(
                        json); // crée des objets Person en fonction du fichier Peoples.json dans une Liste

            // Étape 2 : Proposition de recherche
            IEnumerable<Person> filteredPeople = people; // IEnumerable de Person
            while (true) // boucle infinie 
            {
                Console.WriteLine("Voulez-vous appliquer un filtre de recherche ? (o/n)");
                var applyFilter = Console.ReadLine()?.ToLower() == "o";

                if (!applyFilter) break; // on skip

                Console.WriteLine("Choisissez le champ pour la recherche : 1-Name, 2-Age, 3-City");
                var fieldChoice = Console.ReadLine();
                Console.Write("Entrez les caractères à rechercher (pour Age : >10 ou <10) : ");
                var value = Console.ReadLine();

                switch (fieldChoice)
                {
                    case "1":
                        filteredPeople =
                            filteredPeople.Where(p => p.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "2":
                        if (value.StartsWith(">"))
                        {
                            if (int.TryParse(value[1..], out var minAge))
                                filteredPeople = filteredPeople.Where(p => p.Age > minAge);
                            else
                                Console.WriteLine("Valeur invalide pour l'âge (>).");
                        }
                        else if (value.StartsWith("<"))
                        {
                            if (int.TryParse(value[1..], out var maxAge))
                                filteredPeople = filteredPeople.Where(p => p.Age < maxAge);
                            else
                                Console.WriteLine("Valeur invalide pour l'âge (<).");
                        }
                        else
                        {
                            if (int.TryParse(value, out var ageValue))
                                filteredPeople = filteredPeople.Where(p => p.Age == ageValue);
                            else
                                Console.WriteLine("Valeur invalide pour l'âge.");
                        }

                        break;
                    case "3":
                        filteredPeople =
                            filteredPeople.Where(p => p.City.Contains(value, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        Console.WriteLine("Choix invalide, aucun filtre appliqué.");
                        break;
                }

                // Affichage des données actuelles
                DisplayPeople(filteredPeople);

                Console.WriteLine("Voulez-vous refiltrer ou passer à la prochaine étape ? (r/n)");
                if (Console.ReadLine()?.ToLower() != "r") break; // retour au tri
            }

            // Étape tri : Choix du champ et ordre, en boucle
            var sortedPeople = filteredPeople;
            while (true)
            {
                Console.WriteLine("Choisissez le tri : 1-Par Age, 2-Par Name, 3-Par City");
                var sortChoice = Console.ReadLine();
                Console.WriteLine("Ordre : 1-Croissant, 2-Décroissant");
                var orderChoice = Console.ReadLine();

                switch (sortChoice)
                {
                    case "1":
                        sortedPeople = orderChoice == "1"
                            ? sortedPeople.OrderBy(p => p.Age)
                            : sortedPeople.OrderByDescending(p => p.Age);
                        break;
                    case "2":
                        sortedPeople = orderChoice == "1"
                            ? sortedPeople.OrderBy(p => p.Name)
                            : sortedPeople.OrderByDescending(p => p.Name);
                        break;
                    case "3":
                        sortedPeople = orderChoice == "1"
                            ? sortedPeople.OrderBy(p => p.City)
                            : sortedPeople.OrderByDescending(p => p.City);
                        break;
                    default:
                        Console.WriteLine("Choix invalide, tri par défaut (Age croissant).");
                        sortedPeople = sortedPeople.OrderBy(p => p.Age);
                        break;
                }

                // Affichage des données après tri
                DisplayPeople(sortedPeople);

                Console.WriteLine("Voulez-vous encore trier ? (o/n)");
                if (Console.ReadLine()?.ToLower() != "o") break; // Fin des tri
            }

            // Étape export
            var finalResults = sortedPeople.ToList();
            Console.WriteLine("Voulez-vous exporter les résultats ? (o/n)");
            if (Console.ReadLine()?.ToLower() == "o")
            {
                Console.WriteLine("Choisissez le format : 1-CSV, 2-XML");
                var exportChoice = Console.ReadLine();

                // Affichage des données finales
                DisplayPeople(finalResults);

                Console.WriteLine("Confirmer l'export ? (o/n)");
                if (Console.ReadLine()?.ToLower() == "o")
                {
                    Console.Write("Entrez le nom du fichier (sans extension) : ");
                    var fileName = Console.ReadLine();
                    var exportDir = "../../../Data/serialization";
                    Directory.CreateDirectory(exportDir); // Créer le dossier si inexistant

                    switch (exportChoice)
                    {
                        case "1":
                            ExportToCsv(finalResults, Path.Combine(exportDir, $"{fileName}.csv"));
                            Console.WriteLine($"Exporté vers {Path.Combine(exportDir, $"{fileName}.csv")}");
                            break;
                        case "2":
                            ExportToXml(finalResults, Path.Combine(exportDir, $"{fileName}.xml"));
                            Console.WriteLine($"Exporté vers {Path.Combine(exportDir, $"{fileName}.xml")}");
                            break;
                        default:
                            Console.WriteLine("Choix invalide, export annulé.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Export annulé.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }
    }

    // affiche l'etat de la liste actuelle
    private static void DisplayPeople(IEnumerable<Person> people)
    {
        var results = people.ToList();
        Console.WriteLine("\nDonnées actuelles :");
        foreach (var person in results) Console.WriteLine($"{person.Name}, {person.Age} ans, {person.City}");
    }

    // Export CSV
    private static void ExportToCsv(List<Person> people, string filePath)
    {
        Console.WriteLine("Voulez-vous exporter tous les champs dans un fichier CSV ? (o/n)");
        var exportAllFields = Console.ReadLine()?.ToLower() == "o";
        List<string> fieldsToKeep;
        if (!exportAllFields)
        {
            Console.WriteLine("Veuillez choisir les champs à exporter : 1-Name, 2-Age, 3-City (Ex : 1,2) : ");
            var fieldMap = new Dictionary<string, string> { { "1", "Name" }, { "2", "Age" }, { "3", "City" } };
            fieldsToKeep = Console.ReadLine()
                .Split(',')
                .Select(f => fieldMap.GetValueOrDefault(f.Trim()))
                .Where(f => f != null)
                .ToList();
        }
        else
        {
            fieldsToKeep = new List<string> { "Name", "Age", "City" };
        }

        string EscapeCsvField(string field)
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        var header =
            string.Join(";",
                fieldsToKeep); // la virgule simple ne sépare les champs dans des colonnes différentes, en fonction de la langue de Excel
        var csv = header + "\n" + string.Join("\n",
            people.Select(p =>
                string.Join(";", fieldsToKeep.Select(f =>
                    EscapeCsvField(p.GetType().GetProperty(f)?.GetValue(p)?.ToString() ?? "")))
            ));
        File.WriteAllText(filePath, csv); // sauvegarde le fichier CSV
    }


    // Export vers XML
    private static void ExportToXml(List<Person> people, string filePath)
    {
        Console.WriteLine("Voulez vous exporter tous les champs dans un fichier XML ? (o/n)");
        var exportAllFields = Console.ReadLine()?.ToLower() == "o";
        var doc = new XDocument();
        if (!exportAllFields)
        {
            Console.WriteLine("Veuillez choisir les champs à exporter : 1-Name, 2-Age, 3-City (Ex : 1,2) : ");
            var fieldMap = new Dictionary<string, string> { { "1", "Name" }, { "2", "Age" }, { "3", "City" } };
            var fieldsToKeep = Console.ReadLine()
                .Split(',')
                .Select(f => fieldMap.GetValueOrDefault(f.Trim()))
                .Where(f => f != null)
                .ToList();
            doc.Add(new XElement("People",
                    people.Select(p =>
                    {
                        var personElement = new XElement("Person");
                        foreach (var c in fieldsToKeep)
                            personElement.Add(new XElement(c,
                                p.GetType().GetProperty(c)
                                    ?.GetValue(p))); // ajoute les éléments de la liste à l'élément Person
                        return personElement;
                    })
                )
            );
        }
        else
        {
            doc.Add(
                new XElement("People", // racine du document
                    people.Select(p => new XElement("Person",
                        new XElement("Name", p.Name),
                        new XElement("Age", p.Age),
                        new XElement("City", p.City)
                    ))
                )
            );
        }

        doc.Save(filePath); // sauvegarde
    }
}