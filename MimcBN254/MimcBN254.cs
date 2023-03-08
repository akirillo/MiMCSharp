namespace MimcLibraries;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using System.Text;

public class MimcBN254
{
    // NOTE: THIS IS NOT THE BN254 CURVE!!
    // We really just need the curve object to be able to generate `Org.BouncyCastle.Math.EC.FpFieldElement`s from it,
    // so here we define a dummy curve which is *defined over a field with the same order as the group formed by BN254*
    // (namely, the BabyJubJub prime)
    public static FpCurve Fp = new FpCurve(
        new BigInteger("21888242871839275222246405745257275088548364400416034343698204186575808495617"),
        BigInteger.Zero,
        BigInteger.Zero,
        BigInteger.Zero,
        BigInteger.Zero
    );

    // The initial hash state (0 in the field)
    private static ECFieldElement initialHashState = MimcBN254.Fp.FromBigInteger(BigInteger.Zero);

    // The number of rounds to run the hash with
    // For our field, this *should* be 110 (log5(BabyJubJub)) according to the paper,
    // but we allow it to be configured
    private int numRounds;

    // The round constants to be used in each round of the hash
    // Generated pseudo-randomly using Keccak256 from a seed string
    private ECFieldElement[] roundConstants;

    // The current hash state
    private ECFieldElement hashState;

    // The data (other field elements) to hash
    private List<ECFieldElement> input;

    // Creates a new instance of MiMCHash using the provided seed string for
    // pseudo-random round constant generation, and with the provided number of rounds
    public MimcBN254(string seed = "seed", int numRounds = 110) {
        this.input = new List<ECFieldElement>();
        this.hashState = MimcBN254.initialHashState;

        this.numRounds = numRounds;
        this.roundConstants = new ECFieldElement[this.numRounds];

        // Generate round constants by repeatedly hashing the seed string
        // and parsing the result as a field element
        var digest = new KeccakDigest(256);
        // We use UTF-8 encoding because this is the encoding used in gnark
        var bseed = Encoding.UTF8.GetBytes(seed);
        digest.BlockUpdate(bseed, 0, bseed.Length);
        var rnd = new byte[digest.GetDigestSize()];
        // Calculate a pre-hash before recording round constants
        // Why? Because gnark does it
        digest.DoFinal(rnd, 0);
        digest.Reset();
        digest.BlockUpdate(rnd, 0, rnd.Length);

        for(var i = 0; i < this.numRounds; i++) {
            // Calculate the next Keccak hash
            digest.DoFinal(rnd, 0);
            // Parse the hash bytes as big-endian unsigned integer
            // (gnark expects big-endianness) & translate into field element
            var roundConstant = MimcBN254.Fp.FromBigInteger(
                new BigInteger(1, rnd).Mod(MimcBN254.Fp.Q)
            );
            this.roundConstants[i] = roundConstant;
            digest.Reset();
            digest.BlockUpdate(rnd, 0, rnd.Length);
        }
    }

    // Adds data (field elements) to the hash input
    public void Write(List<ECFieldElement> data) {
        var newInput = new List<ECFieldElement>(this.input.Count + data.Count);
        newInput.AddRange(this.input);
        newInput.AddRange(data);
        this.input = newInput;
    }

    // Reset the hash, i.e. clear the input and set the hash state back to 0
    // This should be called after every time a hash is calculated!
    public void Reset() {
        this.input.Clear();
        this.hashState = MimcBN254.initialHashState;
    }

    /*
    Calculates MiMCHash-n/n using Miyaguchi-Preneel one-way compression scheme.
    Not quite the MiMCHash construction specified in the [paper](https://eprint.iacr.org/2016/492.pdf),
    but matches the implementation in [gnark](https://github.com/ConsenSys/gnark/blob/master/std/hash/mimc/mimc.go),
    which itself matches [this implementation](https://gist.github.com/HarryR/80b5ff2ce13da12edafda6d21c780730) from
    the [ethsnarks](https://github.com/HarryR/ethsnarks) team.

    Note that this differs from the [circomlibjs implementation](https://github.com/iden3/circomlibjs/blob/main/src/mimc7.js),
    which uses the Merkle-Damgard compression scheme. Discussion around this can be found [here](https://github.com/HarryR/ethsnarks/issues/119).

    This is all despite the fact that the authors of the MiMC paper softly
    [advise against using compression schemes](https://mimc.iaik.tugraz.at/pages/faq.php), but doing so lets us keep everything in the same field.
    */
    public ECFieldElement Sum() {
        foreach(ECFieldElement e in this.input) {
            var r = this.encryptPow5(e);
            r = r.Add(e);
            this.hashState = this.hashState.Add(r);
        }
        this.input.Clear();
        return this.hashState;
    }

    // Computes one run of the MiMC-n/n block cipher
    private ECFieldElement encryptPow5(ECFieldElement e) {
        var x = e;
        foreach(ECFieldElement roundConstant in this.roundConstants) {
            x = x.Add(this.hashState).Add(roundConstant);
            x = x.Square().Square().Multiply(x);
        }
        return x.Add(this.hashState);
    }
}