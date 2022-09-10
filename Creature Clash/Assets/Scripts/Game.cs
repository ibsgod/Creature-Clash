using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

public class Game : MonoBehaviourPunCallbacks
{
    public static Game instance;
    public UnityEngine.Object ball;
    public UnityEngine.Object card;
    public List<GameObject> balls = new List<GameObject>();
    public  List<GameObject> objects = new List<GameObject>();
    public bool moved = false;
    public bool doneMove = false;
    public bool myTurn = false;
    public PhotonView pv;
    public float startTime = 0; 
    Transform hand;
    public int mana = 100;
    public GameObject selectedCard; 
    bool gameStart = false;
    public System.Random rand = new System.Random();
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> animCards = new List<GameObject>();
    public List<int> animCardPos = new List<int>();
    public float pauseTime = 0;
    public float pauseStart = -1;
    float cardStart = 0;
    public GameObject deck;
    const float cardAnimTime = 0.5f;
    const float turnTime = 10;
    TextMesh manatext;
    GameObject egg;
    public GameObject leftwall;
    public GameObject rightwall;
    public GameObject topwall;
    public GameObject bottomwall;
    public GameObject winlose;
    public bool gameOver = false;
    public AudioSource audios;
    public AudioClip error;
    public AudioClip place;
    public AudioClip bump;
    public AudioClip cardsound;
    public Collider2D clock;
    public int currCard = 0;
    public UnityEngine.Object msg;
    public GameObject waitText;
    bool started = false;
    float startTime2 = 0;
    void Start()
    {
        mana = 10;
        Debug.Log("start");
        String s = "";
        foreach(String i in Info.deck) {
            s += i + " ";
        }
        print(s);
        instance = this;
        pv = GetComponent<PhotonView>();
        clock = GameObject.Find("clock").GetComponent<Collider2D>();
        hand = GameObject.Find("clock").transform.Find("hand").transform;
        manatext = GameObject.Find("juice").transform.Find("manatext").GetComponent<TextMesh>(); 
        manatext.text = Game.instance.mana.ToString();
    }
    void RealStart() {
        float eggposy = 0;
        cardStart = Time.time;
        audios.PlayOneShot(Resources.Load("Sounds/woosh") as AudioClip);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            eggposy = 4.5f;
            myTurn = true;
            startTime = Time.time;
            GameObject.Find("botarea").SetActive(false);
            int randnum = StaticRandom.Instance.Next(1, 4);
            for (int i = 0; i < randnum; i++) {
                GameObject pondy = (GameObject) PhotonNetwork.Instantiate("pond", new Vector2(
                StaticRandom.Instance.Next((int)GameObject.Find("leftwall").transform.position.x, (int)GameObject.Find("rightwall").transform.position.x),
                StaticRandom.Instance.Next((int)GameObject.Find("bottomwall").transform.position.y, (int)GameObject.Find("topwall").transform.position.y)),
                transform.rotation);
                pondy.transform.localScale = new Vector2(StaticRandom.Instance.Next(10, 20) / 10f, StaticRandom.Instance.Next(10, 20) / 10f);
            }
        } else {
            GameObject.Find("toparea").SetActive(false); 
            eggposy = -2.5f;
            mana = 12;
        }
        while (cards.Count < 3) {
            GameObject cardy = (GameObject) GameObject.Instantiate(card, deck.transform.position, deck.transform.rotation);
            cardy.GetComponent<SummonCard>().enabled = true;
            cardy.transform.localScale = new Vector2(0.3f, 0.3f);
            cards.Add(cardy);
            animCards.Add(cardy);
            animCardPos.Add(cards.Count-1);
        }
        String type = "Egg";
        egg = (GameObject) PhotonNetwork.Instantiate("Circle", new Vector2 (0, eggposy), Quaternion.identity, 0);
        int id = egg.GetPhotonView().ViewID;
        pv.RPC("addBall", RpcTarget.AllBuffered, new object[] {type, id, PhotonNetwork.LocalPlayer.ActorNumber});
        InvokeRepeating("printDelay", 1f, 1f);
    }

    
    void printDelay() {
        // Transform hand = GameObject.Find("clock").transform.Find("hand").transform;
        // Debug.Log(hand.localEulerAngles);
        // Debug.Log((startTime - Time.time) / 5f * 360f);
    }

    public override void OnJoinedRoom() {
        // Debug.Log("joined room");
    }


    void Update()
    {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (!started && PhotonNetwork.CurrentRoom != null){
            // RealStart();
            started = true;
            return;
        }
        if (!gameStart && PhotonNetwork.PlayerList.Length > 1) {
            gameStart = true;
            startTime = Time.time;
            pauseTime = 0;
            if (PhotonNetwork.IsMasterClient) {
                waitText.GetComponent<TextMesh>().text = "Your turn";
            }
            else {
                waitText.GetComponent<TextMesh>().text = "Their turn";
            }
            waitText.GetComponent<MessageAnim>().enabled = true;
            
            RealStart();
        }
        if (gameStart && PhotonNetwork.PlayerList.Length == 1) {
            gameStart = false;
            if (!gameOver) {
                win(true);
            }
        }
        if (animCards.Count > 0) {
            bool finish = true;
            for (int i = 0; i < animCards.Count; i++) {
                float start = animCards[i].GetComponent<SummonCard>().cardStart;
                if (Time.time - start < cardAnimTime) {
                    finish = false;
                }
                animCards[i].transform.position = (new Vector2(1.3f * (animCardPos[i] - 1), -4f) * (Time.time - start) 
                + (Vector2) deck.transform.position * (cardAnimTime - (Time.time - start))) / cardAnimTime;
                animCards[i].transform.localScale = (new Vector2(1.2f, 1.2f) * (Time.time - start) 
                + new Vector2(0.3f, 0.3f) * (cardAnimTime - (Time.time - start))) / cardAnimTime;
                animCards[i].transform.localEulerAngles = new Vector3(0, 0, 360 * (Time.time - start) / cardAnimTime);
            }
            if (finish) {
                for (int i = 0; i < animCards.Count; i++) {
                    animCards[i].transform.position = new Vector2(1.3f * (animCardPos[i] - 1), -4f);
                    animCards[i].transform.localScale = new Vector2(1.2f, 1.2f);
                    animCards[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                animCards.Clear();
                animCardPos.Clear();
            }
        } 
        if (myTurn && !moved) {
            hand.localEulerAngles = new Vector3 (0, 0, -(Time.time - startTime - pauseTime) / turnTime * 360f);
        } 
        
        if (Input.GetMouseButtonDown(0) && !Game.instance.gameOver) { 
            if (!moved && myTurn && selectedCard != null) {
                bool yes = true;
                bool sound = true;
                bool yesyesyes = false;
                foreach (GameObject card in cards) {
                    if (card.GetComponent<Collider2D>().OverlapPoint(mousepos)) {
                        // print("ues");
                        yesyesyes = true;
                    }
                }
                if (PhotonNetwork.LocalPlayer.IsMasterClient) {
                    GameObject areas = GameObject.Find("toparea");
                    if (!areas.GetComponent<Collider2D>().OverlapPoint(mousepos)) {
                        yes = false;
                        if (!yesyesyes) {
                            Game.instance.createError("Not valid area!");
                        }
                    }
                } else {
                    GameObject areas = GameObject.Find("botarea");
                    if (!areas.GetComponent<Collider2D>().OverlapPoint(mousepos)) {
                        if (!yesyesyes) {
                            Game.instance.createError("Not valid area!");
                        }
                        yes = false;
                    }
                }
                GameObject area = GameObject.Find("hud");
                if (area.GetComponent<Collider2D>().OverlapPoint(mousepos)) {
                    if (!yesyesyes) {
                        Game.instance.createError("Not valid area!");
                    }
                    yes = false;
                    sound = false;
                }

                if (yes) {
                    String type = Game.instance.selectedCard.GetComponent<SummonCard>().arch;
                    if (mana >= Info.stats[type]["mana"])
                    {
                        GameObject newball = (GameObject) PhotonNetwork.Instantiate("Circle", mousepos, Quaternion.identity, 0);
                        int id = newball.GetPhotonView().ViewID;
                        pv.RPC("addBall", RpcTarget.AllBuffered, new object[] {type, id, PhotonNetwork.LocalPlayer.ActorNumber});
                        Ball archetype = (Ball)newball.GetComponent(Type.GetType(type));
                        Game.instance.mana -= Info.stats[type]["mana"];
                        GameObject cardy = (GameObject) GameObject.Instantiate(card, deck.transform.position, deck.transform.rotation);
                        cardy.GetComponent<SummonCard>().enabled = true;
                        cardy.transform.localScale = new Vector2(0.3f, 0.3f);
                        animCards.Add(cardy);
                        animCardPos.Add(cards.IndexOf(selectedCard));
                        cards[cards.IndexOf(selectedCard)] = cardy;
                        cardStart = Time.time;
                        Destroy(selectedCard);
                        selectedCard = null;
                        newball.GetComponent<Ball>().updateCC(new string[0]);
                        Game.instance.pv.RPC("playSound", RpcTarget.All, "place");
                    }
                    else {
                        yes = false;
                        Game.instance.createError("Not enough mana!");
                    }
                }
                if (!yes && sound) {
                    audios.PlayOneShot(error);
                }
            }
            bool yesyes = false;
            foreach (GameObject c in cards) {
                if (c != null && c.GetComponent<Collider2D>().OverlapPoint(mousepos)) {
                    yesyes = true;
                    break;
                }
            }
            if (!yesyes && !moved && myTurn) {
                selectedCard = null;
            }
        }
        if (!moved && myTurn && (Time.time - startTime - pauseTime >= turnTime || Input.GetMouseButtonDown(0) && clock.OverlapPoint(mousepos))) {
            if (PhotonNetwork.PlayerListOthers.Length > 0) {
                hand.localEulerAngles = new Vector3(0, 0, 0);
                pv.RPC("changeTurn", RpcTarget.All);
                GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
                GameObject arr = GameObject.Find("arrow");
                if (arr != null) {
                    GameObject.Destroy(arr);
                }
                arr = GameObject.Find("archaim");
                if (arr != null) {
                    GameObject.Destroy(arr);
                }
                for (int i = balls.Count-1; i >= 0; i--) {
                    balls[i].GetComponent<Ball>().pv.RPC("changeTurn", RpcTarget.All);
                }
                foreach (GameObject g in balls) {
                    if (g != null && g.GetPhotonView() != null) {
                        g.GetComponent<Ball>().click = false;
                        g.GetPhotonView().TransferOwnership(PhotonNetwork.PlayerListOthers[0]);
                    }
                }
                // Debug.Log(objects.Count);
                foreach (GameObject g in objects) {
                    g.GetPhotonView().TransferOwnership(PhotonNetwork.PlayerListOthers[0]);
                }
            }
        }
        if (moved) {
            bool yes = true;
            foreach (GameObject g in balls) {
                if (g != null && g.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05) {
                    yes = false;
                    break;
                }
            }
            foreach (GameObject g in objects) {
                if (g != null && g.GetComponent<Rigidbody2D>() != null && g.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05) {
                    yes = false;
                    break;
                }
            }
            if (yes) {
                audios.PlayOneShot(Resources.Load("Sounds/boopy") as AudioClip);
                moved = false;
                pauseTime += Time.time - pauseStart;
            }
        }
        manatext.text = Game.instance.mana.ToString(); 
    }
    

    [PunRPC]
    void addBall(String type, int pvid, int pid)
    {
        PhotonView pv = PhotonView.Find(pvid);
        if (pv == null) {
            return;
        }
        GameObject ball = PhotonView.Find(pvid).gameObject;
        balls.Add(ball);
        Ball archetype = (Ball)ball.AddComponent(Type.GetType(type));
        archetype.pid = pid;

        if (type != "Egg") {
            ball.transform.Find("TeamCircle").GetComponent<SpriteRenderer>().color = pid == PhotonNetwork.LocalPlayer.ActorNumber ? Color.blue : Color.red;
        }
        else {
            ball.GetComponent<SpriteRenderer>().color = pid == PhotonNetwork.LocalPlayer.ActorNumber ? Color.blue : Color.red;
            if (!PhotonNetwork.IsMasterClient) {
                ball.GetPhotonView().TransferOwnership(PhotonNetwork.PlayerListOthers[0]);
            }
        }
        archetype.Init();
    }
    [PunRPC]
    void addObject(int pvid)
    {
        GameObject obj = PhotonView.Find(pvid).gameObject;
        objects.Add(obj);
        // Debug.Log("added " + obj);
    }

    public void callAddObject (int pvid) {
        pv.RPC("addObject", RpcTarget.AllBuffered, pvid);
        // Debug.Log("calladded");
    }

    [PunRPC]
    void changeTurn() {
        audios.PlayOneShot(Resources.Load("Sounds/woosh") as AudioClip);
        myTurn = !myTurn;
        startTime = Time.time;
        if (myTurn) {
            mana += 2;
        }
        selectedCard = null;
        pauseTime = 0;
        Game.instance.doneMove = false;
        createError(myTurn? "Your turn" : "Their turn");
    }

    [PunRPC]
    void removeBalls(int pid) {
        balls.RemoveAll(item => item.GetComponent<Ball>().pid == pid);
    }

    public void refreshCards() {
        if (gameOver || !gameStart) {
            return;
        }
        if (animCards.Count > 0) {
            return;
        }
        Game.instance.mana -= 2;
        Game.instance.audios.PlayOneShot(Game.instance.cardsound);
        for (int i = 0; i < 3; i++){
            Destroy(cards[i]);
            selectedCard = null;
            GameObject cardy = (GameObject) GameObject.Instantiate(card, deck.transform.position, deck.transform.rotation);
            cardy.GetComponent<SummonCard>().enabled = true;
            cardy.transform.localScale = new Vector2(0.3f, 0.3f);
            cards[i] = cardy;
            animCards.Add(cardy);
            animCardPos.Add(i);
        }
    }

    public void lose() {
        // Debug.LogError("You lose!");
        audios.PlayOneShot(Resources.Load("Sounds/lose") as AudioClip);
        Destroy(waitText);
        winlose = (GameObject) GameObject.Instantiate(Resources.Load("winlose"), Vector3.zero, Quaternion.identity);
        gameOver = true;
        pv.RPC("win", RpcTarget.Others, false);
        if (pv.AmOwner) {
            PhotonNetwork.Destroy(pv);
        }
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void win(bool dc) {
        String str = "";    
        winlose = (GameObject) GameObject.Instantiate(Resources.Load("winlose"), Vector3.zero, Quaternion.identity);    
        TextMesh tm = winlose.transform.Find("winlosetext").GetComponent<TextMesh>();
        audios.PlayOneShot(Resources.Load("Sounds/win") as AudioClip);
        if (dc) {
            str = "Opponent disconnected";
            tm.fontSize = 59;
        }
        else {
            str = "You win!";
        }
        tm.text = str;
        // Debug.LogError("You win!");
        gameOver = true;
        tm.text = str;
        
        if (pv.AmOwner) {
            PhotonNetwork.Destroy(pv);
        }
        PhotonNetwork.LeaveRoom();
    }

    public void createError(String str) {
        GameObject bruh = ((GameObject) GameObject.Instantiate(msg, new Vector2(0, 1), Quaternion.identity));
        bruh.GetComponent<TextMesh>().text = str;
        bruh.GetComponent<MessageAnim>().enabled = true;
    }

    [PunRPC]
    public void playSound (String s) {
        if (s == "bump") {
            audios.PlayOneShot(bump);
            return;
        }
        audios.PlayOneShot(Resources.Load("Sounds/" + s) as AudioClip);
    }

}
