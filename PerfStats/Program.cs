using System.Diagnostics;

Console.WriteLine("Calcul de performance.");

var sw = Stopwatch.StartNew();

// Séquentiel
double sumSeq = 1;
for (int i = 0; i < 50_000_000; i++)
{
    sumSeq += Math.Sin(i) + Math.Cos(i);
    sumSeq += Math.Sqrt(i);
    sumSeq += Math.Exp(i % 10) + Math.Log(i);
    sumSeq += Math.Pow(i % 100, 3);
    sumSeq *= 1.0000001;
}
sw.Stop();
Console.WriteLine($"Séquentiel - Temps de calcul : {sw.ElapsedMilliseconds} ms, Résultat : {sumSeq}");

sw.Restart();

// Parallèle
double sumPar = 1;
object lockObj = new object();
Parallel.For(0, 50_000_000, i =>
{
    double local = Math.Sin(i) + Math.Cos(i);
    local += Math.Sqrt(i);
    local += Math.Exp(i % 10) + Math.Log(i);
    local += Math.Pow(i % 100, 3);
    local *= 1.0000001;
    lock (lockObj)
    {
        sumPar += local;
    }
});
sw.Stop();
Console.WriteLine($"Parallèle - Temps de calcul : {sw.ElapsedMilliseconds} ms, Résultat : {sumPar}");