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
using SaveAPI;
using Planetside;

namespace Planetside
{
    public static class RandomPiecesOfStuffToInitialise
    {

        public static void BuildPrefab()
        {
			
            BuildSoulGuon();

			BuildHeartGuon();
			BuildHalfHeartGuon();
			BuildAmmoGuon();
			BuildHalfAmmoGuon();
			BuildArmorGuon();
			BuildKeyGuon();
			BuildBlankguon();

			BuildMedal();

			BuildTargetReticle();
			BuildCursePoof();
			//BuildSW();
			//BuildSpeakerObject();
		}

		public static void BuildTargetReticle()
		{
			KineticStrikeTargetReticle = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/KineticStrike/redmarksthespot", new GameObject("Kinetic Strike Target Reticle"));
			KineticStrikeTargetReticle.SetActive(false);
			tk2dBaseSprite vfxSprite = KineticStrikeTargetReticle.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(KineticStrikeTargetReticle);

			vfxSprite.sprite.usesOverrideMaterial = true;
			vfxSprite.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
			Material mat = vfxSprite.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = vfxSprite.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			vfxSprite.sprite.renderer.material = mat;


			UnityEngine.Object.DontDestroyOnLoad(KineticStrikeTargetReticle);

		}
		public static GameObject KineticStrikeTargetReticle;


		public static void BuildSpeakerObject()
		{
			SpeakerObject = new GameObject();
			SpeakerObject.AddComponent<TextMaker>();
			UnityEngine.Object.DontDestroyOnLoad(SpeakerObject);
		}
		public static GameObject SpeakerObject;


		public static void BuildCursePoof()
        {
			List<string> cursepoofvfxVFXPaths = new List<string>()
			{
				"Planetside/Resources/VFX/CursePoof/cursepoof_001",
				"Planetside/Resources/VFX/CursePoof/cursepoof_002",
				"Planetside/Resources/VFX/CursePoof/cursepoof_003",
				"Planetside/Resources/VFX/CursePoof/cursepoof_004",
				"Planetside/Resources/VFX/CursePoof/cursepoof_005",

			};
			cursepoofvfx = VFXToolbox.CreateVFX("Cursebulon Goop Poof", cursepoofvfxVFXPaths, 14, new IntVector2(12, 12), tk2dBaseSprite.Anchor.LowerCenter, true, 0.18f);
		}
		public static GameObject cursepoofvfx;


		public static void BuildSW()
		{
			GameObject SW = ItemBuilder.AddSpriteToObject("SW", "Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", null);
			tk2dSpriteAnimator animator = SW.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 5;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "idle";
			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("idle");
			animator.playAutomatically = true;

			SpeculativeRigidbody orAddComponent = animator.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
			PixelCollider pixelCollider = new PixelCollider();
			pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
			pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
			pixelCollider.ManualWidth = 0;
			pixelCollider.ManualHeight = 0;
			pixelCollider.ManualOffsetX =0;
			pixelCollider.ManualOffsetY =0;
			orAddComponent.PixelColliders = new List<PixelCollider>
			{
				pixelCollider
			};

			SomethingWicked = animator.gameObject;

			UnityEngine.Object.DontDestroyOnLoad(SomethingWicked);
			FakePrefab.MarkAsFakePrefab(SomethingWicked);
			SomethingWicked.SetActive(false);

		}
		public static GameObject SomethingWicked;

		public static void BuildSoulGuon()
        {
			GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/SoulGuon/guoner.png");
			gameObject.name = $"Soul Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 4f;
			orbitalPrefab.orbitDegreesPerSecond = 40;
			orbitalPrefab.SetOrbitalTier(0);


			ImprovedAfterImage yeah = gameObject.AddComponent<ImprovedAfterImage>();
			yeah.dashColor = new Color(0, 0.7f, 1);
			yeah.spawnShadows = true;
			yeah.shadowTimeDelay = 0.02f;
			yeah.shadowLifetime = 0.2f;

			PlayerOrbital si = gameObject.GetComponent<PlayerOrbital>();

			si.sprite.usesOverrideMaterial = true;
			si.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
			Material mat = si.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = si.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			si.sprite.renderer.material = mat;

			SoulSynergyGuon = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(SoulSynergyGuon);
			FakePrefab.MarkAsFakePrefab(SoulSynergyGuon);
			SoulSynergyGuon.SetActive(false);
		}
		public static GameObject SoulSynergyGuon;
		public static string vfxNamefrailty = "FrailtyVFX";
		public static GameObject MedalObject;
		public static void BuildMedal()
		{
			MedalObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/planetsidemedal", new GameObject("Medal"));
			MedalObject.SetActive(false);
			tk2dBaseSprite vfxSprite = MedalObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(MedalObject);
			UnityEngine.Object.DontDestroyOnLoad(MedalObject);
		}



		public static void BuildHeartGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("heart_guon", "Planetside/Resources/Guons/PickupGuons/HeartGuon/heartguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/HeartGuon/heartguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/HeartGuon/heartguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Heart Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 2f;
			orbitalPrefab.orbitDegreesPerSecond = 60;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			HeartGuon = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(HeartGuon);
			FakePrefab.MarkAsFakePrefab(HeartGuon);
			HeartGuon.SetActive(false);

		}
		public static void BuildHalfHeartGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("half_heart_guon", "Planetside/Resources/Guons/PickupGuons/HalfheartGuon/halfheartguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/HalfheartGuon/halfheartguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/HalfheartGuon/halfheartguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Half Heart Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 3f;
			orbitalPrefab.orbitDegreesPerSecond = 60;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			HalfheartGuon = gameObject;
			//UnityEngine.Object.DontDestroyOnLoad(HalfheartGuon);
			UnityEngine.Object.DontDestroyOnLoad(HalfheartGuon);
			FakePrefab.MarkAsFakePrefab(HalfheartGuon);
			HalfheartGuon.SetActive(false);
		}
		public static void BuildAmmoGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("ammo_guon", "Planetside/Resources/Guons/PickupGuons/AmmoGuon/ammoguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/AmmoGuon/ammoguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/AmmoGuon/ammoguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Ammo Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 2.5f;
			orbitalPrefab.orbitDegreesPerSecond = 40;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			AmmoGuon = gameObject;
			//UnityEngine.Object.DontDestroyOnLoad(AmmoGuon);

			UnityEngine.Object.DontDestroyOnLoad(AmmoGuon);
			FakePrefab.MarkAsFakePrefab(AmmoGuon);
			AmmoGuon.SetActive(false);

		}
		public static void BuildHalfAmmoGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("half_ammo_guon", "Planetside/Resources/Guons/PickupGuons/HalfAmmoguon/halfammoguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/HalfAmmoguon/halfammoguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/HalfAmmoguon/halfammoguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Half Ammo Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 4.5f;
			orbitalPrefab.orbitDegreesPerSecond = 40;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			HalfAmmoGuon = gameObject;
			//UnityEngine.Object.DontDestroyOnLoad(HalfAmmoGuon);
			UnityEngine.Object.DontDestroyOnLoad(HalfAmmoGuon);
			FakePrefab.MarkAsFakePrefab(HalfAmmoGuon);
			HalfAmmoGuon.SetActive(false);

		}
		public static void BuildArmorGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("armor_guon", "Planetside/Resources/Guons/PickupGuons/Armor/armorguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/Armor/armorguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/Armor/armorguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Armor Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 1.5f;
			orbitalPrefab.orbitDegreesPerSecond = 40;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			ArmorGuon = gameObject;
			//UnityEngine.Object.DontDestroyOnLoad(A);
			UnityEngine.Object.DontDestroyOnLoad(ArmorGuon);
			FakePrefab.MarkAsFakePrefab(ArmorGuon);
			ArmorGuon.SetActive(false);

		}
		public static void BuildKeyGuon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("key_guon", "Planetside/Resources/Guons/PickupGuons/keyguon/keyguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/keyguon/keyguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/keyguon/keyguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Key Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 1f;
			orbitalPrefab.orbitDegreesPerSecond = 30;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			KeyGuon = gameObject;
			//UnityEngine.Object.DontDestroyOnLoad(KeyGuon);
			UnityEngine.Object.DontDestroyOnLoad(KeyGuon);
			FakePrefab.MarkAsFakePrefab(KeyGuon);
			KeyGuon.SetActive(false);
		}
		public static void BuildBlankguon()
		{
			GameObject deathmark = ItemBuilder.AddSpriteToObject("blank_guon", "Planetside/Resources/Guons/PickupGuons/BlankGuon/blankguon_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 4;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/PickupGuons/BlankGuon/blankguon_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/PickupGuons/BlankGuon/blankguon_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;

			GameObject gameObject = animator.gameObject;
			gameObject.name = $"Blank Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 5f;
			orbitalPrefab.orbitDegreesPerSecond = 120;
			//orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);
			//UnityEngine.Object.DontDestroyOnLoad(gameObject);
			//FakePrefab.MarkAsFakePrefab(gameObject);
			//gameObject.SetActive(false);
			BlankGuon = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(BlankGuon);
			FakePrefab.MarkAsFakePrefab(BlankGuon);
			BlankGuon.SetActive(false);
		}


		public static GameObject KeyGuon;
		public static GameObject HalfheartGuon;
		public static GameObject HeartGuon;
		public static GameObject AmmoGuon;
		public static GameObject ArmorGuon;
		public static GameObject HalfAmmoGuon;
		public static GameObject BlankGuon;


	}
}


/*
public class SomethingWicked : MonoBehaviour
{
	public SomethingWicked()
	{
		this.ShootTimer = 3f;
		this.MinSpeed = 4f;
		this.MaxSpeed = 9f;
		this.MinSpeedDistance = 8f;
		this.MaxSpeedDistance = 40f;
	}

	// Token: 0x17000DE8 RID: 3560
	public static SomethingWicked Instance
	{
		get
		{
			return SomethingWicked.m_instance;
		}
	}


	private void Start()
	{
		HasTPed = false;
		AkSoundEngine.PostEvent("Play_OBJ_chestglitch_loop_01", base.gameObject);
		SomethingWicked.m_instance = this;
		SpeculativeRigidbody specRigidbody = base.gameObject.GetComponent<SpeculativeRigidbody>();
		specRigidbody.OnEnterTrigger = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(specRigidbody.OnEnterTrigger, new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered));
		//base.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
		//base.aiAnimator.PlayUntilFinished("intro", false, null, -1f, false);
		//tk2dSpriteAnimator spriteAnimator = base.spriteAnimator;
		//spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent));
		/*
		for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
		{
			if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#SREAPER_ENCNAME")
			{
				GameStatsManager.Instance.HandleEncounteredObjectRaw(EncounterDatabase.Instance.Entries[i].myGuid);
			}
		}
		this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
		if (base.encounterTrackable)
		{
			GameStatsManager.Instance.HandleEncounteredObject(base.encounterTrackable);
		}
		*/
/*
		Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
		mat.mainTexture = base.gameObject.GetComponent<tk2dSpriteAnimator>().sprite.renderer.material.mainTexture;
		mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		mat.SetFloat("_EmissiveColorPower", 1.55f);
		mat.SetFloat("_EmissivePower", 100);
		base.gameObject.GetComponent<tk2dSpriteAnimator>().sprite.renderer.material = mat;

		Planetside.ImprovedAfterImage yes = base.gameObject.AddComponent<Planetside.ImprovedAfterImage>();
		yes.spawnShadows = true;
		yes.shadowLifetime = 0.2f;
		yes.shadowTimeDelay = 0.001f;
		yes.dashColor = Color.black;
		yes.name = "SW trail";
	}

	public void OnDestroy()
	{
		SomethingWicked.m_instance = null;
	}

	private void HandleTriggerEntered(SpeculativeRigidbody targetRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
	{
		Projectile projectile = targetRigidbody.projectile;
		if (projectile)
		{
			//projectile.HandleKnockback(base.specRigidbody, targetRigidbody.GetComponent<PlayerController>(), false, false);
		}
	}

	private void Update()
	{
		if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
		{
			return;
		}
		if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
		{
			return;
		}
		if (TimeTubeCreditsController.IsTimeTubing)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.HandleMotion();
		ETGModConsole.Log(base.transform.position.ToString());
		foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
		{
			PlayerController player = proj.Owner as PlayerController;
			bool isBem = proj.GetComponent<BasicBeamController>() != null;
			if (isBem == true && Planetside.BeamToolbox.PosIsNearAnyBoneOnBeam(proj.GetComponent<BasicBeamController>(), base.gameObject.transform.PositionVector2(), 6f) && proj.Owner != null && proj.Owner == player && HasTPed != true)
            {
				AkSoundEngine.PostEvent("Play_VO_gorgun_gasp_01", base.gameObject);
				Vector2 Point = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), 200);
				base.transform.position = base.transform.PositionVector2() + Point;
				MaxSpeed += 0.5f;
				GameManager.Instance.StartCoroutine(this.TPCooldown());
			}
			else if (Vector2.Distance(proj.sprite.WorldCenter, base.gameObject.transform.PositionVector2()) < 6f && proj.Owner != null && proj.Owner == player && HasTPed != true)
            {
				AkSoundEngine.PostEvent("Play_VO_gorgun_gasp_01", base.gameObject);
				Vector2 Point = MathToolbox.GetUnitOnCircle(proj.Direction.ToAngle() + 180, 200);
				base.transform.position = base.transform.PositionVector2() + Point;
				MaxSpeed += 0.5f;
				GameManager.Instance.StartCoroutine(this.TPCooldown());
			}
		}
	}

	private IEnumerator TPCooldown()
    {
		HasTPed = true;
		yield return new WaitForSeconds((MaxSpeed/10)+1.5f);
		HasTPed = false;
		yield break;
    }

	private bool HasTPed;
	private void HandleAttacks()
	{
		/*
		if (base.aiAnimator.IsPlaying("intro"))
		{
			return;
		}
		CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
		if (cellData != null && cellData.type != CellType.WALL)
		{

		}
		
	}
	

/*
	private void HandleMotion()
	{
		//base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity = Vector2.zero;
		if (this.m_currentTargetPlayer == null)
		{
			return;
		}
		if (this.m_currentTargetPlayer.healthHaver.IsDead || this.m_currentTargetPlayer.IsGhost)
		{
			this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
		}
		Vector2 centerPosition = this.m_currentTargetPlayer.CenterPosition;
		Vector2 vector = centerPosition - base.gameObject.GetComponent<SpeculativeRigidbody>().UnitCenter;
		float magnitude = vector.magnitude;
		float d = Mathf.Lerp(this.MinSpeed, this.MaxSpeed, (magnitude - this.MinSpeedDistance) / (this.MaxSpeedDistance - this.MinSpeedDistance));
		base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity = vector.normalized * d;
		//base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity += this.knockbackComponent;
	}

	private static SomethingWicked m_instance;

	// Token: 0x040058CB RID: 22731
	public static bool PreventShooting;

	// Token: 0x040058CC RID: 22732
	public BulletScriptSelector BulletScript;

	public Transform ShootPoint;

	public float ShootTimer;

	public float MinSpeed;

	public float MaxSpeed;

	public float MinSpeedDistance;

	public float MaxSpeedDistance;

	[NonSerialized]
	public Vector2 knockbackComponent;

	// Token: 0x040058D4 RID: 22740
	private PlayerController m_currentTargetPlayer;

}

*/