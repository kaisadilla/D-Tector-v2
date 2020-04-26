using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.CircularTypes {
    public class IllegalBoundsException : System.Exception {
        public IllegalBoundsException() : base() { }
        public IllegalBoundsException(string message) : base(message) { }
        public IllegalBoundsException(string message, System.Exception inner) : base(message, inner) { }
    }
}