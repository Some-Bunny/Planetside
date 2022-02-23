using System;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	public static class DebuffLibrary
	{
		public static void Init()
        {
			InitPossessedGoop();
			InitCursebulonGoop();
			InitFrailPuddle();
		}


		public static FrailtyHealthEffect Frailty = new FrailtyHealthEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Frailty",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.Poison,
			duration = 5f,
			TintColor = new Color(3f, 3f, 3f),
			AppliesTint = true,
			AppliesDeathTint = true,
			PlaysVFXOnActor = true

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

		};
		public static HeatStrokeEffect HeatStroke = new HeatStrokeEffect
		{
			DamagePerSecondToEnemies = 0f,
			effectIdentifier = "Heat Stroke",
			AffectsEnemies = true,
			resistanceType = EffectResistanceType.None,
			duration = 5f,
			TintColor = new Color(0, 0, 0, 0),
			AppliesTint = true,
			AppliesDeathTint = false,

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
			PossesedPuddle = possesedPuddle;
		}
		public static GoopDefinition PossesedPuddle;


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
			cursebulonGoop.lifespan = 13;
			cursebulonGoop.usesAmbientGoopFX = true;
			cursebulonGoop.ambientGoopFX = ObjectMakers.MakeObjectIntoVFX(RandomPiecesOfStuffToInitialise.cursepoofvfx);
			cursebulonGoop.ambientGoopFXChance = 0.0005f;
			cursebulonGoop.AppliesSpeedModifier = true;
			cursebulonGoop.AppliesSpeedModifierContinuously = true;
			cursebulonGoop.SpeedModifierEffect = CurseSpeedGoop;
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
