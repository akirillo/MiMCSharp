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
dotnet run --project CalcHash/CalcHash.csproj
```

GLHF lmk if it breaks

## Run the tests

You really don't trust me, huh? Fine, run the tests:

Same steps 1 & 2 as above, then run:
```shell
dotnet test MimcBN254Test/MimcBN254Test.csproj
```

The test vectors are taken from gnark's MiMCHash test over the same field order: https://github.com/ConsenSys/gnark-crypto/blob/master/ecc/bn254/fr/mimc/test_vectors/vectors.json
