namespace MimcBN254Test;
using MimcLibraries;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System.Linq;

public class RawTestVector {
    public string[] input;
    public string output;
}

public class ParsedTestVector {
    public ECFieldElement[] input;
    public ECFieldElement output;

    public ParsedTestVector(ECFieldElement[] input, ECFieldElement output) {
        this.input = input;
        this.output = output;
    }
}

[TestClass]
public class VectorTests
{

    // Assumes there is a "0x" prefix
    internal static ECFieldElement ParseHex(string hex) {
        var bigInt = new BigInteger(hex.Substring(2), 16).Mod(MimcBN254.Fp.Q);
        return MimcBN254.Fp.FromBigInteger(
            new BigInteger(hex.Substring(2), 16).Mod(MimcBN254.Fp.Q)
        );
    }

    internal static List<ParsedTestVector> ParseTestVectors(List<RawTestVector> rawTestVectors) {
        var testVectors = new List<ParsedTestVector>();
        foreach(RawTestVector rawTestVector in rawTestVectors) {
            var parsedInput = new ECFieldElement[rawTestVector.input.Length];
            for(var i = 0; i < rawTestVector.input.Length; i++) {
                parsedInput[i] = VectorTests.ParseHex(rawTestVector.input[i]);
            }
            var parsedOutput = VectorTests.ParseHex(rawTestVector.output);

            testVectors.Add(new ParsedTestVector(parsedInput, parsedOutput));
        }
        return testVectors;
    }

    [TestMethod]
    public void TestVectors()
    {
        // Read in test vectors from ./vectors.json, taken from the gnark MiMCBN254 test vectors
        // (https://github.com/ConsenSys/gnark-crypto/blob/master/ecc/bn254/fr/mimc/test_vectors/vectors.json)
        List<ParsedTestVector> testVectors;
        using (StreamReader r = new StreamReader("vectors.json"))
        {
            string json = r.ReadToEnd();
            List<RawTestVector> rawTestVectors = JsonConvert.DeserializeObject<List<RawTestVector>>(json);
            testVectors = VectorTests.ParseTestVectors(rawTestVectors);
        }

        var mimc = new MimcBN254();
        foreach(ParsedTestVector testVector in testVectors) {
            mimc.Write(testVector.input.ToList());
            Assert.AreEqual(mimc.Sum(), testVector.output);
            mimc.Reset();
        }
    }
}