using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckCard : MonoBehaviour
{
    public float startTime = float.MaxValue;
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
        transform.Find("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(arch) as Sprite;
        transform.Find("juice").transform.Find("manatext").GetComponent<TextMesh>().text = Info.stats[arch]["mana"].ToString();
    }

    void Update()
    {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (!Input.GetMouseButton(0)|| !coll.OverlapPoint(mousepos)) {
            if (showing) {
                GameObject.Destroy(infobub);
            }  
            startClick = -1;
            showing = false;  
        }
        if (!DeckMaster.instance.swapping && coll.OverlapPoint(mousepos)) {
            if (Input.GetMouseButtonDown(0)) {
                startClick = Time.time;
                startTime = Time.time;
                if (DeckMaster.instance.selectedCard == DeckMaster.instance.midCard  
                && this.gameObject != DeckMaster.instance.midCard && this.gameObject != DeckMaster.instance.leftCard 
                && this.gameObject != DeckMaster.instance.rightCard) {
                    DeckMaster.instance.swap(DeckMaster.instance.selectedCard, this.gameObject);
                }
                else if (DeckMaster.instance.selectedCard != null && DeckMaster.instance.selectedCard != DeckMaster.instance.midCard &&
                DeckMaster.instance.selectedCard != DeckMaster.instance.rightCard && DeckMaster.instance.selectedCard != DeckMaster.instance.leftCard &&
                this.gameObject == DeckMaster.instance.midCard) {
                    DeckMaster.instance.swap(this.gameObject, DeckMaster.instance.selectedCard);
                }
                else if (DeckMaster.instance.rightCard == this.gameObject || DeckMaster.instance.leftCard == this.gameObject) {
                    DeckMaster.instance.dispCard += DeckMaster.instance.rightCard == this.gameObject ? 1 : -1;
                    DeckMaster.instance.changeDisp();
                }
                else {
                    DeckMaster.instance.selectedCard = this.gameObject;
                }
            }
            if (!showing && Time.time - startClick > 1 && startClick > 0) {
                DeckMaster.instance.selectedCard = null; 
                showing = true;
                infobub = (GameObject) GameObject.Instantiate(Resources.Load("infobub"), transform.position + new Vector3(0, 3, 0), transform.rotation);
                SpriteRenderer sr = infobub.GetComponent<SpriteRenderer>();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
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
                if (!(infobub.gameObject.GetComponent<Collider2D>().IsTouching(DeckMaster.instance.leftwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(DeckMaster.instance.rightwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(DeckMaster.instance.topwall.GetComponent<Collider2D>())
                || infobub.gameObject.GetComponent<Collider2D>().IsTouching(DeckMaster.instance.bottomwall.GetComponent<Collider2D>()))) {
                    infobub.transform.position = transform.position;
                }
                infobub.transform.position = new Vector2(Mathf.Min(DeckMaster.instance.rightwall.transform.position.x - infobub.transform.localScale.x  * 4f / 5, 
                    Mathf.Max(DeckMaster.instance.leftwall.transform.position.x + infobub.transform.localScale.x  * 4f / 5, infobub.transform.position.x)),
                    Mathf.Min(DeckMaster.instance.topwall.transform.position.y - infobub.transform.localScale.y  * 1, 
                    Mathf.Max(DeckMaster.instance.bottomwall.transform.position.y + infobub.transform.localScale.y  * 1, infobub.transform.position.y)));
            }
        }
        if (DeckMaster.instance.selectedCard == this.gameObject) {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else {
            GetComponent<SpriteRenderer>().color = new Color(0, 0.3f, 1, 1);
        }

    }


}
