using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace SynergyAPI
{
	/// <summary>
	/// Component that allows switching gun forms when pressing reload with a full clip.
	/// </summary>
	public class AdvancedGunFormeSynergyProcessor : MonoBehaviour
	{
		private void Awake()
		{
			m_gun = GetComponent<Gun>();
			Gun gun = m_gun;
			gun.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(gun.OnReloadPressed, new Action<PlayerController, Gun, bool>(HandleReloadPressed));
		}

		private void Update()
		{
			if (m_gun && !m_gun.CurrentOwner && CurrentForme != 0)
			{
				ChangeForme(Formes[0]);
				CurrentForme = 0;
			}
		}

		private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
		{
			if (manual && !sourceGun.IsReloading)
			{
				int nextValidForme = GetNextValidForme(ownerPlayer);
				if (nextValidForme != CurrentForme)
				{
					ChangeForme(Formes[nextValidForme]);
					CurrentForme = nextValidForme;
				}
			}
		}

		private int GetNextValidForme(PlayerController ownerPlayer)
		{
			for (int i = 0; i < Formes.Length; i++)
			{
				int num = (i + CurrentForme) % Formes.Length;
				if (num != CurrentForme)
				{
					if (Formes[num].IsValid(ownerPlayer))
					{
						return num;
					}
				}
			}
			return CurrentForme;
		}

		private void ChangeForme(AdvancedGunFormeData targetForme)
		{
			Gun gun = PickupObjectDatabase.GetById(targetForme.FormeID) as Gun;
			m_gun.TransformToTargetGun(gun);
			if (m_gun.encounterTrackable && gun.encounterTrackable)
			{
				m_gun.encounterTrackable.journalData.PrimaryDisplayName = gun.encounterTrackable.journalData.PrimaryDisplayName;
				m_gun.encounterTrackable.journalData.ClearCache();
				PlayerController playerController = m_gun.CurrentOwner as PlayerController;
				if (playerController)
				{
					GameUIRoot.Instance.TemporarilyShowGunName(playerController.IsPrimaryPlayer);
				}
			}
		}

		public static void AssignTemporaryOverrideGun(PlayerController targetPlayer, int gunID, float duration)
		{
			if (targetPlayer && !targetPlayer.IsGhost)
			{
				targetPlayer.StartCoroutine(AdvancedGunFormeSynergyProcessor.HandleTransformationDuration(targetPlayer, gunID, duration));
			}
		}

		public static IEnumerator HandleTransformationDuration(PlayerController targetPlayer, int gunID, float duration)
		{
			float elapsed = 0f;
			if (targetPlayer && targetPlayer.inventory.GunLocked.Value && targetPlayer.CurrentGun)
			{
				MimicGunController component = targetPlayer.CurrentGun.GetComponent<MimicGunController>();
				if (component)
				{
					component.ForceClearMimic(false);
				}
			}
			targetPlayer.inventory.GunChangeForgiveness = true;
			Gun limitGun = PickupObjectDatabase.GetById(gunID) as Gun;
			Gun m_extantGun = targetPlayer.inventory.AddGunToInventory(limitGun, true);
			m_extantGun.CanBeDropped = false;
			m_extantGun.CanBeSold = false;
			targetPlayer.inventory.GunLocked.SetOverride("override gun", true, null);
			elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			ClearTemporaryOverrideGun(targetPlayer, m_extantGun);
			yield break;
		}

		public static void ClearTemporaryOverrideGun(PlayerController targetPlayer, Gun m_extantGun)
		{
			if (!targetPlayer || !m_extantGun)
			{
				return;
			}
			if (targetPlayer)
			{
				targetPlayer.inventory.GunLocked.RemoveOverride("override gun");
				targetPlayer.inventory.DestroyGun(m_extantGun);
				m_extantGun = null;
			}
			targetPlayer.inventory.GunChangeForgiveness = false;
		}

		/// <summary>
		/// Gun forms.
		/// </summary>
		public AdvancedGunFormeData[] Formes;
		private Gun m_gun;
		private int CurrentForme;
	}
}
