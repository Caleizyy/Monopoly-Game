using System.Collections.Generic;
using UnityEngine;
using PlayerNamespace;
using BoardNamespace;

namespace CoreNamespace
{
    public class GameManager : MonoBehaviour
    {
        // Nuoroda į JsonLoader
        public JsonLoader jsonLoader;

        // Žaidėjai
        public List<Player> PlayersList = new List<Player>();
        private Queue<Player> PlayerQueue = new Queue<Player>();

        // Žaidimo būsena
        private bool gameStarted = false;
        private Player currentPlayer;

        // Žaidimo taisyklės
        [Header("Žaidimo taisyklės")]
        public int playerCount = 2; // 2 arba 3
        public int startingMoney = 1500;
        public int winningMoney = 3000; // Kiek reikia surinkti pergalei

        // UI
        private int lastDiceRoll = 0;

        void Start()
        {
            // Randame JsonLoader
            jsonLoader = GetComponent<JsonLoader>();
            if (jsonLoader == null)
            {
                Debug.LogError("JsonLoader nerastas! Pridėkite JsonLoader componentą.");
                return;
            }

            Debug.Log("=== MONOPOLIS ===");
            Debug.Log("Spauskite [N] pradėti naują žaidimą");
            Debug.Log("Spauskite [SPACE] mesti kauliuką");
            Debug.Log("Spauskite [B] pirkti laukelį");
            Debug.Log("Spauskite [H] pirkti pastatą");
            Debug.Log("Spauskite [Q] nutraukti žaidimą");
            Debug.Log("Spauskite [P] atspausdinti lentą");
        }

        void Update()
        {
            if (!gameStarted)
            {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    StartNewGame();
                }
                return;
            }

            // Žaidimo metu
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RollDice();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                BuyCurrentProperty();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                BuyBuilding();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                QuitGame();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                jsonLoader.Board.PrintBoard();
            }
        }

        // Pradėti naują žaidimą
        void StartNewGame()
        {
            PlayersList.Clear();
            PlayerQueue = new Queue<Player>();

            // Sukuriame žaidėjus
            string[] playerNames = { "Jonas", "Petras", "Ona" };
            for (int i = 0; i < playerCount; i++)
            {
                Player p = new Player(playerNames[i], startingMoney);
                p.Position = jsonLoader.Board.GetStart(); // Startas
                PlayersList.Add(p);
                PlayerQueue.Enqueue(p);
            }

            currentPlayer = PlayerQueue.Peek();
            gameStarted = true;

            Debug.Log($"🎮 ŽAIDIMAS PRASIDĖJO!");
            Debug.Log($"Žaidėjų skaičius: {playerCount}");
            Debug.Log($"Pergalei reikia: {winningMoney}€");
            PrintGameStatus();
        }

        // Mesti kauliuką
        void RollDice()
        {
            if (currentPlayer.IsInJail)
            {
                currentPlayer.JailTurns--;
                if (currentPlayer.JailTurns <= 0)
                {
                    currentPlayer.IsInJail = false;
                    Debug.Log($"🔓 {currentPlayer.Name} išėjo iš kalėjimo!");
                }
                else
                {
                    Debug.Log($"🔒 {currentPlayer.Name} praleidžia ėjimą kalėjime.");
                    NextTurn();
                    return;
                }
            }

            // Metame kauliuką (1-6)
            lastDiceRoll = Random.Range(1, 7);
            Debug.Log($"🎲 {currentPlayer.Name} išmetė: {lastDiceRoll}");

            // Judame
            currentPlayer.Move(lastDiceRoll);

            // Patikriname pergalės sąlygą
            CheckWinCondition();

            PrintGameStatus();
        }

        // Pirkti laukelį
        void BuyCurrentProperty()
        {
            if (currentPlayer.Position is StreetTile street)
            {
                currentPlayer.BuyProperty(street);
                PrintGameStatus();
            }
            else if (currentPlayer.Position is StationTile station)
            {
                if (currentPlayer.Money >= station.Price && station.Owner == null)
                {
                    currentPlayer.Money -= station.Price;
                    station.Owner = currentPlayer;
                    Debug.Log($"✅ {currentPlayer.Name} nusipirko {station.Name}");
                    PrintGameStatus();
                }
            }
            else if (currentPlayer.Position is UtilityTile utility)
            {
                if (currentPlayer.Money >= utility.Price && utility.Owner == null)
                {
                    currentPlayer.Money -= utility.Price;
                    utility.Owner = currentPlayer;
                    Debug.Log($"✅ {currentPlayer.Name} nusipirko {utility.Name}");
                    PrintGameStatus();
                }
            }
            else
            {
                Debug.Log("❌ Šio laukelio negalima pirkti!");
            }
        }

        // Pirkti pastatą
        void BuyBuilding()
        {
            if (currentPlayer.Position is StreetTile street)
            {
                int buildingCost = street.Price / 2; // Pastato kaina = pusė laukelio kainos
                currentPlayer.BuyBuilding(street, buildingCost);
                PrintGameStatus();
            }
            else
            {
                Debug.Log("❌ Ant šio laukelio negalima statyti pastatų!");
            }
        }

        // Kitas ėjimas
        void NextTurn()
        {
            // Grąžiname dabartinį žaidėją į eilę
            PlayerQueue.Enqueue(currentPlayer);

            // Imame kitą žaidėją
            currentPlayer = PlayerQueue.Dequeue();

            Debug.Log($"--- {currentPlayer.Name} ĖJIMAS ---");
            PrintGameStatus();
        }

        // Nutraukti žaidimą
        void QuitGame()
        {
            gameStarted = false;
            Debug.Log("⛔ Žaidimas nutrauktas!");
            Debug.Log("Spauskite [N] pradėti naują žaidimą");
        }

        // Patikrinti pergalę
        void CheckWinCondition()
        {
            if (currentPlayer.Money >= winningMoney)
            {
                Debug.Log($"🏆🏆🏆 {currentPlayer.Name} LAIMĖJO! 🏆🏆🏆");
                Debug.Log($"Galutinė suma: {currentPlayer.Money}€");
                gameStarted = false;
            }

            // Patikrinti ar žaidėjas pralaimėjo
            if (currentPlayer.IsBankrupt())
            {
                Debug.Log($"💔 {currentPlayer.Name} bankrutavo!");
                PlayersList.Remove(currentPlayer);

                if (PlayersList.Count == 1)
                {
                    Debug.Log($"🏆 {PlayersList[0].Name} LAIMĖJO (vienintelis likęs)!");
                    gameStarted = false;
                }
                else
                {
                    NextTurn();
                }
            }
        }

        // Atspausdinti žaidimo būseną
        void PrintGameStatus()
        {
            Debug.Log("=== ŽAIDĖJŲ BŪSENA ===");
            foreach (var player in PlayersList)
            {
                string marker = (player == currentPlayer) ? "👉 " : "   ";
                Debug.Log($"{marker}{player.GetStatus()} | Pozicija: {player.Position.Name}");
            }

            if (lastDiceRoll > 0)
            {
                Debug.Log($"Paskutinis kauliuko metimas: {lastDiceRoll}");
            }

            Debug.Log("=====================");
        }
    }
}