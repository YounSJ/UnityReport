using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (Random.Range (-40.0f, 40.0f), 0.52f, Random.Range (-40.0f, 40.0f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
