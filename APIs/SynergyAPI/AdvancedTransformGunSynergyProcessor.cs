using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace SynergyAPI
{
    /// <summary>
    /// Component that transforms the gun it's applied to to another gun if the owner has a specific synergy.
    /// </summary>
    public class AdvancedTransformGunSynergyProcessor : MonoBehaviour
    {
        private void Awake()
        {
            m_gun = GetComponent<Gun>();
        }

        private void Update()
        {
            if (Dungeon.IsGenerating || Dungeon.ShouldAttemptToLoadFromMidgameSave)
            {
                return;
            }
            if (m_gun && m_gun.CurrentOwner is PlayerController)
            {
                PlayerController playerController = m_gun.CurrentOwner as PlayerController;
                if (!m_gun.enabled)
                {
                    return;
                }
                if (playerController.PlayerHasActiveSynergy(SynergyToCheck) && !m_transformed)
                {
                    m_transformed = true;
                    m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(SynergyGunId) as Gun);
                    if (ShouldResetAmmoAfterTransformation)
                    {
                        m_gun.ammo = ResetAmmoCount;
                    }
                }
                else if (!playerController.PlayerHasActiveSynergy(SynergyToCheck) && m_transformed)
                {
                    m_transformed = false;
                    m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(NonSynergyGunId) as Gun);
                    if (ShouldResetAmmoAfterTransformation)
                    {
                        m_gun.ammo = ResetAmmoCount;
                    }
                }
            }
            else if (m_gun && !m_gun.CurrentOwner && m_transformed)
            {
                m_transformed = false;
                m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(NonSynergyGunId) as Gun);
                if (ShouldResetAmmoAfterTransformation)
                {
                    m_gun.ammo = ResetAmmoCount;
                }
            }
            ShouldResetAmmoAfterTransformation = false;
        }

        /// <summary>
        /// Name of the synergy required for the transformation.
        /// </summary>
        public string SynergyToCheck;
        /// <summary>
        /// The id of the non-synergy form.
        /// </summary>
        public int NonSynergyGunId;
        /// <summary>
        /// The id of the synergy form.
        /// </summary>
        public int SynergyGunId;
        private Gun m_gun;
        private bool m_transformed;
        /// <summary>
        /// If <see langword="true"/>, after transforming the gun's ammo will be set to <see cref="ResetAmmoCount"/>.
        /// </summary>
        public bool ShouldResetAmmoAfterTransformation;
        /// <summary>
        /// If <see cref="ShouldResetAmmoAfterTransformation"/> is <see langword="true"/>, amount to which the gun's ammo will be set to after transforming.
        /// </summary>
        public int ResetAmmoCount;
    }
}
