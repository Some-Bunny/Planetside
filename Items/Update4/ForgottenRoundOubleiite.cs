﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	public class ForgottenRoundOubliette : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 1";
			string resourcePath = "Planetside/Resources/forgottenroundsewer.png";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundOubliette warVase = gameObject.AddComponent<ForgottenRoundOubliette>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Forgotten First Chamber";
			string longDesc = "This rare, yet false artifact indicates mastery of the first hidden chamber.\n\nDespite being a false copy of a Master Round, its time spent within the mutagenic conditions of the Oubliette has bestowed it similar power to that of a real master round.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog" ,"Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
			ForgottenRoundOubliette.ForgottenRoundOublietteID = warVase.PickupObjectId;
		}
		public static int ForgottenRoundOublietteID;

		public override void Pickup(PlayerController player)
		{base.Pickup(player);}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}		
	}
	public class ForgottenRoundAbbey : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 2";
			string resourcePath = "Planetside/Resources/forgottenroundabbey.png";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundAbbey warVase = gameObject.AddComponent<ForgottenRoundAbbey>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Forgotten Second Chamber";
			string longDesc = "This potent, yet false artifact indicates mastery of the second hidden chamber.\n\nDuring his time sealed away in the Abbey, this false round was bathed in the power of the god-like beings that reside in this Chamber.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog", "Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
			ForgottenRoundAbbey.ForgottenRoundAbbeyID = warVase.PickupObjectId;
		}
		public static int ForgottenRoundAbbeyID;

		public override void Pickup(PlayerController player)
		{ base.Pickup(player); }
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}
	}
	public class ForgottenRoundRNG : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 3";
			string resourcePath = "Planetside/Resources/forgottenroundrng.png";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundRNG warVase = gameObject.AddComponent<ForgottenRoundRNG>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Forgotten Fourth Chamber";
			string longDesc = "This extraordinary, yet false artifact indicates mastery of the fourth hidden chamber.\n\nWhile plotting his masters return, Agunim decorated this false round with his own likeness as a means to test his magics, and armanents, on potential sacrifices.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog", "Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
			ForgottenRoundRNG.ForgottenRoundRNGID = warVase.PickupObjectId;
		}
		public static int ForgottenRoundRNGID;

		public override void Pickup(PlayerController player)
		{ base.Pickup(player); }
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}
	}
}