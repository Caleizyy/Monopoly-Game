using System.Collections.Generic;
using UnityEngine;

namespace BoardNamespace
{
    // Žaidimo lenta - ciklinis vienakryptis sąrašas
    public class GameBoard
    {
        private Tile head; // Sąrašo pradžia
        private int tileCount = 0;

        public GameBoard()
        {
            head = null;
            tileCount = 0;
        }

        // Pridėti laukelį į sąrašo pabaigą
        public void AddTile(Tile tile)
        {
            if (head == null)
            {
                // Pirmas laukelis
                head = tile;
                tile.Next = head; // Ciklas - rodo į save
            }
            else
            {
                // Randa paskutinį laukelį
                Tile current = head;
                while (current.Next != head)
                {
                    current = current.Next;
                }

                // Prijungia naują laukelį
                current.Next = tile;
                tile.Next = head; // Uždaro ciklą
            }

            tileCount++;
        }

        // Įterpti laukelį į konkretų poziciją
        public void InsertTile(Tile tile, int position)
        {
            if (position < 0)
            {
                Debug.LogError("Pozicija negali būti neigiama!");
                return;
            }

            if (position == 0 || head == null)
            {
                // Įterpia į pradžią
                if (head == null)
                {
                    head = tile;
                    tile.Next = head;
                }
                else
                {
                    // Randa paskutinį
                    Tile last = head;
                    while (last.Next != head)
                    {
                        last = last.Next;
                    }

                    tile.Next = head;
                    head = tile;
                    last.Next = head; // Paskutinis rodo į naują head
                }
                tileCount++;
                Debug.Log($"✅ Laukelis {tile.Name} įterptas į poziciją {position}");
                return;
            }

            // Įterpia į vidurį/pabaigą
            Tile current = head;
            for (int i = 0; i < position - 1 && current.Next != head; i++)
            {
                current = current.Next;
            }

            tile.Next = current.Next;
            current.Next = tile;
            tileCount++;

            Debug.Log($"✅ Laukelis {tile.Name} įterptas į poziciją {position}");
        }

        // Ištrinti laukelį pagal poziciją
        public void RemoveTileAt(int position)
        {
            if (head == null)
            {
                Debug.LogError("Lenta tuščia!");
                return;
            }

            if (position < 0 || position >= tileCount)
            {
                Debug.LogError($"Neteisinga pozicija: {position}");
                return;
            }

            // Trinti pirmą laukelį
            if (position == 0)
            {
                if (head.Next == head)
                {
                    // Vienintelis laukelis
                    Debug.Log($"🗑️ Ištrintas laukelis: {head.Name}");
                    head = null;
                }
                else
                {
                    // Randa paskutinį
                    Tile last = head;
                    while (last.Next != head)
                    {
                        last = last.Next;
                    }

                    Tile toRemove = head;
                    head = head.Next;
                    last.Next = head;
                    Debug.Log($"🗑️ Ištrintas laukelis: {toRemove.Name}");
                }
                tileCount--;
                return;
            }

            // Trinti vidurinio/paskutinio laukelio
            Tile current = head;
            for (int i = 0; i < position - 1; i++)
            {
                current = current.Next;
            }

            Tile removed = current.Next;
            current.Next = removed.Next;
            tileCount--;

            Debug.Log($"🗑️ Ištrintas laukelis: {removed.Name}");
        }

        // Ištrinti laukelį pagal pavadinimą (PIRMĄ ATITIKMENĮ)
        public void RemoveTileByName(string name)
        {
            if (head == null)
            {
                Debug.LogError("Lenta tuščia!");
                return;
            }

            // Tikrina pirmą laukelį
            if (head.Name == name)
            {
                RemoveTileAt(0);
                return;
            }

            // Ieško sąraše
            Tile current = head;
            Tile previous = null;
            int position = 0;

            do
            {
                if (current.Name == name)
                {
                    RemoveTileAt(position);
                    return;
                }
                previous = current;
                current = current.Next;
                position++;
            }
            while (current != head);

            Debug.LogWarning($"Laukelis '{name}' nerastas!");
        }

        // Gauti startinį laukelį
        public Tile GetStart()
        {
            return head;
        }

        // Gauti laukelių skaičių
        public int GetTileCount()
        {
            return tileCount;
        }

        // Rasti kalėjimo laukelį (reikės GoToJail funkcijai)
        public Tile FindJailTile()
        {
            if (head == null) return null;

            Tile current = head;
            do
            {
                if (current is JailTile)
                {
                    return current;
                }
                current = current.Next;
            }
            while (current != head);

            return null;
        }

        // Atspausdinti lentą
        public void PrintBoard()
        {
            if (head == null)
            {
                Debug.Log("❌ Lenta tuščia!");
                return;
            }

            Debug.Log("=== ŽAIDIMO LENTA ===");
            Tile current = head;
            int index = 0;

            do
            {
                Debug.Log($"[{index}] {current.GetInfo()}");
                current = current.Next;
                index++;
            }
            while (current != head);

            Debug.Log($"Iš viso laukelių: {tileCount}");
            Debug.Log("====================");
        }
    }
}