using System;
using System.Collections.Generic;
using UnityEngine;
using BoardNamespace;

public class JsonLoader : MonoBehaviour
{
    public List<Tile> BoardTiles = new List<Tile>();

    [Serializable]
    public class TileData
    {
        public string name;
        public string type;
        public int price;
        public int rent;
    }

    [Serializable]
    public class TileList
    {
        public TileData[] tiles;
    }

    void Start()
    {
        LoadBoardFromJson();
    }

    void LoadBoardFromJson()
    {
        // 1️⃣ Nuskaityti JSON failą iš Resources/Data/MonopolyBoard.json
        TextAsset json = Resources.Load<TextAsset>("Data/MonopolyBoard");
        if (json == null)
        {
            Debug.LogError("JSON failas nerastas!");
            return;
        }

        string jsonText = "{\"tiles\":" + json.text + "}"; // JsonUtility reikia wrapper
        TileList tileList = JsonUtility.FromJson<TileList>(jsonText);

        // 2️⃣ Sukurti Tile objektus
        foreach (var data in tileList.tiles)
        {
            Tile t = new Tile();
            t.Name = data.name;
            t.Type = data.type;
            t.Price = data.price;
            t.Rent = data.rent;
            t.Buildings = new Stack<int>();
            BoardTiles.Add(t);
        }

        // 3️⃣ Sudaryti ciklinę lentą
        for (int i = 0; i < BoardTiles.Count; i++)
        {
            BoardTiles[i].Next = BoardTiles[(i + 1) % BoardTiles.Count];
        }

        Debug.Log("Žaidimo lenta sėkmingai užkrauta! Tile skaičius: " + BoardTiles.Count);
    }
}
