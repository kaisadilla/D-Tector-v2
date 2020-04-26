using Kaisa.CircularTypes;
using System;
using System.Collections.Generic;

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
    }
}