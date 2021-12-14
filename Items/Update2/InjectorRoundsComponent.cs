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
		AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
		InjectorRoundsComponent.goopDefs = new List<GoopDefinition>();
		foreach (string text in InjectorRoundsComponent.goops)
		{
			GoopDefinition goopDefinition;
			try
			{
				GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition = gameObject.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			InjectorRoundsComponent.goopDefs.Add(goopDefinition);
		}
		List<GoopDefinition> list = InjectorRoundsComponent.goopDefs;
		base.healthHaver.OnPreDeath += this.OnPreDeath;
	}
	private static List<GoopDefinition> goopDefs;
	private static string[] goops = new string[]
    {
			"assets/data/goops/poison goop.asset",
			"assets/data/goops/napalmgoopquickignite.asset",
			"assets/data/goops/water goop.asset"
	};

	protected override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
		}
		base.OnDestroy();
	}
	private void OnPreDeath(Vector2 obj)
	{
		List<string> Debuffs = new List<string>(){  };
		if (base.aiActor.IsCheezen)
        {
			Debuffs.Add("Cheesed");

		}
		if (base.aiActor.IsFrozen)
		{
			Debuffs.Add("Frozen");
		}
		/*
		if (base.aiActor.IsBlackPhantom)
		{
			Debuffs.Add("Jammed");
		}
		*/
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

		bool FailSafe = false;
		if (Debuffs != null)
		{
			foreach (string DebuffSpecific in Debuffs)
			{
				if (DebuffSpecific != null)
                {
					//ETGModConsole.Log("Debuffs In List: " + DebuffSpecific);
					FailSafe = true;
				}
			}
			if (FailSafe == true)
            {
				string ChosenDebuff = BraveUtility.RandomElement<string>(Debuffs);
				if (ChosenDebuff != null)
				{
					//ETGModConsole.Log("Chosen Debuff: " + ChosenDebuff);
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
				//ETGModConsole.Log("No Debuffs In List");
			}
		}
	}
	public static GoopDefinition CharmGoopDef;
	public static GoopDefinition CheeseDef;
	public float GoopPoolSize;
}
