using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kaisa.Digivice.Extensions {
    public static class MathExt {
        public static int CircularAdd(this int a, int b, int upperBound, int lowerBound = 0) {
            int result = a + b;
            return GetInsideBounds(result, upperBound, lowerBound);
        }

        //Private methods:
        private static int GetInsideBounds(int val, int upperBound, int lowerBound) {
            int range = upperBound - lowerBound + 1;
            if (val < lowerBound) {
                while (val < lowerBound) val += range;
            }
            if (val > upperBound) {
                while (val > upperBound) val -= range;
            }
            return val;
        }
    }
}