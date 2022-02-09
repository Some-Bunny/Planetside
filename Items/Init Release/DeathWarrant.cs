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
			tk2dSpriteAnimator animator = deathmark.GetOrAddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimation animation = deathmark.AddComponent<tk2dSpriteAnimation>();

			tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(deathmark, ("DeathMark_Collection"));

			tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 11 };
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
			
			for (int i = 1; i < 10; i++)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_00{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
			}
			idleClip.frames = frames.ToArray();
			idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animator.Library = animation;
			animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
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
				AIActor randomActiveEnemy;
				randomActiveEnemy = base.Owner.CurrentRoom.GetRandomActiveEnemy(true);
				GameObject lightning = randomActiveEnemy.PlayEffectOnActor(DeathMarkPrefab, new Vector3(0f, -1f, 0f));

				lightning.transform.position.WithZ(transform.position.z + 99999);
				lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(randomActiveEnemy.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				randomActiveEnemy.sprite.AttachRenderer(lightning.GetComponent<tk2dBaseSprite>());

				lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(randomActiveEnemy.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				lightning.transform.position.WithZ(transform.position.z + 2);

				ImprovedAfterImage image = randomActiveEnemy.gameObject.AddComponent<ImprovedAfterImage>();
				image.dashColor = Color.red;
				image.spawnShadows = true;
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


