using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using MimcLibraries;
using System.Linq;

namespace MimcBN254Benchmarks
{
    public class MimcBN254Bench
    {
        [Params(1, 10, 100, 1000)]
        public int numRounds;
        private const string seed = "seed";
        [Params(1, 10, 100, 1000)]
        public int numInputs;
        private readonly ECFieldElement[] input;

        private readonly MimcBN254 mimc;

        public MimcBN254Bench()
        {
            this.mimc = new MimcBN254(seed, numRounds);
            this.input = new ECFieldElement[numInputs];
            for(var i = 0; i < numInputs; i++) {
                this.input[i] = MimcBN254.Fp.FromBigInteger(
                    BigInteger.Arbitrary(MimcBN254.Fp.Q.BitLength)
                );
            }
        }

        [Benchmark]
        public void CalcMimc() {
            this.mimc.Write(this.input.ToList());
            this.mimc.Sum();
            this.mimc.Reset();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MimcBN254Bench>();
        }
    }
}