using MimcLibraries;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string? seed = null;
        Console.WriteLine("What seed would you like to use for generating round constants? (Default: \"seed\")");
        var input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) {
            seed = input;
        }

        int? numRounds = null;
        int readNumRounds;
        Console.WriteLine("How many rounds would you like to run the block cipher for? (Default: 110)");
        if (int.TryParse(Console.ReadLine(), out readNumRounds)) {
            numRounds = readNumRounds;
        }

        var mimc = new MimcBN254(seed, numRounds);

        Console.WriteLine("What value(s) would you like to hash? Please provide a comma-separated list containing either:");
        Console.WriteLine("1) Numbers in the prime field of order 21888242871839275222246405745257275088548364400416034343698204186575808495617");
        Console.WriteLine("2) Strings, which will be Keccak256-hashed and interpreted as big-endian numbers in the field");
        Console.WriteLine("(Note: empty/whitespace strings will be ignored)");

        string[] strValues = Console.ReadLine().Split(",");
        List<ECFieldElement> values = new List<ECFieldElement>();
        var digest = new KeccakDigest(256);
        var digestSum = new byte[digest.GetDigestSize()];
        foreach(string strValue in strValues) {
            var trimmedStrVal = strValue.Trim();
            if(trimmedStrVal.Length > 0) {
                try {
                    values.Append(MimcBN254.Fp.FromBigInteger(
                        new BigInteger(trimmedStrVal).Mod(MimcBN254.Fp.Q)
                    ));
                } catch (System.FormatException) {
                    digest.BlockUpdate(
                        Encoding.UTF8.GetBytes(trimmedStrVal),
                        0,
                        trimmedStrVal.Length
                    );
                    digest.DoFinal(digestSum, 0);
                    values.Append(MimcBN254.Fp.FromBigInteger(
                        new BigInteger(1, digestSum).Mod(MimcBN254.Fp.Q)
                    ));
                }
            }
        }

        mimc.Write(values);
        var res = mimc.Sum();
        Console.WriteLine("MiMCHash:");
        Console.WriteLine(res.ToBigInteger().ToString());
    }
}