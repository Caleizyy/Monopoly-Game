using System;
using UnityEngine;
using BoardNamespace;

public class JsonLoader : MonoBehaviour
{
    public GameBoard Board;

    [Serializable]
    public class TileData
    {
        public string name;
        public string type;
        public int price;
        public int rent;
        public int amount; // Bonusams ir mokesčiams
    }

    [Serializable]
    public class TileList
    {
        public TileData[] tiles;
    }

    void Awake()
    {
        Board = new GameBoard();
        LoadBoardFromJson();
    }

    void LoadBoardFromJson()
    {
        // Nuskaityti JSON failą iš Unity Projekto
        TextAsset json = Resources.Load<TextAsset>("Data/MonopolyBoard");

        if (json == null)
        {
            Debug.LogError("JSON failas nerastas! Patikrinkite: Resources/Data/MonopolyBoard.json");
            return;
        }

        // Prideda wrapper JSON masyvui
        string jsonText = "{\"tiles\":" + json.text + "}";
        TileList tileList = JsonUtility.FromJson<TileList>(jsonText);

        if (tileList == null || tileList.tiles == null)
        {
            Debug.LogError("Klaida skaitant JSON!");
            return;
        }

        // Sukuria laukelius pagal tipą
        foreach (var data in tileList.tiles)
        {
            Tile tile = CreateTileFromData(data);
            if (tile != null)
            {
                Board.AddTile(tile);
            }
        }

        Debug.Log($"Žaidimo lenta užkrauta! Laukelių skaičius: {Board.GetTileCount()}");
        Board.PrintBoard();
    }

    // Sukuria laukelį pagal tipą
    Tile CreateTileFromData(TileData data)
    {
        Tile tile = null;

        switch (data.type.ToLower())
        {
            case "start":
                tile = new StartTile(data.name);
                break;

            case "street":
                tile = new StreetTile(data.name, data.price, data.rent);
                break;

            case "station":
                tile = new StationTile(data.name, data.price, data.rent);
                break;

            case "utility":
                tile = new UtilityTile(data.name, data.price, data.rent);
                break;

            case "jail":
                tile = new JailTile(data.name);
                break;

            case "goto_jail":
                tile = new GoToJailTile(data.name);
                break;

            case "bonus":
                tile = new BonusTile(data.name, data.amount);
                break;

            case "tax":
                tile = new TaxTile(data.name, data.amount);
                break;

            case "free_parking":
                tile = new FreeParkingTile(data.name);
                break;

            default:
                Debug.LogWarning($"Nežinomas laukelio tipas: {data.type}");
                break;
        }

        return tile;
    }

    // Funkcija lentai redaguoti - sukurti naują laukelį
    public void CreateAndAddTile(string name, string type, int price = 0, int rent = 0, int amount = 0)
    {
        TileData data = new TileData
        {
            name = name,
            type = type,
            price = price,
            rent = rent,
            amount = amount
        };

        Tile tile = CreateTileFromData(data);
        if (tile != null)
        {
            Board.AddTile(tile);
            Debug.Log($"Sukurtas naujas laukelis: {tile.GetInfo()}");
        }
    }

    // Įterpti laukelį į konkretų vietą
    public void InsertTileAt(int position, string name, string type, int price = 0, int rent = 0, int amount = 0)
    {
        TileData data = new TileData
        {
            name = name,
            type = type,
            price = price,
            rent = rent,
            amount = amount
        };

        Tile tile = CreateTileFromData(data);
        if (tile != null)
        {
            Board.InsertTile(tile, position);
        }
    }

    // Ištrinti laukelį pagal poziciją
    public void RemoveTileAtPosition(int position)
    {
        Board.RemoveTileAt(position);
    }

    // Ištrinti laukelį pagal pavadinimą
    public void RemoveTileByName(string name)
    {
        Board.RemoveTileByName(name);
    }
}