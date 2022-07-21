using System;
using System.Collections;
using System.Collections.Generic;
using CrazySlap;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{


    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Connect();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
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
        GameObject player = PhotonNetwork.Instantiate("player", spawnPoints[index].position, Quaternion.identity);
        PhotonNetwork.LocalPlayer.NickName = UnityEngine.Random.Range(0, 500).ToString();
        CameraManager.Instance.SetFollow(player.transform);
    }

}
