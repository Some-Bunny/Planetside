using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;
using Planetside;
using UnityEngine.Serialization;


public class UmbraMinionsbehavior : BraveBehaviour
{
	public UmbraMinionsbehavior()
	{
		this.TimeUntilInvulnerabilityGone = 10f;
		this.DropsChests = false;
		this.DropsPickups = false;
		this.PickupAmount = 1;
		this.ChestAmount = 1;
		this.TriggersUnlock = false;
		this.HPMultiplier = 1f;
		this.GainsSkulls = false;
		this.SkullAmount = 3;
		this.CooldownMulitplier = 1f;
	}
	public void Start()
	{
		base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetCurrentHealth() * HPMultiplier, null, true);
		base.aiActor.GetAbsoluteParentRoom().SealRoom();
		base.aiActor.behaviorSpeculator.CooldownScale *= CooldownMulitplier;
		base.StartCoroutine(this.Speed(base.aiActor));
		base.aiActor.MovementSpeed *= 1.25f;

		base.healthHaver.OnPreDeath += this.OnPreDeath;
		base.healthHaver.OnDeath += this.Die;
		base.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
		
		if (GainsSkulls == true)
		{
			base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
			GameObject shootpoint = new GameObject();
			shootpoint = new GameObject("fuck");
			shootpoint.transform.parent = base.aiActor.transform;
			shootpoint.transform.position = base.aiActor.sprite.WorldCenter;
			GameObject m_CachedGunAttachPoint = base.aiActor.transform.Find("fuck").gameObject;
			CustomSpinBulletsBehavior yeah = new CustomSpinBulletsBehavior()
			{
				ShootPoint = m_CachedGunAttachPoint,
				OverrideBulletName = "homing",
				NumBullets = 2,
				BulletMinRadius = 2.4f,
				BulletMaxRadius = 2.5f,
				BulletCircleSpeed = 75,
				BulletsIgnoreTiles = true,
				RegenTimer = 0.1f,
				AmountOFLines = SkullAmount,
			};
			base.aiActor.behaviorSpeculator.OtherBehaviors.Add(yeah);
		}
	}
	public IEnumerator Speed(AIActor actor)
	{
		bool flag = actor != null;
		if (flag)
		{
			base.aiActor.HasBeenEngaged = false;
			base.aiActor.healthHaver.PreventAllDamage = true;
			yield return new WaitForSeconds(TimeUntilInvulnerabilityGone);
			base.aiActor.healthHaver.PreventAllDamage = false;
			base.aiActor.HasBeenEngaged = true;
			AkSoundEngine.PostEvent("Play_Baboom", base.gameObject);
			Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
			

		}
		yield break;
	}
	private void Die(Vector2 finalDamageDirection)
	{
		PlayerController player = GameManager.Instance.PrimaryPlayer;
		base.aiActor.ParentRoom.UnsealRoom();
		if (DropsPickups == true)
        {
			for(int i = 0; i < PickupAmount; i++)
            {
				int id = BraveUtility.RandomElement<int>(UmbraMinionsbehavior.Lootdrops);
				IntVector2 bestRewardLocation = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, bestRewardLocation.ToCenterVector3(0), new Vector2(0, 0), 1.2f, false, true, false);
				LootEngine.DoDefaultItemPoof(bestRewardLocation.ToVector2(), false, true);
			}
		}
		if (DropsChests == true)
		{
			for (int i = 0; i < ChestAmount; i++)
			{
				IntVector2 bestRewardLocation = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
				Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
			}
		}
		if (TriggersUnlock == true)
        {

        }
	}
	private void OnPreDeath(Vector2 obj)
	{
		RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
		AkSoundEngine.PostEvent("Play_BOSS_lichB_grab_01", gameObject);
		GameObject hand = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.hellDragController.HellDragVFX);
		tk2dBaseSprite component1 = hand.GetComponent<tk2dBaseSprite>();
		component1.usesOverrideMaterial = true;
		component1.PlaceAtLocalPositionByAnchor(base.aiActor.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.LowerCenter);
		component1.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");
		//base.StartCoroutine(this.Speed(hand));
		//actor.healthHaver.ApplyDamage(10000000, Vector2.zero, "Eat Shit And Go To Hell^2", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
		base.aiActor.sprite.renderer.enabled = false;
		//actor.shadowDeathType = AIActor.ShadowDeathType.None;
		//tk2dBaseSprite corpsesprite = base.aiActor.CorpseObject.GetComponent<tk2dBaseSprite>();
		//corpsesprite.sprite.renderer.enabled = false;
	}
	public static List<int> Lootdrops = new List<int>
		{
			73,//half-heart
			85,//full-heart
			120,//armor
			//67,//key
			224,//blank
			600,//partial-ammo
			78,//ammo
			565//glass guon stone
		};
	public bool DropsChests;
	public int ChestAmount;

	public bool DropsPickups;
	public int PickupAmount;

	public bool TriggersUnlock;

	public float HPMultiplier;
	public float CooldownMulitplier;

	public bool GainsSkulls;
	public int SkullAmount;

	public float distortionMaxRadius = 50f;
	public float distortionDuration = 1f;
	public float distortionIntensity = 0.5f;
	public float distortionThickness = 0.2f;
	public float TimeUntilInvulnerabilityGone;
}
