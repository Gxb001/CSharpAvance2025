using System.Diagnostics;

Console.WriteLine("Calcul de performance.");
var sw = Stopwatch.StartNew();

// on éxécute 50 millions de calculs
double sum = 1;
for (int i = 0; i < 50_000_000; i++)
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