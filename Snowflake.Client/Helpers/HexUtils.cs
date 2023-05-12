using System;
using System.IO;

namespace Snowflake.Client.Helpers
{
    internal class HexUtils
    {
        const char __base64PaddingChar = '=';

        // The base64 encoding table.
        private static readonly char[] __base64Table =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
        };

        private static readonly int[] __hexValueTable = new int[256];

        static HexUtils()
        {
            // Build hex character to value lookup table.
            // Init all elements to -1 to begin with.
            for(int i = 0; i < __hexValueTable.Length; i++)
                __hexValueTable[i] = -1;
            
            // Set the mappings for the hex characters (upper and lower case).
            for(char c = '0'; c <= '9'; c++) __hexValueTable[c] = c - '0';
            for(char c = 'A'; c <= 'F'; c++) __hexValueTable[c] = c - 'A' + 10;
            for(char c = 'a'; c <= 'f'; c++) __hexValueTable[c] = c - 'a' + 10;
        }

        public static void HexToBase64(string hex, TextWriter tw)
        {
            if(hex.Length % 2 != 0)
                throw new ArgumentException("The hexadecimal string cannot have an odd length.");

            // Get a span over the input string, for fast/efficient processing of the characters.
            ReadOnlySpan<char> hexSpan = hex.AsSpan();

            // Allocate temp storage for decoded hex chars, and encoded base64 characters.
            Span<byte> inBytes = stackalloc byte[3];
            char[] outChars = new char[4];

            // Loop over hexSpan in six character chunks (6 hex chars represents 3 bytes).
            while(hexSpan.Length >= 6)
            {
                // Decode 6 hex chars to 3 bytes.
                inBytes[0] = HexToByte(hexSpan);
                inBytes[1] = HexToByte(hexSpan.Slice(2));
                inBytes[2] = HexToByte(hexSpan.Slice(4));

                // Encode the three bytes as a base64 block (3 bytes becomes 4 base64 characters).
                EncodeBase64Block(inBytes, outChars);

                // Write the base64 chars to the string builder.
                tw.Write(outChars);

                // Move hexSpan forward six chars.
                hexSpan = hexSpan.Slice(6);
            }

            // Handle any remaining hex chars / bytes. I.e., when hex.Length is not a multiple of 6.
            switch(hexSpan.Length)
            {
                case 0:
                    // No more hex chars; exit.
                    break;

                case 2:
                    // Two hex chars remaining (i.e., one byte).
                    inBytes[0] = HexToByte(hexSpan);
                    inBytes[1] = 0;
                    inBytes[2] = 0;
                    EncodeBase64Block(inBytes, outChars);
                    outChars[2] = __base64PaddingChar;
                    outChars[3] = __base64PaddingChar;
                    tw.Write(outChars);
                    break;

                case 4:
                    // Four hex chars remaining (i.e., two bytes).
                    inBytes[0] = HexToByte(hexSpan);
                    inBytes[1] = HexToByte(hexSpan.Slice(2));
                    inBytes[2] = 0;
                    EncodeBase64Block(inBytes, outChars);
                    outChars[3] = __base64PaddingChar;
                    tw.Write(outChars);
                    break;
            }
        }

        private static byte HexToByte(ReadOnlySpan<char> hexSpan)
        {
            return (byte)(GetHexValue(hexSpan[0]) << 4 | GetHexValue(hexSpan[1]));
        }

        private static int GetHexValue(char hexCharacter)
        {
            if(hexCharacter > 255)
                throw new ArgumentException("Invalid hexadecimal character.");

            // Lookup the character's value.
            int value = __hexValueTable[hexCharacter];

            // If the value is -1 then the character is not a hexadecimal character.
            if(value >= 0)
                return value;

            throw new ArgumentException("Invalid hexadecimal character.");
        }

        private static void EncodeBase64Block(ReadOnlySpan<byte> inBytes, Span<char> outChars)
        {
            // Encode the three bytes as a base64 block (3 bytes becomes 4 base64 characters).
            outChars[0] = __base64Table[(inBytes[0] & 0xfc) >> 2];
            outChars[1] = __base64Table[((inBytes[0] & 0x03) << 4) | ((inBytes[1] & 0xf0) >> 4)];
            outChars[2] = __base64Table[((inBytes[1] & 0x0f) << 2) | ((inBytes[2] & 0xc0) >> 6)];
            outChars[3] = __base64Table[inBytes[2] & 0x3f];
        }
    }
}
