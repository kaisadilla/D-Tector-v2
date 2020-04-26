using UnityEngine.UIElements;

namespace Kaisa.CircularTypes {
    public class CircularType {

    }
    /// <summary>
    /// Represents a Circular Int, which is an int that loops back to its lower bound once its upper bound is reached.
    /// </summary>
    public class C_Int : CircularType {
        private int _varInt;
        private int lowerBound, upperBound;
        private int Range {
            get => upperBound - lowerBound + 1;
        }
        /// <summary>
        /// Creates a new C_Int, with a value, an upper bound, and a lower bound. If not explicitly defined, the lower bound will be 0.
        /// An exception will be thrown if any value is illegal.
        /// </summary>
        /// <param name="val">The value of the C_Int.</param>
        /// <param name="upperBound">The maximum number (inclusive) of the C_Int.</param>
        /// <param name="lowerBound">The minimum number (inclusive) of the C_Int.</param>
        public C_Int(int val, int upperBound, int lowerBound = 0) {
            if (upperBound <= lowerBound) {
                throw new IllegalBoundsException("The upper bound of the VarInt must be higher than the lower bound");
            }
            this.upperBound = upperBound;
            this.lowerBound = lowerBound;
            SetValue(val);
        }

        /// <summary>
        /// Assigns a new value to the C_Int. An exception will be thrown if the new value is off bounds.
        /// </summary>
        /// <param name="val">The new value of the C_Int.</param>
        /// <returns></returns>
        public C_Int SetValue(int val) {
            if (val < lowerBound || val > upperBound) {
                throw new IllegalBoundsException("The value of the VarInt must be within its bounds.");
            }
            else {
                _varInt = val;
            }
            return this;
        }
        public override string ToString() => _varInt.ToString();

        public static C_Int operator +(C_Int a, C_Int b) => a + b._varInt;
        public static C_Int operator +(C_Int a, int b) {
            C_Int c = new C_Int(a._varInt, a.upperBound, a.lowerBound);
            c._varInt = c.GetInsideBounds(c._varInt + b);
            return c;
        }
        public static C_Int operator ++(C_Int a) {
            a._varInt = a.GetInsideBounds(a._varInt + 1);
            return a;
        }
        public static C_Int operator -(C_Int a, C_Int b) => a - b._varInt;
        public static C_Int operator -(C_Int a, int b) {
            C_Int c = new C_Int(a._varInt, a.upperBound, a.lowerBound);
            c._varInt = c.GetInsideBounds(c._varInt - b);
            return c;
        }
        public static C_Int operator --(C_Int a) {
            a._varInt = a.GetInsideBounds(a._varInt - 1);
            return a;
        }

        public static C_Int operator *(C_Int a, C_Int b) => a * b._varInt;
        public static C_Int operator *(C_Int a, int b) {
            a._varInt = a.GetInsideBounds(a._varInt * b);
            return a;
        }
        public static C_Int operator /(C_Int a, C_Int b) => a / b._varInt;
        public static C_Int operator /(C_Int a, int b) {
            a._varInt = a.GetInsideBounds(a._varInt * b);
            return a;
        }

        public static C_Int operator %(C_Int a, C_Int b) => a % b._varInt;
        public static C_Int operator %(C_Int a, int b) {
            a._varInt %= b;
            return a;
        }
        public static C_Int operator <<(C_Int a, int b) {
            a._varInt = a.GetInsideBounds(a._varInt << b);
            return a;
        }
        public static C_Int operator >>(C_Int a, int b) {
            a._varInt = a.GetInsideBounds(a._varInt >> b);
            return a;
        }

        public static bool operator ==(C_Int a, C_Int b) => a == b._varInt;
        public static bool operator ==(C_Int a, int b) {
            return a._varInt == b;
        }
        public static bool operator !=(C_Int a, C_Int b) => a != b._varInt;
        public static bool operator !=(C_Int a, int b) {
            return a._varInt != b;
        }
        public static bool operator <(C_Int a, C_Int b) => a < b._varInt;
        public static bool operator <(C_Int a, int b) {
            return a._varInt < b;
        }
        public static bool operator >(C_Int a, C_Int b) => a > b._varInt;
        public static bool operator >(C_Int a, int b) {
            return a._varInt > b;
        }
        public static bool operator <=(C_Int a, C_Int b) => a <= b._varInt;
        public static bool operator <=(C_Int a, int b) {
            return a._varInt <= b;
        }
        public static bool operator >=(C_Int a, C_Int b) => a >= b._varInt;
        public static bool operator >=(C_Int a, int b) {
            return a._varInt >= b;
        }

        public static explicit operator int(C_Int a) {
            return a._varInt;
        }

        private int GetInsideBounds(int val) {
            if(val < lowerBound) {
                while (val < lowerBound) val += Range;
            }
            if(val > upperBound) {
                while (val > upperBound) val -= Range;
            }
            return val;
        }

        public override bool Equals(object obj) {
            return obj is C_Int @int && _varInt == @int._varInt && lowerBound == @int.lowerBound && upperBound == @int.upperBound && Range == @int.Range;
        }

        public override int GetHashCode() {
            int hashCode = -242517843;
            hashCode = hashCode * -1521134295 + _varInt.GetHashCode();
            hashCode = hashCode * -1521134295 + lowerBound.GetHashCode();
            hashCode = hashCode * -1521134295 + upperBound.GetHashCode();
            hashCode = hashCode * -1521134295 + Range.GetHashCode();
            return hashCode;
        }
    }
}