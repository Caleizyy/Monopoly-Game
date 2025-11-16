#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardEditor))]
public class BoardEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BoardEditor boardEditor = (BoardEditor)target;

        // Parodome visus standartines laukus
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        // RODYTI JSONLOADER STATUS?
        var jsonLoaderField = boardEditor.GetType().GetField("jsonLoader",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var jsonLoader = jsonLoaderField?.GetValue(boardEditor) as JsonLoader;

        if (jsonLoader == null)
        {
            EditorGUILayout.HelpBox(
                "JsonLoader nerastas!\n\n" +
                "?sitikinkite kad:\n" +
                "1. JsonLoader komponentas yra ant to paties GameObject\n" +
                "2. Abu komponentai (JsonLoader ir BoardEditor) yra ant to paties objekto",
                MessageType.Error
            );
            return;
        }
        else
        {
            EditorGUILayout.HelpBox("JsonLoader rastas!", MessageType.Info);
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("VEIKSMAI", EditorStyles.boldLabel);

        // PRID?TI MYGTUKAS
        if (GUILayout.Button("PRID?TI laukel? ? pabaig?", GUILayout.Height(30)))
        {
            if (jsonLoader == null)
            {
                Debug.LogError("JsonLoader n?ra priskirtas!");
            }
            else
            {
                jsonLoader.CreateAndAddTile(
                    boardEditor.newTileName,
                    boardEditor.newTileType,
                    boardEditor.newTilePrice,
                    boardEditor.newTileRent,
                    boardEditor.newTileAmount
                );
                Debug.Log($"Prid?tas: {boardEditor.newTileName} ({boardEditor.newTileType})");
            }
        }

        // ?TERPTI MYGTUKAS
        if (GUILayout.Button($"?TERPTI laukel? ? pozicij? [{boardEditor.insertPosition}]", GUILayout.Height(30)))
        {
            if (jsonLoader == null)
            {
                Debug.LogError("? JsonLoader n?ra priskirtas!");
            }
            else
            {
                if (boardEditor.insertPosition < 0)
                {
                    jsonLoader.CreateAndAddTile(
                        boardEditor.newTileName,
                        boardEditor.newTileType,
                        boardEditor.newTilePrice,
                        boardEditor.newTileRent,
                        boardEditor.newTileAmount
                    );
                }
                else
                {
                    jsonLoader.InsertTileAt(
                        boardEditor.insertPosition,
                        boardEditor.newTileName,
                        boardEditor.newTileType,
                        boardEditor.newTilePrice,
                        boardEditor.newTileRent,
                        boardEditor.newTileAmount
                    );
                }
                Debug.Log($"?terptas: {boardEditor.newTileName}");
            }
        }

        EditorGUILayout.Space(5);

        // IŠTRINTI PAGAL POZICIJ?
        if (GUILayout.Button($"IŠTRINTI laukel? pozicijoje [{boardEditor.deletePosition}]", GUILayout.Height(30)))
        {
            if (jsonLoader == null)
            {
                Debug.LogError("JsonLoader n?ra priskirtas!");
            }
            else
            {
                jsonLoader.RemoveTileAtPosition(boardEditor.deletePosition);
            }
        }

        // IŠTRINTI PAGAL PAVADINIM?
        if (GUILayout.Button($"IŠTRINTI laukel? \"{boardEditor.deleteName}\"", GUILayout.Height(30)))
        {
            if (jsonLoader == null)
            {
                Debug.LogError("JsonLoader n?ra priskirtas!");
            }
            else if (string.IsNullOrEmpty(boardEditor.deleteName))
            {
                Debug.LogError("?veskite laukelio pavadinim?!");
            }
            else
            {
                jsonLoader.RemoveTileByName(boardEditor.deleteName);
            }
        }

        EditorGUILayout.Space(5);

        // ATSPAUSDINTI
        if (GUILayout.Button("ATSPAUSDINTI lent?", GUILayout.Height(30)))
        {
            if (jsonLoader == null)
            {
                Debug.LogError("JsonLoader n?ra priskirtas!");
            }
            else
            {
                jsonLoader.Board.PrintBoard();
            }
        }

        EditorGUILayout.Space(10);

        // INFO
        EditorGUILayout.HelpBox(
            "INSTRUKCIJOS:\n\n" +
            "1. Užpildyk laukus viršuje (pavadinimas, tipas, kaina...)\n" +
            "2. Spausk mygtuk? atlikti veiksmui\n\n" +
            "LAUKELI? TIPAI:\n" +
            "• start - Startas\n" +
            "• street - Gatv? (reikia price, rent)\n" +
            "• station - Stotis (reikia price, rent)\n" +
            "• utility - Komunalin?s (reikia price, rent)\n" +
            "• jail - Kal?jimas\n" +
            "• goto_jail - Eini ? kal?jim?\n" +
            "• bonus - Loterija (reikia amount)\n" +
            "• tax - Mokes?iai (reikia amount)\n" +
            "• free_parking - Nemokamas poilsis",
            MessageType.Info
        );
    }
}
#endif