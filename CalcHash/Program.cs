using MimcLibraries;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string seed = "seed";
        Console.WriteLine("\nWhat seed would you like to use for generating round constants? (Default: \"seed\")\n");
        var input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) {
            seed = input;
        }

        int numRounds;
        Console.WriteLine("\nHow many rounds would you like to run the block cipher for? (Default: 110)\n");
        if (!int.TryParse(Console.ReadLine(), out numRounds)) {
            numRounds = 110;
        }

        var mimc = new MimcBN254(seed, numRounds);

        Console.WriteLine("\nWhat value(s) would you like to hash? Please provide a comma-separated list containing either:");
        Console.WriteLine("1) Numbers in the prime field of order 21888242871839275222246405745257275088548364400416034343698204186575808495617");
        Console.WriteLine("2) Strings, which will be Keccak256-hashed and interpreted as big-endian numbers in the field");
        Console.WriteLine("(Note: empty/whitespace strings will be ignored)\n");

        List<string> strValues = new List<string>();
        while(strValues.Count == 0) {
            input = Console.ReadLine();
            if(!string.IsNullOrEmpty(input)) {
                strValues = input.Split(",").ToList();
            } else {
                Console.WriteLine("\nPlease enter a valid input.\n");
            }
        }
        List<ECFieldElement> values = new List<ECFieldElement>();
        var digest = new KeccakDigest(256);
        var digestSum = new byte[digest.GetDigestSize()];
        foreach(string strValue in strValues) {
            var trimmedStrVal = strValue.Trim();
            if(trimmedStrVal.Length > 0) {
                try {
                    values.Add(MimcBN254.Fp.FromBigInteger(
                        new BigInteger(trimmedStrVal).Mod(MimcBN254.Fp.Q)
                    ));
                } catch (System.FormatException) {
                    digest.BlockUpdate(
                        Encoding.UTF8.GetBytes(trimmedStrVal),
                        0,
                        trimmedStrVal.Length
                    );
                    digest.DoFinal(digestSum, 0);
                    values.Add(MimcBN254.Fp.FromBigInteger(
                        new BigInteger(1, digestSum).Mod(MimcBN254.Fp.Q)
                    ));
                }
            }
        }

        mimc.Write(values);
        var res = mimc.Sum();
        Console.WriteLine("\nMiMCHash:\n");
        Console.WriteLine(res.ToBigInteger().ToString());
    }
}