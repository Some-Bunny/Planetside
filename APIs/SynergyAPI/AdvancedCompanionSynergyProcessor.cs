using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SynergyAPI
{
    /// <summary>
    /// Component that spawns a companion when the item's owner has a synergy.
    /// </summary>
    public class AdvancedCompanionSynergyProcessor : MonoBehaviour
    {
        public GameObject ExtantCompanion
        {
            get
            {
                return m_extantCompanion;
            }
        }

        private void CreateCompanion(PlayerController owner)
        {
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(CompanionGuid);
            Vector3 position = owner.transform.position;
            GameObject extantCompanion = Instantiate(orLoadByGuid.gameObject, position, Quaternion.identity);
            m_extantCompanion = extantCompanion;
            CompanionController orAddComponent = m_extantCompanion.GetOrAddComponent<CompanionController>();
            orAddComponent.Initialize(owner);
            if (orAddComponent.specRigidbody)
            {
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
            }
        }

        private void DestroyCompanion()
        {
            if (!m_extantCompanion)
            {
                return;
            }
            Destroy(m_extantCompanion);
            m_extantCompanion = null;
        }

        private void Awake()
        {
            m_gun = GetComponent<Gun>();
            m_item = GetComponent<PassiveItem>();
            m_activeItem = GetComponent<PlayerItem>();
        }

        public void Update()
        {
            PlayerController playerController = ManuallyAssignedPlayer;
            if (!playerController && m_item)
            {
                playerController = m_item.Owner;
            }
            if (!playerController && m_activeItem && m_activeItem.PickedUp && m_activeItem.LastOwner)
            {
                playerController = m_activeItem.LastOwner;
            }
            if (!playerController && m_gun && m_gun.CurrentOwner is PlayerController)
            {
                playerController = (m_gun.CurrentOwner as PlayerController);
            }
            if (playerController && (RequiresNoSynergy || playerController.PlayerHasActiveSynergy(RequiredSynergy)) && !m_active)
            {
                m_active = true;
                m_cachedPlayer = playerController;
                ActivateSynergy(playerController);
            }
            else if (!playerController || (!RequiresNoSynergy && !playerController.PlayerHasActiveSynergy(RequiredSynergy) && m_active))
            {
                DeactivateSynergy();
                m_active = false;
            }
        }

        private void OnDisable()
        {
            if (m_active && !PersistsOnDisable)
            {
                DeactivateSynergy();
                m_active = false;
            }
            else if (m_active && m_cachedPlayer)
            {
                m_cachedPlayer.StartCoroutine(HandleDisabledChecks());
            }
        }

        private IEnumerator HandleDisabledChecks()
        {
            yield return null;
            while (this && m_cachedPlayer && !isActiveAndEnabled && m_active)
            {
                if (!m_cachedPlayer.PlayerHasActiveSynergy(RequiredSynergy))
                {
                    DeactivateSynergy();
                    m_active = false;
                    yield break;
                }
                yield return null;
            }
            yield break;
        }

        private void OnDestroy()
        {
            if (m_active)
            {
                DeactivateSynergy();
                m_active = false;
            }
        }

        public void ActivateSynergy(PlayerController player)
        {
            player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Combine(player.OnNewFloorLoaded, new Action<PlayerController>(HandleNewFloor));
            CreateCompanion(player);
        }

        private void HandleNewFloor(PlayerController obj)
        {
            DestroyCompanion();
        }

        public void DeactivateSynergy()
        {
            if (m_cachedPlayer != null)
            {
                PlayerController cachedPlayer = m_cachedPlayer;
                cachedPlayer.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(cachedPlayer.OnNewFloorLoaded, new Action<PlayerController>(HandleNewFloor));
                m_cachedPlayer = null;
            }
            DestroyCompanion();
        }

        /// <summary>
        /// Name of the synergy that is required for spawning the companion.
        /// </summary>
        public string RequiredSynergy;
        /// <summary>
        /// If true, no synergy will be required for the companion to spawn.
        /// </summary>
        public bool RequiresNoSynergy;
        /// <summary>
        /// If true, the companion will persist even when the owner is not holding the gun. Doesn't make any difference for passives and actives.
        /// </summary>
        public bool PersistsOnDisable;
        /// <summary>
        /// Guid of the companion that the synergy will spawn.
        /// </summary>
        public string CompanionGuid;
        private Gun m_gun;
        private PassiveItem m_item;
        private PlayerItem m_activeItem;
        public PlayerController ManuallyAssignedPlayer;
        private GameObject m_extantCompanion;
        private bool m_active;
        private PlayerController m_cachedPlayer;
    }

}
