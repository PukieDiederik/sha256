using System;

namespace sha256 {
    class Program {
        static void Main(string[] args) {
            //initialize original hashes
            //(first 32 bits of the fractional parts of the sqrt of the first 8 prime numbers)
            uint hash0 = 0x6a09e667;
            uint hash1 = 0xbb67ae85;
            uint hash2 = 0x3c6ef372;
            uint hash3 = 0xa54ff53a;
            uint hash4 = 0x510e527f;
            uint hash5 = 0x9b05688c;
            uint hash6 = 0x1f83d9ab;
            uint hash7 = 0x5be0cd19;
        }
    }
}