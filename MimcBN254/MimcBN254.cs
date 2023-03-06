namespace MimcLibraries;
using System.Numerics;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;

public class MimcBN254
{
    private BigInteger babyJubJub = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");
    private int numRounds;
    private BigInteger[] roundConstants;
    private BigInteger hashState;
    private BigInteger[]? input;

    public MimcBN254(string seed = "seed", int numRounds = 110) {
        this.numRounds = numRounds;
        this.roundConstants = new BigInteger[this.numRounds];

        // Calculate round constants using Keccak256
        // Hoping this is the same padding scheme as what is used in gnark...
        var digest = new KeccakDigest(256);
        var bseed = Encoding.UTF8.GetBytes(seed);
        digest.BlockUpdate(bseed, 0, bseed.Length);
        var rnd = new byte[digest.GetDigestSize()];
        digest.DoFinal(rnd, 0);
        digest.Reset();
        digest.BlockUpdate(rnd, 0, rnd.Length);

        for(var i = 0; i < this.numRounds; i++) {
            digest.DoFinal(rnd, 0);
            // Parses byte[] rnd in big-endian order
            // TODO: FFA library to take modulo here? Oh god...
            // LOOK INTO BOUNCYCASTLE ECFIELDELEMENT
            var roundConstant = new BigInteger(rnd, true, true);
            Console.WriteLine($"round constant {i}: {roundConstant}");
            this.roundConstants[i] = roundConstant;
            digest.Reset();
            digest.BlockUpdate(rnd, 0, rnd.Length);
        }
    }

    public BigInteger[] GetRoundConstants() {
        return this.roundConstants;
    }
}