using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class pressStartEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Text t = GetComponent<Text>();
        t.DOFade(0.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
