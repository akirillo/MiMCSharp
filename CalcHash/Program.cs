using MimcLibraries;

class Program
{
    static void Main(string[] args)
    {
        // LETS BITE THE BULLET EARLY AND BE SURE WE HAVE ROUND CONSTANT
        // EQUIVALNCE W/ GNARK FROM THE SAME SEED STRING
        var mimc = new MimcBN254(numRounds: 1);
        var roundConstants = mimc.GetRoundConstants();
        Console.WriteLine($"roundConstants: {roundConstants}");
    }
}