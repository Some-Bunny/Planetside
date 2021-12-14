using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Gungeon;

namespace Planetside
{
    public class RevenantSynergyPlus : MonoBehaviour
    {
		public void Awake()
		{

			this.m_gun = base.GetComponent<Gun>();
			this.m_gun.PostProcessProjectile += this.PostProcessProjectile;
		}

		public void PostProcessProjectile(Projectile projectile)
		{
			PlayerController player = projectile.PossibleSourceGun.CurrentOwner as PlayerController;
			if (player.PlayerHasActiveSynergy(SynergyNameToCheck))
            {
				projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
				//PlayerOrbitalItem.CreateOrbital(player, RandomPiecesOfStuffToInitialise.SoulSynergyGuon, false);
			}
		}
		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{

			PlayerController playerController = arg1.Owner as PlayerController;
			if (arg2 != null && arg2.healthHaver != null)
			{
				AkSoundEngine.PostEvent("Play_CHR_skull_death_01", playerController.gameObject);
				GameObject oerb = PlayerOrbitalItem.CreateOrbital(playerController, RandomPiecesOfStuffToInitialise.SoulSynergyGuon, false);
				SelfDestructComponent nuke = oerb.AddComponent<SelfDestructComponent>();
				nuke.maxDuration = 10;
			}
		}
		public string SynergyNameToCheck;
		private Gun m_gun;
	}

}
