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

            //constants which will later be used for calculations
            //(first 32 bits of the fractional parts of the cubed roots of the first 64 prime numbers)
            uint[] roundConstants = {0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
                                     0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
                                     0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
                                     0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
                                     0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
                                     0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
                                     0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
                                     0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2};
            
            //pre-processing
            string input = args[1];
            byte[] inputBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(input);

            //make a byte array where everything fits
            // length + ( 64 + 1 (appended bit) + 8 (the ulong length at the end) - (length + 9 (added things)) % 64)
            byte[] preProcessedInput = new byte[inputBytes.Length + (73 - ((inputBytes.Length + 9) % 64))];
            //fill the array
            Array.Copy(inputBytes, 0, preProcessedInput, 0, inputBytes.Length);
            preProcessedInput[inputBytes.Length] = 128; //add a 1 to the far left of this byte
            for(int i = inputBytes.Length + 1; i < preProcessedInput.Length - 8; i++) preProcessedInput[i] = 0; //set the filler bytes to 0
            Byte[] lengthBytes = BitConverter.GetBytes((ulong)(inputBytes.Length*8));
            if(BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
            Array.Copy(lengthBytes, 0, preProcessedInput, preProcessedInput.Length - 8, 8);

            //chunkify preprocessed input in 512 bit chunks
            byte[][] chunks = new byte[preProcessedInput.Length >> 6][];
            for(uint i = 0; i < preProcessedInput.Length; i += 64){
                chunks[i>>6] = new byte[64];
                Array.Copy(preProcessedInput, i, chunks[i >> 6], 0, 64);
            }

            //process the input in 512 bit chunks
            for(int i = 0; i < chunks.Length; i++){
                uint[] words = new uint[64];
                //copy chunk into the first 16 words
                for (int j = 0; j < 64; j += 4){
                    words[j >> 2] = ((uint)chunks[i][j] << 24) | ((uint)chunks[i][j+1] << 16) | ((uint)chunks[i][j+2] << 8) | (uint)chunks[i][j+3];
                }
                //fill in the remaining words
                for(int j = 16; j < 64; j++){
                    uint s0 = ((words[j-15] >> 7) | (words[j-15] << 25)) ^ ((words[j-15] >> 18) | (words[j-15] << 14)) ^ (words[j-15] >> 3);
                    uint s1 = ((words[j-2] >> 17) | (words[j-2]  << 15)) ^ ((words[j-2] >> 19) | (words[j-2]  << 13)) ^ (words[j-2] >> 10);
                    words[j] = words[j-16] + s0 + words[j-7] + s1;
                }

                uint a = hash0;
                uint b = hash1;
                uint c = hash2;
                uint d = hash3;
                uint e = hash4;
                uint f = hash5;
                uint g = hash6;
                uint h = hash7;

                //compression function
                for( int j = 0; j < 64; j++){
                    uint s1 = ((e >> 6) | (e << 26)) ^ ((e >> 11) | (e << 21)) ^ ((e >> 25) | (e << 7));
                    uint ch = (e & f) ^ ((~e) & g);
                    uint temp1 = h + s1 + ch + roundConstants[j] + words[j];
                    uint s0 = ((a >> 2) | (a << 30)) ^ ((a >> 13) | (a << 19)) ^ ((a >> 22) | (a << 10));
                    uint maj = (a & b) ^ (a & c) ^ (b & c);
                    uint temp2 = s0 + maj;
                    

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;


                }
                hash0 += a;
                hash1 += b;
                hash2 += c;
                hash3 += d;
                hash4 += e;
                hash5 += f;
                hash6 += g;
                hash7 += h;
            }
            Console.WriteLine((hash0.ToString("X").PadLeft(8, '0') + hash1.ToString("X").PadLeft(8, '0') + hash2.ToString("X").PadLeft(8, '0') + hash3.ToString("X").PadLeft(8, '0') + 
                               hash4.ToString("X").PadLeft(8, '0') + hash5.ToString("X").PadLeft(8, '0') + hash6.ToString("X").PadLeft(8, '0') + hash7.ToString("X").PadLeft(8, '0')).ToLower());
        }

        int rightRotate(int val, int amount){
            return (val >> amount) | (val << 32-amount);
        }
    }
}