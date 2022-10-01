using System;
using ItemAPI;
using UnityEngine;
using HarmonyLib;

namespace Planetside
{
	public static class DebuffLibrary
	{

		public static void Init()
		{
			InitPossessedGoop();
			InitCursebulonGoop();
			InitFrailPuddle();
			InitTarnishedGoop();

		}
		/*
		 *  Gun crossbow = ETGMod.Databases.Items["triple_crossbow"] as Gun;
            GameActorSpeedEffect speedEffect = crossbow.DefaultModule.projectiles[0].speedEffect;
            WebSlow = new GameActorSpeedEffect
            {
                duration = 10,
                TintColor = new Color32(184, 181, 147, 255),
                DeathTintColor = new Color32(184, 181, 147, 255),
                effectIdentifier = "web",
                AppliesTint = true,
                AppliesDeathTint = true,
                resistanceType = EffectResistanceType.None,
                SpeedMultiplier = 0.166f,
                OverheadVFX = speedEffect.OverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                OutlineTintColor = new Color32(184, 181, 147, 255),
                PlaysVFXOnActor = false
            };

            BlobSlow = new GameActorSpeedEffect
            {
                duration = 6,
                TintColor = new Color32(213, 77, 77, 255),
                DeathTintColor = new Color32(213, 77, 77, 255),
                effectIdentifier = "blob",
                AppliesTint = true,
                AppliesDeathTint = true,
                resistanceType = EffectResistanceType.None,
                SpeedMultiplier = 0.6f,
                OverheadVFX = speedEffect.OverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                OutlineTintColor = new Color32(213, 77, 77, 255),
                PlaysVFXOnActor = false
            };
		*/
		public static GameActorSpeedEffect MopWebEffect = new GameActorSpeedEffect
		{
            duration = 10,
            TintColor = new Color32(184, 181, 147, 255),
            DeathTintColor = new Color32(184, 181, 147, 255),
            effectIdentifier = "web",
            AppliesTint = true,
            AppliesDeathTint = true,
            resistanceType = EffectResistanceType.None,
            SpeedMultiplier = 0.166f,
            OverheadVFX = (ETGMod.Databases.Items["triple_crossbow"] as Gun).DefaultModule.projectiles[0].speedEffect.OverheadVFX,
            AffectsEnemies = true,
            AffectsPlayers = false,
            AppliesOutlineTint = false,
            OutlineTintColor = new Color32(184, 181, 147, 255),
            PlaysVFXOnActor = false
        };

        public static GameActorSpeedEffect MopBlobEffect = new GameActorSpeedEffect
        {
            duration = 6,
            TintColor = new Color32(213, 77, 77, 255),
            DeathTintColor = new Color32(213, 77, 77, 255),
            effectIdentifier = "blob",
            AppliesTint = true,
            AppliesDeathTint = true,
            resistanceType = EffectResistanceType.None,
            SpeedMultiplier = 0.6f,
            OverheadVFX = (ETGMod.Databases.Items["triple_crossbow"] as Gun).DefaultModule.projectiles[0].speedEffect.OverheadVFX,
            AffectsEnemies = true,
            AffectsPlayers = false,
            AppliesOutlineTint = false,
            OutlineTintColor = new Color32(213, 77, 77, 255),
            PlaysVFXOnActor = false
        };


        public static InfectedEnemyEffect InfectedEnemyEffect = new InfectedEnemyEffect
		{
			crystalNum = 4,
			debrisAngleVariance = 30,
			AffectsPlayers = false,
			crystalRot = 4,
			

			AffectsEnemies = true,
			stackMode = GameActorEffect.EffectStackingMode.Stack,
			duration = 3600,
			FreezeAmount = 0.1f,
			resistanceType = EffectResistanceType.None,
			UnfreezeDamagePercent = 1,
			debrisMaxForce = 1,
			effectIdentifier = "Infection",
			FreezeCrystals = InfectedEnemyEffect.GeneratedInfectionCrystals,
			TintColor = new Color(0.01f, 0.5f, 0.5f),
			AppliesTint = true,
			
		};

		public static InfectedBossEffect InfectedBossEffect = new InfectedBossEffect
		{
			crystalNum = 9,
			debrisAngleVariance = 30,
			AffectsPlayers = false,
			crystalRot = 4,
			AffectsEnemies = true,
			stackMode = GameActorEffect.EffectStackingMode.Stack,
			duration = 3600,
			FreezeAmount = 0.1f,
			resistanceType = EffectResistanceType.None,
			UnfreezeDamagePercent = 1,
			debrisMaxForce = 1,
			effectIdentifier = "Infection",
			FreezeCrystals = InfectedBossEffect.GeneratedInfectionCrystals,
			TintColor = new Color(0.01f, 0.5f, 0.5f),
			AppliesTint = true,
		};

		public static BrainHostDummyBuff BrainHostBuff = new BrainHostDummyBuff
		{
			effectIdentifier = "BrainHost",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 1f,
			AppliesTint = true,
			AppliesDeathTint = true,
			OverheadVFX = BrainHostDummyBuff.BrainHostVFX,
			AffectsPlayers = false,
			stackMode = GameActorEffect.EffectStackingMode.Refresh
		};


		public static TarnishEffect Corrosion = new TarnishEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Tarnish",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 15f,
			//TintColor = new Color(0.2f, 3f, 0.05f),
			AppliesTint = true,
			AppliesDeathTint = true,
			PlaysVFXOnActor = false,
			OverheadVFX = TarnishEffect.TarnishVFXObject,
			AffectsPlayers = false,
			stackMode = GameActorEffect.EffectStackingMode.Refresh
		};


		public static FrailtyHealthEffect Frailty = new FrailtyHealthEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Frailty",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.Poison,
			duration = 8f,
			TintColor = new Color(3f, 3f, 3f),
			AppliesTint = true,
			AppliesDeathTint = true,
			//PlaysVFXOnActor = true
			OverheadVFX = FrailtyHealthEffect.frailtyVFXObject
		};
		public static PossessedEffect Possessed = new PossessedEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Possession",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 60f,
			TintColor = new Color(3f, 2f, 0f),
			AppliesTint = true,
			AppliesDeathTint = false,
			OverheadVFX = PossessedEffect.posessedVFXObject


		};
		public static HeatStrokeEffect HeatStroke = new HeatStrokeEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Heat Stroke",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.Fire,
			duration = 0.2f,
			TintColor = new Color(0, 0, 0, 0),
			AppliesTint = true,
			AppliesDeathTint = false,
			OverheadVFX = HeatStrokeEffect.heatstrokeVFXObject,
			maxStackedDuration = 1,
			
			stackMode = GameActorEffect.EffectStackingMode.Refresh
        };
		public static BrokenArmorEffect brokenArmor = new BrokenArmorEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "broken Armor",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 6f,
			//TintColor = new Color(3f, 2f, 0f),
			AppliesTint = false,
			AppliesDeathTint = false,

		};

		public static HolyBlessingEffect Holy = new HolyBlessingEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Holy",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 5f,
			AppliesTint = false,
			AppliesDeathTint = false,

		};

		public static CurseSpeedEffect CurseSpeedGoop = new CurseSpeedEffect
		{
			effectIdentifier = "CurseGoop",
			AffectsEnemies = false,
			resistanceType = EffectResistanceType.None,
			duration = 0.1f,
		};


		public static void InitPossessedGoop()
        {
			GoopDefinition possesedPuddle = ScriptableObject.CreateInstance<GoopDefinition>();
			possesedPuddle.name = "Possessive Goop";
			possesedPuddle.CanBeIgnited = false;
			possesedPuddle.damagesEnemies = false;
			possesedPuddle.damagesPlayers = false;
			possesedPuddle.baseColor32 = new Color32(49, 100, 100, byte.MaxValue);
			possesedPuddle.goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/possessed_standard_base_001.png");
			possesedPuddle.AppliesDamageOverTime = true;
			possesedPuddle.HealthModifierEffect = DebuffLibrary.Possessed;
			possesedPuddle.goopDamageTypeInteractions = new System.Collections.Generic.List<GoopDefinition.GoopDamageTypeInteraction>()
			{
				new GoopDefinition.GoopDamageTypeInteraction()
                {
					freezesGoop = false,
					electrifiesGoop = false,
					ignitionMode = GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.IGNITE,
					damageType = CoreDamageTypes.Fire
                },
			};
			PossesedPuddle = possesedPuddle;
		}
		public static GoopDefinition PossesedPuddle;

		public static void InitTarnishedGoop()
		{
			GoopDefinition GoopDef = ScriptableObject.CreateInstance<GoopDefinition>();
			GoopDef.name = "Tarished Goop";
			GoopDef.CanBeIgnited = false;
			GoopDef.damagesEnemies = false;
			GoopDef.damagesPlayers = false;
			GoopDef.lifespan = 6f;
			GoopDef.baseColor32 = new Color32(156, 155, 0, byte.MaxValue);
			GoopDef.goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/goop_standard_base_001.png");
			GoopDef.AppliesDamageOverTime = true;
			GoopDef.HealthModifierEffect = DebuffLibrary.Corrosion;
			GoopDef.goopDamageTypeInteractions = new System.Collections.Generic.List<GoopDefinition.GoopDamageTypeInteraction>()
			{
				new GoopDefinition.GoopDamageTypeInteraction()
				{
					freezesGoop = false,
					electrifiesGoop = false,
					ignitionMode = GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.DOUSE,
					damageType = CoreDamageTypes.Water
				},
			};
			TarnishedGoop = GoopDef;
		}
		public static GoopDefinition TarnishedGoop;


		public static void InitCursebulonGoop()
		{
			GoopDefinition cursebulonGoop = ScriptableObject.CreateInstance<GoopDefinition>();
			cursebulonGoop.name = "Cursebulon Goop";
			cursebulonGoop.CanBeIgnited = false;
			cursebulonGoop.damagesEnemies = false;
			cursebulonGoop.damagesPlayers = false;
			cursebulonGoop.baseColor32 = new Color32(105, 0, 105, byte.MaxValue);
			cursebulonGoop.goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/goop_standard_base_001.png");
			cursebulonGoop.AppliesDamageOverTime = false;
			cursebulonGoop.lifespan = 11.5f;
			cursebulonGoop.usesAmbientGoopFX = true;
			cursebulonGoop.ambientGoopFX = ObjectMakers.MakeObjectIntoVFX(RandomPiecesOfStuffToInitialise.cursepoofvfx);
			cursebulonGoop.ambientGoopFXChance = 0.0005f;
			cursebulonGoop.AppliesSpeedModifier = true;
			cursebulonGoop.AppliesSpeedModifierContinuously = true;
			cursebulonGoop.SpeedModifierEffect = CurseSpeedGoop;
			cursebulonGoop.goopDamageTypeInteractions = new System.Collections.Generic.List<GoopDefinition.GoopDamageTypeInteraction>()
			{
				new GoopDefinition.GoopDamageTypeInteraction()
				{
					freezesGoop = false,
					electrifiesGoop = false,
					ignitionMode = GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.NONE,
					damageType = CoreDamageTypes.None
				},
			};
			CursebulonGoop = cursebulonGoop;
		}
		public static GoopDefinition CursebulonGoop;


		public static void InitFrailPuddle()
		{
			GoopDefinition frailPuddle = ScriptableObject.CreateInstance<GoopDefinition>();
			frailPuddle.name = "Frail Goop";
			frailPuddle.CanBeIgnited = false;
			frailPuddle.damagesEnemies = false;
			frailPuddle.damagesPlayers = false;
			frailPuddle.baseColor32 = new Color32(170, 0, 170, byte.MaxValue);
			frailPuddle.goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/goop_standard_base_001.png");
			frailPuddle.AppliesDamageOverTime = true;
			frailPuddle.HealthModifierEffect = DebuffLibrary.Frailty;
			frailPuddle.goopDamageTypeInteractions = new System.Collections.Generic.List<GoopDefinition.GoopDamageTypeInteraction>()
			{
				new GoopDefinition.GoopDamageTypeInteraction()
				{
					freezesGoop = false,
					electrifiesGoop = false,
					ignitionMode = GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.NONE,
					damageType = CoreDamageTypes.Poison
				},
			};
			FrailPuddle = frailPuddle;
		}

		public static GoopDefinition FrailPuddle;


		public static GoopDefinition HolyPuddle = new GoopDefinition
		{
			CanBeIgnited = false,
			damagesEnemies = false,
			damagesPlayers = false,
			baseColor32 = new Color32(255, 255, 255, byte.MaxValue),
			goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/possessed_standard_base_001.png"),
			AppliesDamageOverTime = true,
			HealthModifierEffect = DebuffLibrary.Holy

		};
		public static Color LightGreen = new Color(0.55f, 1.76428568f, 0.871428549f);
	}
}
