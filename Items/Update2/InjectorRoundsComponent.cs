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

	public bool ContainsSpecificValue(GameActorEffect gameActorEffect)
	{
		if (InjectorRounds.DebuffKeys.ContainsKey(gameActorEffect)) { return true; }
        if (InjectorRounds.DebuffKeys.ContainsValue(gameActorEffect.effectIdentifier)) { return true; }
		return false;
    }


    private void OnPreDeath(Vector2 obj)
	{
		Dictionary<string, GameActorEffect> effectKeys = new Dictionary<string, GameActorEffect>();
		List<string> Keys = new List<string>();
		List<GameActorEffect> list = PlanetsideReflectionHelper.ReflectGetField<List<GameActorEffect>>(typeof(AIActor), "m_activeEffects", base.aiActor);
		foreach (GameActorEffect gameActorEffect in list)
		{

            

            if (gameActorEffect != null && gameActorEffect.effectIdentifier != null && InjectorRounds.BlacklistedNames.Contains(gameActorEffect.effectIdentifier))
			{
				//no
			}
            else if (gameActorEffect != null && (InjectorRounds.DebuffKeys.ContainsKey(gameActorEffect)) && !InjectorRounds.BlacklistedKeys.Contains(gameActorEffect))
            {
				string LocalValue = gameActorEffect.effectIdentifier ?? "H";
                InjectorRounds.DebuffKeys.TryGetValue(gameActorEffect, out LocalValue);
				if (LocalValue != null && LocalValue.ToLower() != "h") { Keys.Add(LocalValue); }

			}
			else if (!gameActorEffect.GetType().IsSubclassOf(typeof(GameActorDecorationEffect)))
            {
				//ETGModConsole.Log("starting logging of effect storage");
                string LocalValue = gameActorEffect.effectIdentifier != null ? gameActorEffect.effectIdentifier : UnityEngine.Random.Range(1, 1000).ToString();
				if (gameActorEffect.effectIdentifier != null && !InjectorRounds.BlacklistedNames.Contains(gameActorEffect.effectIdentifier))
				{
                    //ETGModConsole.Log("name of key: " + LocalValue);
                    Keys.Add(LocalValue);
                    effectKeys.Add(LocalValue, gameActorEffect);

                    InjectorRounds.DebuffKeys.Add(gameActorEffect, gameActorEffect.effectIdentifier);
                    //ETGModConsole.Log("added: " + gameActorEffect.effectIdentifier + " to effectKeys with key: " + LocalValue);
                }
			}
		}
		if (Keys.Count > 0 && Keys != null)
		{
			string ChosenDebuff = BraveUtility.RandomElement<string>(Keys);
			if (ChosenDebuff != null)
			{
				if (InjectorRounds.GoopKeys.ContainsKey(ChosenDebuff))
                {
					GoopDefinition def = null;
                    InjectorRounds.GoopKeys.TryGetValue(ChosenDebuff, out def);
					if (def != null)	
					{
						DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(def).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 0.5f, false);
					}
				}
				else if (InjectorRounds.DebuffKeys.ContainsValue(ChosenDebuff))
                {

					GameActorEffect effectToCopy = null;
					//ETGModConsole.Log("attempting access of debuff storage");
					effectKeys.TryGetValue(ChosenDebuff, out effectToCopy);
					if (effectToCopy != null)
                    {
						GoopDefinition def = InjectorRounds.templateDef;
						def.damagesEnemies = true;
						def.name = effectToCopy.effectIdentifier + " Dupe Goop";
						if (effectToCopy is GameActorHealthEffect) { def.HealthModifierEffect = effectToCopy as GameActorHealthEffect; def.AppliesDamageOverTime = true; }
						if (effectToCopy is GameActorSpeedEffect) { def.SpeedModifierEffect = effectToCopy as GameActorSpeedEffect; def.AppliesSpeedModifier = true; }
						if (effectToCopy is GameActorCharmEffect) { def.CharmModifierEffect = effectToCopy as GameActorCharmEffect; def.AppliesCharm = true; }
						if (effectToCopy is GameActorCheeseEffect) { def.CheeseModifierEffect = effectToCopy as GameActorCheeseEffect; def.AppliesCheese = true; }
						if (effectToCopy is GameActorFireEffect) { def.fireEffect = effectToCopy as GameActorFireEffect; def.fireBurnsEnemies = true; }
						if (effectToCopy.TintColor != null) 
						{
							Color colorMult = effectToCopy.TintColor * 255;
							byte r = (byte)colorMult.r;
							byte g = (byte)colorMult.g;
							byte b = (byte)colorMult.b;
							def.baseColor32 = new Color32(r, g, b, byte.MaxValue); 
						}
						def.CanBeIgnited = false;
						def.damagePerSecondtoEnemies = 0;
						def.damagesPlayers = false;
                        def.AppliesDamageOverTime = false;
						def.damageToPlayers = 0;
                         

                        def.lifespan = Mathf.Max(7f, effectToCopy.duration);
						DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(def).TimedAddGoopCircle(base.aiActor.sprite.WorldBottomCenter, GoopPoolSize, 0.5f, false);
					}
				}
			}
		}
	}
	public float GoopPoolSize;
}
