using System;
using System.Collections.Generic;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Linq;
using System.Text;
using Planetside;


namespace GungeonAPI
{
	public class ShrineFactory
	{
		public static void Init()
		{
			bool initialized = ShrineFactory.m_initialized;
			bool flag = !initialized;
			if (flag)
			{
				DungeonHooks.OnFoyerAwake += ShrineFactory.PlaceBreachShrines;
				DungeonHooks.OnPreDungeonGeneration += delegate (LoopDungeonGenerator generator, Dungeon dungeon, DungeonFlow flow, int dungeonSeed)
				{
					bool flag2 = flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer;
					bool flag3 = flag2;
					if (flag3)
					{
						foreach (ShrineFactory.CustomShrineController customShrineController in UnityEngine.Object.FindObjectsOfType<ShrineFactory.CustomShrineController>())
						{
							bool flag4 = !ShrineFakePrefab.IsFakePrefab(customShrineController);
							bool flag5 = flag4;
							if (flag5)
							{
								UnityEngine.Object.Destroy(customShrineController.gameObject);
							}
						}
						ShrineFactory.m_builtShrines = false;
					}
				};
				ShrineFactory.m_initialized = true;
			}
		}



		public class ShrineShadowHandler : MonoBehaviour
		{
			public ShrineShadowHandler()
			{
				this.shadowObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("DefaultShadowSprite"));
				this.Offset = new Vector2(0, 0);
			}

			public void Start()
			{
				GameObject shadowObj = (GameObject)UnityEngine.Object.Instantiate(shadowObject);
				shadowObj.transform.parent = base.gameObject.transform;
				tk2dSprite shadowSprite = shadowObj.GetComponent<tk2dSprite>();
				shadowSprite.renderer.enabled = true;
				shadowSprite.HeightOffGround = base.gameObject.GetComponent<tk2dSprite>().HeightOffGround - 0.1f;
				shadowObj.transform.position.WithZ(base.gameObject.transform.position.z + 99999f);
				shadowObj.transform.position = base.gameObject.transform.position + Offset;
				DepthLookupManager.ProcessRenderer(shadowObj.GetComponent<Renderer>(), DepthLookupManager.GungeonSortingLayer.BACKGROUND);
				shadowSprite.usesOverrideMaterial = true;
				shadowSprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
				shadowSprite.renderer.material.SetFloat("_Fade", 0.66f);
			}


			public Vector3 Offset;
			public GameObject shadowObject;
		}

		public static Dictionary<string, GameObject> registeredShrines = new Dictionary<string, GameObject>();

		public GameObject Build()
		{
			GameObject result;
			try
			{
				Texture2D textureFromResource = ResourceExtractor.GetTextureFromResource(this.spritePath);
				GameObject gameObject = SpriteBuilder.SpriteFromResource(this.spritePath, null, false);
				string text = (this.modID + ":" + this.name).ToLower().Replace(" ", "_");
				//string roomPath = this.roomPath;
				gameObject.name = text;
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.IsPerpendicular = true;
				component.PlaceAtPositionByAnchor(this.offset, tk2dBaseSprite.Anchor.LowerCenter);
				Transform transform = new GameObject("talkpoint").transform;
				transform.position = gameObject.transform.position + this.talkPointOffset;
				transform.SetParent(gameObject.transform);
				bool flag = !this.usesCustomColliderOffsetAndSize;
				bool flag2 = flag;
				if (flag2)
				{
					IntVector2 intVector = new IntVector2(textureFromResource.width, textureFromResource.height);
					this.colliderOffset = new IntVector2(0, 0);
					this.colliderSize = new IntVector2(intVector.x, intVector.y / 2);
				}
				SpeculativeRigidbody speculativeRigidbody = component.SetUpSpeculativeRigidbody(this.colliderOffset, this.colliderSize);


				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.PlayerBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.BulletBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyBulletBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.BeamBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				ShrineFactory.CustomShrineController customShrineController = gameObject.AddComponent<ShrineFactory.CustomShrineController>();
				customShrineController.ID = text;
				customShrineController.roomStyles = this.roomStyles;
				customShrineController.isBreachShrine = true;
				customShrineController.offset = this.offset;
				customShrineController.pixelColliders = speculativeRigidbody.specRigidbody.PixelColliders;
				customShrineController.factory = this;
				customShrineController.OnAccept = this.OnAccept;
				customShrineController.OnDecline = this.OnDecline;
				customShrineController.CanUse = this.CanUse;
				customShrineController.text = this.text;
				customShrineController.acceptText = this.acceptText;
				customShrineController.declineText = this.declineText;

				bool flag3 = this.interactableComponent != null;
				bool flag4 = flag3;

				if (shadowPath != null)
                {
					GameObject shadowObject = SpriteBuilder.SpriteFromResource(shadowPath, null, false);
					shadowObject.name = "Shadow_" + name;
					tk2dSprite orAddComponent3 = shadowObject.GetOrAddComponent<tk2dSprite>();
					orAddComponent3.SetSprite(orAddComponent3.spriteId);
					FakePrefab.MarkAsFakePrefab(shadowObject);
					ShrineShadowHandler orAddComponent4 = gameObject.gameObject.GetOrAddComponent<ShrineShadowHandler>();
					orAddComponent4.shadowObject = orAddComponent3.gameObject;
					orAddComponent4.Offset = new Vector3(ShadowOffsetX, ShadowOffsetY);
				}
				IPlayerInteractable item;
				if (flag4)
				{
					item = (gameObject.AddComponent(this.interactableComponent) as IPlayerInteractable);
				}
				else
				{
					SimpleShrine simpleShrine = gameObject.AddComponent<SimpleShrine>();
					simpleShrine.isToggle = this.isToggle;
					simpleShrine.OnAccept = this.OnAccept;
					simpleShrine.OnDecline = this.OnDecline;
					simpleShrine.CanUse = this.CanUse;
					simpleShrine.text = this.text;
					simpleShrine.acceptText = this.acceptText;
					simpleShrine.declineText = this.declineText;
					simpleShrine.talkPoint = transform;
					item = simpleShrine;
				}
				GameObject gameObject2 = ShrineFakePrefab.Clone(gameObject);
				gameObject2.GetComponent<ShrineFactory.CustomShrineController>().Copy(customShrineController);
				gameObject2.name = text;
				bool flag5 = this.isBreachShrine;
				bool flag6 = flag5;
				if (flag6)
				{
					bool flag7 = !RoomHandler.unassignedInteractableObjects.Contains(item);
					bool flag8 = flag7;
					if (flag8)
					{
						RoomHandler.unassignedInteractableObjects.Add(item);
					}
				}
				else
				{
					bool flag9 = !this.room;
					bool flag10 = flag9;
					if (flag10)
					{
						this.room = RoomFactory.CreateEmptyRoom(12, 12);
					}
					ShrineFactory.RegisterShrineRoom(gameObject2, this.room, text, this.offset, this.RoomWeight);
				}
				ShrineFactory.registeredShrines.Add(text, gameObject2);
				result = gameObject;
			}
			catch (Exception e)
			{
				Tools.PrintException(e, "FF0000");
				result = null;
			}
			return result;
		}

		public GameObject BuildWithAnimations(string[] idlePaths, int idleFPS, string[] usePaths, int useFPS)
		{
			GameObject result;
			try
			{
				Texture2D textureFromResource = ResourceExtractor.GetTextureFromResource(this.spritePath);
				GameObject gameObject = SpriteBuilder.SpriteFromResource(this.spritePath, null, false);
				string text = (this.modID + ":" + this.name).ToLower().Replace(" ", "_");
				//string roomPath = this.roomPath;
				gameObject.name = text;
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.IsPerpendicular = true;
				component.PlaceAtPositionByAnchor(this.offset, tk2dBaseSprite.Anchor.LowerCenter);


				tk2dSpriteCollectionData SpriteObjectSpriteCollection = SpriteBuilder.ConstructCollection(gameObject, (name + "_Collection"));
				int spriteID = SpriteBuilder.AddSpriteToCollection(idlePaths[0], SpriteObjectSpriteCollection);
				tk2dSprite sprite = gameObject.GetOrAddComponent<tk2dSprite>();
				sprite.SetSprite(SpriteObjectSpriteCollection, spriteID);


				tk2dSpriteAnimator animator = gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
				tk2dSpriteAnimation animation = gameObject.AddComponent<tk2dSpriteAnimation>();
				animation.clips = new tk2dSpriteAnimationClip[0];
				animator.Library = animation;

				List<tk2dSpriteAnimationClip> clips = new List<tk2dSpriteAnimationClip>();
				if (idlePaths.Length >= 1)
				{
					tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = idleFPS };
					List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
					for (int i = 0; i < idlePaths.Length; i++)
					{
						tk2dSpriteCollectionData collection = SpriteObjectSpriteCollection;
						int frameSpriteId = SpriteBuilder.AddSpriteToCollection(idlePaths[i], collection);
						tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
						frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
						frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
					}
					idleClip.frames = frames.ToArray();
					idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
					animator.Library.clips = animation.clips.Concat(new tk2dSpriteAnimationClip[] { idleClip }).ToArray();
					animator.playAutomatically = true;
					animator.DefaultClipId = animator.GetClipIdByName("idle");
					clips.Add(idleClip);
					tk2dSpriteAnimationClip[] array = clips.ToArray();
					animator.Library.clips = array;
					animator.playAutomatically = true;
					animator.DefaultClipId = animator.GetClipIdByName("idle");
				}
				if (usePaths != null)
				{
					tk2dSpriteAnimation breakAnimation = gameObject.AddComponent<tk2dSpriteAnimation>();
					breakAnimation.clips = new tk2dSpriteAnimationClip[0];
					tk2dSpriteAnimationClip breakClip = new tk2dSpriteAnimationClip() { name = "use", frames = new tk2dSpriteAnimationFrame[0], fps = useFPS };
					List<tk2dSpriteAnimationFrame> breakFrames = new List<tk2dSpriteAnimationFrame>();
					for (int i = 0; i < usePaths.Length; i++)
					{
						tk2dSpriteCollectionData collection = SpriteObjectSpriteCollection;
						int frameSpriteId = SpriteBuilder.AddSpriteToCollection(usePaths[i], collection);
						tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
						frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
						breakFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
					}
					breakClip.frames = breakFrames.ToArray();
					breakClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
					clips.Add(breakClip);
					tk2dSpriteAnimationClip[] array = clips.ToArray();
					animator.Library.clips = array;
					animator.playAutomatically = true;
					animator.DefaultClipId = animator.GetClipIdByName("idle");
				}



				Transform transform = new GameObject("talkpoint").transform;
				transform.position = gameObject.transform.position + this.talkPointOffset;
				transform.SetParent(gameObject.transform);
				bool flag = !this.usesCustomColliderOffsetAndSize;
				bool flag2 = flag;
				if (flag2)
				{
					IntVector2 intVector = new IntVector2(textureFromResource.width, textureFromResource.height);
					this.colliderOffset = new IntVector2(0, 0);
					this.colliderSize = new IntVector2(intVector.x, intVector.y / 2);
				}
				SpeculativeRigidbody speculativeRigidbody = component.SetUpSpeculativeRigidbody(this.colliderOffset, this.colliderSize);


				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.PlayerBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.BulletBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyBulletBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				speculativeRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.BeamBlocker,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = this.colliderOffset.x,
					ManualOffsetY = this.colliderOffset.y,
					ManualWidth = this.colliderSize.x,
					ManualHeight = this.colliderSize.y,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				ShrineFactory.CustomShrineController customShrineController = gameObject.AddComponent<ShrineFactory.CustomShrineController>();
				customShrineController.ID = text;
				customShrineController.roomStyles = this.roomStyles;
				customShrineController.isBreachShrine = true;
				customShrineController.offset = this.offset;
				customShrineController.pixelColliders = speculativeRigidbody.specRigidbody.PixelColliders;
				customShrineController.factory = this;
				customShrineController.OnAccept = this.OnAccept;
				customShrineController.OnDecline = this.OnDecline;
				customShrineController.CanUse = this.CanUse;
				customShrineController.text = this.text;
				customShrineController.acceptText = this.acceptText;
				customShrineController.declineText = this.declineText;



				if (shadowPath != null)
				{
					GameObject shadowObject = SpriteBuilder.SpriteFromResource(shadowPath, null, false);
					shadowObject.name = "Shadow_" + name;
					tk2dSprite orAddComponent3 = shadowObject.GetOrAddComponent<tk2dSprite>();
					orAddComponent3.SetSprite(orAddComponent3.spriteId);
					FakePrefab.MarkAsFakePrefab(shadowObject);
					ShrineShadowHandler orAddComponent4 = gameObject.gameObject.GetOrAddComponent<ShrineShadowHandler>();
					orAddComponent4.shadowObject = orAddComponent3.gameObject;
					orAddComponent4.Offset = new Vector3(ShadowOffsetX, ShadowOffsetY);
				}

				bool flag3 = this.interactableComponent != null;
				bool flag4 = flag3;

				IPlayerInteractable item;
				if (flag4)
				{
					item = (gameObject.AddComponent(this.interactableComponent) as IPlayerInteractable);
				}
				else
				{
					SimpleShrine simpleShrine = gameObject.AddComponent<SimpleShrine>();
					simpleShrine.isToggle = this.isToggle;
					simpleShrine.OnAccept = this.OnAccept;
					simpleShrine.OnDecline = this.OnDecline;
					simpleShrine.CanUse = this.CanUse;
					simpleShrine.text = this.text;
					simpleShrine.acceptText = this.acceptText;
					simpleShrine.declineText = this.declineText;
					simpleShrine.talkPoint = transform;
					item = simpleShrine;
				}

				if (AdditionalComponent != null)
                {
					gameObject.AddComponent(this.AdditionalComponent);
				}
				GameObject gameObject2 = ShrineFakePrefab.Clone(gameObject);
				gameObject2.GetComponent<ShrineFactory.CustomShrineController>().Copy(customShrineController);
				gameObject2.name = text;
				bool flag5 = this.isBreachShrine;
				bool flag6 = flag5;
				if (flag6)
				{
					bool flag7 = !RoomHandler.unassignedInteractableObjects.Contains(item);
					bool flag8 = flag7;
					if (flag8)
					{
						RoomHandler.unassignedInteractableObjects.Add(item);
					}
				}
				else
				{
					bool flag9 = !this.room;
					bool flag10 = flag9;
					if (flag10)
					{
						this.room = RoomFactory.CreateEmptyRoom(12, 12);
					}
					ShrineFactory.RegisterShrineRoom(gameObject2, this.room, text, this.offset, this.RoomWeight);
				}
				ShrineFactory.registeredShrines.Add(text, gameObject2);
				result = gameObject;
			}
			catch (Exception e)
			{
				Tools.PrintException(e, "FF0000");
				result = null;
			}
			return result;
		}



		public float RoomWeight;


		public GameObject BuildWithoutBaseGameInterference()
		{
			GameObject result;
			try
			{
				Texture2D textureFromResource = ResourceExtractor.GetTextureFromResource(this.spritePath);
				GameObject gameObject = SpriteBuilder.SpriteFromResource(this.spritePath, null, false);
				string text = (this.modID + ":" + this.name).ToLower().Replace(" ", "_");
				gameObject.name = text;
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.IsPerpendicular = true;
				component.PlaceAtPositionByAnchor(this.offset, tk2dBaseSprite.Anchor.LowerCenter);
				Transform transform = new GameObject("talkpoint").transform;
				transform.position = gameObject.transform.position + this.talkPointOffset;
				transform.SetParent(gameObject.transform);
				bool flag = !this.usesCustomColliderOffsetAndSize;
				bool flag2 = flag;
				bool flag3 = flag2;
				if (flag3)
				{
					IntVector2 intVector = new IntVector2(textureFromResource.width, textureFromResource.height);
					this.colliderOffset = new IntVector2(0, 0);
					this.colliderSize = new IntVector2(intVector.x, intVector.y / 2);
				}
				SpeculativeRigidbody speculativeRigidbody = component.SetUpSpeculativeRigidbody(this.colliderOffset, this.colliderSize);
				ShrineFactory.CustomShrineController customShrineController = gameObject.AddComponent<ShrineFactory.CustomShrineController>();
				customShrineController.ID = text;
				customShrineController.roomStyles = this.roomStyles;
				customShrineController.isBreachShrine = true;
				customShrineController.offset = this.offset;
				customShrineController.pixelColliders = speculativeRigidbody.specRigidbody.PixelColliders;
				customShrineController.factory = this;
				customShrineController.OnAccept = this.OnAccept;
				customShrineController.OnDecline = this.OnDecline;
				customShrineController.CanUse = this.CanUse;
				customShrineController.text = this.text;
				customShrineController.acceptText = this.acceptText;
				customShrineController.declineText = this.declineText;
				bool flag4 = this.interactableComponent == null;
				bool flag5 = flag4;
				bool flag6 = flag5;
				if (flag6)
				{
					SimpleShrine simpleShrine = gameObject.AddComponent<SimpleShrine>();
					simpleShrine.isToggle = this.isToggle;
					simpleShrine.OnAccept = this.OnAccept;
					simpleShrine.OnDecline = this.OnDecline;
					simpleShrine.CanUse = this.CanUse;
					simpleShrine.text = this.text;
					simpleShrine.acceptText = this.acceptText;
					simpleShrine.declineText = this.declineText;
					simpleShrine.talkPoint = transform;
				}
				else
				{
					gameObject.AddComponent(this.interactableComponent);
				}
				gameObject.name = text;
				if (!this.isBreachShrine)
				{
					DungeonPlaceable table = StaticReferences.GetAsset<DungeonPlaceable>("WhichShrineWillItBe");
					if (table != null)
					{
						table.variantTiers.Add(new DungeonPlaceableVariant()
						{
							percentChance = ShrinePercentageChance,
							prerequisites = preRequisites ?? new DungeonPrerequisite[0],
							nonDatabasePlaceable = gameObject,
							unitOffset = roomOffset == null ? new Vector2(0, 0) : roomOffset
						});
					}
				}
				FakePrefab.MarkAsFakePrefab(gameObject);
				ShrineFactory.registeredShrines.Add(text, gameObject);
				result = gameObject;
			}
			catch (Exception e)
			{
				Tools.PrintException(e, "FF0000");
				result = null;
			}
			return result;
		}

		public Vector2 roomOffset;
		public float ShrinePercentageChance;
		public DungeonPrerequisite[] preRequisites;

		public static void RegisterShrineRoom(GameObject shrine, PrototypeDungeonRoom protoroom, string ID, Vector2 offset, float roomweight)
		{		
			DungeonPrerequisite[] array = new DungeonPrerequisite[0];
			Vector2 vector = new Vector2((float)(protoroom.Width / 2) + offset.x, (float)(protoroom.Height / 2) + offset.y);
			protoroom.placedObjectPositions.Add(vector);

			DungeonPlaceable placeableContents = ScriptableObject.CreateInstance<DungeonPlaceable>();
			placeableContents.width = 2;
			placeableContents.height = 2;
			placeableContents.respectsEncounterableDifferentiator = true;
			placeableContents.variantTiers = new List<DungeonPlaceableVariant>
			{
				new DungeonPlaceableVariant
				{
					percentChance = 1f,
					nonDatabasePlaceable = shrine,
					prerequisites = array,
					materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
				}
			};

			protoroom.placedObjects.Add(new PrototypePlacedObjectData
			{

				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = placeableContents

			});

			/*protoroom.placedObjects.Add(new PrototypePlacedObjectData
			{
				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = new DungeonPlaceable
				{
					width = 2,
					height = 2,
					respectsEncounterableDifferentiator = true,
					variantTiers = new List<DungeonPlaceableVariant>
					{
						new DungeonPlaceableVariant
						{
							percentChance = 1f,
							nonDatabasePlaceable = shrine,
							prerequisites = array,
							materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
						}
					}
				}
			});

			*/
			RoomFactory.RoomData roomData = new RoomFactory.RoomData
			{
				room = protoroom,
				category = protoroom.category.ToString(),
				weight = roomweight,
		};
			RoomFactory.rooms.Add(ID, roomData);
			DungeonHandler.RegisterForShrine(roomData);
		}
		public static void RegisterShrineRoomNoObject(PrototypeDungeonRoom protoroom, string ID,float roomweight)
		{
			
			RoomFactory.RoomData roomData = new RoomFactory.RoomData
			{
				room = protoroom,
				category = protoroom.category.ToString(),
				weight = roomweight,
			};
			RoomFactory.rooms.Add(ID, roomData);
			DungeonHandler.RegisterForShrine(roomData);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003ADC File Offset: 0x00001CDC
		public static void PlaceBreachShrines()
		{
			bool flag = ShrineFactory.m_builtShrines;
			bool flag2 = !flag;
			if (flag2)
			{
				foreach (GameObject gameObject in ShrineFactory.registeredShrines.Values)
				{
					try
					{
						ShrineFactory.CustomShrineController component = gameObject.GetComponent<ShrineFactory.CustomShrineController>();
						bool flag3 = !component.isBreachShrine;
						bool flag4 = !flag3;
						if (flag4)
						{
							ShrineFactory.CustomShrineController component2 = UnityEngine.Object.Instantiate<GameObject>(gameObject).GetComponent<ShrineFactory.CustomShrineController>();
							component2.Copy(component);
							component2.gameObject.SetActive(true);
							component2.sprite.PlaceAtPositionByAnchor(component2.offset, tk2dBaseSprite.Anchor.LowerCenter);
							IPlayerInteractable component3 = component2.GetComponent<IPlayerInteractable>();
							bool flag5 = component3 is SimpleInteractable;
							bool flag6 = flag5;
							if (flag6)
							{
								((SimpleInteractable)component3).OnAccept = component2.OnAccept;
								((SimpleInteractable)component3).OnDecline = component2.OnDecline;
								((SimpleInteractable)component3).CanUse = component2.CanUse;
							}
							bool flag7 = !RoomHandler.unassignedInteractableObjects.Contains(component3);
							bool flag8 = flag7;
							if (flag8)
							{
								RoomHandler.unassignedInteractableObjects.Add(component3);
							}
						}
					}
					catch (Exception e)
					{
						Tools.PrintException(e, "FF0000");
					}
				}
				ShrineFactory.m_builtShrines = true;
			}
		}

		public string name;

		public string modID;

		public string spritePath;

		public string roomPath;

		public string text;

		public string acceptText;

		public string declineText;

		public Action<PlayerController, GameObject> OnAccept;

		public Action<PlayerController, GameObject> OnDecline;

		public Func<PlayerController, GameObject, bool> CanUse;

		public Vector3 talkPointOffset;

		public Vector3 offset = new Vector3(43.8f, 42.4f, 42.9f);

		public IntVector2 colliderOffset;

		public IntVector2 colliderSize;

		public bool isToggle;

		public string shadowPath;

		public float ShadowOffsetX;

		public float ShadowOffsetY;

		public bool usesCustomColliderOffsetAndSize;

		public Type interactableComponent = null;

		public Type AdditionalComponent = null;

		public bool isBreachShrine = false;

		public PrototypeDungeonRoom room;

		public Dictionary<string, int> roomStyles;

		private static bool m_initialized;

		private static bool m_builtShrines;

		public class CustomShrineController : DungeonPlaceableBehaviour
		{
			private void Start()
			{
				string text = base.name.Replace("(Clone)", "");

				if (ShrineFactory.registeredShrines.ContainsKey(text))
				{
					this.Copy(ShrineFactory.registeredShrines[text].GetComponent<ShrineFactory.CustomShrineController>());
				}
				else
				{
					Tools.PrintError<string>("Was this shrine registered correctly?: " + text, "FF0000");
				}
				SimpleInteractable component = base.GetComponent<SimpleInteractable>();
				bool flag4 = !component;
				bool flag5 = !flag4;
				bool flag6 = flag5;
				if (flag6)
				{
					component.OnAccept = this.OnAccept;
					component.OnDecline = this.OnDecline;
					component.CanUse = this.CanUse;
					component.text = this.text;
					component.acceptText = this.acceptText;
					component.declineText = this.declineText;
				}
			}

			public void Copy(ShrineFactory.CustomShrineController other)
			{
				this.ID = other.ID;
				this.roomStyles = other.roomStyles;
				this.isBreachShrine = other.isBreachShrine;
				this.offset = other.offset;
				this.pixelColliders = other.pixelColliders;
				this.factory = other.factory;
				this.OnAccept = other.OnAccept;
				this.OnDecline = other.OnDecline;
				this.CanUse = other.CanUse;
				this.text = other.text;
				this.acceptText = other.acceptText;
				this.declineText = other.declineText;
			}

			public void ConfigureOnPlacement(RoomHandler room)
			{
				this.m_parentRoom = room;
				this.RegisterMinimapIcon();
			}

			public void RegisterMinimapIcon()
			{
				this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
			}

			public void GetRidOfMinimapIcon()
			{
				if (this.m_instanceMinimapIcon != null)
				{
					Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
					this.m_instanceMinimapIcon = null;
				}
			}

			public string ID;

			public bool isBreachShrine;

			public Vector3 offset;

			public List<PixelCollider> pixelColliders;

			public Dictionary<string, int> roomStyles;

			public ShrineFactory factory;

			public Action<PlayerController, GameObject> OnAccept;

			public Action<PlayerController, GameObject> OnDecline;

			public Func<PlayerController, GameObject, bool> CanUse;

			private RoomHandler m_parentRoom;

			private GameObject m_instanceMinimapIcon;

			public int numUses = 0;

			public string text;

			public string acceptText;

			public string declineText;
		}
	}
}
