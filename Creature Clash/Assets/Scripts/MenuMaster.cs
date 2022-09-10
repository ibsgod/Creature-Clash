using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuMaster : MonoBehaviour
{
    public GameObject img;
    // Start is called before the first frame update
    void Start()
    {
        // print(PhotonNetwork.NickName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(GameObject g) {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("ooga", roomOptions, TypedLobby.Default);
        g.GetComponent<TextMeshProUGUI>().text = "Joining...";
        PhotonNetwork.LoadLevel("GameScene");
    }
    public void Deck() {
        SceneManager.LoadScene("DeckScene");
    }
    public void Image() {
        img.SetActive(true);
    }
    public void noImage() {
        img.SetActive(false);
    }

}
