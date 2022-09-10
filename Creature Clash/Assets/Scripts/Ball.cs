using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Ball : MonoBehaviour
{
    public float maxhp;
    public float hp;
    public float atk;
    public Rigidbody2D rb;
    public float spd = 20;
    public bool click = false;
    public Vector2 startDrag;
    public GameObject arrobj;
    public Sprite arrowsprite;
    public SpriteRenderer sr;
    public SpriteRenderer arrsr;
    public PhotonView pv;
    public GameObject lastcoll;
    public bool startMove = false;
    public int pid;
    public Ball archscript;
    public float lastClick = -1;
    public CircleCollider2D coll;
    public bool canMove = true;
    public float angle = 0;
    public Sprite aimsprite;
    public Transform hbtrans;
    public AudioSource audios;
    public Dictionary<string, int> cc = new Dictionary<string, int>
    {
        ["poison"] = 0,
        ["heal"] = 0,
        ["paralyze"] = 0
    };
    public AudioClip bumpsound;
    public AudioClip die;
    List<GameObject> statuses = new List<GameObject>();
    

    public virtual void Init() {
        hp = Info.stats[GetType().Name]["hp"];
        maxhp = hp;
        atk = Info.stats[GetType().Name]["atk"];
        spd = Info.stats[GetType().Name]["spd"];
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
        arrowsprite = Resources.Load<Sprite>("arrow") as Sprite;
        transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
        sr.sprite = Resources.Load<Sprite>(this.GetType().Name) as Sprite;
        aimsprite = Resources.Load<Sprite>("archaim") as Sprite;
        hbtrans = transform.Find("HealthBar");
        audios = GetComponent<AudioSource>();
        bumpsound = Resources.Load("Sounds/bump") as AudioClip;
        die = Resources.Load("Sounds/die") as AudioClip;
        
    }
    void Update()
    {   
        if (Game.instance.gameOver) {
            return;
        }
        if (pv.IsMine) {
            move();
            if (startMove && rb.velocity.magnitude > 0.05) {
                Game.instance.moved = true;
                Game.instance.pauseStart = Time.time;
                startMove = false;
            }
        }
        if (!(hbtrans.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.leftwall.GetComponent<Collider2D>())
        || hbtrans.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.leftwall.GetComponent<Collider2D>())
        || hbtrans.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.leftwall.GetComponent<Collider2D>())
        || hbtrans.gameObject.GetComponent<Collider2D>().IsTouching(Game.instance.leftwall.GetComponent<Collider2D>()))) {
            hbtrans.localPosition = new Vector2(0, 0.8f);
        }
        hbtrans.position = new Vector2(Mathf.Min(Game.instance.rightwall.transform.position.x - 1, 
            Mathf.Max(Game.instance.leftwall.transform.position.x + 1, hbtrans.position.x)),
            Mathf.Min(Game.instance.topwall.transform.position.y - 0.5f, 
            Mathf.Max(Game.instance.bottomwall.transform.position.y + 0.5f, hbtrans.position.y)));
    }
    public virtual bool beforeHit (GameObject collball) {
        return true;
    }
    public virtual void extraHit(GameObject collball) {

    }

    public virtual void move() {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (!Game.instance.moved && coll.OverlapPoint(mousepos) && Input.GetMouseButtonDown(0) && pid == PhotonNetwork.LocalPlayer.ActorNumber) {
            if (cc["paralyze"] == 0) {
                click = true;
                if (Time.time - lastClick < 0.5 && lastClick > 0) 
                {
                    click = false;
                    doubleClick();
                    lastClick = -1;
                } else {
                    lastClick = Time.time;
                }
                if (canMove && !Game.instance.doneMove) {
                    startDrag = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
                    arrobj = new GameObject("arrow");
                    arrsr = arrobj.AddComponent<SpriteRenderer>();
                    arrsr.sprite = arrowsprite;
                    arrobj.transform.position = transform.position;
                    arrsr.drawMode = SpriteDrawMode.Sliced;
                    arrsr.enabled = false;
                    arrsr.sortingLayerName = "UI";
                    arrsr.sortingOrder = 1;
                }
            }
            else {
                audios.PlayOneShot(Game.instance.error);
                Game.instance.createError("Paralyzed!");
            }

        }
        beforeMove();
        if (canMove && !Game.instance.doneMove && Game.instance.myTurn) {
            if (click && arrobj != null) { 
                Vector2 currpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
                Vector2 diff = startDrag - currpos;
                diff = new Vector2(diff.x, diff.y);
                angle = 0;
                if (!(diff.x == 0 && diff.y == 0)) {
                    arrsr.enabled = true;
                    angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90;
                }
                arrobj.transform.eulerAngles = new Vector3(0, 0, angle);
                if (diff.x * diff.x + diff.y * diff.y > 25) {
                    diff = new Vector2(5, 0);
                }
                arrsr.size = new Vector2(0.6f, diff.magnitude / 4);
                arrsr.color = new Color(255, 255, 255, 0.5f);
            }
            Vector2 endDrag = startDrag - (Vector2) Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            if (Input.GetMouseButtonUp(0) && click) {
                if (endDrag.magnitude > 1) {
                    Game.instance.mana -= 2;
                    startMove = true;
                    // Game.instance.doneMove = true;
                    rb.AddForce(endDrag * spd);
                }
                click = false;
                Destroy(arrobj);
                arrobj = null;
            }
        }
        pondSlow();
        
    }

    public virtual void doubleClick() {
        
    }

    public virtual void beforeMove() {

    }

    public void OnCollisionEnter2D(Collision2D coll) {   
        if (pv != null && pv.IsMine) {    
            GameObject collball = coll.collider.gameObject;
            if (lastcoll == null || !collball.Equals(lastcoll)) {
                lastcoll = collball;
                if (collball.tag == "Player") {
                    Game.instance.pv.RPC("playSound", RpcTarget.All, "bump");
                    if (collball.GetComponent<Ball>().pid != pid) {
                        if (beforeHit(collball)) {
                            pv.RPC("getHit", RpcTarget.All, collball.GetComponent<Ball>().atk);
                        }
                    }
                    extraHit(collball);
                }
            }
        }
    }

    public void OnCollisionExit2D(Collision2D coll) {
        lastcoll = null;
    }

    [PunRPC]
    public virtual void getHit(float dmg) {
        // Debug.LogError("got hit");
        hp = Mathf.Min(maxhp, Mathf.Max(0, hp - dmg));
        hbtrans.transform.Find("Bar").transform.localScale = new Vector3(hp / maxhp, 1, 1);
        if (hp <= 0) {
            // Debug.LogError("hi");
            Game.instance.pv.RPC("playSound", RpcTarget.All, "die");
            pv.RPC("removeBall", RpcTarget.All);
            if (pv.IsMine) {
                // Debug.LogError("bye");
                PhotonNetwork.Destroy(pv);
            }
        }
    }
    
    void setScript (Ball script) {
        archscript = script;
    }

    [PunRPC]
    public void removeBall() {
        Game.instance.balls.Remove(this.gameObject);
    }

    [PunRPC]
    public virtual void changeTurn() {
        updateCC(new string[0]);
        if (arrobj != null) {
            Destroy(arrobj);
            arrobj = null;
        }
        changeTurnExtra();
    }

    public virtual void changeTurnExtra() {

    }

    [PunRPC]
    public void updateCC(string[] newcc) {
        for (int i = statuses.Count-1; i >= 0; i--) {
            Destroy(statuses[i]);
            statuses.Remove(statuses[i]);
        }
        if (newcc.Length > 0) {
            cc[newcc[0].ToString()] = int.Parse(newcc[1]); 
        } 
        else {
            if (cc["poison"] > 0) {            
                getHit(2f);
                cc["poison"] -= 1;
                audios.PlayOneShot(Resources.Load("Sounds/poison") as AudioClip);
            }
            if (cc["heal"] > 0) {            
                getHit(-2f);
                cc["heal"] -= 1;
                audios.PlayOneShot(Resources.Load("Sounds/heal") as AudioClip);
            } 
            if (cc["paralyze"] > 0) {            
                cc["paralyze"] -= 1;
                audios.PlayOneShot(Resources.Load("Sounds/paralyze") as AudioClip);
            } 
        }
        int ccs = 0;
        foreach(var item in cc){
            if (item.Value > 0) {
                GameObject circ = (GameObject) GameObject.Instantiate(Resources.Load("cc"), hbtrans.position + new Vector3(-.38f + 0.17f * ccs, .14f, 0), Quaternion.identity, hbtrans);
                circ.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(item.Key) as Sprite;
                statuses.Add(circ);
                ccs += 1;
            }
        }
    }


    public virtual void slow(float coef) {
        rb.velocity *= coef;
    }

    public virtual void pondSlow() {
        bool yes = false;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("pond")) {
            if (coll.IsTouching(p.GetComponent<PolygonCollider2D>())) {
                yes = true;
                break;
            }
        }
        if (yes) {
            slow(0.997f);
        }
    }
}
