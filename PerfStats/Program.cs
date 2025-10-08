using System.Diagnostics;

Console.WriteLine("Calcul de performance.");
// boucle 10 fois 
// on éxécute 50 millions de calculs
var swloop = Stopwatch.StartNew();
double sumloop = 1;
for (var e = 0; e < 10; e++)
{
    for (var i = 0; i < 50_000_000; i++)
    {
        //cosinus 
        sumloop += Math.Sin(i) + Math.Cos(i);
        //Racine carrée
        sumloop += Math.Sqrt(i);
        // Exp + Log
        sumloop += Math.Exp(i % 10) + Math.Log(i);
        //Puissances
        sumloop += Math.Pow(i % 100, 3);
        //Multiplication rule
        sumloop *= 1.0000001;
    }
}

swloop.Stop();
Console.WriteLine($"Temps de calcul séquentiel (10 fois) : {swloop.ElapsedMilliseconds} ms");

var sw = Stopwatch.StartNew();

// on éxécute 50 millions de calculs
double sum = 1;
for (var i = 0; i < 50_000_000; i++)
{
    //cosinus 
    sum += Math.Sin(i) + Math.Cos(i);
    //Racine carrée
    sum += Math.Sqrt(i);
    // Exp + Log
    sum += Math.Exp(i % 10) + Math.Log(i);
    //Puissances
    sum += Math.Pow(i % 100, 3);
    //Multiplication rule
    sum *= 1.0000001;
}

sw.Stop();
Console.WriteLine($"Temps de calcul séquentiel : {sw.ElapsedMilliseconds} ms");


Console.WriteLine("Calcul de performance.");
sw.Restart();
// on éxécute 50 millions de calculs
sum = 1;
Parallel.For(0, 50_000_000, (i, state) =>
{
    //cosinus 
    sum += Math.Sin(i) + Math.Cos(i);
    //Racine carrée
    sum += Math.Sqrt(i);
    // Exp + Log
    sum += Math.Exp(i % 10) + Math.Log(i);
    //Puissances
    sum += Math.Pow(i % 100, 3);
    //Multiplication rule
    sum *= 1.0000001;
});

sw.Stop();
Console.WriteLine($"Temps de calcul parallèle : {sw.ElapsedMilliseconds} ms");

Console.WriteLine("Calcul de performance.");
sw.Restart();
// on éxécute 50 millions de calculs
sum = 1;
await Task.Run(() =>
{
    Parallel.For(0, 50_000_000, (i, state) =>
    {
        //cosinus 
        sum += Math.Sin(i) + Math.Cos(i);
        //Racine carrée
        sum += Math.Sqrt(i);
        // Exp + Log
        sum += Math.Exp(i % 10) + Math.Log(i);
        //Puissances
        sum += Math.Pow(i % 100, 3);
        //Multiplication rule
        sum *= 1.0000001;
    });
});

sw.Stop();
Console.WriteLine($"Temps de calcul parallèle (async) : {sw.ElapsedMilliseconds} ms");

/*// démo
var urls = Enumerable.Range(1, 10)
    .Select(i => $"https://example.com/image{i}.jpg")
    .ToList();

async Task DownloadImagesSequentialAsync(List<string> imageUrls)
{
    using var httpClient = new HttpClient();
    for (int i = 0; i < imageUrls.Count; i++)
    {
        var url = imageUrls[i];
        var data = await httpClient.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync($"image_{i + 1}.jpg", data);
        Console.WriteLine($"Downloaded: {url}");
    }
}

await DownloadImagesSequentialAsync(urls);// await pour laisser le reste du code s'exec*/