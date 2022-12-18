using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MakeJsons : MonoBehaviour {

	public string[] names;
	public MonoBehaviour[] monoBehaviours;

	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < monoBehaviours.Length; i++)
        {

			string path = "Assets/Resources/" + names[i] + ".txt";

			File.WriteAllText(path, UnityEngine.JsonUtility.ToJson(monoBehaviours[i], true));
		}	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
