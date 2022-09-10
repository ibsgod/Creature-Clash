using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0.0.0";
    [SerializeField] private string nickName = "Punfish";
    
    // Start is called before the first frame update
    private void Start() {
        PhotonNetwork.NickName = nickName + Random.Range(0, 9999).ToString();
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster() {
        // print("Connected");
        // print(PhotonNetwork.LocalPlayer.NickName);
        SceneManager.LoadScene("MenuScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // print("Disconnected from server for reason " + cause.ToString());
    }
}
