using System.Collections.Generic;
using UnityEngine;
using PlayerNamespace; // kad galėtume naudoti Player klasę

namespace CoreNamespace
{
    public class GameManager : MonoBehaviour
    {
        public List<Player> PlayersList = new List<Player>();
        private Queue<Player> PlayerQueue = new Queue<Player>();

        void Start()
        {
            // 1️⃣ Sukuriame žaidėjus
            PlayersList.Add(new Player("Jonas", 1500));
            PlayersList.Add(new Player("Petras", 1500));

            // 2️⃣ Įdedame juos į eilę
            foreach (var player in PlayersList)
            {
                PlayerQueue.Enqueue(player);
            }

            Debug.Log("Žaidėjai sukurti ir įdėti į eilę!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextTurn();
            }
        }

        // 3️⃣ Funkcija, kuri iškviečia kitą žaidėjo ėjimą
        public void NextTurn()
        {
            if (PlayerQueue.Count == 0) return;

            Player currentPlayer = PlayerQueue.Dequeue();

            Debug.Log(currentPlayer.Name + " dabar ėjo!");

            // čia ateityje bus metamas kauliukas ir judėjimas

            PlayerQueue.Enqueue(currentPlayer); // grąžiname į eilę
        }
    }
}
