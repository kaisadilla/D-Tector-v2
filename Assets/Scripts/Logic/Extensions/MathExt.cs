using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kaisa.Digivice.Extensions {
    public static class MathExt {
        /// <summary>
        /// Performs a circular add: Upper bound + 1 equals Lower bound.
        /// </summary>
        /// <param name="a">The base byte.</param>
        /// <param name="b">An int to add.</param>
        /// <param name="upperBound">The maximum value (inclusive)</param>
        /// <param name="lowerBound">The minimum value (inclusive)</param>
        public static byte CircularAdd(this byte a, int b, byte upperBound, byte lowerBound = 0) {
            int result = a + b;
            return (byte)GetInsideBounds(result, upperBound, lowerBound);
        }
        /// <summary>
        /// Performs a circular add: Upper bound + 1 equals Lower bound.
        /// </summary>
        /// <param name="a">The base integer.</param>
        /// <param name="b">The integer to add.</param>
        /// <param name="upperBound">The maximum value (inclusive)</param>
        /// <param name="lowerBound">The minimum value (inclusive)</param>
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