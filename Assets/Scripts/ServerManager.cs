using System;
using System.Collections;
using System.Collections.Generic;
using CrazySlap;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{


    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Material[] characterMaterials;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private GameObject myPlayer;
    private string nickname;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public void Connect()
    {
        this.nickname = _textMeshProUGUI.text;
        UIManager.Instance.MainMenu.SetActive(false);
        UIManager.Instance.Gameplay.SetActive(true);
        Debug.Log("my nickname is" + this.nickname);
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            // PhotonNetwork.GameVersion = Constants.GAME_VERSION;
        }
    }

    public void RandomGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void ResetGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        int index = PhotonNetwork.LocalPlayer.ActorNumber % Constants.MAX_PLAYER;
        myPlayer = PhotonNetwork.Instantiate("player", spawnPoints[index].position, Quaternion.identity);
        myPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material = characterMaterials[index];
        PhotonNetwork.LocalPlayer.NickName = this.nickname;
        GameManager.Instance.myPlayer = myPlayer.GetComponent<PlayerController>();
        CameraManager.Instance.SetFollow(myPlayer);
        GameManager.Instance.UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameManager.Instance.UpdatePlayerList();
    }
}
