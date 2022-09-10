using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScale : MonoBehaviour {
    public SpriteRenderer outline;

	// Use this for initialization
	void Start () {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = outline.bounds.size.x / outline.bounds.size.y;
        if(screenRatio >= targetRatio){
            Camera.main.orthographicSize = outline.bounds.size.y / 2;
        }else{
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = outline.bounds.size.y / 2 * differenceInSize;
        }
	}
}