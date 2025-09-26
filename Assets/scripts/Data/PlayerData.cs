using System.Collections.Generic;

// Represents the data for a single player.
[System.Serializable]
public class PlayerData
{
// The name of the player.

    public string Name;


// The score of the player.
    public int Score;
}


// Contains a list of player data.

[System.Serializable]
public class PlayersDataList
{

// List of all players' data.

    public List<PlayerData> PlayersData;
}