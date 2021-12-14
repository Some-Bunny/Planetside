using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;
using Pathfinding;


namespace Planetside
{
	public class Step2 : BraveBehaviour
	{
		public void Start()
		{
			base.aiActor.SetIsFlying(true, "wing");
            base.aiActor.RegisterOverrideColor(GhostColor, "oiled");

            float fard = UnityEngine.Random.Range(1, 25);
			if (fard == 1)
            {
				if (base.aiActor != null && !base.aiActor.healthHaver.IsBoss)
				{
					GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(base.aiActor));
					base.aiActor.EraseFromExistence(true);
				}
			}
		}
		public void Update()
		{

		}
        private IEnumerator HandleEnemySuck(AIActor target)
        {
            float oddset = UnityEngine.Random.Range(5, -5);
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            target.RegisterOverrideColor(GhostColor, "oiled");

            float elapsed = 0f;
            float duration = UnityEngine.Random.Range(3, 8);
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag3 = copySprite;
                if (flag3)
                {
                    Vector3 position = startPosition+ new Vector3(oddset, 30);
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    Vector3 pos = Vector3.Lerp(startPosition, position, t);
                    float ? startLifetime = new float?(UnityEngine.Random.Range(0.3f, 2f));


                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag4 = copySprite;
            if (flag4)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            yield break;
        }
        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        public Color GhostColor = new Color(6f, 1f, 2f, 0.25f);
		public float minimumHealth;
		public float CheatDeath = 20f;
	}
}