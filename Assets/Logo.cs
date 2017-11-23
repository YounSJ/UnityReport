using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : MonoBehaviour {
	
	public float delayTime = 1f;

	IEnumerator Start () {
		
		yield return new WaitForSeconds (delayTime);

		Application.LoadLevel ("HowToPlay");
	}

}