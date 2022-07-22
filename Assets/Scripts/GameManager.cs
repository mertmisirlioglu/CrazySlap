using System;
using UnityEngine;

namespace CrazySlap
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public PlayerController myPlayer;

        public PlayerController[] playerList;

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject);
            }else{
                Instance = this;
            }

        }

        public void UpdatePlayerList()
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            Debug.Log(JsonUtility.ToJson(players));
        }


        public void Slap()
        {
            myPlayer.Slap();
        }
    }
}
