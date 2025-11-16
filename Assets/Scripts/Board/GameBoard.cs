using System.Collections.Generic;
using UnityEngine;

namespace BoardNamespace
{

    public class GameBoard
    {
        private LinkedList<Tile> tiles = new LinkedList<Tile>();

        public GameBoard()
        {
            tiles = new LinkedList<Tile>();
        }

        // Pridėti laukelį į sąrašo pabaigą
        public void AddTile(Tile tile)
        {
            tiles.AddLast(tile);
            UpdateCyclicLinks();
        }

        // Įterpti laukelį į konkretų poziciją
        public void InsertTile(Tile tile, int position)
        {
            if (position < 0)
            {
                Debug.LogError("Pozicija negali būti neigiama!");
                return;
            }

            if (position == 0 || tiles.Count == 0)
            {
                tiles.AddFirst(tile);
            }
            else if (position >= tiles.Count)
            {
                tiles.AddLast(tile);
            }
            else
            {
                // Randame mazgą pozicijoje
                LinkedListNode<Tile> node = GetNodeAt(position);
                tiles.AddBefore(node, tile);
            }

            UpdateCyclicLinks();
            Debug.Log($"Laukelis {tile.Name} įterptas į poziciją {position}");
        }

        // Ištrinti laukelį pagal poziciją
        public void RemoveTileAt(int position)
        {
            if (tiles.Count == 0)
            {
                Debug.LogError("Lenta tuščia!");
                return;
            }

            if (position < 0 || position >= tiles.Count)
            {
                Debug.LogError($"Neteisinga pozicija: {position}");
                return;
            }

            LinkedListNode<Tile> node = GetNodeAt(position);
            string name = node.Value.Name;
            tiles.Remove(node);

            UpdateCyclicLinks();
            Debug.Log($"Ištrintas laukelis: {name}");
        }

        // Ištrinti laukelį pagal pavadinimą (PIRMĄ ATITIKMENĮ)
        public void RemoveTileByName(string name)
        {
            if (tiles.Count == 0)
            {
                Debug.LogError("Lenta tuščia!");
                return;
            }

            LinkedListNode<Tile> current = tiles.First;
            int position = 0;

            while (current != null)
            {
                if (current.Value.Name == name)
                {
                    tiles.Remove(current);
                    UpdateCyclicLinks();
                    Debug.Log($"Ištrintas laukelis: {name} (pozicija {position})");
                    return;
                }
                current = current.Next;
                position++;
            }

            Debug.LogWarning($"Laukelis '{name}' nerastas!");
        }

        // Gauti startinį laukelį
        public Tile GetStart()
        {
            if (tiles.Count == 0) return null;
            return tiles.First.Value;
        }

        // Gauti laukelių skaičių
        public int GetTileCount()
        {
            return tiles.Count;
        }

        // Rasti kalėjimo laukelį
        public Tile FindJailTile()
        {
            LinkedListNode<Tile> current = tiles.First;

            while (current != null)
            {
                if (current.Value is JailTile)
                {
                    return current.Value;
                }
                current = current.Next;
            }

            return null;
        }

        private LinkedListNode<Tile> GetNodeAt(int position)
        {
            LinkedListNode<Tile> current = tiles.First;
            for (int i = 0; i < position && current != null; i++)
            {
                current = current.Next;
            }
            return current;
        }

        private void UpdateCyclicLinks()
        {
            if (tiles.Count == 0) return;

            LinkedListNode<Tile> current = tiles.First;

            // Einame per visus mazgus ir nustatome Next rodykles
            while (current != null)
            {
                if (current.Next != null)
                {
                    current.Value.Next = current.Next.Value;
                }
                else
                {
                    current.Value.Next = tiles.First.Value;
                }

                current = current.Next;
            }
        }

        // Atspausdinti lentą
        public void PrintBoard()
        {
            if (tiles.Count == 0)
            {
                Debug.Log("Lenta tuščia!");
                return;
            }

            Debug.Log("=== ŽAIDIMO LENTA ===");
            LinkedListNode<Tile> current = tiles.First;
            int index = 0;

            while (current != null)
            {
                Debug.Log($"[{index}] {current.Value.GetInfo()}");
                current = current.Next;
                index++;
            }

            Debug.Log($"Iš viso laukelių: {tiles.Count}");
            Debug.Log("====================");
        }
    }
}