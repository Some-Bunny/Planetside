using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SynergyAPI
{
	public class AdvancedGunFormeData : ScriptableObject
	{
		public AdvancedGunFormeData()
		{
			RequiresSynergy = true;
		}

		public bool IsValid(PlayerController p)
		{
			return !RequiresSynergy || p.PlayerHasActiveSynergy(RequiredSynergy);
		}

		public bool RequiresSynergy;
		public string RequiredSynergy;
		public int FormeID;
	}
}
