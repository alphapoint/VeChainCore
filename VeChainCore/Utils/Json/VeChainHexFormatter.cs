using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Math;
using Utf8Json;
using Utf8Json.Formatters;

namespace VeChainCore.Utils.Json
{
    public sealed class VeChainHexFormatter
        :
            IJsonFormatter<decimal>,
            IJsonFormatter<BigInteger>,
            IJsonFormatter<ulong>,
            IJsonFormatter<byte[]>
    {
        public static readonly IJsonFormatter Default = new VeChainHexFormatter();

        public void Serialize(
            ref JsonWriter writer,
            Decimal value,
            IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value.ToBigInteger(), formatterResolver);
        }

        public void Serialize(ref JsonWriter writer, BigInteger value, IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value.ToByteArrayUnsigned(), formatterResolver);
        }

        public void Serialize(ref JsonWriter writer, ulong value, IJsonFormatterResolver formatterResolver)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();
            Serialize(ref writer, bytes, formatterResolver);
        }

        private static void WriteHexChar(in JsonWriter writer, int nib)
        {
            if (nib <= 9)
                writer.WriteRaw((byte) ('0' + nib));
            else
                writer.WriteRaw((byte) ('a' + (nib - 0xa)));
        }

        public void Serialize(ref JsonWriter writer, byte[] value, IJsonFormatterResolver formatterResolver)
        {
            int length = value.Length;
            bool noBytes = length == 0;
            writer.EnsureCapacity((noBytes ? 1 : length) * 2 + 4);
            writer.WriteRaw((byte) '"');
            writer.WriteRaw((byte) '0');
            writer.WriteRaw((byte) 'x');
            if (noBytes)
            {
                writer.WriteRaw((byte) '0');
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    byte aByte = value[i];

                    int hiNib = aByte >> 4;
                    int loNib = aByte & 0xF;

                    WriteHexChar(writer, hiNib);
                    WriteHexChar(writer, loNib);
                }
            }

            writer.WriteRaw((byte) '"');
        }


        public byte[] DeserializeBytes(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var currentJsonToken = reader.GetCurrentJsonToken();
            switch (currentJsonToken)
            {
                case JsonToken.String:
                {
                    string s = reader.ReadString();
                    return s.HexToByteArray();
                }

                case JsonToken.Number:
                {
                    var n = reader.ReadUInt64();
                    return BitConverter.GetBytes(n).Reverse().ToArray();
                }

                default:
                    throw new InvalidOperationException("Invalid Json Token for VeChainHexFormatter:" + currentJsonToken);
            }
        }

        public BigInteger DeserializeBigInteger(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => new BigInteger(1, DeserializeBytes(ref reader, formatterResolver));

        public ulong DeserializeUInt64(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var bytes = DeserializeBytes(ref reader, formatterResolver);
            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();
            return Unsafe.ReadUnaligned<ulong>(ref bytes[0]);
        }

        public Decimal DeserializeDecimal(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBigInteger(ref reader, formatterResolver).ToDecimal();

        byte[] IJsonFormatter<byte[]>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBytes(ref reader, formatterResolver);

        ulong IJsonFormatter<ulong>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeUInt64(ref reader, formatterResolver);

        BigInteger IJsonFormatter<BigInteger>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBigInteger(ref reader, formatterResolver);

        decimal IJsonFormatter<decimal>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeDecimal(ref reader, formatterResolver);
    }
}