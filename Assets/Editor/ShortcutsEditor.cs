using UnityEditor;
using UnityEngine;
using System.IO;

public class ShortcutsEditor
{
    [MenuItem("Joc/Obrir Carpeta de Saves")]
    public static void OpenSavesFolder()
    {
        // Utilitzem Application.persistentDataPath perquè és multiplataforma,
        // però hi afegim la subcarpeta /saves/ que hem creat al SaveSystem.
        string path = Path.Combine(Application.persistentDataPath, "saves");

        if (Directory.Exists(path))
        {
            EditorUtility.RevealInFinder(path);
        }
        else
        {
            Debug.LogWarning("La carpeta de saves encara no existeix. Guarda la partida primer!");
            // Opcional: Obrir la carpeta arrel si la de saves no existeix
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}