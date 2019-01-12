﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nethereum.RLP;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Utils
{
    public enum Prefix
    {
        Empty,
        ZeroLowerX
    }

    public enum StringType
    {
        Hex,
        Plain
    }

    public static class Hex
    {
        public static BigInteger HexToBigInt(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);


            // To counteract the strange sign bit https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.parse?redirectedfrom=MSDN&view=netframework-4.7.2#System_Numerics_BigInteger_Parse_System_String_System_Globalization_NumberStyles_
            hex = "00" + hex;

            return new BigInteger(hex);
        }

        public static decimal ToHumanReadable(decimal dec)
        {
            return dec / 1_000_000_000_000_000_000;
        }

        public static byte[] BigIntegerToBytes(this BigInteger bigInt)
        {
            return bigInt.ToByteArray().TrimLeadingZeroBytes();
        }

        public static string ByteArrayToString(this byte[] ba, StringType type = StringType.Hex, Prefix prefix = Prefix.Empty)
        {
            if(type == StringType.Plain)
                return Encoding.UTF8.GetString(ba);

            var hex = new StringBuilder(ba.Length * 2 + 2);
            if (prefix != Prefix.Empty)
                hex.Append("0x");
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(this string input)
        {
            return input.ToBytesForRLPEncoding();
        }

        public static byte[] HexStringToByteArray(this string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static bool OnlyHexInString(string test)
        {
            if (test.StartsWith("0x"))
                test = test.Substring(2);
            return Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static byte[] ToBytesPadded(BigInteger value, int length)
        {
            byte[] result = new byte[length];
            byte[] bytes = value.BigIntegerToBytes();
            int bytesLength;
            byte srcOffset;
            if (bytes[0] == 0)
            {
                bytesLength = bytes.Length - 1;
                srcOffset = 1;
            }
            else
            {
                bytesLength = bytes.Length;
                srcOffset = 0;
            }

            if (bytesLength > length)
            {
                throw new Exception("Input is too large to put in byte array of size " + length);
            }
            int destOffset = length - bytesLength;
            Array.Copy(bytes, srcOffset, result, destOffset, bytesLength);
            return result;
        }


        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            return new BigInteger(1, bytes);
        }
    }
}
