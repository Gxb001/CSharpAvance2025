using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

internal class ImageProcessor
{
    private static readonly int[] Resolutions = { 1080, 720, 480 }; // définition des résolutions
    private static string InputFolder; // déclaration de variables
    private static string OutputFolder;

    private static async Task Main() // méthode principale asynchrone
    {
        // demande au user les dossiers d'entrée et de sortie
        Console.WriteLine("Entrez le chemin du dossier contenant les images (ex. C:\\Images\\Input) :");
        Console.WriteLine(); // saut de ligne
        InputFolder = Console.ReadLine()?.Trim(); // Trim pour enlever les espaces inutiles

        Console.WriteLine(
            "Entrez le chemin du dossier de sortie pour les images redimensionnées (ex. C:\\Images\\Output) :");
        Console.WriteLine(); // saut de ligne
        OutputFolder = Console.ReadLine()?.Trim();

        // verif des chemins
        if (string.IsNullOrWhiteSpace(InputFolder) || !Directory.Exists(InputFolder))
        {
            Console.WriteLine($"Erreur : Le dossier d'entrée '{InputFolder}' n'existe pas ou est invalide.");
            return;
        }

        // créer le dossier de sortie s'il n'existe pas
        if (string.IsNullOrWhiteSpace(OutputFolder))
        {
            Console.WriteLine("Erreur : Le dossier de sortie n'est pas spécifié.");
            return;
        }

        // sous dossiers
        var sequentialOutput = Path.Combine(OutputFolder, "Sequentielle");
        var parallelOutput = Path.Combine(OutputFolder, "Parallele");
        Directory.CreateDirectory(sequentialOutput); // création
        Directory.CreateDirectory(parallelOutput);

        // Vérifier les fichiers valides (JPG et PNG)
        var jpgFiles = Directory.GetFiles(InputFolder, "*.jpg"); // liste des fichiers jpg
        var pngFiles = Directory.GetFiles(InputFolder, "*.png"); // liste des fichiers png
        var files = jpgFiles.Concat(pngFiles).ToArray(); // concaténation des deux listes
        var validFiles = new List<string>();
        foreach (var file in files)
            if (await IsValidImageAsync(file)) // vérification de la validité
                validFiles.Add(file);
            else
                Console.WriteLine($"Fichier ignoré : {file} (image non valide ou inaccessible)");

        if (!validFiles.Any()) // Any : vérifie si la liste n'est pas vide
        {
            Console.WriteLine($"Erreur : Aucun fichier JPG ou PNG valide trouvé dans '{InputFolder}'.");
            return;
        }

        // demande du mode de traitement
        Console.WriteLine("Choisissez le mode de traitement (1 = Séquentiel, 2 = Parallèle, 3 = Les deux) :");
        var choice = Console.ReadLine()?.Trim();

        var sequentialTime = TimeSpan.Zero;
        var parallelTime = TimeSpan.Zero;

        if (choice == "1" || choice == "3")
        {
            OutputFolder = sequentialOutput; // Rediriger vers le sous-dossier séquentiel
            sequentialTime = await ProcessSequential(validFiles.ToArray());
        }

        if (choice == "2" || choice == "3")
        {
            OutputFolder = parallelOutput; // Rediriger vers le sous-dossier parallèle
            parallelTime = await ProcessParallel(validFiles.ToArray());
        }

        if (choice != "1" && choice != "2" && choice != "3")
        {
            Console.WriteLine("Choix invalide. Fin du programme.");
            return;
        }

        // génération du readme
        GenerateReadme(sequentialTime, parallelTime);
    }

    // fonction de verif du type de fichier
    private static async Task<bool> IsValidImageAsync(string filePath)
    {
        try
        {
            using var image = await Image.LoadAsync(filePath);
            var format = image.Metadata.DecodedImageFormat?.Name;
            return format != null && (format.Equals("JPEG", StringComparison.OrdinalIgnoreCase) ||
                                      format.Equals("PNG", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    // Version sequentielle
    private static async Task<TimeSpan> ProcessSequential(string[] files)
    {
        var stopwatch = Stopwatch.StartNew();

        foreach (var file in files) await ProcessImage(file);

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    // Version parallele
    private static async Task<TimeSpan> ProcessParallel(string[] files)
    {
        var stopwatch = Stopwatch.StartNew();

        await Parallel.ForEachAsync(files, async (file, ct) => { await ProcessImage(file); });

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    // ImageSharp pour le traitement des images
    private static async Task ProcessImage(string filePath)
    {
        try
        {
            using var image = await Image.LoadAsync(filePath); // using permet de libérer les ressources
            var fileName = Path.GetFileNameWithoutExtension(filePath); // nom du fichier sans extension
            var extension = Path.GetExtension(filePath).ToLower(); // que l'extension
            var isJpeg = extension == ".jpg" || extension == ".jpeg"; // verif du type

            foreach (var resolution in Resolutions)
                await Task.Run(async () => // await Task.Run pour ne pas bloquer le thread principal
                {
                    var newWidth = resolution;
                    var newHeight =
                        (int)(image.Height *
                              ((float)resolution /
                               image.Width)); // calcul de la nouvelle hauteur pour garder les proportions

                    using var
                        resizedImage =
                            image.Clone(ctx =>
                                ctx.Resize(newWidth, newHeight)); // clone permet de ne pas modifier l'image originale
                    var outputPath = Path.Combine(OutputFolder, $"{fileName}_{resolution}p{extension}");

                    if (isJpeg)
                        await resizedImage.SaveAsJpegAsync(outputPath);
                    else
                        await resizedImage.SaveAsPngAsync(outputPath);
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du traitement de {filePath}: {ex.Message}");
        }
    }

    // Génération du README
    private static void GenerateReadme(TimeSpan sequentialTime, TimeSpan parallelTime)
    {
        // $@ pour les chaînes multilignes
        // F2 pour 2 decimales
        // ? pour nullable
        var readmeContent = $@"# Optimisation des images

## Résultats des performances

- **Version séquentielle**: {(sequentialTime == TimeSpan.Zero ? "Non exécutée" : $"{sequentialTime.TotalSeconds:F2} secondes")}
- **Version parallèle**: {(parallelTime == TimeSpan.Zero ? "Non exécutée" : $"{parallelTime.TotalSeconds:F2} secondes")}
{(sequentialTime != TimeSpan.Zero && parallelTime != TimeSpan.Zero ? $"- **Gain de performance**: {sequentialTime.TotalSeconds / parallelTime.TotalSeconds:F2}x plus rapide" : "")}


## Prérequis
- Images aux formats JPG ou PNG dans le dossier d'entrée spécifié ('{InputFolder}')
- .NET SDK installé
- Package NuGet SixLabors.ImageSharp installé (`dotnet add package SixLabors.ImageSharp`)

## Dossiers de sortie
- Version séquentielle : {Path.Combine(OutputFolder, "Sequential")}
- Version parallèle : {Path.Combine(OutputFolder, "Parallel")}
";

        // écrire le fichier
        File.WriteAllText(Path.Combine(OutputFolder, "README.md"), readmeContent);
        Console.WriteLine($"README.md généré dans {OutputFolder}");
    }
}