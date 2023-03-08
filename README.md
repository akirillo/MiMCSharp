# MiMCSharp

### 🚨 UNAUDITED 🚨

This is an implementation of a hash function using the [MiMC](https://byt3bit.github.io/primesym/mimc/) block cipher, mirroring the implementation from the [gnark](https://docs.gnark.consensys.net/) library.

It currently computes the MiMCHash over a field of order 21888242871839275222246405745257275088548364400416034343698204186575808495617.
This is the BabyJubJub prime, and is the order of the group formed by the BN254 curve.
