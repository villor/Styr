using System;
using System.Text;

namespace StyrClient
{
	public static class PacketConverter
	{
		public static ushort GetUShort(byte[] array, int index)
		{
			byte[] shortArr = { array[index], array[index + 1] };
			fixEndianess (shortArr);
			return BitConverter.ToUInt16 (shortArr, 0);
		}

		public static string getUTF8String(byte[] array, int index, ushort length){
			Console.WriteLine (array.Length);
			Console.WriteLine (length);

			byte[] stringArr = new byte[length];
			Array.Copy (array, index, stringArr, 0, length);
			return Encoding.UTF8.GetString (stringArr);
		}

		private static void fixEndianess(byte[] array) {
			if (BitConverter.IsLittleEndian) {
				Array.Reverse (array);
			}
		}
	}
}

