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

namespace Planetside
{
	public class OscilaltingBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Oscillating Bullets";
			//string resourceName = "Planetside/Resources/oscillatingbullets.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<OscilaltingBullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("oscillatingbullets"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Back & Forth";
			string longDesc = "These bullets were forged by a smith who couldn't decide how to forge them, so they ended up forged half-and-half in different ways." +
				"\n\nThis makes them fight over the bullets speed on how they should move.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.A;
			OscilaltingBullets.OscillatingBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

			curve = new AnimationCurve()
			{
				postWrapMode = WrapMode.Loop,
				keys = new Keyframe[] {
				new Keyframe(){time = 0, value = 0.7f, inTangent = 0.75f, outTangent = 0.25f},
				new Keyframe(){time = 0.4f, value = 0.3f, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.6f, value = -0.2f, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.95f, value = 0.7f, inTangent = 0.75f, outTangent = 0.25f},
                }
            };
			
		}
		public static AnimationCurve curve;
        public static int OscillatingBulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				sourceProjectile.StartCoroutine(DoOscillate(sourceProjectile));
            }
            catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}

		public static IEnumerator DoOscillate(Projectile p)
		{
            float f = p.baseData.speed;
            float e = 0;
			while (p)
			{

                e += BraveTime.DeltaTime;
                p.baseData.speed = f * curve.Evaluate(e);
				p.UpdateSpeed();
				yield return null;
			}
			yield break;
		}



		private void PostProcessBeam(BeamController obj)
		{

			try
			{
                obj.projectile.OverrideMotionModule = new OscillatingeMotionModule();
                //obj.projectile.baseData.UsesCustomAccelerationCurve = true;
                //obj.projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 0.5f, -0.2f);
            }
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;
		}

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
			}
			base.OnDestroy();
		}
	}
}