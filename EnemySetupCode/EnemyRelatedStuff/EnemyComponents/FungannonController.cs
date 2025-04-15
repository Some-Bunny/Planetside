using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Planetside;
using Brave.BulletScript;
using Dungeonator;

public class TeleportationImmunity : BraveBehaviour { }


public class FungannonController : BraveBehaviour
{
	public void Start()
	{
		base.aiActor.HasBeenEngaged = false;
		m_StartRoom = aiActor.GetAbsoluteParentRoom();
		base.healthHaver.OnPreDeath += this.OnPreDeath;
	}

	public List<GameObject> extantReticles = new List<GameObject>();

	private RoomHandler m_StartRoom;
	private void Update()
	{
		if (!base.aiActor.HasBeenEngaged)
		{
			CheckPlayerRoom();

		}
	}
	private void CheckPlayerRoom()
	{
		if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
		{
			GameManager.Instance.StartCoroutine(LateEngage());
		}
		else
        {
			base.aiActor.HasBeenEngaged = false;
		}
	}
	private IEnumerator LateEngage()
	{
		yield return new WaitForSeconds(0.5f); 
		if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
		{
			base.aiActor.HasBeenEngaged = true;
		}
		yield break;
	}
	public override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
		}
		base.OnDestroy();
	}
	private void OnPreDeath(Vector2 obj)
	{
		base.StartCoroutine(this.DIE());
	}


	private IEnumerator DIE()
	{
		base.aiAnimator.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;

		base.aiAnimator.sprite.usesOverrideMaterial = true;
		//base.aiActor.sprite.out
		base.aiAnimator.sprite.renderer.material.EnableKeyword("_BurnAmount");
		base.aiAnimator.sprite.renderer.material.DisableKeyword("TINTING_OFF");
		base.aiAnimator.sprite.renderer.material.EnableKeyword("TINTING_ON");
		base.aiAnimator.sprite.renderer.material.EnableKeyword("EMISSIVE_ON");
		base.aiAnimator.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
		base.aiAnimator.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 10f);
		base.aiAnimator.sprite.renderer.material.SetFloat("_EmissiveColorPower", 2f);
		base.aiAnimator.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
		//Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.aiAnimator.sprite);
		//outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
		int emId = Shader.PropertyToID("_EmissivePower");

		SpriteOutlineManager.RemoveOutlineFromSprite(base.aiAnimator.sprite, false);
		//int emId = Shader.PropertyToID("_EmissivePower");
		Vector3 vector = base.sprite.WorldBottomLeft.ToVector3ZisY(0f);
		Vector3 vector2 = base.sprite.WorldTopRight.ToVector3ZisY(0f);
		Vector3 a = vector2 - vector;
		vector += a * 0.1f;
		vector2 -= a * 0.1f;
		float num = (vector2.y - vector.y) * (vector2.x - vector.x);
		int num2 = Mathf.CeilToInt(40f * num);
		int num3 = num2;
		Vector3 minPosition = vector;
		Vector3 maxPosition = vector2;
		Vector3 direction = Vector3.up / 3f;
		float angleVariance = 360f;
		float magnitudeVariance = 0.3f;
		float? startLifetime = new float?(UnityEngine.Random.Range(4f, 18f));
		GlobalSparksDoer.DoRandomParticleBurst(num3, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);


		Material targetMaterial = base.aiAnimator.sprite.renderer.material;
		float ela = 0f;
		float dura = 4f;
		while (ela < dura)
		{
			ela += BraveTime.DeltaTime;
			float t = ela / dura;
			targetMaterial.SetFloat(emId, Mathf.Lerp(2f, 3f, t));
			targetMaterial.SetFloat("_BurnAmount", t * 2);
			yield return null;
		}
		yield break;
	}
	public GameObject LaserShit;

}
