using MimcLibraries;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;

class Program
{
    static void Main(string[] args)
    {
        // LETS BITE THE BULLET EARLY AND BE SURE WE HAVE ROUND CONSTANT
        // EQUIVALNCE W/ GNARK FROM THE SAME SEED STRING
        var mimc = new MimcBN254(numRounds: 5);
        mimc.Write(new List<ECFieldElement>(){
            MimcBN254.BN254Curve.FromBigInteger(new BigInteger("1"))
        });
        var res = mimc.Sum();
        Console.WriteLine(res.ToBigInteger().ToString());
    }
}