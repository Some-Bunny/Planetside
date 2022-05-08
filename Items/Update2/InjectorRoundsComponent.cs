using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Planetside;
using Brave.BulletScript;
using Dungeonator;

public class InjectorRoundsComponent : BraveBehaviour
{

	public InjectorRoundsComponent()
	{
		this.GoopPoolSize = 2.5f;
	}
	public void Start()
	{
		base.healthHaver.OnPreDeath += this.OnPreDeath;
	}
	

	protected override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
		}
		base.OnDestroy();
	}

	public Dictionary<GameActorEffect, string> DebuffKeys = new Dictionary<GameActorEffect, string>()
	{
		{DebuffStatics.irradiatedLeadEffect, "poison" },
		{DebuffStatics.hotLeadEffect, "fire" },
		{DebuffStatics.cheeseeffect, "cheese" },
		{DebuffStatics.charmingRoundsEffect, "charm" },
		{DebuffStatics.frostBulletsEffect, "freeze" },
		{DebuffStatics.chaosBulletsFreeze, "freeze" },
		{DebuffStatics.greenFireEffect, "hellfire" },
		{DebuffLibrary.Possessed, "possessed" },
		{DebuffLibrary.Frailty, "frailty" },
		{DebuffLibrary.Corrosion, "tarnish" },
	};
	public List<GameActorEffect> BlacklistedKeys = new List<GameActorEffect>()
	{
		DebuffLibrary.HeatStroke,
		DebuffLibrary.brokenArmor,
		DebuffLibrary.Holy,
	};


	public Dictionary<string, GoopDefinition> GoopKeys = new Dictionary<string, GoopDefinition>()
	{
		{"poison",  EasyGoopDefinitions.PoisonDef},
		{"fire",  EasyGoopDefinitions.FireDef},
		{"cheese",  EasyGoopDefinitions.CheeseDef},
		{"charm",  EasyGoopDefinitions.CharmGoopDef},
		{"freeze",  EasyGoopDefinitions.WaterGoop},
		{"hellfire",  EasyGoopDefinitions.GreenFireDef},
		{"possessed",  DebuffLibrary.PossesedPuddle},
		{"frailty",  DebuffLibrary.FrailPuddle},
		{"tarnish",  DebuffLibrary.TarnishedGoop},

	};



	private void OnPreDeath(Vector2 obj)
	{
		Dictionary<string, GameActorEffect> effectKeys = new Dictionary<string, GameActorEffect>();
		List<string> Keys = new List<string>();
		List<GameActorEffect> list = PlanetsideReflectionHelper.ReflectGetField<List<GameActorEffect>>(typeof(AIActor), "m_activeEffects", base.aiActor);
		foreach (GameActorEffect gameActorEffect in list)
		{
			
			if (gameActorEffect != null && DebuffKeys.ContainsKey(gameActorEffect) && !BlacklistedKeys.Contains(gameActorEffect))
            {
				string LocalValue = "H";
				DebuffKeys.TryGetValue(gameActorEffect, out LocalValue);
				if (LocalValue != null && LocalValue.ToLower() != "h") { Keys.Add(LocalValue); }

			}
			else if (!gameActorEffect.GetType().IsSubclassOf(typeof(GameActorDecorationEffect)))
            {
				ETGModConsole.Log("starting logging of effect storage");
                string LocalValue = gameActorEffect.effectIdentifier != null ? gameActorEffect.effectIdentifier : UnityEngine.Random.Range(1, 1000).ToString();
				ETGModConsole.Log("name of key: " + LocalValue);
				Keys.Add(LocalValue);
				effectKeys.Add(LocalValue, gameActorEffect);
				ETGModConsole.Log("added: " + gameActorEffect.effectIdentifier + " to effectKeys with key: "+ LocalValue);

			}
		}
		if (Keys.Count > 0 && Keys != null)
		{
			string ChosenDebuff = BraveUtility.RandomElement<string>(Keys);
			if (ChosenDebuff != null)
			{
				if (GoopKeys.ContainsKey(ChosenDebuff))
                {
					GoopDefinition def = null;
					GoopKeys.TryGetValue(ChosenDebuff, out def);
					if (def != null)
					{
						DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(def).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 0.5f, false);
					}
				}
				else if (effectKeys.ContainsKey(ChosenDebuff))
                {

					GameActorEffect effectToCopy = null;
					ETGModConsole.Log("attempting access of debuff storage");
					effectKeys.TryGetValue(ChosenDebuff, out effectToCopy);
					if (effectToCopy != null)
                    {
						GoopDefinition def = InjectorRounds.templateDef;
						def.damagesEnemies = true;
						def.name = effectToCopy.effectIdentifier + " Dupe Goop";
						if (effectToCopy is GameActorHealthEffect) { def.HealthModifierEffect = effectToCopy as GameActorHealthEffect; }
						if (effectToCopy is GameActorSpeedEffect) { def.SpeedModifierEffect = effectToCopy as GameActorSpeedEffect; }
						if (effectToCopy is GameActorCharmEffect) { def.CharmModifierEffect = effectToCopy as GameActorCharmEffect; }
						if (effectToCopy is GameActorCheeseEffect) { def.CheeseModifierEffect = effectToCopy as GameActorCheeseEffect; }
						if (effectToCopy is GameActorFireEffect) { def.fireEffect = effectToCopy as GameActorFireEffect; }
						if (effectToCopy.TintColor != null) 
						{
							Color colorMult = effectToCopy.TintColor * 255;
							byte r = (byte)colorMult.r;
							byte g = (byte)colorMult.g;
							byte b = (byte)colorMult.b;
							def.baseColor32 = new Color32(r, g, b, byte.MaxValue); 
						}
						def.CanBeIgnited = false;
						def.damagesPlayers = false;
						def.AppliesDamageOverTime = true;
						def.lifespan = Mathf.Max(7f, effectToCopy.duration);
						DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(def).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 0.5f, false);
					}
				}
			}
		}
		//List<string> Debuffs = new List<string>(){  };




					/*
					if (base.aiActor.IsCheezen)
					{
						Debuffs.Add("Cheesed");

					}
					if (base.aiActor.IsFrozen)
					{
						Debuffs.Add("Frozen");
					}




					if (base.aiActor.CanTargetEnemies && !base.aiActor.CanTargetPlayers)
					{
						Debuffs.Add("Charmed");
					}
					GameActorEffect effect = base.aiActor.GetEffect(DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(InjectorRoundsComponent.goopDefs[0]).goopDefinition.HealthModifierEffect.effectIdentifier);
					bool flag2 = effect != null;
					if (flag2)
					{
						Debuffs.Add("Poisoned");
					}		
					GameActorEffect lol = base.aiActor.GetEffect(EffectResistanceType.Fire);
					bool yeet = lol != null;
					if (yeet)
					{
						Debuffs.Add("Burning");
					}

					var hand = base.aiActor.transform.Find("PosessedVFX");
					bool poss = hand != null;
					if (poss)
					{
						Debuffs.Add("Possessed");
					}
					var Frail = base.aiActor.transform.Find("FrailtyVFX");
					bool fra = Frail != null;
					if (fra)
					{
						Debuffs.Add("Frailed");
					}
					*/
					//GameAc


					/*
					bool FailSafe = false;
					if (Debuffs != null)
					{
						foreach (string DebuffSpecific in Debuffs)
						{
							if (DebuffSpecific != null)
							{
								FailSafe = true;
							}
						}
						if (FailSafe == true)
						{
							string ChosenDebuff = BraveUtility.RandomElement<string>(Debuffs);
							if (ChosenDebuff != null)
							{
								if (ChosenDebuff == "Poisoned")
								{
									DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(InjectorRoundsComponent.goopDefs[0]).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 1, false);
								}
								if (ChosenDebuff == "Cheesed")
								{
									DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CheeseDef).TimedAddGoopCircle(base.aiActor.sprite.WorldCenter, GoopPoolSize, 1f, false);
								}
								if (ChosenDebuff == "Charmed")
								{
									DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CharmGoopDef).TimedAddGoopCircle(base.aiActor.sprite.WorldCenter, GoopPoolSize, 1f, false);
								}
								if (ChosenDebuff == "Frozen")
								{
									DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(InjectorRoundsComponent.goopDefs[2]).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 1, false);
									DeadlyDeadlyGoopManager.FreezeGoopsCircle(base.aiActor.sprite.WorldCenter, 4f);

								}
								if (ChosenDebuff == "Jammed")
								{
									DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.HolyPuddle);
									goopManagerForGoopType.TimedAddGoopCircle(base.aiActor.sprite.WorldCenter, GoopPoolSize, 1f, false);
								}
								if (ChosenDebuff == "Burning")
								{
									DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(InjectorRoundsComponent.goopDefs[1]).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 1, false);
								}
								if (ChosenDebuff == "Possessed")
								{
									DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.PossesedPuddle);
									goopManagerForGoopType.TimedAddGoopCircle(base.aiActor.sprite.WorldCenter, GoopPoolSize, 1f, false);
								}
								if (ChosenDebuff == "Frailed")
								{
									DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.FrailPuddle);
									goopManagerForGoopType.TimedAddGoopCircle(base.aiActor.sprite.WorldCenter, GoopPoolSize, 1f, false);
								}
							}
						}
						if (FailSafe == false)
						{
						}
					}
					*/
	}
	public static GoopDefinition CharmGoopDef;
	public static GoopDefinition CheeseDef;
	public float GoopPoolSize;
}
