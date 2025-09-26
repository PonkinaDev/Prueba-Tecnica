using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour{   

    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Transform rankingListContainer;
    [SerializeField] private RankingJsonHandler rankingJsonHandler;

    private PlayersDataList players;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        if (rankingJsonHandler == null)
        {
            if (!TryGetComponent(out rankingJsonHandler))
            {
                enabled = false;
                return;
            }
        }

        players = rankingJsonHandler.LoadPlayers();
        LoadRanking();
    }

    private void LoadRanking(){
        if(players == null || players.PlayersData == null || players.PlayersData.Count == 0){
            return;
        }

        if(playerEntryPrefab == null || rankingListContainer == null){
            return;
        }

        if(players.PlayersData.Count == 0){
            return;
        }

        List<PlayerData> sortedPlayers;

        if (players.PlayersData.Count == 1){
            sortedPlayers = players.PlayersData;
        }else{
            sortedPlayers = players.PlayersData.OrderByDescending(p => p.Score).ToList();
        }

        for(int i = 0; i < sortedPlayers.Count; i++){
            var playerData = sortedPlayers[i];
            var entry = Instantiate(playerEntryPrefab, rankingListContainer);
            TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (i + 1).ToString();
            texts[1].text = playerData.Name;
            texts[2].text = playerData.Score.ToString();
        }
    }
}
