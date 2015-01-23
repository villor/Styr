using System;

namespace StyrServer
{
	public static class PacketConverter
	{
		public static ushort GetUShort(byte[] array, int index)
		{
			byte[] shortArr = { array[index], array[index + 1] };

			if (BitConverter.IsLittleEndian) {
				Array.Reverse (shortArr);
			}

			return BitConverter.ToUInt16 (shortArr, 0);
		}
			
		public static void PutUShort(ushort val, byte[] array, int index)
		{
			byte[] ushortArr = BitConverter.GetBytes(val);

			if (BitConverter.IsLittleEndian) {
				Array.Reverse(ushortArr);
			}

			Array.Copy (ushortArr, 0, array, index, 2);
		}

		public static float GetFloat(byte[] array, int index)
		{
			byte[] floatArr = new byte[4];
			Array.Copy (array, index, floatArr, 0, 4);

			if (BitConverter.IsLittleEndian) {
				Array.Reverse (floatArr);
			}

			return BitConverter.ToSingle (floatArr, 0);
		}

	}
}

