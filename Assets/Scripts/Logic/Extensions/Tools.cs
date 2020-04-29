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

        public static int GetRandomIndex<T>(IEnumerable<T> array) {
            Random rng = new Random();
            return rng.Next(array.Count());
        }
    }
}