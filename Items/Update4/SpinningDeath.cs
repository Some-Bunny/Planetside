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
			bool flag2 = this.timer <= 0f;
			if (flag2)
			{
				this.DoTick();
				this.timer = this.tickDelay;
			}
		}

		private void DoTick()
		{
			LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", this.basicBeamController);
			for (int k = 0; k < linkedList.Count; k++)
            {
				if (UnityEngine.Random.value > 0.33f)
                {
					Vector2 bonePosition = this.basicBeamController.GetBonePosition(linkedList.ElementAt(k));
					ParticleSystem particleSystem = UnityEngine.Object.Instantiate(StaticVFXStorage.PerfectedParticleSystem).GetComponent<ParticleSystem>();
					particleSystem.gameObject.transform.parent = this.projectile.transform;
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
			}
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
            string resourceName = "Planetside/Resources/spinningDeath.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SpinningDeath>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "LET IT RIP!";
			string longDesc = "Embeds laser-cutters into some of your projectiles. \n\nThe remnants of a crowdfunded laser blender that ended up very, VERY horribly. Keep your fingers away...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;

			SpinningDeath.SpinningDeathID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

            List<string> BeamAnimPaths = new List<string>()
            {
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_mid_001",

            };
            List<string> StartAnimPaths = new List<string>()
            {
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_mid_001",

			};
			List<string> EndAnimPaths = new List<string>()
			{"Planetside/Resources/Beams/SpinningDeath/spinningDeath_end_001",

			};

			List<string> ImpactAnimPaths = new List<string>()
            {
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_impact_001",
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_impact_002",
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_impact_001",
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_impact_003",
			};
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefab(
				"Planetside/Resources/Beams/SpinningDeath/spinningDeath_mid_001",
                new Vector2(5, 5),
                new Vector2(0, 0),
                BeamAnimPaths,
                1,
                //Beam Impact
                ImpactAnimPaths,
                1,
                new Vector2(5, 5),
                new Vector2(0, 0),
				//End of the Beam
				EndAnimPaths,
                1,
                null,
                null,
                //Start of the Beam
                StartAnimPaths,
                12,
                new Vector2(5, 5),
                new Vector2(0, 0)
                );
            projectile.gameObject.SetActive(false);
			projectile.baseData.damage = 30;
			projectile.baseData.range = 2;
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
			emiss.EmissiveColorPower = 10f;
			emiss.EmissivePower = 100f;
			
			

			SpinningDeathBeam = projectile;
		}
		public static Projectile SpinningDeathBeam; 
		public static int SpinningDeathID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			float procChance = 0.25f;
			procChance *= effectChanceScalar;
			SpinningDeathController cont = sourceProjectile.gameObject.GetComponent<SpinningDeathController>();
			if (UnityEngine.Random.value <= procChance && cont == null)
			{
				try
				{
					AkSoundEngine.PostEvent("Play_EnergySwirl", sourceProjectile.gameObject);
					sourceProjectile.baseData.speed *= 0.3f;
					sourceProjectile.UpdateSpeed();

					PierceProjModifier spook = sourceProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
					spook.penetration = 10;
					spook.penetratesBreakables = true;


					bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
					for (int i = 0; i < 2; i++)
					{
						BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(SpinningDeathBeam, base.Owner, sourceProjectile.gameObject, sourceProjectile.gameObject.transform.PositionVector2(), false, 180f * i, 100f, true, true, Flipped ? -720 : 720);
						Projectile component3 = beamController3.GetComponent<Projectile>();
						float Dmg = sourceProjectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
						component3.baseData.damage = Dmg * 4f;
						component3.AdditionalScaleMultiplier *= 0.66f;
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
			float procChance = 0.4f; //Chance per second or some shit idk
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

				for (int i = 0; i < 2; i++)
				{
					BeamController beamController3 = BeamToolbox.FreeFireBeamFromPosition(SpinningDeath.SpinningDeathBeam, GameManager.Instance.PrimaryPlayer, bonePosition, 180f*i, 4f, true, true, Flipped ? -720 : 720);
					
					Projectile component3 = beamController3.GetComponent<Projectile>();
					float Dmg = beam.projectile.baseData.damage *= base.Owner != null ? base.Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
					component3.baseData.damage = Dmg * 0.85f;
					component3.AdditionalScaleMultiplier *= 0.66f;
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

		

		protected override void OnDestroy()
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


