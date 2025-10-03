
class CSharpAvance2025
{
    static void Main(string[] args)
    {
        Console.WriteLine("Entrez un calcul simple: "); // permet de saisir une expression simple
        string input = Console.ReadLine(); // lire la saisie de l'utilisateur
        
        char op = '+';
        int a = 0, b = 0;
        
        foreach (char c in input)
        {
            if (c == '+' || c == '-' || c == '*' || c == '/')
            {
                op = c;
                break;
            }
        }

        string[] parts = input.Split(op); // permet de diviser l'expression en deux parties
        if (parts.Length == 2)
        {
            a = int.Parse(parts[0].Trim()); // trim pour supprimer les espaces avant et apres le nombre
            b = int.Parse(parts[1].Trim());
        }

        double result = 0;
        switch (op)
        {
            case '+':
                result = a + b;
                break;
            case '-':
                result = a - b;
                break;
            case '*':
                result = a * b;
                break;
            case '/':
                if (b != 0)
                    result = (double)a / b; // (double) permet de convertir un entier en double
                else
                    Console.WriteLine("Erreur: Division par zéro");
                break;
            default:
                Console.WriteLine("Opérateur non supporté");
                return;
        }
        Console.WriteLine("Résultat: " + result);
    }
}