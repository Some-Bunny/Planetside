using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SynergyAPI
{
    /// <summary>
    /// A synergy component that makes the owner dual wield two guns when the player has a specific synergy.
    /// </summary>
    public class AdvancedDualWieldSynergyProcessor : MonoBehaviour
    {
        public void Awake()
        {
            m_gun = GetComponent<Gun>();
        }

        private bool EffectValid(PlayerController p)
        {
            if (!p)
            {
                return false;
            }
            if (!p.PlayerHasActiveSynergy(SynergyNameToCheck))
            {
                return false;
            }
            if (m_gun.CurrentAmmo == 0)
            {
                return false;
            }
            if (p.inventory.GunLocked.Value)
            {
                return false;
            }
            if (!m_isCurrentlyActive)
            {
                int indexForGun = GetIndexForGun(p, PartnerGunID);
                if (indexForGun < 0)
                {
                    return false;
                }
                if (p.inventory.AllGuns[indexForGun].CurrentAmmo == 0)
                {
                    return false;
                }
            }
            else if (p.CurrentSecondaryGun != null && p.CurrentSecondaryGun.PickupObjectId == PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0)
            {
                return false;
            }
            return true;
        }

        private bool PlayerUsingCorrectGuns()
        {
            return m_gun && m_gun.CurrentOwner && m_cachedPlayer && m_cachedPlayer.inventory.DualWielding && m_cachedPlayer.PlayerHasActiveSynergy(SynergyNameToCheck) && (!(m_cachedPlayer.CurrentGun != m_gun) || m_cachedPlayer.CurrentGun.PickupObjectId == PartnerGunID) && (!(m_cachedPlayer.CurrentSecondaryGun != m_gun) || m_cachedPlayer.CurrentSecondaryGun.PickupObjectId == PartnerGunID);
        }

        private void Update()
        {
            CheckStatus();
        }

        private void CheckStatus()
        {
            if (m_isCurrentlyActive)
            {
                if (!PlayerUsingCorrectGuns() || !EffectValid(m_cachedPlayer))
                {
                    DisableEffect();
                }
            }
            else if (m_gun && m_gun.CurrentOwner is PlayerController)
            {
                PlayerController playerController = m_gun.CurrentOwner as PlayerController;
                if (playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == m_gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == PartnerGunID)
                {
                    m_isCurrentlyActive = true;
                    m_cachedPlayer = playerController;
                    return;
                }
                AttemptActivation(playerController);
            }
        }

        private void AttemptActivation(PlayerController ownerPlayer)
        {
            if (EffectValid(ownerPlayer))
            {
                m_isCurrentlyActive = true;
                m_cachedPlayer = ownerPlayer;
                ownerPlayer.inventory.SetDualWielding(true, "synergy");
                int indexForGun = GetIndexForGun(ownerPlayer, m_gun.PickupObjectId);
                int indexForGun2 = GetIndexForGun(ownerPlayer, PartnerGunID);
                ownerPlayer.inventory.SwapDualGuns();
                if (indexForGun >= 0 && indexForGun2 >= 0)
                {
                    while (ownerPlayer.inventory.CurrentGun.PickupObjectId != PartnerGunID)
                    {
                        ownerPlayer.inventory.ChangeGun(1, false, false);
                    }
                }
                ownerPlayer.inventory.SwapDualGuns();
                if (ownerPlayer.CurrentGun && !ownerPlayer.CurrentGun.gameObject.activeSelf)
                {
                    ownerPlayer.CurrentGun.gameObject.SetActive(true);
                }
                if (ownerPlayer.CurrentSecondaryGun && !ownerPlayer.CurrentSecondaryGun.gameObject.activeSelf)
                {
                    ownerPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
                }
                m_cachedPlayer.GunChanged += HandleGunChanged;
            }
        }

        private int GetIndexForGun(PlayerController p, int gunID)
        {
            for (int i = 0; i < p.inventory.AllGuns.Count; i++)
            {
                if (p.inventory.AllGuns[i].PickupObjectId == gunID)
                {
                    return i;
                }
            }
            return -1;
        }

        private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3)
        {
            CheckStatus();
        }

        private void DisableEffect()
        {
            if (m_isCurrentlyActive)
            {
                m_isCurrentlyActive = false;
                m_cachedPlayer.inventory.SetDualWielding(false, "synergy");
                m_cachedPlayer.GunChanged -= HandleGunChanged;
                m_cachedPlayer.stats.RecalculateStats(m_cachedPlayer, false, false);
                m_cachedPlayer = null;
            }
        }

        /// <summary>
        /// Name of the synergy required for the dual wield.
        /// </summary>
        public string SynergyNameToCheck;
        /// <summary>
        /// Id of the other gun that will be dual wielded.
        /// </summary>
        public int PartnerGunID;
        private Gun m_gun;
        private bool m_isCurrentlyActive;
        private PlayerController m_cachedPlayer;
    }
}
