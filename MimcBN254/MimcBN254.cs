namespace MimcLibraries;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using System.Text;

public class MimcBN254
{
    // Don't really need to use the curve, just need it to generate FpFieldElements
    // This feels like a huge can of worms...
    public static FpCurve BN254Curve = new FpCurve(
        new BigInteger("21888242871839275222246405745257275088548364400416034343698204186575808495617"),
        BigInteger.Zero,
        BigInteger.Zero,
        BigInteger.Zero,
        BigInteger.Zero
    );

    private static ECFieldElement initialHashState = MimcBN254.BN254Curve.FromBigInteger(BigInteger.Zero);

    private int numRounds;

    private ECFieldElement[] roundConstants;

    private ECFieldElement hashState;

    private List<ECFieldElement> input;

    public MimcBN254(string seed = "seed", int numRounds = 110) {
        this.input = new List<ECFieldElement>();
        this.hashState = MimcBN254.initialHashState;

        this.numRounds = numRounds;
        this.roundConstants = new ECFieldElement[this.numRounds];

        var digest = new KeccakDigest(256);
        var bseed = Encoding.UTF8.GetBytes(seed);
        digest.BlockUpdate(bseed, 0, bseed.Length);
        var rnd = new byte[digest.GetDigestSize()];
        digest.DoFinal(rnd, 0);
        digest.Reset();
        digest.BlockUpdate(rnd, 0, rnd.Length);

        for(var i = 0; i < this.numRounds; i++) {
            digest.DoFinal(rnd, 0);
            // Parses byte[] rnd as big-endian unsigned integer
            var roundConstant = MimcBN254.BN254Curve.FromBigInteger(
                new BigInteger(1, rnd).Mod(MimcBN254.BN254Curve.Q)
            );
            this.roundConstants[i] = roundConstant;
            digest.Reset();
            digest.BlockUpdate(rnd, 0, rnd.Length);
        }
    }

    public void Write(List<ECFieldElement> data) {
        var newInput = new List<ECFieldElement>(this.input.Count + data.Count);
        newInput.AddRange(this.input);
        newInput.AddRange(data);
        this.input = newInput;
    }

    public void Reset() {
        this.input.Clear();
        this.hashState = MimcBN254.initialHashState;
    }

    // Calculates MiMCHash-n/n using Miyaguchi-Preneel one-way compression scheme
    public ECFieldElement Sum() {
        foreach(ECFieldElement e in this.input) {
            var r = this.encryptPow5(e);
            r = r.Add(e);
            this.hashState = this.hashState.Add(r);
        }
        this.input.Clear();
        return this.hashState;
    }

    private ECFieldElement encryptPow5(ECFieldElement e) {
        var x = e;
        foreach(ECFieldElement roundConstant in this.roundConstants) {
            x = x.Add(this.hashState).Add(roundConstant);
            x = x.Square().Square().Multiply(x);
        }
        return x.Add(this.hashState);
    }
}