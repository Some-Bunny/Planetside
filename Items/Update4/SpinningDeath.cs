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
using SaveAPI;
using Alexandria.Assetbundle;

namespace Planetside
{
	public class SpinningDeathController : MonoBehaviour
	{
		public SpinningDeathController()
		{
			this.ParticleObjectToUse = StaticVFXStorage.PerfectedParticleSystem;
			this.tickDelay = 0.05f;
		}

		private void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			this.beamController = base.GetComponent<BeamController>();
			this.basicBeamController = base.GetComponent<BasicBeamController>();
			bool flag = this.projectile.Owner is PlayerController;

			if (flag)
			{
				this.owner = (this.projectile.Owner as PlayerController);
			}
		}

		private void Update()
		{
			bool flag = this.timer > 0f;
			if (flag)
			{
				this.timer -= BraveTime.DeltaTime;
			}
			if (this.timer <= 0f)
			{
				this.DoTick();
				this.timer = this.tickDelay;
			}
		}

		private void DoTick()
		{
			LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", this.basicBeamController);
			Vector2 bonePosition = this.basicBeamController.GetBonePosition(linkedList.ElementAt(UnityEngine.Random.Range(0, linkedList.Count())));
            ParticleSystem particleSystem = StaticVFXStorage.PerfectedParticleSystem.GetComponent<ParticleSystem>();
            var trails = particleSystem.trails;
            trails.worldSpace = false;
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = bonePosition,
                randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
            };
            var emission = particleSystem.emission;
            emission.enabled = false;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Emit(emitParams, 1);
        }



		private float timer;
		public float tickDelay;

		public ParticleSystem ParticleObjectToUse;

		private Projectile projectile;

		private BasicBeamController basicBeamController;

		private BeamController beamController;

		private PlayerController owner;
	}


	
	public class SpinningDeath : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Spinning Death";
            //string resourceName = "Planetside/Resources/spinningDeath.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SpinningDeath>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("spinningDeath"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "LET IT RIP!";
			string longDesc = "Embeds laser-cutters into some of your projectiles. \n\nThe remnants of a crowdfunded laser blender that ended up very, VERY horribly. Keep your fingers away...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;

			SpinningDeath.SpinningDeathID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefabBundle(
			"spinningDeath_mid_001",
			StaticSpriteDefinitions.Beam_Sheet_Data,
			StaticSpriteDefinitions.Beam_Animation_Data,
            "spinndingdeath_mid", new Vector2(6, 6), new Vector2(0, 0), //Main
            "spinndingdeath_impact", new Vector2(6, 6), new Vector2(0, 0), //Impact
            "spinndingdeath_end", new Vector2(6, 6), new Vector2(0, 0), //End Of beam
			null, null, null, //Start Of Beam
			true);
            projectile.gameObject.SetActive(false);
			projectile.baseData.damage = 26;
			projectile.baseData.range = 2.25f;
			projectile.baseData.speed = 10;
			projectile.baseData.force = 0;

			projectile.gameObject.AddComponent<SpinningDeathController>();

			PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			spook.penetration = 10;
			spook.penetratesBreakables = true;

			MaintainDamageOnPierce noDamageLoss = projectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
			noDamageLoss.damageMultOnPierce = 1;

			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);

			beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
			beamComp.interpolateStretchedBones = false;
			beamComp.ContinueBeamArtToWall = true;
			beamComp.endAudioEvent = "Stop_WPN_All";
			beamComp.startAudioEvent = "Play_WPN_moonscraperLaser_shot_01";

			EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
			emiss.EmissiveColorPower = 20f;
			emiss.EmissivePower = 50f;
			
			SpinningDeathBeam = projectile;

			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:spinning_death",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"directional_pad"
			};
			CustomSynergies.Add("D-rectional", mandatoryConsoleIDs, optionalConsoleIDs, true);

            List<string> optionalConsoleIDs2 = new List<string>
            {
                "psog:shockchain"
            };
            CustomSynergies.Add("Overclocked", mandatoryConsoleIDs, optionalConsoleIDs2, true);
        }


		public static Projectile SpinningDeathBeam; 
		public static int SpinningDeathID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			float procChance = 0.25f;
			procChance *= effectChanceScalar;
			SpinningDeathController cont = sourceProjectile.gameObject.GetComponent<SpinningDeathController>();
            ShockChainProjectile sh = sourceProjectile.gameObject.GetComponent<ShockChainProjectile>();

			if (sh != null && Owner.PlayerHasActiveSynergy("Overclocked"))
			{
                AkSoundEngine.PostEvent("Play_EnergySwirl", sourceProjectile.gameObject);
                PierceProjModifier spook = sourceProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                spook.penetration += 5;
                spook.penetratesBreakables = true;
                bool Flipped = sh.IsLeft;

                if (Owner.PlayerHasActiveSynergy("D-rectional"))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(SpinningDeath.SpinningDeathBeam, Owner, sourceProjectile.gameObject, sourceProjectile.gameObject.transform.PositionVector2(), false, (90f * i), 30f, true, true, Flipped ? 360 : -360);
                        Projectile component3 = beamController3.GetComponent<Projectile>();
                        float Dmg = sourceProjectile.baseData.damage *= Owner != null ? Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                        component3.baseData.damage = Dmg * 3f;
                        component3.AdditionalScaleMultiplier *= 0.66f;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(SpinningDeathBeam, base.Owner, sourceProjectile.gameObject, sourceProjectile.gameObject.transform.PositionVector2(), false, 180f * i, 30f, true, true, Flipped ? -720 : 720);
                        Projectile component3 = beamController3.GetComponent<Projectile>();
                        float Dmg = sourceProjectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                        component3.baseData.damage = Dmg * 4.5f;
                        component3.AdditionalScaleMultiplier *= 0.66f;
                    }
                }

            }
			else if (UnityEngine.Random.value <= procChance && cont == null)// && SynergyCheck == false)
			{
				try
				{
					AkSoundEngine.PostEvent("Play_EnergySwirl", sourceProjectile.gameObject);
					sourceProjectile.baseData.speed *= 0.3f;
					sourceProjectile.UpdateSpeed();

					PierceProjModifier spook = sourceProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
					spook.penetration += 10;
					spook.penetratesBreakables = true;


					if (Owner.PlayerHasActiveSynergy("D-rectional"))
                    {
						bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
						float FlippedORama = UnityEngine.Random.value > 0.5f ? 45 : 0;
						for (int i = 0; i < 4; i++)
						{
							BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(SpinningDeath.SpinningDeathBeam, Owner, sourceProjectile.gameObject, sourceProjectile.gameObject.transform.PositionVector2(), false, (90f * i) + FlippedORama, 30f, true, true, Flipped ? 360 : -360);
							Projectile component3 = beamController3.GetComponent<Projectile>();
							float Dmg = sourceProjectile.baseData.damage *= Owner != null ? Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
							component3.baseData.damage = Dmg * 2f;
							component3.AdditionalScaleMultiplier *= 0.66f;
						}
					}
					else
                    {
						bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
						for (int i = 0; i < 2; i++)
						{
							BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(SpinningDeathBeam, base.Owner, sourceProjectile.gameObject, sourceProjectile.gameObject.transform.PositionVector2(), false, 180f * i, 30f, true, true, Flipped ? -720 : 720);
							Projectile component3 = beamController3.GetComponent<Projectile>();
							float Dmg = sourceProjectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
							component3.baseData.damage = Dmg * 3f;
							component3.AdditionalScaleMultiplier *= 0.66f;
						}
					}

				

				}
				catch (Exception ex)
				{
					ETGModConsole.Log(ex.Message, false);
				}
			}
		
		}

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			//BeamChainController chain = beam.gameObject.GetOrAddComponent<BeamChainController>();
			//chain.beam = beam.GetComponent<BasicBeamController>();
			float procChance = 0.333f; //Chance per second or some shit idk
			GameActor gameActor = hitRigidBody.gameActor;
			if (!gameActor)
			{
				return;
			}
			SpinningDeathController cont = beam.projectile.gameObject.GetComponent<SpinningDeathController>();
			if (UnityEngine.Random.value < BraveMathCollege.SliceProbability(procChance, tickrate) && cont == null)
			{
				AkSoundEngine.PostEvent("Play_EnergySwirl", beam.gameObject);
				bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
				BasicBeamController basicBeam = beam.GetComponent<BasicBeamController>();
				LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", basicBeam);
				LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
				Vector2 bonePosition = basicBeam.GetBonePosition(last.Value);
                if (Owner.PlayerHasActiveSynergy("D-rectional"))
				{
                    for (int i = 0; i < 4; i++)
                    {
                        BeamController beamController3 = BeamToolbox.FreeFireBeamFromPosition(SpinningDeath.SpinningDeathBeam, GameManager.Instance.PrimaryPlayer, bonePosition, 90f * i, 4f, true, true, Flipped ? -360 : 360);

                        Projectile component3 = beamController3.GetComponent<Projectile>();
                        float Dmg = beam.projectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                        component3.baseData.damage = Dmg * 0.66f;
                        component3.AdditionalScaleMultiplier *= 0.66f;
                    }
                }
				else
				{
                    for (int i = 0; i < 2; i++)
                    {
                        BeamController beamController3 = BeamToolbox.FreeFireBeamFromPosition(SpinningDeath.SpinningDeathBeam, GameManager.Instance.PrimaryPlayer, bonePosition, 180f * i, 4f, true, true, Flipped ? -720 : 720);

                        Projectile component3 = beamController3.GetComponent<Projectile>();
                        float Dmg = beam.projectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                        component3.baseData.damage = Dmg * 0.85f;
                        component3.AdditionalScaleMultiplier *= 0.66f;
                    }
                }


                    

			}
		}

		

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;

			return result;
		}

		

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			}
			base.OnDestroy();
		}
	}
}


