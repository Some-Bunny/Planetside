
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;

namespace Planetside
{

	public static class VoidMuncher
	{
		public class VoidMuncherController : MonoBehaviour
        {
			public void Start()
            {
				shrineSelf = this.GetComponent<SimpleShrine>();
				GameObject bubble = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
				MeshRenderer rend = bubble.GetComponent<MeshRenderer>();
				rend.allowOcclusionWhenDynamic = true;
				bubble.transform.position = this.transform.position + new Vector3(1, 1.5f, 0);
				bubble.transform.localScale = Vector3.one;
				bubble.name = "yes";
				bubble.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
				bubble.transform.localScale *= Full() == false? 0.75f : 0;
				BigCoolBubble = bubble;
				
				if (base.gameObject != null)
				{
					tk2dBaseSprite sprite = base.gameObject.GetComponent<tk2dBaseSprite>();	
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					sprite.usesOverrideMaterial = true;
					mat.mainTexture = sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 70);
					sprite.renderer.material = mat;

				}
			}
			public bool Full()
            {return CrossGameDataStorage.CrossGameStorage.primaryGunSaved != string.Empty && CrossGameDataStorage.CrossGameStorage.secondaryGunSaved != string.Empty;}

			public void TossObjectIntoVoid(tk2dBaseSprite spriteSource, Vector3 startPosition)
			{
				base.StartCoroutine(this.HandleObjectPotToss(spriteSource, startPosition));
			}


			public void ProcessDevouredGun(Gun gun)
            {
				string primaryDisplayName = gun.encounterTrackable.journalData.GetPrimaryDisplayName(false);
				if (CrossGameDataStorage.CrossGameStorage.primaryGunSaved == string.Empty) 
				{
					CrossGameDataStorage.CrossGameStorage.primaryGunSaved = primaryDisplayName;
					CrossGameDataStorage.UpdateConfiguration();
					return;
				}
				if (CrossGameDataStorage.CrossGameStorage.secondaryGunSaved == string.Empty)
				{
					CrossGameDataStorage.CrossGameStorage.secondaryGunSaved = primaryDisplayName;
					CrossGameDataStorage.UpdateConfiguration();
					return;
				}
			}

			private IEnumerator HandleObjectPotToss(tk2dBaseSprite spriteSource, Vector3 startPosition)
			{
				shrineSelf.instanceRoom.DeregisterInteractable(shrineSelf);
				GameManager.Instance.StartCoroutine(LerpBubbleToSize(Vector3.one * 0.75f, Vector3.one * 2.5f, 1.25f));
				GameObject fakeObject = new GameObject("cauldron temp object");
				tk2dSprite sprite = tk2dBaseSprite.AddComponent<tk2dSprite>(fakeObject, spriteSource.Collection, spriteSource.spriteId);
				sprite.HeightOffGround = 2f;
				sprite.PlaceAtPositionByAnchor(startPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				Vector3 endPosition = BigCoolBubble.transform.position;
				float duration = 1.25f;
				float elapsed = 0f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime;
					float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
					Vector3 targetPosition = Vector3.Lerp(startPosition, endPosition, t);
					sprite.PlaceAtPositionByAnchor(targetPosition, tk2dBaseSprite.Anchor.MiddleCenter);
					sprite.UpdateZDepth();
					yield return null;
				}
				AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);
				AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);
				AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);
				AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);

				GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
				tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
				objanimator.ignoreTimeScale = true;
				objanimator.AlwaysIgnoreTimeScale = true;
				objanimator.AnimateDuringBossIntros = true;
				objanimator.alwaysUpdateOffscreen = true;
				objanimator.playAutomatically = true;
				ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
				var main = objparticles.main;
				main.useUnscaledTime = true;
				GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, BigCoolBubble.transform.position, Quaternion.identity);
				UnityEngine.Object.Destroy(blankObj, 2f);
				UnityEngine.Object.Destroy(fakeObject);
				yield return new WaitForSeconds(0.25f);
				shrineSelf.instanceRoom.RegisterInteractable(shrineSelf);
				if (Full() == true)
                {
					GameManager.Instance.StartCoroutine(LerpBubbleToSize(Vector3.one * 2.5f, Vector3.one * 0f, 0.25f));
				}
				else
                {
					GameManager.Instance.StartCoroutine(LerpBubbleToSize(Vector3.one * 2.5f, Vector3.one * 0.75f, 0.25f));
				}

				yield break;
			}




			public void Update()
            {
				if (this.gameObject && shrineSelf != null)
                {
					if (Full() == true)
                    {
						shrineSelf.text = "The portal has closed, and no more weapons can be deposited.";
                    }
					else if (CrossGameDataStorage.CrossGameStorage.secondaryGunSaved == string.Empty && CrossGameDataStorage.CrossGameStorage.primaryGunSaved != string.Empty)
                    {
						shrineSelf.text = "The portal has room for one more gun.";
					}
					else
                    {
						shrineSelf.text = "A small, controlled portal between worlds. You feel like you should toss something in...";
					}
				}
            }



			/*
			GameManager.Instance.StartCoroutine(LerpToSize(Vector3.one * 2, Vector3.zero, 0.9f));
            base.StartCoroutine(LerpShaderValue(0.1f, 0.00f, 0.8f, "_OutlineWidth"));
            base.StartCoroutine(LerpShaderValue(45f, 6f, 0.3f, "_OutlinePower"));
			*/
			public IEnumerator LerpBubbleShaderValue(float prevSize, float afterSize, float duration, string KeyWord)
			{
				float elaWait = 0f;
				float duraWait = duration;
				while (elaWait < duraWait)
				{
					elaWait += BraveTime.DeltaTime;
					float t = elaWait / duraWait;
					if (BigCoolBubble == null) { yield break; }
					if (BigCoolBubble != null)
					{
						BigCoolBubble.GetComponent<MeshRenderer>().material.SetFloat(KeyWord, Mathf.Lerp(prevSize, afterSize, t));
					}
					yield return null;
				}
				yield break;
			}
			public IEnumerator LerpBubbleToSize(Vector3 prevSize, Vector3 afterSize, float duration)
			{
				if (BigCoolBubble != null)
				{
					BigCoolBubble.transform.localScale = prevSize;
					float elaWait = 0f;
					float duraWait = duration;
					while (elaWait < duraWait)
					{
						elaWait += BraveTime.DeltaTime;
						float t = elaWait / duraWait;
						float t1 = Mathf.Sin(t * (Mathf.PI / 2));
						if (BigCoolBubble == null) { yield break; }
						if (BigCoolBubble != null)
						{
							BigCoolBubble.transform.localScale = Vector3.Lerp(prevSize, afterSize, t1);
						}
						yield return null;
					}
				}
				yield break;
			}



			public RoomHandler currentRoom;
			public SimpleShrine shrineSelf;
			public GameObject BigCoolBubble;
        }

		public static void Add()
		{
			ShrineFactory iei = new ShrineFactory
			{
				name = "VoidMuncher",
				modID = "psog",
				text = "A small, controlled portal between worlds. You feel like you should toss something in...",
				spritePath = "Planetside/Resources/Shrines/voidGunmuncher.png",
				acceptText = "Offer your held gun to it.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				shadowPath = "Planetside/Resources/Shrines/voidGunmunchershadow.png",
				ShadowOffsetX = 0.125f,
				ShadowOffsetY = -0.25f,
				CanUse = CanUse,
				colliderSize = new IntVector2(32, 32),
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 0, 0),
				isToggle = false,
				isBreachShrine = false,
				AdditionalComponent = typeof(VoidMuncherController),
				RoomIconSpritePath = "Planetside/Resources/Shrines/voidMuncherRoomIcon.png"
			};
			iei.Build();
			Actions.OnRunStart += OnRunStart;

		}

		public static void OnRunStart(PlayerController self, PlayerController player2, GameManager.GameMode gameMode)
		{
			GameManager.Instance.StartCoroutine(DelaySpawn(self, gameMode));  
		}

		public static IEnumerator DelaySpawn(PlayerController player, GameManager.GameMode gameMode)
		{

            float d = 1.75f;
            if (gameMode != GameManager.GameMode.NORMAL) { d = 1f; }
            yield return new WaitForSeconds(d);

            Vector3 offset = new Vector3(0.5f, 1.5f, 0);
            if (gameMode != GameManager.GameMode.NORMAL) { offset = new Vector3(0.5f, -3f, 0); }
            if (CrossGameDataStorage.CrossGameStorage.primaryGunSaved != string.Empty && CrossGameDataStorage.CrossGameStorage.secondaryGunSaved != string.Empty)
            {
                PickupObject pickup = PickupObjectDatabase.GetByEncounterName(UnityEngine.Random.value > 0.5f ? CrossGameDataStorage.CrossGameStorage.primaryGunSaved : CrossGameDataStorage.CrossGameStorage.secondaryGunSaved);
                if (pickup == null) { pickup = PickupObjectDatabase.GetById(GTEE.fuckinGhELL); }

                GameObject bubble = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
                MeshRenderer rend = bubble.GetComponent<MeshRenderer>();
                rend.allowOcclusionWhenDynamic = true;
                bubble.transform.position = player.transform.position + offset;
                bubble.transform.localScale = Vector3.one;
                bubble.name = "yes";
                bubble.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                bubble.transform.localScale *= 0;
                GameManager.Instance.StartCoroutine(LerpBubbleToSize(bubble, Vector3.zero, Vector3.one * 2.5f, 1.5f, pickup));
                CrossGameDataStorage.CrossGameStorage.primaryGunSaved = string.Empty;
                CrossGameDataStorage.CrossGameStorage.secondaryGunSaved = string.Empty;
                CrossGameDataStorage.UpdateConfiguration();
            }
        }


            public static IEnumerator LerpBubbleToSize(GameObject bubble, Vector3 prevSize, Vector3 afterSize, float duration, PickupObject pickup)
		{
			if (bubble != null)
			{
				var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
				partObj.transform.position = bubble.transform.position;
				partObj.transform.localScale *= 4f;
				UnityEngine.Object.Destroy(partObj, 3.5f);

				AkSoundEngine.PostEvent("Play_PortalOpen", bubble.gameObject);
				bubble.transform.localScale = prevSize;
				float elaWait = 0f;
				float duraWait = duration;
				while (elaWait < duraWait)
				{
					elaWait += BraveTime.DeltaTime;
					float t = elaWait / duraWait;
					float t1 = Mathf.Sin(t * (Mathf.PI / 2));
					if (bubble == null) { yield break; }
					if (bubble != null)
					{
						bubble.transform.localScale = Vector3.Lerp(prevSize, afterSize, t1);
					}
					yield return null;
				}
				DebrisObject debris = LootEngine.SpawnItem(pickup.gameObject, bubble.transform.position + new Vector3(-1, 0.5f), Vector2.zero, 0, true, true);
				
				GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
				tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
				objanimator.ignoreTimeScale = true;
				objanimator.AlwaysIgnoreTimeScale = true;
				objanimator.AnimateDuringBossIntros = true;
				objanimator.alwaysUpdateOffscreen = true;
				objanimator.playAutomatically = true;
				ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
				var main = objparticles.main;
				main.useUnscaledTime = true;
				GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, debris.transform.position, Quaternion.identity);
				UnityEngine.Object.Destroy(blankObj, 2f);
				elaWait = 0f;
				duraWait = (duration/3f);
				while (elaWait < duraWait)
				{
					elaWait += BraveTime.DeltaTime;
					float t = elaWait / duraWait;
					float t1 = Mathf.Sin(t * (Mathf.PI / 2));
					if (bubble == null) { yield break; }
					if (bubble != null)
					{
						bubble.transform.localScale = Vector3.Lerp(afterSize, prevSize, t1);
					}
					yield return null;
				}

			}
			yield break;
		}

		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			if (player.CurrentGun.InfiniteAmmo == true) { return false; }
			if (player.CurrentGun.quality == PickupObject.ItemQuality.EXCLUDED) { return false; }
			if (player.CurrentGun.quality == PickupObject.ItemQuality.SPECIAL) { return false; }
			return CrossGameDataStorage.CrossGameStorage.primaryGunSaved == string.Empty || CrossGameDataStorage.CrossGameStorage.secondaryGunSaved == string.Empty;
		}
		public static void Accept(PlayerController player, GameObject shrine)
		{
			shrine.GetComponent<VoidMuncherController>().ProcessDevouredGun(player.CurrentGun);
			shrine.GetComponent<VoidMuncherController>().TossObjectIntoVoid(player.CurrentGun.GetSprite(), player.CenterPosition);
			player.inventory.DestroyCurrentGun();
		}
	}
}



