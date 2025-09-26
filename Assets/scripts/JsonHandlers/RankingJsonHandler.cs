using System.IO;
using UnityEngine;

/// <summary>
/// Handles loading and saving of player ranking data in JSON format.
/// </summary>
public class RankingJsonHandler : JsonHandler
{
    /// <summary>
    /// Loads the list of player data from persistent storage.
    /// </summary>
    /// <returns>PlayersDataList loaded from JSON.</returns>
    public PlayersDataList LoadPlayers()
    {
        return LoadJsonToGame<PlayersDataList>();
    }

    // Método para guardar toda la lista de jugadores (comentado por ahora)
    // public void SavePlayers(PlayersDataList playersDataList){
    //     SaveDataToJson(playersDataList);
    // }

    /// <summary>
    /// Adds a new player to the ranking, sorts by score, keeps top 10, and saves.
    /// </summary>
    /// <param name="newPlayer">New player data to add.</param>
    public void SavePlayerData(PlayerData newPlayer)
    {
        PlayersDataList dataList;

        // Leer el JSON actual si existe, si no, crear una nueva lista
        if (File.Exists(persistentPath))
        {
            string existingJson = File.ReadAllText(persistentPath);
            dataList = JsonUtility.FromJson<PlayersDataList>(existingJson);

            // Si la lista es nula, inicializarla
            if (dataList == null || dataList.PlayersData == null)
                dataList = new PlayersDataList();
        }
        else
        {
            dataList = new PlayersDataList();
        }

        // Agregar el nuevo jugador
        dataList.PlayersData.Add(newPlayer);

        // Ordenar la lista por Score descendente
        dataList.PlayersData.Sort((a, b) => b.Score.CompareTo(a.Score));

        // Mantener solo los 10 mejores
        if (dataList.PlayersData.Count > 10)
            dataList.PlayersData = dataList.PlayersData.GetRange(0, 10);

        // Guardar la lista actualizada en JSON
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(persistentPath, json);
    }

    /// <summary>
    /// Validación después de cargar los datos.
    /// </summary>
    protected override bool OnAfterLoad<T>(T data)
    {
        // Verifica que los datos cargados sean válidos
        if (data is not PlayersDataList list || list == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validación antes de guardar los datos.
    /// </summary>
    protected override bool OnBeforeSave<T>(T data)
    {
        // Verifica que los datos a guardar sean válidos
        if (data is not PlayersDataList list || list == null)
        {
            return false;
        }
        return true;
    }
}
