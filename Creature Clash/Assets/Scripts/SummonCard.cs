using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SummonCard : MonoBehaviour
{
    float startTime = float.MaxValue;
    Collider2D coll;
    public string arch;
    public float startClick = -1;
    public bool showing = false;
    public GameObject infobub;
    public float cardStart;
    
    void Awake() {
        cardStart = Time.time;
    }
    void Start()
    {
        coll = GetComponent<Collider2D>();
        arch = Info.deck[Game.instance.currCard % Info.deck.Length];
        Game.instance.currCard += 1;
        transform.Find("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(arch) as Sprite;
        transform.Find("juice").transform.Find("manatext").GetComponent<TextMesh>().text = Info.stats[arch]["mana"].ToString();
        
    }

    void Update()
    {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (Game.instance.gameOver) {
            return;
        }
        if (Input.GetMouseButtonUp(0) || !coll.OverlapPoint(mousepos)) {
                if (showing) {
                    GameObject.Destroy(infobub);
                }  
                startClick = -1;
                showing = false;   
            }
        if (coll.OverlapPoint(mousepos))  {
            if (Input.GetMouseButtonDown(0) && !Game.instance.animCards.Contains(this.gameObject)) {
                if (Game.instance.myTurn) {
                    startClick = Time.time;
                    startTime = Time.time;
                    Game.instance.selectedCard = this.gameObject;
                }
            }
            if (!showing && Time.time - startClick > 1 && startClick > 0) {
                showing = true;
                infobub = (GameObject) GameObject.Instantiate(Resources.Load("infobub"), transform.position + new Vector3(0, 3, 0), transform.rotation);
                infobub.transform.Find("nametext").GetComponent<TextMesh>().text = arch;
                infobub.transform.Find("hptext").GetComponent<TextMesh>().text = Info.stats[arch]["hp"].ToString();
                infobub.transform.Find("atktext").GetComponent<TextMesh>().text = Info.stats[arch]["atk"].ToString();
                infobub.transform.Find("spdtext").GetComponent<TextMesh>().text = Info.stats[arch]["spd"].ToString();
                String passtext = "";
                String[] pass1 = ((String)Info.desc[arch]).Split(' ');
                int currlength = 0;
                for (int i = 0; i < pass1.Length; i++) {
                    if (pass1[i][0] == '\n') {
                        currlength = 0;
                    }
                    else if (currlength + pass1[i].Length > 25) {
                        passtext += "\n";
                        currlength = 0;
                    }
                    passtext += pass1[i];
                    passtext += " ";
                    currlength += pass1[i].Length + 1;
                }
                infobub.transform.Find("passivetext").GetComponent<TextMesh>().text = passtext;
                if (!(infobub.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.leftwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.rightwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.topwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.bottomwall.GetComponent<Collider2D>()))) {
                    infobub.transform.position = transform.position + new Vector3(0, 3, 0);
                }
                infobub.transform.position = new Vector2(Mathf.Min(Game.instance.rightwall.transform.position.x - infobub.transform.localScale.x  * 4f / 5, 
                    Mathf.Max(Game.instance.leftwall.transform.position.x + infobub.transform.localScale.x  * 4f / 5, infobub.transform.position.x)),
                    Mathf.Min(Game.instance.topwall.transform.position.y - infobub.transform.localScale.y  * 1, 
                    Mathf.Max(Game.instance.bottomwall.transform.position.y + infobub.transform.localScale.y  * 1, infobub.transform.position.y)));
            }
        }
        if (Game.instance.selectedCard == this.gameObject) {
            // lighty.intensity = Mathf.Min(Mathf.Max(0, (Time.time - startTime)*2.5f), 1.5f);
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else {
            GetComponent<SpriteRenderer>().color = new Color(0, 0.3f, 1, 1);
        }

    }


}
