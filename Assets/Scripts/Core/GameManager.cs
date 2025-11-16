using System.Collections.Generic;
using UnityEngine;
using PlayerNamespace;
using BoardNamespace;

namespace CoreNamespace
{
    public class GameManager : MonoBehaviour
    {
        // Nuoroda į JsonLoader
        private JsonLoader jsonLoader;

        // Žaidėjai
        public List<Player> PlayersList = new List<Player>();
        private Queue<Player> PlayerQueue = new Queue<Player>();

        // Žaidimo būsena
        private bool gameStarted = false;
        private Player currentPlayer;
        private bool hasRolled = false; // Ar šiame ėjime jau metė kauliuką

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
            Debug.Log("\n📖 ŽAIDIMO TAISYKLĖS:");
            Debug.Log("  1️⃣ [SPACE] - Mesti kauliuką ir judėti");
            Debug.Log("  2️⃣ [B] - Pirkti laukelį (PO judėjimo, jei nori)");
            Debug.Log("  3️⃣ [H] - Pirkti pastatą (PO judėjimo, ant savo gatvės)");
            Debug.Log("  4️⃣ [D] - Nugriauti pastatą ir atgauti 50% pinigų");
            Debug.Log("  5️⃣ [SPACE] - Mesti kitą kauliuką (automatiškai kitas žaidėjas)");
            Debug.Log("\n  [P] - Atspausdinti lentą");
            Debug.Log("  [Q] - Nutraukti žaidimą");
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

            if (Input.GetKeyDown(KeyCode.D))
            {
                DemolishBuilding();
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
                p.Position = jsonLoader.Board.GetStart();
                PlayersList.Add(p);
                PlayerQueue.Enqueue(p);
            }
            currentPlayer = PlayerQueue.Dequeue();
            hasRolled = false;
            gameStarted = true;

            Debug.Log($"\n🎮 ŽAIDIMAS PRASIDĖJO!");
            Debug.Log($"Žaidėjų skaičius: {playerCount}");
            Debug.Log($"Pergalei reikia: {winningMoney}€");
            PrintGameStatus();
            Debug.Log("\n💡 Spausk [SPACE] mesti kauliuką!");
        }

        // Mesti kauliuką
        void RollDice()
        {
            if (hasRolled)
            {
                Debug.Log($"⏭️  {currentPlayer.Name} baigia ėjimą. Kitas žaidėjas...\n");
                NextTurn();
                return;
            }

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
            Debug.Log($"\n🎲 {currentPlayer.Name} išmetė: {lastDiceRoll}");
            hasRolled = true;
            currentPlayer.Move(lastDiceRoll);
            CheckWinCondition();
            PrintGameStatus();
            Debug.Log("\n💡 Gali pirkti [B], statyti [H], griauti [D], arba spausk [SPACE] baigti ėjimą");
        }

        // Pirkti laukelį
        void BuyCurrentProperty()
        {
            if (!hasRolled)
            {
                Debug.Log("⚠️ Pirma reikia mesti kauliuką [SPACE] ir atsistoti ant laukelio!");
                return;
            }

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
                else if (station.Owner != null)
                {
                    Debug.Log($"❌ {station.Name} jau turi savininką!");
                }
                else
                {
                    Debug.Log($"❌ Neužtenka pinigų!");
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
                else if (utility.Owner != null)
                {
                    Debug.Log($"❌ {utility.Name} jau turi savininką!");
                }
                else
                {
                    Debug.Log($"❌ Neužtenka pinigų!");
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
            if (!hasRolled)
            {
                Debug.Log("⚠️ Pirma reikia mesti kauliuką [SPACE] ir atsistoti ant laukelio!");
                return;
            }

            if (currentPlayer.Position is StreetTile street)
            {
                int buildingCost = street.Price / 2;
                currentPlayer.BuyBuilding(street, buildingCost);
                PrintGameStatus();
            }
            else
            {
                Debug.Log("❌ Ant šio laukelio negalima statyti pastatų!");
            }
        }

        void DemolishBuilding()
        {
            if (!hasRolled)
            {
                Debug.Log("⚠️ Pirma reikia mesti kauliuką [SPACE] ir atsistoti ant laukelio!");
                return;
            }

            if (currentPlayer.Position is StreetTile street)
            {
                if (street.Owner != currentPlayer)
                {
                    Debug.Log($"❌ {currentPlayer.Name} nevaldo {street.Name}!");
                    return;
                }

                if (street.Buildings.Count == 0)
                {
                    Debug.Log($"❌ {street.Name} neturi pastatų!");
                    return;
                }

                // Nugriauti viršutinį pastatą (LIFO - paskutinis įdėtas, pirmas išimtas)
                int buildingRent = street.Buildings.Pop();
                int refund = (street.Price / 2) / 2; // Grąžiname 50% pastato kainos
                currentPlayer.Money += refund;

                Debug.Log($"🔨 {currentPlayer.Name} nugriovė pastatą ant {street.Name}");
                Debug.Log($"💰 Grąžinta: {refund}€ (50% pastato kainos)");
                Debug.Log($"📊 Liko pastatų: {street.Buildings.Count}");

                PrintGameStatus();
            }
            else
            {
                Debug.Log("❌ Ant šio laukelio nėra pastatų!");
            }
        }

        // Kitas ėjimas
        void NextTurn()
        {
            // Grąžiname dabartinį žaidėją į eilę
            PlayerQueue.Enqueue(currentPlayer);

            // Imame kitą žaidėją
            currentPlayer = PlayerQueue.Dequeue();
            hasRolled = false; // Naujas ėjimas - gali mesti kauliuką

            Debug.Log($"--- {currentPlayer.Name} ĖJIMAS ---");
            PrintGameStatus();
            Debug.Log("💡 Spausk [SPACE] mesti kauliuką");
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
                Debug.Log($"\n🏆🏆🏆 {currentPlayer.Name} LAIMĖJO! 🏆🏆🏆");
                Debug.Log($"Galutinė suma: {currentPlayer.Money}€");
                gameStarted = false;
                return;
            }

            // Patikrinti ar žaidėjas pralaimėjo
            if (currentPlayer.IsBankrupt())
            {
                Debug.Log($"💔 {currentPlayer.Name} bankrutavo!");
                PlayersList.Remove(currentPlayer);

                if (PlayersList.Count == 1)
                {
                    Debug.Log($"\n🏆 {PlayersList[0].Name} LAIMĖJO (vienintelis likęs)!");
                    gameStarted = false;
                }
                else
                {
                    // Nebandome Enqueue bankrutavusio
                    currentPlayer = PlayerQueue.Dequeue();
                    hasRolled = false;
                    Debug.Log($"\n--- {currentPlayer.Name} ĖJIMAS ---");
                    PrintGameStatus();
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