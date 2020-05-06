using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaisa.Digivice.Extensions {
    public static class Tools {
        public static void Shuffle<T>(this IList<T> list) {
            int i = list.Count;
            Random rng = new Random();

            while(i > 1) {
                i--;
                int k = rng.Next(i + 1);
                T tempValue = list[k];
                list[k] = list[i];
                list[i] = tempValue;
            }
        }

        public static T[] Fill<T>(this T[] array, T value) {
            for(int i = 0; i < array.Length; i++) {
                array[i] = value;
            }
            return array;
        }
        /// <summary>
        /// Returns a random index within the bounds of the IEnumerable.
        /// </summary>
        public static int GetRandomIndex<T>(this IEnumerable<T> array) {
            Random rng = new Random();
            return rng.Next(array.Count());
        }
        /// <summary>
        /// Returns a random element from the IEnumerable.
        /// </summary>
        public static T GetRandomElement<T>(this IEnumerable<T> array) {
            return array.ElementAt(array.GetRandomIndex());
        }

        /// <summary>
        /// Returns a copy of the array from the index position to the end.
        /// </summary>
        public static T[] SubArray<T>(this T[] array, int index) {
            T[] subArray = new T[array.Length - index];
            Array.Copy(array, index, subArray, 0, array.Length - index);
            return subArray;
        }
        /// <summary>
        /// Returns a sub array containing length elements from the original array, starting at index position.
        /// </summary>
        public static T[] SubArray<T>(this T[] array, int index, int length) {
            T[] subArray = new T[length];
            Array.Copy(array, index, subArray, 0, length);
            return subArray;
        }

        /// <summary>
        /// Returns a copy of the array containing, in the specified order, the elements in the indices given.
        /// Will throw an IndexOutOfBoundsException if one of the indices specified is out of the bounds of the original array.
        /// </summary>
        /// <param name="array">The array to be copied with a new order.</param>
        /// <param name="indices">The list of indices of the array, ordered by their new order.</param>
        /// <returns></returns>
        public static T[] ReorderedAs<T>(this T[] array, params int[] indices) {
            T[] newArray = new T[indices.Length];
            for(int i = 0; i < newArray.Length; i++) {
                newArray[i] = array[indices[i]];
            }
            return newArray;
        }
    }
}