using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Planetside
{
	public class GildedPots : PassiveItem
	{
		public static void Init()
		{
			string name = "Gilded Pot";
			string resourcePath = "Planetside/Resources/gildedceramic.png";
			GameObject gameObject = new GameObject(name);
			GildedPots warVase = gameObject.AddComponent<GildedPots>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Destruction Therapy";
			string longDesc = "Decorative breakables have a chance to drop a casing.\n\nOriginally a trinket carried around by the Lost Adventurer, he lost it while traversing the halls of the Gungeon.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.D;
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:gilded_pot",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"coin_crown",
				"gold_ammolet",
				"gilded_bullets"
			};
			CustomSynergies.Add("Expert Demolitionist", mandatoryConsoleIDs, optionalConsoleIDs, true);
			GildedPots.GildedPotsID = warVase.PickupObjectId;
			ItemIDs.AddToList(warVase.PickupObjectId);
			warVase.gameObject.AddComponent<RustyItemPool>();
			new Hook(typeof(MinorBreakable).GetMethod("OnBreakAnimationComplete", BindingFlags.Instance | BindingFlags.NonPublic), typeof(GildedPots).GetMethod("CoinChance"));
		}
		public static int GildedPotsID;



		public static void CoinChance(Action<MinorBreakable> orig, MinorBreakable self)
		{
			orig(self);
			if (self != null)
			{
				for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
				{
					PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player.HasPickupID(GildedPots.GildedPotsID) && player != null && self != null)
					{
						float coinchance = 0.04f;
						bool flagA = player.PlayerHasActiveSynergy("Expert Demolitionist");
						if (flagA)
						{
							coinchance *= 2;
						}
						float num = UnityEngine.Random.Range(0f, 1f);
						bool flag2 = (double)num < coinchance;
						if (flag2)
						{
							LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, self.transform.PositionVector2(), Vector2.zero, 1f, false, false, false);
						}
					}
				}
			}
		}
		public override void Pickup(PlayerController player)
		{
			//player.ReceivesTouchDamage = false;
			/*
			TrailRenderer tr;
			var tro = player.gameObject.AddChild("trail object");
			tro.transform.position = player.transform.position;
			tro.transform.localPosition = new Vector3(0f, 0f, 0f);

			tr = tro.AddComponent<TrailRenderer>();
			tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			tr.receiveShadows = false;
			var mat = new Material(Shader.Find("Sprites/Default"));
			mat.mainTexture = _gradTexture;
			mat.SetColor("_Color", new Color(7f, 0f, 0f, 2f));
			tr.material = mat;
			tr.time = 0.7f;
			tr.minVertexDistance = 0.1f;
			tr.startWidth = 2f;
			tr.endWidth = 0f;
			tr.startColor = Color.white;
			tr.endColor = new Color(7f, 0f, 1f, 0f);
			*/
			base.Pickup(player);
			//player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.EnterRoom));
		}
		public Texture _gradTexture;

		// Token: 0x06000199 RID: 409 RVA: 0x00010168 File Offset: 0x0000E368
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			//player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.EnterRoom));
			return result;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000101A8 File Offset: 0x0000E3A8
		private void EnterRoom()
		{
			RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
			for (int i = 0; i < StaticReferenceManager.AllMinorBreakables.Count; i++)
			{
				MinorBreakable minorBreakable = StaticReferenceManager.AllMinorBreakables[i];
				bool flag = minorBreakable && !minorBreakable.IsBroken && minorBreakable.CenterPoint.GetAbsoluteRoom() == currentRoom && !minorBreakable.IgnoredForPotShotsModifier;
				if (flag)
				{
					MinorBreakable minorBreakable2 = minorBreakable;
					minorBreakable2.OnBreakContext = (Action<MinorBreakable>)Delegate.Combine(minorBreakable2.OnBreakContext, new Action<MinorBreakable>(this.HandleBroken));
				}
			}
		}

	
		private void HandleBroken(MinorBreakable mb)
		{
			float coinchance = 0.04f;
			PlayerController player = base.Owner;
			bool flagA = player.PlayerHasActiveSynergy("Expert Demolitionist");
			if (flagA)
            {
				coinchance *= 2;
            }
				PlayerController owner = base.Owner;
			float num = UnityEngine.Random.Range(0f, 1f);
			bool flag2 = (double)num < coinchance;
			if (flag2)
			{
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, mb.sprite.WorldCenter, Vector2.zero, 1f, false, false, false);
			}
		}
		public void PostProcessProjectile(Projectile proj, float f)
		{
			proj.collidesWithProjectiles = true;
			SpeculativeRigidbody specRigidbody = proj.specRigidbody;

			specRigidbody.OnPreRigidbodyCollision += this.HandlePreCollision;
		}
		private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.projectile;
			if (flag)
			{
				bool flag2 = otherRigidbody.projectile.Owner is AIActor;
				if (flag2)
				{
					bool isBlackBullet = otherRigidbody.projectile.IsBlackBullet;
					if (isBlackBullet)
					{
						otherRigidbody.projectile.BecomeBlackBullet();
					}
				}
				PhysicsEngine.SkipCollision = true;
			}
		}
	}
}
