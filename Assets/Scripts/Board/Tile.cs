using System.Collections.Generic;
using UnityEngine;

namespace BoardNamespace
{
    // Bazinė laukelio klasė
    public abstract class Tile
    {
        public string Name;
        public string Type;
        public Tile Next; // Cikliniam sąrašui

        // Abstraktus metodas - kiekvienas laukelio tipas turi savo logiką
        public abstract void OnPlayerLand(PlayerNamespace.Player player);

        // Virtual metodas - galima override'inti
        public virtual string GetInfo()
        {
            return $"{Name} ({Type})";
        }
    }

    // 1️⃣ GATVĖ - galima pirkti, turi savininką
    public class StreetTile : Tile
    {
        public int Price;
        public int BaseRent;
        public PlayerNamespace.Player Owner;
        public Stack<int> Buildings = new Stack<int>(); // Pastatai

        public StreetTile(string name, int price, int baseRent)
        {
            Name = name;
            Type = "street";
            Price = price;
            BaseRent = baseRent;
            Owner = null;
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            if (Owner == null)
            {
                // Laukelis be savininko - gali pirkti
                Debug.Log($"{player.Name} gali nusipirkti {Name} už {Price}€");
            }
            else if (Owner != player)
            {
                // Turi mokėti nuomą
                int rent = CalculateRent();
                player.Money -= rent;
                Owner.Money += rent;
                Debug.Log($"{player.Name} sumoka {rent}€ nuomos {Owner.Name}");
            }
            else
            {
                // Tai žaidėjo nuosavybė - gali statyti
                Debug.Log($"{player.Name} atsistojo ant savo laukelio {Name}");
            }
        }

        public int CalculateRent()
        {
            int rent = BaseRent;
            // Kiekvienas pastatas didina nuomą
            int buildingCount = Buildings.Count;
            for (int i = 0; i < buildingCount; i++)
            {
                rent += Buildings.ToArray()[i];
            }
            return rent;
        }

        public void AddBuilding(int additionalRent)
        {
            if (Buildings.Count >= 3)
            {
                Debug.Log("Jau yra maksimalus pastatų skaičius (3)!");
                return;
            }
            Buildings.Push(additionalRent);
            Debug.Log($"Pastatytas naujas pastatas! Papildoma nuoma: {additionalRent}€");
        }

        public override string GetInfo()
        {
            string info = base.GetInfo() + $" | Kaina: {Price}€ | Nuoma: {CalculateRent()}€";
            if (Owner != null)
                info += $" | Savininkas: {Owner.Name}";
            if (Buildings.Count > 0)
                info += $" | Pastatai: {Buildings.Count}";
            return info;
        }
    }

    // 2️⃣ STARTAS - gauni pinigų
    public class StartTile : Tile
    {
        public int BonusMoney = 200;

        public StartTile(string name)
        {
            Name = name;
            Type = "start";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            player.Money += BonusMoney;
            Debug.Log($"{player.Name} perėjo startą ir gavo {BonusMoney}€!");
        }
    }

    // 3️⃣ KALĖJIMAS - praleidžia ėjimą
    public class JailTile : Tile
    {
        public JailTile(string name)
        {
            Name = name;
            Type = "jail";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            player.IsInJail = true;
            player.JailTurns = 1; // Praleidžia 1 ėjimą
            Debug.Log($"{player.Name} pakliuvo į kalėjimą! Praleidžia ėjimą.");
        }
    }

    // 4️⃣ LOTERIJA/BONUS - visada gauni pinigų
    public class BonusTile : Tile
    {
        public int Amount;

        public BonusTile(string name, int amount)
        {
            Name = name;
            Type = "bonus";
            Amount = amount;
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            player.Money += Amount;
            Debug.Log($"{player.Name} laimėjo loteriją ir gavo {Amount}€!");
        }
    }

    // 5️⃣ MOKESČIAI - moki pinigus
    public class TaxTile : Tile
    {
        public int Amount;

        public TaxTile(string name, int amount)
        {
            Name = name;
            Type = "tax";
            Amount = amount;
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            player.Money -= Amount;
            Debug.Log($"{player.Name} sumokėjo {Amount}€ mokesčių.");
        }
    }

    // 6️⃣ STOTIS - speciali nuosavybė
    public class StationTile : Tile
    {
        public int Price;
        public int BaseRent;
        public PlayerNamespace.Player Owner;

        public StationTile(string name, int price, int baseRent)
        {
            Name = name;
            Type = "station";
            Price = price;
            BaseRent = baseRent;
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            if (Owner == null)
            {
                Debug.Log($"{player.Name} gali nusipirkti stotį {Name} už {Price}€");
            }
            else if (Owner != player)
            {
                player.Money -= BaseRent;
                Owner.Money += BaseRent;
                Debug.Log($"{player.Name} sumoka {BaseRent}€ nuomos {Owner.Name}");
            }
        }

        public override string GetInfo()
        {
            string info = base.GetInfo() + $" | Kaina: {Price}€ | Nuoma: {BaseRent}€";
            if (Owner != null)
                info += $" | Savininkas: {Owner.Name}";
            return info;
        }
    }

    // 7️⃣ KOMUNALINĖS PASLAUGOS
    public class UtilityTile : Tile
    {
        public int Price;
        public int BaseRent;
        public PlayerNamespace.Player Owner;

        public UtilityTile(string name, int price, int baseRent)
        {
            Name = name;
            Type = "utility";
            Price = price;
            BaseRent = baseRent;
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            if (Owner == null)
            {
                Debug.Log($"{player.Name} gali nusipirkti {Name} už {Price}€");
            }
            else if (Owner != player)
            {
                player.Money -= BaseRent;
                Owner.Money += BaseRent;
                Debug.Log($"{player.Name} sumoka {BaseRent}€ nuomos {Owner.Name}");
            }
        }

        public override string GetInfo()
        {
            string info = base.GetInfo() + $" | Kaina: {Price}€ | Nuoma: {BaseRent}€";
            if (Owner != null)
                info += $" | Savininkas: {Owner.Name}";
            return info;
        }
    }

    // 8️⃣ NEMOKAMAS POILSIS - nieko nevyksta
    public class FreeParkingTile : Tile
    {
        public FreeParkingTile(string name)
        {
            Name = name;
            Type = "free_parking";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            Debug.Log($"{player.Name} ilsisi nemokamame parkinge!");
        }
    }

    // 9️⃣ EINI Į KALĖJIMĄ
    public class GoToJailTile : Tile
    {
        public GoToJailTile(string name)
        {
            Name = name;
            Type = "goto_jail";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            player.IsInJail = true;
            player.JailTurns = 1;
            Debug.Log($"{player.Name} eina į kalėjimą!");
            // Reikės perkelti žaidėją į kalėjimo laukelį
        }
    }
}