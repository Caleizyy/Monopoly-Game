using System.Collections.Generic;
using UnityEngine;
using PlayerNamespace;

namespace BoardNamespace
{
    public class Tile
    {
        public string Name;
        public string Type;
        public int Price;
        public int Rent;
        public Player Owner;
        public Stack<int> Buildings = new Stack<int>();
        public Tile Next;
    }
}
