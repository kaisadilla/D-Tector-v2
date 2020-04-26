using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kaisa.Digivice.Extensions {
    public static class Enums {
        /// <summary>
        /// Returns the next value of this enum.
        /// </summary>
        public static T Next<T>(this T thisEnum) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException($"{typeof(T).FullName} is not an enum.");
            }

            T[] values = (T[])Enum.GetValues(thisEnum.GetType());
            int i = Array.IndexOf(values, thisEnum) + 1;
            return (i == values.Length) ? values[0] : values[i];
        }
        /// <summary>
        /// Returns the last value of this enum.
        /// </summary>
        public static T Last<T>(this T thisEnum) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException($"{typeof(T).FullName} is not an enum.");
            }

            T[] values = (T[])Enum.GetValues(thisEnum.GetType());
            int i = Array.IndexOf(values, thisEnum) - 1;
            return (i == -1) ? values[values.Length - 1] : values[i];
        }
        /// <summary>
        /// Sets the value of this enum to be the next value to the current one.
        /// </summary>
        public static void SetNext<T>(this ref T thisEnum) where T : struct {
            thisEnum = thisEnum.Next();
        }
        /// <summary>
        /// Sets the value of this enum to be the last value from the current one.
        /// </summary>
        public static void SetLast<T>(this ref T thisEnum) where T : struct {
            thisEnum = thisEnum.Last();
        }
    }
}