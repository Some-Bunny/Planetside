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

//Garbage Code Incoming
namespace Planetside
{
    public class DeathWarrant : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Death Warrant";
            string resourceName = "Planetside/Resources/gundeathwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<DeathWarrant>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Sign It For Everyone";
            string longDesc = "A death warrant granted to you by law." +
                "\n\nOn it are the 10 billion signatures of all the Gundead that roam in the Gungeon.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.C;

			GameObject deathmark = ItemBuilder.AddSpriteToObject("deathmark_vfx", "Planetside/Resources/VFX/DeathMark/markedfordeathvfx_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 11;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 9; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;


			DeathMarkPrefab = deathmark;
			DeathWarrant.DeathWarrantID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int DeathWarrantID;
		private static GameObject DeathMarkPrefab;

		private void MarkForDeath()
		{


			List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			bool flag = activeEnemies != null;
			if (flag)
			{
				RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
				AIActor randomActiveEnemy;
				randomActiveEnemy = base.Owner.CurrentRoom.GetRandomActiveEnemy(true);
				GameObject lightning = randomActiveEnemy.PlayEffectOnActor(DeathMarkPrefab, new Vector3(0f, -1f, 0f));

				lightning.transform.position.WithZ(transform.position.z + 99999);
				lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(randomActiveEnemy.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				randomActiveEnemy.sprite.AttachRenderer(lightning.GetComponent<tk2dBaseSprite>());

				lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(randomActiveEnemy.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				lightning.transform.position.WithZ(transform.position.z + 2);
				lightning.GetComponent<tk2dSpriteAnimator>().Play();

				this.Trailer.spawnShadows = true;
				randomActiveEnemy.gameObject.AddComponent(this.Trailer);
				if (randomActiveEnemy.healthHaver.IsBoss)
                {
					randomActiveEnemy.healthHaver.AllDamageMultiplier += .25f;
				}
				else
                {
					randomActiveEnemy.healthHaver.AllDamageMultiplier *= 2.5f;
				}
			}
		}
		private readonly ImprovedAfterImage Trailer = new ImprovedAfterImage
		{
			dashColor = Color.red,
			spawnShadows = true
		};
		public override DebrisObject Drop(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.MarkForDeath));
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.MarkForDeath));
            base.Pickup(player);
		}
	}
}


