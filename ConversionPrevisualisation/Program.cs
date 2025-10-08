using System.Text.Json;
using System.Xml.Linq;

internal class DynamicObject
{
    public Dictionary<string, object> Properties { get; set; } = new(); // Dictionnaire pour stocker les propriétés dynamiques
}

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            string jsonFilePath;
            while (true) // boucle infinie
            {
                Console.WriteLine("Entrez le chemin du fichier JSON à importer :");
                jsonFilePath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(jsonFilePath) || !jsonFilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Erreur : L'extension du fichier doit être .json. Veuillez réessayer.");
                    continue;
                }
                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine($"Erreur : Le fichier {jsonFilePath} n'existe pas. Veuillez réessayer.");
                    continue;
                }
                break;
            }
            var json = File.ReadAllText(jsonFilePath);
            var objects = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json)
                ?.Select(dict => new DynamicObject { Properties = dict })
                .ToList(); // Convertir chaque dictionnaire en DynamicObject

            if (objects == null || objects.Count == 0)
            {
                Console.WriteLine("Aucune donnée trouvée dans le fichier JSON.");
                return;
            }

            var availableFields = objects.First().Properties.Keys.ToList(); // Obtenir les champs disponibles à partir du premier objet

            Console.WriteLine("\nÉtat initial de la liste :");
            DisplayObjects(objects, availableFields);

            IEnumerable<DynamicObject> filteredObjects = objects; // Commencer avec la liste complète
            while (true)
            {
                Console.WriteLine("Voulez-vous appliquer un filtre de recherche ? (o/n)");
                var applyFilter = GetValidYesNoInput() == "o";
                if (!applyFilter) break; // Sortir de la boucle si l'utilisateur ne veut pas filtrer

                Console.WriteLine("Choisissez un champ pour la recherche :");
                for (int i = 0; i < availableFields.Count; i++)
                    Console.WriteLine($"{i + 1}-{availableFields[i]}"); // Afficher les champs disponibles
                var fieldChoice = Console.ReadLine();
                if (!int.TryParse(fieldChoice, out int fieldIndex) || fieldIndex < 1 || fieldIndex > availableFields.Count) // Valider le choix
                {
                    Console.WriteLine("Choix invalide, aucun filtre appliqué.");
                    continue;
                }
                var fieldName = availableFields[fieldIndex - 1]; // Obtenir le nom du champ choisi

                Console.Write($"Entrez la valeur à rechercher (pour les nombres : >10 ou <10) : ");
                var value = Console.ReadLine();

                filteredObjects = FilterObjects(filteredObjects, fieldName, value); // Appliquer le filtre

                Console.WriteLine("\nÉtat de la liste après filtrage :");
                DisplayObjects(filteredObjects, availableFields);

                Console.WriteLine("Voulez-vous filtrer à nouveau ou passer à l'étape suivante ? (o/n)");
                if (GetValidYesNoInput() != "o") break;
            }

            var sortedObjects = filteredObjects;
            while (true)
            {
                Console.WriteLine("Choisissez un champ pour le tri :");
                for (int i = 0; i < availableFields.Count; i++) // Afficher les champs disponibles
                    Console.WriteLine($"{i + 1}-{availableFields[i]}");
                var sortChoice = Console.ReadLine();
                if (!int.TryParse(sortChoice, out int sortIndex) || sortIndex < 1 || sortIndex > availableFields.Count)
                {
                    Console.WriteLine("Choix invalide, tri par le premier champ par défaut.");
                    sortIndex = 1;
                }
                var sortField = availableFields[sortIndex - 1];

                Console.WriteLine("Ordre : 1-Croissant, 2-Décroissant");
                var orderChoice = Console.ReadLine();

                sortedObjects = SortObjects(sortedObjects, sortField, orderChoice == "2"); // Appliquer le tri

                Console.WriteLine("\nÉtat de la liste après tri :");
                DisplayObjects(sortedObjects, availableFields);

                Console.WriteLine("Voulez-vous trier à nouveau ? (o/n)");
                if (GetValidYesNoInput() != "o") break;
            }

            var finalResults = sortedObjects.ToList(); // Convertir en liste pour l'export
            Console.WriteLine("Voulez-vous exporter les résultats ? (o/n)");
            if (GetValidYesNoInput() == "o")
            {
                Console.WriteLine("Choisissez le format : 1-CSV, 2-XML");
                var exportChoice = Console.ReadLine();

                Console.WriteLine("\nListe finale à exporter :");
                DisplayObjects(finalResults, availableFields);

                Console.WriteLine("Confirmer l'export ? (o/n)");
                if (GetValidYesNoInput() == "o")
                {
                    Console.Write("Entrez le nom du fichier (sans extension) : ");
                    var fileName = Console.ReadLine();
                    var exportDir = "../../../Data/serialization";
                    Directory.CreateDirectory(exportDir);

                    switch (exportChoice)
                    {
                        case "1":
                            ExportToCsv(finalResults, Path.Combine(exportDir, $"{fileName}.csv"), availableFields); // Combine : chemin et nom de fichier
                            Console.WriteLine($"Exporté vers {Path.Combine(exportDir, $"{fileName}.csv")}");
                            break;
                        case "2":
                            ExportToXml(finalResults, Path.Combine(exportDir, $"{fileName}.xml"), availableFields);
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

    private static string GetValidYesNoInput()
    {
        while (true)
        {
            var input = Console.ReadLine()?.ToLower(); // Lire et convertir en minuscule
            if (input == "o" || input == "n")
                return input;
            Console.WriteLine("Erreur : Veuillez entrer 'o' ou 'n' uniquement. Réessayez :");
        }
    }

    private static IEnumerable<DynamicObject> FilterObjects(IEnumerable<DynamicObject> objects, string field, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return objects; // Si la valeur est vide, ne pas filtrer
        if (value.StartsWith(">"))
        {
            if (double.TryParse(value[1..], out var min))
                return objects.Where(o => o.Properties.TryGetValue(field, out var v) && double.TryParse(v?.ToString(), out var num) && num > min);
        }
        else if (value.StartsWith("<"))
        {
            if (double.TryParse(value[1..], out var max))
                return objects.Where(o => o.Properties.TryGetValue(field, out var v) && double.TryParse(v?.ToString(), out var num) && num < max);
        }
        else
        {
            return objects.Where(o => o.Properties.TryGetValue(field, out var v) && v?.ToString().Contains(value, StringComparison.OrdinalIgnoreCase) == true);
        }
        return objects;
    }

    private static IEnumerable<DynamicObject> SortObjects(IEnumerable<DynamicObject> objects, string field, bool descending)
    {
        return descending
            ? objects.OrderByDescending(o => o.Properties.TryGetValue(field, out var v) ? v?.ToString() : "")
            : objects.OrderBy(o => o.Properties.TryGetValue(field, out var v) ? v?.ToString() : ""); // Gérer les valeurs nulles
    }

    private static void DisplayObjects(IEnumerable<DynamicObject> objects, List<string> fields)
    {
        var results = objects.ToList(); // Convertir en liste pour multiple itérations
        if (results.Count == 0)
        {
            Console.WriteLine("La liste est vide.");
            return;
        }
        foreach (var obj in results)
        {
            Console.WriteLine(string.Join(", ", fields.Select(f => $"{f}: {obj.Properties.GetValueOrDefault(f)}"))); // Afficher toutes les propriétés
        }
    }

    private static void ExportToCsv(List<DynamicObject> objects, string filePath, List<string> fields)
    {
        Console.WriteLine("Exporter tous les champs en CSV ? (o/n)");
        var exportAllFields = GetValidYesNoInput() == "o";
        List<string> fieldsToKeep; // Champs à exporter
        if (!exportAllFields)
        {
            Console.WriteLine("Choisissez les champs à exporter (ex : 1,2) :");
            for (int i = 0; i < fields.Count; i++)
                Console.WriteLine($"{i + 1}-{fields[i]}"); // Afficher les champs disponibles
            fieldsToKeep = Console.ReadLine()
                .Split(',')
                .Select(f => int.TryParse(f.Trim(), out int idx) && idx >= 1 && idx <= fields.Count ? fields[idx - 1] : null)
                .Where(f => f != null)
                .ToList();
        }
        else
        {
            fieldsToKeep = fields;
        }

        string EscapeCsvField(string field) => $"\"{field.Replace("\"", "\"\"")}\""; // fonction récuperée pour échapper les champs CSV

        var header = string.Join(",", fieldsToKeep); // si export csv mal rangé alors utiliser ; et pas ,
        var csv = header + "\n" + string.Join("\n",
            objects.Select(o =>
                string.Join(",", fieldsToKeep.Select(f => // si export csv mal rangé alors utiliser ; et pas ,
                    EscapeCsvField(o.Properties.GetValueOrDefault(f)?.ToString() ?? ""))) // Échapper les champs
            ));
        File.WriteAllText(filePath, csv);
    }

    private static void ExportToXml(List<DynamicObject> objects, string filePath, List<string> fields)
    {
        Console.WriteLine("Exporter tous les champs en XML ? (o/n)");
        var exportAllFields = GetValidYesNoInput() == "o";
        List<string> fieldsToKeep;
        if (!exportAllFields)
        {
            Console.WriteLine("Choisissez les champs à exporter (ex : 1,2) :");
            for (int i = 0; i < fields.Count; i++)
                Console.WriteLine($"{i + 1}-{fields[i]}"); 
            fieldsToKeep = Console.ReadLine()
                .Split(',')
                .Select(f => int.TryParse(f.Trim(), out int idx) && idx >= 1 && idx <= fields.Count ? fields[idx - 1] : null)
                .Where(f => f != null)
                .ToList();
        }
        else
        {
            fieldsToKeep = fields; // Tous les champs
        }

        // creation d'un fichier xml avec les champs selectionnés
        var doc = new XDocument(
            new XElement("Objets",
                objects.Select(o =>
                {
                    var objElement = new XElement("Objet");
                    foreach (var f in fieldsToKeep)
                        objElement.Add(new XElement(f, o.Properties.GetValueOrDefault(f)));
                    return objElement;
                })
            )
        );
        doc.Save(filePath);
    }
}