using UnityEngine;
using BoardNamespace;

namespace PlayerNamespace
{
    public class Player
    {
        public string Name;
        public int Money;
        public Tile Position;

        public Player(string name, int startingMoney)
        {
            Name = name;
            Money = startingMoney;
            Position = null;
        }
    }
}
