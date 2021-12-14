using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Gungeon;

namespace Planetside
{
	// Token: 0x02000002 RID: 2
	public class AdvancedDualWieldSynergyProcessor : MonoBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void Awake()
		{
			this.m_gun = base.GetComponent<Gun>();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		private bool EffectValid(PlayerController p)
		{
			bool flag = !p;
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !p.PlayerHasActiveSynergy(this.SynergyNameToCheck);
				bool flag4 = flag3;
				if (flag4)
				{
					result = false;
				}
				else
				{
					bool flag5 = this.m_gun.CurrentAmmo == 0;
					bool flag6 = flag5;
					if (flag6)
					{
						result = false;
					}
					else
					{
						bool value = p.inventory.GunLocked.Value;
						bool flag7 = value;
						if (flag7)
						{
							result = false;
						}
						else
						{
							bool flag8 = !this.m_isCurrentlyActive;
							bool flag9 = flag8;
							if (flag9)
							{
								int indexForGun = this.GetIndexForGun(p, this.PartnerGunID);
								bool flag10 = indexForGun < 0;
								bool flag11 = flag10;
								if (flag11)
								{
									return false;
								}
								bool flag12 = p.inventory.AllGuns[indexForGun].CurrentAmmo == 0;
								bool flag13 = flag12;
								if (flag13)
								{
									return false;
								}
							}
							else
							{
								bool flag14 = p.CurrentSecondaryGun != null && p.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0;
								bool flag15 = flag14;
								if (flag15)
								{
									return false;
								}
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021A4 File Offset: 0x000003A4
		private bool PlayerUsingCorrectGuns()
		{
			return this.m_gun && this.m_gun.CurrentOwner && this.m_cachedPlayer && this.m_cachedPlayer.inventory.DualWielding && this.m_cachedPlayer.PlayerHasActiveSynergy(this.SynergyNameToCheck) && (!(this.m_cachedPlayer.CurrentGun != this.m_gun) || this.m_cachedPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!(this.m_cachedPlayer.CurrentSecondaryGun != this.m_gun) || this.m_cachedPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002276 File Offset: 0x00000476
		private void Update()
		{
			this.CheckStatus();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002280 File Offset: 0x00000480
		private void CheckStatus()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				bool flag2 = !this.PlayerUsingCorrectGuns() || !this.EffectValid(this.m_cachedPlayer);
				bool flag3 = flag2;
				if (flag3)
				{
					this.DisableEffect();
				}
			}
			else
			{
				bool flag4 = this.m_gun && this.m_gun.CurrentOwner is PlayerController;
				bool flag5 = flag4;
				if (flag5)
				{
					PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
					bool flag6 = playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == this.m_gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == this.PartnerGunID;
					bool flag7 = flag6;
					if (flag7)
					{
						this.m_isCurrentlyActive = true;
						this.m_cachedPlayer = playerController;
					}
					else
					{
						this.AttemptActivation(playerController);
					}
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002374 File Offset: 0x00000574
		private void AttemptActivation(PlayerController ownerPlayer)
		{
			bool flag = this.EffectValid(ownerPlayer);
			bool flag2 = flag;
			if (flag2)
			{
				this.m_isCurrentlyActive = true;
				this.m_cachedPlayer = ownerPlayer;
				ownerPlayer.inventory.SetDualWielding(true, "synergy");
				int indexForGun = this.GetIndexForGun(ownerPlayer, this.m_gun.PickupObjectId);
				int indexForGun2 = this.GetIndexForGun(ownerPlayer, this.PartnerGunID);
				ownerPlayer.inventory.SwapDualGuns();
				bool flag3 = indexForGun >= 0 && indexForGun2 >= 0;
				bool flag4 = flag3;
				if (flag4)
				{
					while (ownerPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
					{
						ownerPlayer.inventory.ChangeGun(1, false, false);
					}
				}
				ownerPlayer.inventory.SwapDualGuns();
				bool flag5 = ownerPlayer.CurrentGun && !ownerPlayer.CurrentGun.gameObject.activeSelf;
				bool flag6 = flag5;
				if (flag6)
				{
					ownerPlayer.CurrentGun.gameObject.SetActive(true);
				}
				bool flag7 = ownerPlayer.CurrentSecondaryGun && !ownerPlayer.CurrentSecondaryGun.gameObject.activeSelf;
				bool flag8 = flag7;
				if (flag8)
				{
					ownerPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
				}
				this.m_cachedPlayer.GunChanged += this.HandleGunChanged;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000024D8 File Offset: 0x000006D8
		private int GetIndexForGun(PlayerController p, int gunID)
		{
			for (int i = 0; i < p.inventory.AllGuns.Count; i++)
			{
				bool flag = p.inventory.AllGuns[i].PickupObjectId == gunID;
				bool flag2 = flag;
				if (flag2)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002532 File Offset: 0x00000732
		private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3)
		{
			this.CheckStatus();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000253C File Offset: 0x0000073C
		private void DisableEffect()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				this.m_isCurrentlyActive = false;
				this.m_cachedPlayer.inventory.SetDualWielding(false, "synergy");
				this.m_cachedPlayer.GunChanged -= this.HandleGunChanged;
				this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer, false, false);
				this.m_cachedPlayer = null;
			}
		}

		// Token: 0x04000001 RID: 1
		public string SynergyNameToCheck;

		// Token: 0x04000002 RID: 2
		public int PartnerGunID;

		// Token: 0x04000003 RID: 3
		private Gun m_gun;

		// Token: 0x04000004 RID: 4
		private bool m_isCurrentlyActive;

		// Token: 0x04000005 RID: 5
		private PlayerController m_cachedPlayer;
	}
}
