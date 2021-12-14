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

using UnityEngine.Serialization;

namespace Planetside
{
	internal class ImmateriaProjectile : MonoBehaviour
	{
		public ImmateriaProjectile()
		{
			this.Pulses = 1;
		}
		public void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			this.player = (this.projectile.Owner as PlayerController);

			Projectile projectile = this.projectile;
			PlayerController playerController = projectile.Owner as PlayerController;
			Projectile component = base.gameObject.GetComponent<Projectile>();
			bool flag = component != null;
			bool flag2 = flag;
			if (flag2)
			{
				var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");

				Material material = component.sprite.renderer.material;
				material.shader = Shader.Find("Brave/PlayerShaderEevee");

				material.SetTexture("_EeveeTex", texture);
				material.SetFloat("_StencilVal", 10000);
				material.SetFloat("_FlatColor", 100000f);
				material.SetFloat("_Perpendicular", 0);

				material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
				material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
				GameManager.Instance.StartCoroutine(this.EndTime(component));
				component.StartCoroutine(this.Speed(component));

			}
		}
		public IEnumerator Speed(Projectile projectile)
		{
			bool flag = projectile != null;
			bool flag3 = flag;
			if (flag3)
			{
				float speed = projectile.baseData.speed / 15;
				for (int i = 0; i < 15; i++)
				{
					projectile.baseData.speed -= speed;
					projectile.UpdateSpeed();
					yield return new WaitForSeconds(0.05f);
				}
			}
			yield break;
		}
		private IEnumerator EndTime(Projectile position)
		{
			float DamageScalar = 0f;
			float Scaler = 1f;
			Vector2 centerPosition = position.sprite.WorldCenter;
			yield return new WaitForSeconds(2f);
			for (int i = 0; i < Pulses; i++)
			{
				GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(position.transform.position, 5+(i*0.5f), 0.5f-(i/3), 3f+(i*.5f), 0.4f+(i/10)));
				AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_charge_01", position.gameObject);
				List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag = activeEnemies != null;
				if (flag)
				{
					foreach (AIActor aiactor in activeEnemies)
                    {
						if (aiactor != null)
                        {
							//Vector2 a = aiactor.transform.position - position.transform.position;
							//aiactor.knockbackDoer.ApplyKnockback(-a, 1.25f * (Vector2.Distance(position.transform.position, aiactor.transform.position) + 0.001f), false);
							bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4.5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
							if (ae)
							{
								if (aiactor.healthHaver.IsBoss)
								{
									Scaler = 5f * Pulses;
								}
								DamageScalar += ((40 - (Pulses * 3.33f)) + (i * 2)) * Scaler;
							}
							if (aiactor.healthHaver.IsBoss)
							{
								Scaler = 5f * Pulses;
							}
							DamageScalar += ((40 - (Pulses * 3.33f)) + (i * 2)) * Scaler;
						}


					}
				}
				yield return new WaitForSeconds(2f);
			}
			Exploder.DoDistortionWave(position.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
			AkSoundEngine.PostEvent("Play_BOSS_queenship_explode_01", position.gameObject);
			List<AIActor> activeEnemies1 = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			if (activeEnemies1 != null)
			{
				foreach (AIActor aiactor in activeEnemies1)
				{
					aiactor.healthHaver.ApplyDamage(DamageScalar, Vector2.zero, "fuck you", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
				}
			}
			position.ForceDestruction();
			yield break;
		}
		public float distortionMaxRadius = 30f;
		public float distortionDuration = 0.5f;
		public float distortionIntensity = 2f;
		public float distortionThickness = 1f;


		private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
		{
			Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
			return new Vector4(vector.x, vector.y, 0f, 0f);
		}

		private Projectile projectile;
		private PlayerController player;
		public int Pulses;
	}
}

