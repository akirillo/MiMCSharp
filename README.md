# MiMCSharp

### 🚨 UNAUDITED 🚨

This is an implementation of a hash function using the [MiMC](https://byt3bit.github.io/primesym/mimc/) block cipher, mirroring the implementation from the [gnark](https://docs.gnark.consensys.net/) library.

It currently computes the MiMCHash over a field of order $$21888242871839275222246405745257275088548364400416034343698204186575808495617$$

This is the BabyJubJub prime, and is the order of the BN254 curve's scalar field.

---

**To preface: this is my first time using C#, so don't expect much adherence to idiomatic language use or project structure.**

---

## Run the console app

1. Clone this puppy
2. `cd` into the root folder `MiMCSharp`
3. Run the console app:
```shell
dotnet run --project CalcHash/CalcHash.csproj -c Release
```

GLHF lmk if it breaks

## Run the tests

You really don't trust me, huh? Fine, run the tests:

Same steps 1 & 2 as above, then run:
```shell
dotnet test MimcBN254Test/MimcBN254Test.csproj
```

The test vectors are taken from gnark's MiMCHash test over the same field order: https://github.com/ConsenSys/gnark-crypto/blob/master/ecc/bn254/fr/mimc/test_vectors/vectors.json

## Run the benchmarks

Same steps 1 & 2 as above, then run:
```shell
dotnet run --project MimcBN254Bench/MimcBN254Bench.csproj -c Release
```

Benchmarks should take on the order of 5 minutes. See a sample output below, on a 16GB 2.4 GHz 4-core i5 2019 Macbook Pro running macOS Ventura 13.2:
```
|   Method | numRounds | numInputs |     Mean |    Error |   StdDev |
|--------- |---------- |---------- |---------:|---------:|---------:|
| CalcMimc |         1 |         1 | 72.39 ns | 0.747 ns | 0.583 ns |
| CalcMimc |         1 |        10 | 72.39 ns | 0.648 ns | 0.541 ns |
| CalcMimc |         1 |       100 | 73.68 ns | 1.182 ns | 1.106 ns |
| CalcMimc |         1 |      1000 | 74.95 ns | 0.629 ns | 0.525 ns |
| CalcMimc |        10 |         1 | 74.54 ns | 1.115 ns | 0.931 ns |
| CalcMimc |        10 |        10 | 75.36 ns | 1.552 ns | 1.376 ns |
| CalcMimc |        10 |       100 | 78.74 ns | 1.308 ns | 1.022 ns |
| CalcMimc |        10 |      1000 | 72.43 ns | 0.694 ns | 0.580 ns |
| CalcMimc |       100 |         1 | 72.76 ns | 0.464 ns | 0.387 ns |
| CalcMimc |       100 |        10 | 75.90 ns | 1.413 ns | 1.388 ns |
| CalcMimc |       100 |       100 | 78.14 ns | 1.520 ns | 1.493 ns |
| CalcMimc |       100 |      1000 | 72.77 ns | 0.729 ns | 0.609 ns |
| CalcMimc |      1000 |         1 | 77.00 ns | 1.361 ns | 1.207 ns |
| CalcMimc |      1000 |        10 | 80.23 ns | 1.276 ns | 1.131 ns |
| CalcMimc |      1000 |       100 | 77.79 ns | 1.540 ns | 2.442 ns |
| CalcMimc |      1000 |      1000 | 78.31 ns | 1.286 ns | 1.803 ns |
```
