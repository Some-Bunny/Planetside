using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;

namespace Planetside
{
	public class GildedPots : PassiveItem
	{
		public static void Init()
		{
			string name = "Gilded Pot";
			GameObject gameObject = new GameObject(name);
			GildedPots warVase = gameObject.AddComponent<GildedPots>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("gildedceramic"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Destruction Therapy";
			string longDesc = "Decorative breakables have a chance to be replaced by special coin-plated pottery.\n\nOriginally a trinket carried around by the Lost Adventurer, it was lost only by a technicality in that the Lost Adventurer got himself lost after placing the pot down to relax.";
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
				"gilded_bullets",
				"bomb",
				"c4",
				"cluster_mine",
				"proximity_mine",
				"blast_helmet"
			};


            CustomSynergies.Add("Expert Demolitionist", mandatoryConsoleIDs, optionalConsoleIDs, true);
			GildedPots.GildedPotsID = warVase.PickupObjectId;
			ItemIDs.AddToList(warVase.PickupObjectId);
			warVase.gameObject.AddComponent<RustyItemPool>();
			new Hook(typeof(MinorBreakable).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic), typeof(GildedPots).GetMethod("CoinChance"));
            //new Hook(typeof(MinorBreakable).GetMethod("Break", BindingFlags.Instance | BindingFlags.Public), typeof(GildedPots).GetMethod("BoomChance"));

            GameManager.Instance.RainbowRunForceExcludedIDs.Add(warVase.PickupObjectId);

		}
		public static int GildedPotsID;

     
		public static void CoinChance(Action<MinorBreakable> orig, MinorBreakable self)
		{
			orig(self);

			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				PlayerController player = GameManager.Instance.AllPlayers[i];
				if (player != null && player.inventory != null && player.passiveItems.Count >= 0)
				{
                    if (player.HasPickupID(GildedPots.GildedPotsID) && self != null)
                    {
                        if (self.transform?.parent?.gameObject?.GetComponent<MirrorController>() == null && self.transform?.parent?.gameObject?.GetComponent<KickableObject>() == null)
                        {
                            bool Synergy = player.PlayerHasActiveSynergy("Expert Demolitionist");
                            float coinchance = Synergy ? 0.05f : 0.035f;
                            if (self.GetComponent<MoneyPots.MoneyPotBehavior>() == null && UnityEngine.Random.value < coinchance)
                            {
                                Vector2 position = self.transform.position;
                                DungeonPlaceable bom = ScriptableObject.CreateInstance<DungeonPlaceable>();
                                StaticReferences.StoredDungeonPlaceables.TryGetValue("moneyPotRandom", out bom);
                                bom.InstantiateObject(position.GetAbsoluteRoom(), position.ToIntVector2() - position.GetAbsoluteRoom().area.basePosition);
                                Destroy(self.gameObject);
                            }
                        }
                    }
                }
				
			}
			self.OnBreak = () =>
			{
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player != null)
					{
                        bool Synergy = player.PlayerHasActiveSynergy("Expert Demolitionist");
                        if (Synergy && self.GetComponent<MoneyPots.MoneyPotBehavior>() == null && UnityEngine.Random.value < 0.2f)
                        {
                            ExplosionData ex = StaticExplosionDatas.genericLargeExplosion.CopyExplosionData();
                            ex.ignoreList.Add(player.specRigidbody);
                            Exploder.Explode(self.transform.position, ex, self.transform.position);
                        }
                    }
                   
                }
            };
        }
        public override void Pickup(PlayerController player)
		{
			if (m_pickedUpThisRun == false)
			{
				for (int i = 0; i < StaticReferenceManager.AllMinorBreakables.Count; i++)
				{
					MinorBreakable self = StaticReferenceManager.AllMinorBreakables[i];
					{
						if (self)
						{
                            if (self.transform?.parent?.gameObject?.GetComponent<MirrorController>() == null) 
							{
                                bool Synergy = player.PlayerHasActiveSynergy("Expert Demolitionist");
                                float coinchance = Synergy ? 0.05f : 0.035f;
                                if (self.GetComponent<MoneyPots.MoneyPotBehavior>() == null && UnityEngine.Random.value < coinchance)
                                {
                                    Vector2 position = self.transform.position;
                                    DungeonPlaceable bom = ScriptableObject.CreateInstance<DungeonPlaceable>();
                                    StaticReferences.StoredDungeonPlaceables.TryGetValue("moneyPotRandom", out bom);
                                    bom.InstantiateObject(position.GetAbsoluteRoom(), position.ToIntVector2() - position.GetAbsoluteRoom().area.basePosition);
                                    Destroy(self.gameObject);
                                    LootEngine.DoDefaultItemPoof(position, true, true);
                                }
                            }
                        }
					}       
                }
			}

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
		}
		public Texture _gradTexture;

		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}
	}
}
