using System;
using Photon.Pun;
using UnityEngine;

namespace CrazySlap
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public PlayerController myPlayer;

        public PlayerController[] playerList;

        private PhotonView pV;

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject);
            }else{
                Instance = this;
            }

            pV = GetComponent<PhotonView>();
        }

        public void UpdatePlayerList()
        {
            this.playerList = FindObjectsOfType<PlayerController>();
            Debug.Log("playerlsit length" + this.playerList.Length);
        }

        public void CheckGameStatus()
        {
            int count = 0;
            string nickname = "";
            foreach (var player in playerList)
            {
                if (player.isAlive)
                {
                    count++;
                    nickname = player.nickname;
                }
            }

            Debug.Log("alive count " + count);
            if (count == 1)
            {
                pV.RPC(nameof(EndGame), RpcTarget.All, nickname);
            }
        }


        public void Slap()
        {
            myPlayer.Slap();
        }

        [PunRPC]
        void EndGame(string winner)
        {
            if (this.myPlayer.nickname == winner)
            {
                UIManager.Instance.Gameplay.SetActive(false);
                UIManager.Instance.WinScreen.SetActive(true);
                UIManager.Instance.SetWinnerName(winner);
            }
            Debug.Log("game bitti sen bittin");
        }
    }
}
