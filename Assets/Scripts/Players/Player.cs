using UnityEngine;
using BoardNamespace;

namespace PlayerNamespace
{
    public class Player
    {
        public string Name;
        public int Money;
        public Tile Position;

        // Kalėjimo būsena
        public bool IsInJail = false;
        public int JailTurns = 0; // Kiek ėjimų turi praleisti

        public Player(string name, int startingMoney)
        {
            Name = name;
            Money = startingMoney;
            Position = null; // Bus nustatyta žaidimo pradžioje
        }

        // Judėjimas lenta
        public void Move(int steps)
        {
            if (Position == null)
            {
                Debug.LogError($"{Name} neturi pozicijos!");
                return;
            }

            Debug.Log($"{Name} juda {steps} žingsnius...");

            // Iteruojame per sąrašą
            for (int i = 0; i < steps; i++)
            {
                Position = Position.Next;

                // Jei praėjome startą (grįžome į pradžią)
                if (Position is StartTile && i > 0)
                {
                    Money += 200; // Bonus už praeita startą
                    Debug.Log($"{Name} praėjo startą ir gavo 200€!");
                }
            }

            Debug.Log($"{Name} atsistojo ant: {Position.GetInfo()}");

            // Aktyvuojame laukelio logiką
            Position.OnPlayerLand(this);
        }

        // Pirkti laukelį
        public bool BuyProperty(StreetTile street)
        {
            if (Money >= street.Price && street.Owner == null)
            {
                Money -= street.Price;
                street.Owner = this;
                Debug.Log($"{Name} nusipirko {street.Name} už {street.Price}€");
                return true;
            }
            else if (street.Owner != null)
            {
                Debug.Log($"{street.Name} jau turi savininką!");
                return false;
            }
            else
            {
                Debug.Log($"{Name} neturi pakankamai pinigų!");
                return false;
            }
        }

        // Pirkti pastatą
        public bool BuyBuilding(StreetTile street, int buildingCost)
        {
            if (street.Owner != this)
            {
                Debug.Log($"{Name} nevaldo {street.Name}!");
                return false;
            }

            if (Money < buildingCost)
            {
                Debug.Log($"{Name} neturi pakankamai pinigų pastatui!");
                return false;
            }

            if (street.Buildings.Count >= 3)
            {
                Debug.Log($"{street.Name} jau turi maksimalų pastatų skaičių!");
                return false;
            }

            Money -= buildingCost;
            street.AddBuilding(buildingCost / 2); // Papildoma nuoma = pusė pastato kainos
            Debug.Log($"{Name} pastatė pastatą ant {street.Name}");
            return true;
        }

        // Patikrinti ar žaidėjas pralaimėjo
        public bool IsBankrupt()
        {
            return Money < 0;
        }

        // Info apie žaidėją
        public string GetStatus()
        {
            string status = $"{Name}: {Money}€";
            if (IsInJail)
                status += " [KALĖJIME]";
            return status;
        }
    }
}