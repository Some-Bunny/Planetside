using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SynergyAPI
{
    /// <summary>
    /// Component that creates a hovering gun for it's owner on specific circumstances.
    /// </summary>
    public class AdvancedHoveringGunSynergyProcessor : MonoBehaviour
    {
        public AdvancedHoveringGunSynergyProcessor()
        {
            FireCooldown = 1f;
            FireDuration = 2f;
            NumToTrigger = 1;
            TriggerDuration = -1f;
            ChanceToConsumeTargetGunAmmo = 0.5f;
            m_hovers = new List<HoveringGunController>();
            m_initialized = new List<bool>();
        }

        public void Awake()
        {
            m_gun = GetComponent<Gun>();
            m_item = GetComponent<PassiveItem>();
        }

        private bool IsInitialized(int index)
        {
            return m_initialized.Count > index && m_initialized[index];
        }

        public void Update()
        {
            if (Trigger == HoveringGunSynergyProcessor.TriggerStyle.CONSTANT)
            {
                if (m_gun)
                {
                    if (m_gun && m_gun.isActiveAndEnabled && m_gun.CurrentOwner && m_gun.OwnerHasSynergy(RequiredSynergy))
                    {
                        for (int i = 0; i < NumToTrigger; i++)
                        {
                            if (!IsInitialized(i))
                            {
                                Enable(i);
                            }
                        }
                    }
                    else
                    {
                        DisableAll();
                    }
                }
                else if (m_item)
                {
                    if (m_item && m_item.Owner && m_item.Owner.PlayerHasActiveSynergy(RequiredSynergy))
                    {
                        for (int j = 0; j < NumToTrigger; j++)
                        {
                            if (!IsInitialized(j))
                            {
                                Enable(j);
                            }
                        }
                    }
                    else
                    {
                        DisableAll();
                    }
                }
            }
            else if (Trigger == HoveringGunSynergyProcessor.TriggerStyle.ON_DAMAGE)
            {
                if (!m_actionsLinked && m_gun && m_gun.CurrentOwner)
                {
                    PlayerController playerController = m_gun.CurrentOwner as PlayerController;
                    m_cachedLinkedPlayer = playerController;
                    playerController.OnReceivedDamage += HandleOwnerDamaged;
                    m_actionsLinked = true;
                }
                else if (m_actionsLinked && m_gun && !m_gun.CurrentOwner && m_cachedLinkedPlayer)
                {
                    m_cachedLinkedPlayer.OnReceivedDamage -= HandleOwnerDamaged;
                    m_cachedLinkedPlayer = null;
                    m_actionsLinked = false;
                }
            }
            else if (Trigger == HoveringGunSynergyProcessor.TriggerStyle.ON_ACTIVE_ITEM)
            {
                if (!m_actionsLinked && m_gun && m_gun.CurrentOwner)
                {
                    PlayerController playerController2 = m_gun.CurrentOwner as PlayerController;
                    m_cachedLinkedPlayer = playerController2;
                    playerController2.OnUsedPlayerItem += HandleOwnerItemUsed;
                    m_actionsLinked = true;
                }
                else if (m_actionsLinked && m_gun && !m_gun.CurrentOwner && m_cachedLinkedPlayer)
                {
                    m_cachedLinkedPlayer.OnUsedPlayerItem -= HandleOwnerItemUsed;
                    m_cachedLinkedPlayer = null;
                    m_actionsLinked = false;
                }
            }
        }

        private void HandleOwnerItemUsed(PlayerController sourcePlayer, PlayerItem sourceItem)
        {
            if (sourcePlayer.PlayerHasActiveSynergy(RequiredSynergy) && GetOwner())
            {
                for (int i = 0; i < NumToTrigger; i++)
                {
                    int num = 0;
                    while (IsInitialized(num))
                    {
                        num++;
                    }
                    Enable(num);
                    StartCoroutine(ActiveItemDisable(num, sourcePlayer));
                }
            }
        }

        private void HandleOwnerDamaged(PlayerController sourcePlayer)
        {
            if (sourcePlayer.PlayerHasActiveSynergy(RequiredSynergy))
            {
                for (int i = 0; i < NumToTrigger; i++)
                {
                    int num = 0;
                    while (IsInitialized(num))
                    {
                        num++;
                    }
                    Enable(num);
                    StartCoroutine(TimedDisable(num, TriggerDuration));
                }
            }
        }

        private IEnumerator ActiveItemDisable(int index, PlayerController player)
        {
            yield return null;
            while (player && player.CurrentItem && player.CurrentItem.IsActive)
            {
                yield return null;
            }
            Disable(index);
            yield break;
        }

        private IEnumerator TimedDisable(int index, float duration)
        {
            yield return new WaitForSeconds(duration);
            Disable(index);
            yield break;
        }

        private void OnDisable()
        {
            DisableAll();
        }

        private PlayerController GetOwner()
        {
            if (m_gun)
            {
                return m_gun.CurrentOwner as PlayerController;
            }
            if (m_item)
            {
                return m_item.Owner;
            }
            return null;
        }

        private void Enable(int index)
        {
            if (m_initialized.Count > index && m_initialized[index])
            {
                return;
            }
            PlayerController owner = GetOwner();
            GameObject gameObject = Instantiate(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
            gameObject.transform.parent = owner.transform;
            while (m_hovers.Count < index + 1)
            {
                m_hovers.Add(null);
                m_initialized.Add(false);
            }
            m_hovers[index] = gameObject.GetComponent<HoveringGunController>();
            m_hovers[index].ShootAudioEvent = ShootAudioEvent;
            m_hovers[index].OnEveryShotAudioEvent = OnEveryShotAudioEvent;
            m_hovers[index].FinishedShootingAudioEvent = FinishedShootingAudioEvent;
            m_hovers[index].ConsumesTargetGunAmmo = ConsumesTargetGunAmmo;
            m_hovers[index].ChanceToConsumeTargetGunAmmo = ChanceToConsumeTargetGunAmmo;
            m_hovers[index].Position = PositionType;
            m_hovers[index].Aim = AimType;
            m_hovers[index].Trigger = FireType;
            m_hovers[index].CooldownTime = FireCooldown;
            m_hovers[index].ShootDuration = FireDuration;
            m_hovers[index].OnlyOnEmptyReload = OnlyOnEmptyReload;
            Gun gun = null;
            int num = TargetGunID;
            if (UsesMultipleGuns)
            {
                num = TargetGunIDs[index];
            }
            for (int i = 0; i < owner.inventory.AllGuns.Count; i++)
            {
                if (owner.inventory.AllGuns[i].PickupObjectId == num)
                {
                    gun = owner.inventory.AllGuns[i];
                }
            }
            if (!gun)
            {
                gun = (PickupObjectDatabase.Instance.InternalGetById(num) as Gun);
            }
            m_hovers[index].Initialize(gun, owner);
            m_initialized[index] = true;
        }

        private void Disable(int index)
        {
            if (m_hovers[index])
            {
                Destroy(m_hovers[index].gameObject);
            }
        }

        private void DisableAll()
        {
            for (int i = 0; i < m_hovers.Count; i++)
            {
                if (m_hovers[i])
                {
                    Destroy(m_hovers[i].gameObject);
                }
            }
            m_hovers.Clear();
            m_initialized.Clear();
        }

        public void OnDestroy()
        {
            if (m_actionsLinked && m_cachedLinkedPlayer)
            {
                m_cachedLinkedPlayer.OnReceivedDamage -= HandleOwnerDamaged;
                m_cachedLinkedPlayer = null;
                m_actionsLinked = false;
            }
        }

        /// <summary>
        /// Name of the synergy required for the hovering gun to be created.
        /// </summary>
        public string RequiredSynergy;
        /// <summary>
        /// Id of the gun that will be created.
        /// </summary>
        public int TargetGunID;
        /// <summary>
        /// If <see langword="true"/>, instead of creating one gun with id of <see cref="TargetGunID"/> it will instead create multiple guns from all ids in <see cref="TargetGunIDs"/>.
        /// </summary>
        public bool UsesMultipleGuns;
        /// <summary>
        /// If <see cref="UsesMultipleGuns"/> is <see langword="true"/>, ids of the guns it will create. This array's length must be the same as <see cref="NumToTrigger"/>.
        /// </summary>
        public int[] TargetGunIDs;
        /// <summary>
        /// How the gun will move when created.
        /// </summary>
        public HoveringGunController.HoverPosition PositionType;
        /// <summary>
        /// Where the gun will aim when created.
        /// </summary>
        public HoveringGunController.AimType AimType;
        /// <summary>
        /// When the gun will fire when created.
        /// </summary>
        public HoveringGunController.FireType FireType;
        /// <summary>
        /// Firing cooldown of the gun.
        /// </summary>
        public float FireCooldown;
        /// <summary>
        /// Firing duration of the gun. If zero or less, the gun will just shoot a single time every time the fire condition is met. If greater than zero, after the fire condition is met the gun will shoot for this duration.
        /// </summary>
        public float FireDuration;
        /// <summary>
        /// If <see cref="FireType"/> is <see cref="HoveringGunController.FireType.ON_RELOAD"/>, the gun will only fire on empty reload.
        /// </summary>
        public bool OnlyOnEmptyReload;
        /// <summary>
        /// Sound that the gun makes when shooting. If <see cref="FireDuration"/> is greater than zero, the sound will only play at the start.
        /// </summary>
        public string ShootAudioEvent;
        /// <summary>
        /// Sound that the gun makes when shooting. If <see cref="FireDuration"/> is greater than zero, unlike <see cref="ShootAudioEvent"/>, the sound will play every shot.
        /// </summary>
        public string OnEveryShotAudioEvent;
        /// <summary>
        /// If <see cref="FireDuration"/> is greater than zero, this sound will play when the firing duration finishes.
        /// </summary>
        public string FinishedShootingAudioEvent;
        /// <summary>
        /// Condition that creates the gun.
        /// </summary>
        public HoveringGunSynergyProcessor.TriggerStyle Trigger;
        /// <summary>
        /// Number of guns that will be created.
        /// </summary>
        public int NumToTrigger;
        /// <summary>
        /// If <see cref="Trigger"/> is <see cref="HoveringGunSynergyProcessor.TriggerStyle.ON_DAMAGE"/>, the gun will only stay for this long (in seconds).
        /// </summary>
        public float TriggerDuration;
        /// <summary>
        /// If <see langword="true"/> and the owner has the hovering gun as a normal gun, the hovering gun will have a chance to consume that gun's ammo equal to <see cref="ChanceToConsumeTargetGunAmmo"/>.
        /// </summary>
        public bool ConsumesTargetGunAmmo;
        /// <summary>
        /// If <see cref="ConsumesTargetGunAmmo"/> is <see langword="true"/>, the chance that the hovering gun will consume the "original" gun's ammo. Ranges from 0 to 1 (0 = 0%, 1 = 100%)
        /// </summary>
        public float ChanceToConsumeTargetGunAmmo;
        private bool m_actionsLinked;
        private PlayerController m_cachedLinkedPlayer;
        private Gun m_gun;
        private PassiveItem m_item;
        private List<HoveringGunController> m_hovers;
        private List<bool> m_initialized;
    }
}
