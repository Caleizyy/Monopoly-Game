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
        public int JailTurns = 2; // Kiek ėjimų turi praleisti

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

            Debug.Log($"🚶 {Name} juda {steps} žingsnius...");

            // Iteruoja per sąrašą
            for (int i = 0; i < steps; i++)
            {
                Position = Position.Next;
                if (Position is StartTile && i < steps - 1)
                {
                    Money += 200; // Bonus už praėjimą pro startą
                    Debug.Log($"💰 {Name} praėjo startą ir gavo 200€!");
                }
            }

            Debug.Log($"📍 {Name} atsistojo ant: {Position.GetInfo()}");

            // Aktyvuoja laukelio logiką
            Position.OnPlayerLand(this);
        }
        public bool BuyProperty(StreetTile street)
        {
            if (Money >= street.Price && street.Owner == null)
            {
                Money -= street.Price;
                street.Owner = this;
                Debug.Log($"✅ {Name} nusipirko {street.Name} už {street.Price}€");
                return true;
            }
            else if (street.Owner != null)
            {
                Debug.Log($"❌ {street.Name} jau turi savininką ({street.Owner.Name})!");
                return false;
            }
            else
            {
                Debug.Log($"❌ {Name} neturi pakankamai pinigų! (turi: {Money}€, reikia: {street.Price}€)");
                return false;
            }
        }
        public bool BuyBuilding(StreetTile street, int buildingCost)
        {
            if (street.Owner != this)
            {
                Debug.Log($"❌ {Name} nevaldo {street.Name}!");
                return false;
            }

            if (Money < buildingCost)
            {
                Debug.Log($"❌ {Name} neturi pakankamai pinigų pastatui! (turi: {Money}€, reikia: {buildingCost}€)");
                return false;
            }

            if (street.Buildings.Count >= 3)
            {
                Debug.Log($"❌ {street.Name} jau turi maksimalų pastatų skaičių (3)!");
                return false;
            }

            Money -= buildingCost;
            street.AddBuilding(buildingCost / 2); // Papildoma nuoma = pusė pastato kainos
            Debug.Log($"🏗️ {Name} pastatė pastatą ant {street.Name}");
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