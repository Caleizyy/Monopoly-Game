using System.Collections.Generic;
using UnityEngine;
using BoardNamespace;

namespace BoardNamespace
{
    // Ciklinė žaidimo lenta (ciklinis vienakryptis sąrašas)
    public class GameBoard
    {
        private Tile head; // Pirmasis laukelis (startas)
        private int tileCount = 0;

        // Pridėti laukelį į pabaigą
        public void AddTile(Tile tile)
        {
            if (head == null)
            {
                // Pirmas laukelis
                head = tile;
                tile.Next = tile; // Rodo į save (ciklas)
            }
            else
            {
                // Randame paskutinį laukelį
                Tile current = head;
                while (current.Next != head)
                {
                    current = current.Next;
                }

                // Prijungiame naują laukelį
                current.Next = tile;
                tile.Next = head; // Uždarome ciklą
            }

            tileCount++;
        }

        // Pridėti laukelį į konkretią poziciją
        public void InsertTile(Tile tile, int position)
        {
            if (position < 0 || position > tileCount)
            {
                Debug.LogError("Neteisinga pozicija!");
                return;
            }

            if (position == 0)
            {
                // Įterpiame į pradžią
                if (head == null)
                {
                    head = tile;
                    tile.Next = tile;
                }
                else
                {
                    // Randame paskutinį
                    Tile last = GetTileAt(tileCount - 1);
                    tile.Next = head;
                    last.Next = tile;
                    head = tile;
                }
            }
            else
            {
                // Įterpiame į vidurį/pabaigą
                Tile previous = GetTileAt(position - 1);
                tile.Next = previous.Next;
                previous.Next = tile;
            }

            tileCount++;
            Debug.Log($"Laukelis '{tile.Name}' įterptas į poziciją {position}");
        }

        // Ištrinti laukelį pagal poziciją
        public bool RemoveTileAt(int position)
        {
            if (position < 0 || position >= tileCount || head == null)
            {
                Debug.LogError("Neteisinga pozicija arba lenta tuščia!");
                return false;
            }

            if (tileCount == 1)
            {
                // Vienintelis laukelis
                Debug.Log($"Ištrintas laukelis: {head.Name}");
                head = null;
                tileCount = 0;
                return true;
            }

            if (position == 0)
            {
                // Triname pirmą
                Tile last = GetTileAt(tileCount - 1);
                Debug.Log($"Ištrintas laukelis: {head.Name}");
                head = head.Next;
                last.Next = head;
            }
            else
            {
                // Triname iš vidurio/pabaigos
                Tile previous = GetTileAt(position - 1);
                Tile toRemove = previous.Next;
                Debug.Log($"Ištrintas laukelis: {toRemove.Name}");
                previous.Next = toRemove.Next;
            }

            tileCount--;
            return true;
        }

        // Ištrinti laukelį pagal pavadinimą
        public int RemoveTileByName(string name)
        {
            if (head == null) return 0;

            int removedCount = 0;
            int position = 0;

            // Patikriname ar yra keletas laukelių su tuo pačiu pavadinimu
            List<int> positionsToRemove = new List<int>();

            Tile current = head;
            do
            {
                if (current.Name == name)
                {
                    positionsToRemove.Add(position);
                }
                current = current.Next;
                position++;
            } while (current != head && position < tileCount);

            if (positionsToRemove.Count > 1)
            {
                Debug.LogWarning($"Rasti {positionsToRemove.Count} laukeliai vardu '{name}'!");
                Debug.LogWarning("Trinamas tik pirmas rastas laukelis.");
            }

            // Triname tik pirmą rastą
            if (positionsToRemove.Count > 0)
            {
                RemoveTileAt(positionsToRemove[0]);
                removedCount = 1;
            }
            else
            {
                Debug.LogWarning($"Laukelis '{name}' nerastas!");
            }

            return removedCount;
        }

        // Gauti laukelį pagal poziciją
        public Tile GetTileAt(int position)
        {
            if (position < 0 || position >= tileCount || head == null)
                return null;

            Tile current = head;
            for (int i = 0; i < position; i++)
            {
                current = current.Next;
            }

            return current;
        }

        // Gauti startą (pirmą laukelį)
        public Tile GetStart()
        {
            return head;
        }

        // Gauti laukelių skaičių
        public int GetTileCount()
        {
            return tileCount;
        }

        // Spausdinti visą lentą
        public void PrintBoard()
        {
            if (head == null)
            {
                Debug.Log("Lenta tuščia!");
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
            } while (current != head && index < tileCount);
        }

        // Rasti konkretų laukelio tipą (pvz., kalėjimą)
        public Tile FindTileByType(string type)
        {
            if (head == null) return null;

            Tile current = head;
            do
            {
                if (current.Type == type)
                    return current;
                current = current.Next;
            } while (current != head);

            return null;
        }

        // Iterator - judėjimas lenta
        public Tile MoveFromTile(Tile startTile, int steps)
        {
            if (startTile == null || steps <= 0) return startTile;

            Tile current = startTile;
            for (int i = 0; i < steps; i++)
            {
                current = current.Next;
            }

            return current;
        }
    }
}