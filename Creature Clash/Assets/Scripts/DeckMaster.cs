using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckMaster : MonoBehaviour
{
    public Object card;
    public static DeckMaster instance;
    public GameObject selectedCard;
    public int dispCard = 0;
    public GameObject leftCard;
    public GameObject midCard;
    public GameObject rightCard;
    public Collider2D left;
    public Collider2D right;
    public Collider2D back;
    public bool swapping = false;
    float swapStart = 0;
    Vector2 swap1;
    Vector2 swap2;
    public GameObject swapper1;
    public GameObject swapper2;
    public float swapDur;
    public SpriteRenderer sr;
    public TextMesh cardName;
    public GameObject leftwall;
    public GameObject rightwall;
    public GameObject topwall;
    public GameObject bottomwall;
    public AudioSource audios;
    public AudioClip cardsound;
    public AudioClip boop;
    
    void Start()
    {
        instance = this;
        for (int i = 0; i < 8; i++) {
            GameObject cardy = (GameObject) GameObject.Instantiate(card, new Vector2(-2.1f + 1.4f * (i % 4), (i < 4) ? 2 : 0.3f), transform.rotation);
            DeckCard scr = cardy.GetComponent<DeckCard>();
            scr.enabled = true;
            scr.arch = Info.deck[i];
        }
        changeDisp();
        selectedCard = null;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (Input.GetMouseButtonDown(0) && left.OverlapPoint(mousepos)) {
            dispCard = Mathf.Max(0, dispCard - 1);
            changeDisp();
        }
        else if (Input.GetMouseButtonDown(0) && right.OverlapPoint(mousepos)) {
            dispCard = Mathf.Min(Info.animals.Length - 1, dispCard + 1);
            changeDisp();
        }
        else if (Input.GetMouseButtonDown(0) && back.OverlapPoint(mousepos)) {
            SceneManager.LoadScene("MenuScene");
        }
        if (swapping && Time.time - swapStart < swapDur) {
            swapper1.transform.position = swap2 * (Time.time - swapStart) / swapDur + swap1 * (swapDur - (Time.time - swapStart)) / swapDur;
            float scale = 1.2f * (Time.time - swapStart) / swapDur + 2 * (swapDur - (Time.time - swapStart)) / swapDur;
            swapper1.transform.localScale = new Vector2(scale, scale);
            swapper2.transform.position = new Vector2 (swap2.x, 10) * (Time.time - swapStart) / swapDur + swap2 * (swapDur - (Time.time - swapStart)) / swapDur;
        }
        else if (swapping) {
            audios.PlayOneShot(cardsound);
            swapper1.transform.position = swap2;
            swapper1.transform.localScale = new Vector2(1.2f, 1.2f);
            swapping = false;
            int id = (int) (((swap2.x + 2.1) / 1.4) + (swap2.y == 2 ? 0 : 4) + 0.5);
            Info.deck[id] = swapper1.GetComponent<DeckCard>().arch;
            Destroy(swapper2);
            string s = "";
            foreach(string i in Info.deck) {
                s += i + " ";
            }
            // print(s);
        }
        
    }

    public void changeDisp() {
        audios.PlayOneShot(boop);
        Destroy(midCard);
        Destroy(leftCard);
        Destroy(rightCard);
        midCard = (GameObject) GameObject.Instantiate(card, new Vector2(0, -2.8f), transform.rotation);
        midCard.transform.localScale = new Vector2(2, 2);
        DeckCard scr1 = midCard.GetComponent<DeckCard>();
        scr1.enabled = true;
        scr1.arch = Info.animals[dispCard];
        if (dispCard < Info.animals.Length-1) {
            rightCard = (GameObject) GameObject.Instantiate(card, new Vector2(1.8f, -2.8f), transform.rotation);
            DeckCard scr2 = rightCard.GetComponent<DeckCard>();
            scr2.enabled = true;
            scr2.arch = Info.animals[dispCard+1];
        }
        if (dispCard > 0) {
            leftCard = (GameObject) GameObject.Instantiate(card, new Vector2(-1.8f, -2.8f), transform.rotation);
            DeckCard scr2 = leftCard.GetComponent<DeckCard>();
            scr2.enabled = true;
            scr2.arch = Info.animals[dispCard-1];
        }
        selectedCard = midCard;
        scr1.startClick = Time.time;
        scr1.startTime = Time.time;
        cardName.text = scr1.arch;
    }

    public void swap(GameObject card1, GameObject card2) {
        selectedCard = null;
        swap1 = card1.transform.position;
        swap2 = card2.transform.position;
        swapper1 = card1;
        swapper2 = card2;
        swapStart = Time.time;
        swapping = true;
        midCard = (GameObject) GameObject.Instantiate(card, new Vector2(0, -2.8f), transform.rotation);
        midCard.transform.localScale = new Vector2(2, 2);
        DeckCard scr1 = midCard.GetComponent<DeckCard>();
        scr1.enabled = true;
        scr1.arch = Info.animals[dispCard];
        // print("swap");
    }
}
