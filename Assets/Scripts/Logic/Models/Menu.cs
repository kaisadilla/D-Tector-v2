using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Kaisa.Digivice {
    public class Menu {
        public string Name { get; private set; }
        public int Order { get; private set; }
        private Dictionary<string, Menu> children = new Dictionary<string, Menu>();

        public Menu(string name) {
            Name = name;
        }

        public Menu this[string key] {
            get {
                if(children.TryGetValue(key, out Menu val)) {
                    return val;
                }
                else {
                    children[key] = key;
                    return children[key];
                }
            }
        }

        public void Add(Menu childMenu) {
            children[childMenu.Name] = childMenu;
        }

        public bool HasChildren() => (children.Count != 0);

        public Menu SetOrder(int order) {
            Order = order;
            return this;
        }

        public static implicit operator Menu (string name) {
            return new Menu(name);
        }
    }
}