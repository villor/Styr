using System;
using System.Text;

namespace Styr.Core
{
	public static class PacketConverter
	{
		private static void FixEndianess(byte[] packet)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(packet);
			}
		}

		public static ushort GetUShort(byte[] packet, int index)
		{
			byte[] shortArr = { packet[index], packet[index + 1] };
			FixEndianess(shortArr);
			return BitConverter.ToUInt16(shortArr, 0);
		}

		public static void PutUShort(ushort val, byte[] packet, int index)
		{
			byte[] ushortArr = BitConverter.GetBytes(val);
			FixEndianess(ushortArr);
			Array.Copy(ushortArr, 0, packet, index, 2);
		}

		public static float GetFloat(byte[] packet, int index)
		{
			byte[] floatArr = new byte[4];
			Array.Copy(packet, index, floatArr, 0, 4);
			FixEndianess(floatArr);
			return BitConverter.ToSingle(floatArr, 0);
		}

		public static char GetChar(byte[] packet, int index)
		{
			byte[] charArr = new byte[2];
			Array.Copy(packet, index, charArr, 0, 2);
			FixEndianess(charArr);
			return BitConverter.ToChar(charArr, 0);
		}

		public static string GetUTF8String(byte[] packet, int index, int length)
		{
			byte[] stringArr = new byte[length];
			Array.Copy(packet, index, stringArr, 0, length);
			return Encoding.UTF8.GetString(stringArr);
		}
	}
}
