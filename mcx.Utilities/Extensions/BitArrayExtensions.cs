using System.Collections;

namespace mcx.Utilities.Extensions
{
    /// <summary>
    /// Extensions targeting <see cref="BitArray"/>
    /// </summary>
    public static class BitArrayExtensions
    {
        /// <summary>
        /// Convert BitArray to int (Specifies the start index and bit length)
        /// </summary>
        /// <param name="startIndex">Start index</param>
        /// <param name="bitLength">Bit length</param>
        /// <returns>Converted int</returns>
        public static int GetNumeral(this BitArray array, int startIndex, int bitLength)
        {
            var newArray = new BitArray(bitLength);

            for (int i = 0; i < bitLength; i++)
            {
                if (array.Length <= startIndex + i)
                {
                    newArray[i] = false;
                }
                else
                {
                    bool bit = array.Get(startIndex + i);
                    newArray[i] = bit;
                }
            }

            return newArray.ToNumeral();
        }

        /// <summary>
        /// Convert BitArray to int
        /// </summary>
        /// <returns>Converted int</returns>
        public static int ToNumeral(this BitArray array)
        {
            if (array == null)
                return 0;

            if (array.Length > 32)
                return 0;

            var result = new int[1];

            array.CopyTo(result, 0);
            return result[0];
        }
    }
}
