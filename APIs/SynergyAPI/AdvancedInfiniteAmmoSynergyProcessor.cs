using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SynergyAPI
{
    /// <summary>
    /// Component that gives infinite ammo to a gun if the owner has a specific synergy.
    /// </summary>
    public class AdvancedInfiniteAmmoSynergyProcessor : MonoBehaviour
    {
        public AdvancedInfiniteAmmoSynergyProcessor()
        {
            PreventsReload = true;
            m_cachedReloadTime = -1f;
        }

        public void Awake()
        {
            m_gun = base.GetComponent<Gun>();
        }

        public void Update()
        {
            bool flag = m_gun && m_gun.OwnerHasSynergy(RequiredSynergy);
            if (flag && !m_processed)
            {
                m_gun.GainAmmo(m_gun.AdjustedMaxAmmo);
                m_gun.InfiniteAmmo = true;
                m_processed = true;
                if (PreventsReload)
                {
                    m_cachedReloadTime = m_gun.reloadTime;
                    m_gun.reloadTime = 0f;
                }
            }
            else if (!flag && m_processed)
            {
                m_gun.InfiniteAmmo = false;
                m_processed = false;
                if (PreventsReload)
                {
                    m_gun.reloadTime = m_cachedReloadTime;
                }
            }
        }

        /// <summary>
        /// Name of the synergy required for infinite ammo.
        /// </summary>
        public string RequiredSynergy;
        /// <summary>
        /// If true, will also set reload time to 0.
        /// </summary>
        public bool PreventsReload;
        private bool m_processed;
        private Gun m_gun;
        private float m_cachedReloadTime;
    }
}
