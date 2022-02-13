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
	internal class VeteranShotgunProjectile : MonoBehaviour
	{
		public VeteranShotgunProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
            {
				if (projectile.specRigidbody && projectile.sprite)
				{
					tk2dSpriteDefinition currentSpriteDef = projectile.sprite.GetCurrentSpriteDef();
					Bounds bounds = currentSpriteDef.GetBounds();
					float num = Mathf.Max(bounds.size.x, bounds.size.y);
					if (num < 0.5f)
					{
						float num2 = 0.5f / num;
						//UnityEngine.Debug.Log(num + "|" + num2);
						projectile.sprite.scale = new Vector3(num2, num2, num2);
						if (num2 != 1f && projectile.specRigidbody != null)
						{
							projectile.specRigidbody.UpdateCollidersOnScale = true;
							projectile.specRigidbody.ForceRegenerate(null, null);
						}
					}
				}
				if (projectile.sprite && projectile.sprite.renderer)
				{
					Material sharedMaterial = projectile.sprite.renderer.sharedMaterial;
					projectile.sprite.usesOverrideMaterial = true;
					Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
					material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
					material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
					this.LerpMaterialGlow(material, 0f, 22f, 0.4f);
					material.SetFloat("_EmissiveColorPower", 8f);
					material.SetColor("_EmissiveColor", Color.red);
					SpriteOutlineManager.AddOutlineToSprite(projectile.sprite, Color.red);
					projectile.sprite.renderer.material = material;
				}
			}
        }
		public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			base.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
		}
		private IEnumerator LerpMaterialGlowCR(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime; ;
				float t = elapsed / duration;
				if (targetMaterial != null)
				{
					targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, t));
				}
				yield return null;
			}
			yield break;
		}
		private Projectile projectile;
	}
}

