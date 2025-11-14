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

    // GATVĖ - galima pirkti, turi savininką
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
                // Laukelis be savininko
                Debug.Log($"💰 {player.Name} gali nusipirkti {Name} už {Price}€");
                Debug.Log($"   💡 Spausk [B] jei nori pirkti, arba [SPACE] tęsti be pirkimo");
            }
            else if (Owner != player)
            {
                // Turi mokėti nuomą
                int rent = CalculateRent();
                player.Money -= rent;
                Owner.Money += rent;
                Debug.Log($"💸 {player.Name} sumoka {rent}€ nuomos {Owner.Name}");
            }
            else
            {
                // Tai žaidėjo nuosavybė - gali statyti
                Debug.Log($"🏠 {player.Name} atsistojo ant savo laukelio {Name}");
                Debug.Log($"   💡 Spausk [H] pastatui, [D] nugriauti, arba [SPACE] tęsti");
            }
        }

        public int CalculateRent()
        {
            int rent = BaseRent;
            // Kiekvienas pastatas didina nuomą
            foreach (int buildingRent in Buildings)
            {
                rent += buildingRent;
            }
            return rent;
        }

        public void AddBuilding(int additionalRent)
        {
            if (Buildings.Count >= 3)
            {
                Debug.Log("❌ Jau yra maksimalus pastatų skaičius (3)!");
                return;
            }
            Buildings.Push(additionalRent);
            Debug.Log($"🏗️ Pastatytas naujas pastatas! Papildoma nuoma: {additionalRent}€");
        }

        // Parduoti pastatą (nugriauti)
        public bool SellBuilding(PlayerNamespace.Player player)
        {
            if (Owner != player)
            {
                Debug.Log($"❌ {player.Name} nevaldo {Name}!");
                return false;
            }

            if (Buildings.Count == 0)
            {
                Debug.Log($"❌ {Name} neturi pastatų!");
                return false;
            }

            // Nuimame viršutinį pastatą iš steko
            int rentValue = Buildings.Pop();
            int refund = rentValue * 2; // Gražiname dvigubai (nes kaina buvo Price/2, o rent = kaina/4)
            player.Money += refund;

            Debug.Log($"🏚️ {player.Name} nugriavo pastatą ant {Name} ir gavo {refund}€ atgal!");
            return true;
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

    // STARTAS - bonusas TIKTAI už praėjimą (ne už atsistojimą)
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
            Debug.Log($"🏁 {player.Name} yra starte!");
        }
    }

    // KALĖJIMAS - tik lankosi (ne bausmė)
    public class JailTile : Tile
    {
        public JailTile(string name)
        {
            Name = name;
            Type = "jail";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            // Tik lankosi, ne bausmė
            Debug.Log($"👮 {player.Name} tik lanko kalėjimą (nėra baudžiamas)");
        }
    }

    // LOTERIJA/BONUS - visada gauni pinigų
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
            Debug.Log($"🎉 {player.Name} laimėjo loteriją ir gavo {Amount}€!");
        }
    }

    // MOKESČIAI - moki pinigus
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
            Debug.Log($"💰 {player.Name} sumokėjo {Amount}€ mokesčių.");
        }
    }

    // STOTIS - speciali nuosavybė
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
                Debug.Log($"🚂 {player.Name} gali nusipirkti stotį {Name} už {Price}€");
                Debug.Log($"   💡 Spausk [B] jei nori pirkti, arba [SPACE] tęsti be pirkimo");
            }
            else if (Owner != player)
            {
                player.Money -= BaseRent;
                Owner.Money += BaseRent;
                Debug.Log($"💸 {player.Name} sumoka {BaseRent}€ nuomos {Owner.Name}");
            }
            else
            {
                Debug.Log($"🚂 {player.Name} yra savo stotyje {Name}");
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

    // KOMUNALINĖS PASLAUGOS
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
                Debug.Log($"⚡ {player.Name} gali nusipirkti {Name} už {Price}€");
                Debug.Log($"   💡 Spausk [B] jei nori pirkti, arba [SPACE] tęsti be pirkimo");
            }
            else if (Owner != player)
            {
                player.Money -= BaseRent;
                Owner.Money += BaseRent;
                Debug.Log($"💸 {player.Name} sumoka {BaseRent}€ nuomos {Owner.Name}");
            }
            else
            {
                Debug.Log($"⚡ {player.Name} yra savo komunalinėje paslaugoje {Name}");
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

    // NEMOKAMAS POILSIS - nieko nevyksta
    public class FreeParkingTile : Tile
    {
        public FreeParkingTile(string name)
        {
            Name = name;
            Type = "free_parking";
        }

        public override void OnPlayerLand(PlayerNamespace.Player player)
        {
            Debug.Log($"🅿️ {player.Name} ilsisi nemokamame parkinge!");
        }
    }

    // EINI Į KALĖJIMĄ
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
            Debug.Log($"🚔 {player.Name} eina į kalėjimą!");

            Tile current = this.Next;
            while (current != this)
            {
                if (current is JailTile)
                {
                    player.Position = current;
                    Debug.Log($"🔒 {player.Name} perkeltas į kalėjimą!");
                    return;
                }
                current = current.Next;
            }

            Debug.LogWarning("⚠️ Kalėjimo laukelis nerastas lentoje!");
        }
    }
}