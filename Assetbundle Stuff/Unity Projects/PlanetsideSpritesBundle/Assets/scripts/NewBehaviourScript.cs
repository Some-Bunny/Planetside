using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
	public tk2dSpriteCollectionData animationCurve;
	public TextAsset TextAsset;
	// Use this for initialization
	void Start()
	{
		JsonUtility.FromJsonOverwrite(TextAsset.text, animationCurve);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
