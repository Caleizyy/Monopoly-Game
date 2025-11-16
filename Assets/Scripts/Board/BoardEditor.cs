using UnityEngine;
using BoardNamespace;

public class BoardEditor : MonoBehaviour
{
    [Header("Priklausomyb?s")]
    [Tooltip("Automatiškai randamas JsonLoader (ant to paties GameObject)")]
    [SerializeField] private JsonLoader jsonLoader;

    void Awake()
    {
        if (jsonLoader == null)
        {
            jsonLoader = GetComponent<JsonLoader>();
        }

        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader nerastas! ?sitikinkite kad JsonLoader komponentas yra ant to paties GameObject.");
        }
    }

    [Header("PRID?TI NAUJ? LAUKEL?")]
    [Tooltip("Naujo laukelio pavadinimas")]
    public string newTileName = "Naujas laukelis";

    [Tooltip("Laukelio tipas: start, street, station, utility, jail, goto_jail, bonus, tax, free_parking")]
    public string newTileType = "street";

    [Tooltip("Kaina (street, station, utility)")]
    public int newTilePrice = 100;

    [Tooltip("Nuoma (street, station, utility)")]
    public int newTileRent = 20;

    [Tooltip("Suma (bonus, tax)")]
    public int newTileAmount = 0;

    [Header("?TERPTI ? POZICIJ?")]
    [Tooltip("Pozicija kur ?terpti (0 = pradžia, -1 = pabaiga)")]
    public int insertPosition = -1;

    [Header("IŠTRINTI LAUKEL?")]
    [Tooltip("Ištrinti pagal pozicij? (0, 1, 2...)")]
    public int deletePosition = 0;

    [Tooltip("Ištrinti pagal pavadinim?")]
    public string deleteName = "";

    [Header("VEIKSMAI")]
    [Tooltip("Spausk kad prid?tum laukel? ? pabaig?")]
    public bool addTileToEnd = false;

    [Tooltip("Spausk kad ?terptum laukel? ? pozicij?")]
    public bool insertTileAtPosition = false;

    [Tooltip("Spausk kad ištrintum pagal pozicij?")]
    public bool deleteTileByPosition = false;

    [Tooltip("Spausk kad ištrintum pagal pavadinim?")]
    public bool deleteTileByName = false;

    [Tooltip("Spausk kad atspausdintum lent?")]
    public bool printBoard = false;

    void Update()
    {
        // PRID?TI ? PABAIG?
        if (addTileToEnd)
        {
            addTileToEnd = false;
            AddTileToEnd();
        }

        // ?TERPTI ? POZICIJ?
        if (insertTileAtPosition)
        {
            insertTileAtPosition = false;
            InsertTileAt();
        }

        // IŠTRINTI PAGAL POZICIJ?
        if (deleteTileByPosition)
        {
            deleteTileByPosition = false;
            DeleteTileByPosition();
        }

        // IŠTRINTI PAGAL PAVADINIM?
        if (deleteTileByName)
        {
            deleteTileByName = false;
            DeleteTileByName();
        }

        // ATSPAUSDINTI LENT?
        if (printBoard)
        {
            printBoard = false;
            PrintBoard();
        }
    }

    void AddTileToEnd()
    {
        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader n?ra priskirtas! Priskirkite Inspector lange.");
            return;
        }

        jsonLoader.CreateAndAddTile(newTileName, newTileType, newTilePrice, newTileRent, newTileAmount);
        Debug.Log($"Prid?tas: {newTileName} ({newTileType})");
    }

    void InsertTileAt()
    {
        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader n?ra priskirtas!");
            return;
        }

        if (insertPosition < 0)
        {
            // -1 reiškia pabaig?
            AddTileToEnd();
            return;
        }

        jsonLoader.InsertTileAt(insertPosition, newTileName, newTileType, newTilePrice, newTileRent, newTileAmount);
        Debug.Log($"?terptas: {newTileName} ? pozicij? {insertPosition}");
    }

    void DeleteTileByPosition()
    {
        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader n?ra priskirtas!");
            return;
        }

        jsonLoader.RemoveTileAtPosition(deletePosition);
    }

    void DeleteTileByName()
    {
        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader n?ra priskirtas!");
            return;
        }

        if (string.IsNullOrEmpty(deleteName))
        {
            Debug.LogError("?veskite laukelio pavadinim?!");
            return;
        }

        jsonLoader.RemoveTileByName(deleteName);
    }

    void PrintBoard()
    {
        if (jsonLoader == null)
        {
            Debug.LogError("JsonLoader n?ra priskirtas!");
            return;
        }

        jsonLoader.Board.PrintBoard();
    }
}