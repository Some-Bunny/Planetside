 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{

    public class AdvancedHoveringGunProcessor : MonoBehaviour
    {
        public AdvancedHoveringGunProcessor()
        {
            this.FireCooldown = 1f;
            this.FireDuration = 2f;
            this.NumToTrigger = 1;
            this.TriggerDuration = -1f;
            this.ChanceToConsumeTargetGunAmmo = 0.5f;
            this.m_hovers = new List<CustomHoveringGunController>();
            this.m_initialized = new List<bool>();
            this.Radius = 2.5f;
            this.RotationSpeed = 60;
        }

        public void Awake()
        {
            this.m_gun = base.GetComponent<Gun>();
            this.m_item = base.GetComponent<PassiveItem>();
        }

        private bool IsInitialized(int index)
        {
            return this.m_initialized.Count > index && this.m_initialized[index];
        }

        public void Update()
        {
            bool flag = this.Trigger == AdvancedHoveringGunProcessor.TriggerStyle.CONSTANT;
            if (flag)
            {
                bool flag2 = this.m_gun;
                if (flag2)
                {
                    bool flag3 = this.m_gun && this.m_gun.isActiveAndEnabled && this.m_gun.CurrentOwner && Activate;
                    if (flag3)
                    {
                        for (int i = 0; i < this.NumToTrigger; i++)
                        {
                            bool flag4 = !this.IsInitialized(i);
                            if (flag4)
                            {
                                this.Enable(i);
                            }
                        }
                    }
                    else
                    {
                        this.DisableAll();
                    }
                }
                else
                {
                    bool flag5 = this.m_item;
                    if (flag5)
                    {
                        bool flag6 = this.m_item && this.m_item.Owner && Activate;
                        if (flag6)
                        {
                            for (int j = 0; j < this.NumToTrigger; j++)
                            {
                                bool flag7 = !this.IsInitialized(j);
                                if (flag7)
                                {
                                    this.Enable(j);
                                }
                            }
                        }
                        else
                        {
                            this.DisableAll();
                        }
                    }
                }
            }
            else
            {
                bool flag8 = this.Trigger == AdvancedHoveringGunProcessor.TriggerStyle.ON_DAMAGE;
                if (flag8)
                {
                    bool flag9 = !this.m_actionsLinked && this.m_gun && this.m_gun.CurrentOwner;
                    if (flag9)
                    {
                        PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
                        this.m_cachedLinkedPlayer = playerController;
                        playerController.OnReceivedDamage += this.HandleOwnerDamaged;
                        this.m_actionsLinked = true;
                    }
                    else
                    {
                        bool flag10 = this.m_actionsLinked && this.m_gun && !this.m_gun.CurrentOwner && this.m_cachedLinkedPlayer;
                        if (flag10)
                        {
                            this.m_cachedLinkedPlayer.OnReceivedDamage -= this.HandleOwnerDamaged;
                            this.m_cachedLinkedPlayer = null;
                            this.m_actionsLinked = false;
                        }
                        /* else
                         {
                             bool flag14 = this.m_actionsLinked && this.m_item && this.m_item.Owner;
                             if (flag14)
                             {
                                 PlayerController playerController3 = this.m_item.Owner;
                                 this.m_cachedLinkedPlayer = playerController3;
                                 playerController3.OnReceivedDamage += this.HandleOwnerDamaged;
                                 this.m_actionsLinked = true;
                             }
                             else
                             {
                                 bool flag15 = this.m_actionsLinked && this.m_item && !this.m_item.Owner && this.m_cachedLinkedPlayer;
                                 if (flag15)
                                 {
                                     this.m_cachedLinkedPlayer.OnReceivedDamage -= this.HandleOwnerDamaged;
                                     this.m_cachedLinkedPlayer = null;
                                     this.m_actionsLinked = false;
                                 }
                             }
                         }*/
                    }
                }
                else
                {
                    bool flag11 = this.Trigger == AdvancedHoveringGunProcessor.TriggerStyle.ON_ACTIVE_ITEM;
                    if (flag11)
                    {
                        bool flag12 = !this.m_actionsLinked && this.m_gun && this.m_gun.CurrentOwner;
                        if (flag12)
                        {
                            PlayerController playerController2 = this.m_gun.CurrentOwner as PlayerController;
                            this.m_cachedLinkedPlayer = playerController2;
                            playerController2.OnUsedPlayerItem += this.HandleOwnerItemUsed;
                            this.m_actionsLinked = true;
                        }
                        else
                        {
                            bool flag13 = this.m_actionsLinked && this.m_gun && !this.m_gun.CurrentOwner && this.m_cachedLinkedPlayer;
                            if (flag13)
                            {
                                this.m_cachedLinkedPlayer.OnUsedPlayerItem -= this.HandleOwnerItemUsed;
                                this.m_cachedLinkedPlayer = null;
                                this.m_actionsLinked = false;
                            }
                            /*else
                            {
                                bool flag16 = this.m_actionsLinked && this.m_item && this.m_item.Owner;
                                if (flag16)
                                {
                                    PlayerController playerController4 = this.m_item.Owner;
                                    this.m_cachedLinkedPlayer = playerController4;
                                    playerController4.OnUsedPlayerItem -= this.HandleOwnerItemUsed;
                                    this.m_actionsLinked = true;
                                }
                                else
                                {
                                    bool flag17 = this.m_actionsLinked && this.m_item && !this.m_item.Owner && this.m_cachedLinkedPlayer;
                                    if (flag17)
                                    {
                                        this.m_cachedLinkedPlayer.OnUsedPlayerItem -= this.HandleOwnerItemUsed;
                                        this.m_cachedLinkedPlayer = null;
                                        this.m_actionsLinked = false;
                                    }
                                }
                            }
                            */
                        }
                    }
                }
            }
        }

        private void HandleOwnerItemUsed(PlayerController sourcePlayer, PlayerItem sourceItem)
        {
            bool flag = Activate && this.GetOwner();
            if (flag)
            {
                for (int i = 0; i < this.NumToTrigger; i++)
                {
                    int num = 0;
                    while (this.IsInitialized(num))
                    {
                        num++;
                    }
                    this.Enable(num);
                    base.StartCoroutine(this.ActiveItemDisable(num, sourcePlayer));
                }
            }
        }

        private void HandleOwnerDamaged(PlayerController sourcePlayer)
        {
            bool flag = Activate;
            if (flag)
            {
                for (int i = 0; i < this.NumToTrigger; i++)
                {
                    int num = 0;
                    while (this.IsInitialized(num))
                    {
                        num++;
                    }
                    this.Enable(num);
                    base.StartCoroutine(this.TimedDisable(num, this.TriggerDuration));
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
            this.Disable(index);
            yield break;
        }

        private IEnumerator TimedDisable(int index, float duration)
        {
            yield return new WaitForSeconds(duration);
            this.Disable(index);
            yield break;
        }

        private void OnDisable()
        {
            this.DisableAll();
        }

        private PlayerController GetOwner()
        {
            bool flag = this.m_gun;
            PlayerController result;
            if (flag)
            {
                result = (this.m_gun.CurrentOwner as PlayerController);
            }
            else
            {
                bool flag2 = this.m_item;
                if (flag2)
                {
                    result = this.m_item.Owner;
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }

        private void Enable(int index)
        {
            bool flag = this.m_initialized.Count > index && this.m_initialized[index];
            if (!flag)
            {
                PlayerController owner = this.GetOwner();
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                gameObject.transform.parent = owner.transform;
                while (this.m_hovers.Count < index + 1)
                {
                    this.m_hovers.Add(null);
                    this.m_initialized.Add(false);
                }
                Destroy(gameObject.GetComponent<HoveringGunController>());

                this.m_hovers[index] = gameObject.GetOrAddComponent<CustomHoveringGunController>();
                this.m_hovers[index].ShootAudioEvent = this.ShootAudioEvent;
                this.m_hovers[index].OnEveryShotAudioEvent = this.OnEveryShotAudioEvent;
                this.m_hovers[index].FinishedShootingAudioEvent = this.FinishedShootingAudioEvent;
                this.m_hovers[index].ConsumesTargetGunAmmo = this.ConsumesTargetGunAmmo;
                this.m_hovers[index].ChanceToConsumeTargetGunAmmo = this.ChanceToConsumeTargetGunAmmo;
                this.m_hovers[index].Position = this.PositionType;
                this.m_hovers[index].Aim = this.AimType;
                this.m_hovers[index].Trigger = this.FireType;
                this.m_hovers[index].CooldownTime = this.FireCooldown;
                this.m_hovers[index].ShootDuration = this.FireDuration;
                this.m_hovers[index].OnlyOnEmptyReload = this.OnlyOnEmptyReload;
                this.m_hovers[index].RotationSpeed = this.RotationSpeed;
                this.m_hovers[index].Radius = this.Radius;
                this.m_hovers[index].material = this.Material_To_Use;

                //Material_To_Use
                Gun gun = null;
                int num = this.TargetGunID;
                bool usesMultipleGuns = this.UsesMultipleGuns;
                if (usesMultipleGuns)
                {
                    num = this.TargetGunIDs[index];
                }
                for (int i = 0; i < owner.inventory.AllGuns.Count; i++)
                {
                    bool flag2 = owner.inventory.AllGuns[i].PickupObjectId == num;
                    if (flag2)
                    {
                        gun = owner.inventory.AllGuns[i];
                    }
                }
                bool flag3 = !gun;
                if (flag3)
                {
                    gun = (PickupObjectDatabase.Instance.InternalGetById(num) as Gun);
                }
                this.m_hovers[index].Initialize(gun, owner);
                this.m_initialized[index] = true;
            }
        }

        private void Disable(int index)
        {
            bool flag = this.m_hovers[index];
            if (flag)
            {
                UnityEngine.Object.Destroy(this.m_hovers[index].gameObject);
            }
        }

        private void DisableAll()
        {
            for (int i = 0; i < this.m_hovers.Count; i++)
            {
                bool flag = this.m_hovers[i];
                if (flag)
                {
                    UnityEngine.Object.Destroy(this.m_hovers[i].gameObject);
                }
            }
            this.m_hovers.Clear();
            this.m_initialized.Clear();
        }

        public void OnDestroy()
        {
            bool flag = this.m_actionsLinked && this.m_cachedLinkedPlayer;
            if (flag)
            {
                this.m_cachedLinkedPlayer.OnReceivedDamage -= this.HandleOwnerDamaged;
                this.m_cachedLinkedPlayer = null;
                this.m_actionsLinked = false;
            }
        }

        public string RequiredSynergy;

        public bool Activate;

        public int TargetGunID;

        public bool UsesMultipleGuns;

        public List<int> TargetGunIDs;

        public CustomHoveringGunController.HoverPosition PositionType;

        public CustomHoveringGunController.AimType AimType;

        public CustomHoveringGunController.FireType FireType;

        public float FireCooldown;

        public float Radius;
        public float RotationSpeed;
        public Material Material_To_Use;



        public float FireDuration;

        public bool OnlyOnEmptyReload;

        public string ShootAudioEvent;

        public string OnEveryShotAudioEvent;

        public string FinishedShootingAudioEvent;

        public AdvancedHoveringGunProcessor.TriggerStyle Trigger;

        public int NumToTrigger;

        public float TriggerDuration;

        public bool ConsumesTargetGunAmmo;

        public float ChanceToConsumeTargetGunAmmo;

        private bool m_actionsLinked;

        private PlayerController m_cachedLinkedPlayer;

        private Gun m_gun;

        private PassiveItem m_item;

        private List<CustomHoveringGunController> m_hovers;

        private List<bool> m_initialized;

        public enum TriggerStyle
        {
            CONSTANT,
            ON_DAMAGE,
            ON_ACTIVE_ITEM
        }
    }
}