using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Rename : MonoBehaviour {


	public tk2dSpriteCollectionData collection;

	// Use this for initialization
	void Start () 
	{
		foreach(var sprite in collection.spriteDefinitions)
        {
			sprite.name = sprite.name.Replace("guide", "lost");
			sprite.texelSize = new Vector2(0.0625f, 0.0625f);

		}

		/*foreach(string path in Directory.GetFiles("C:\\Users\\noaht\\Documents\\unity\\TK2DSpriteProject\\Assets\\LostSprites"))
        {
			//File.Move(path, path.Replace("guide", "lost"));
        }*/
	}


	// Update is called once per frame
	void Update () {
		
	}
}
