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
	public class TestShaderItem : PassiveItem
	{
		public static void Init()
		{
			string itemName = "PsogTestShaderItem";
			string resourceName = "Planetside/Resources/sirpaler.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<TestShaderItem>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Downwards Spiral, Downwards Spiral...";
			string longDesc = "Warped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\n." +
				"Warped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\n";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.EXCLUDED;

            var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Static\\static1.png");
            Material shade = new Material(ShaderCache.Acquire("Brave/PlayerShaderEevee"));

            shade.SetTexture("_EeveeTex", texture);

            shade.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            shade.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            item.m_glintShader = shade;
        }
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		private void PostProcessBeam(BeamController obj)
		{

			try
			{
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
        private void HandleGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            Material material = UnityEngine.Object.Instantiate<Material>(newGun.renderer.material);

            newGun.renderer.material = material;
            this.RemoveGunShader(oldGun);
            this.ProcessGunShader(newGun);
        }
        private void RemoveGunShader(Gun g)
        {
            if (!g)
            {
                return;
            }
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            List<Material> list = new List<Material>();
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader != this.m_glintShader.shader)
                {
                    list.Add(sharedMaterials[i]);
                }
            }
            component.sharedMaterials = list.ToArray();
        }
        private void ProcessGunShader(Gun g)
        {
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == this.m_glintShader.shader)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(this.m_glintShader);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
        }
        protected override void Update()
        {
            if (m_glintShader != null)
            {
                float RNG = UnityEngine.Random.Range(1, 5);
                string num = RNG.ToString();
                var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Static\\static"+ num+".png");
                m_glintShader.SetTexture("_EeveeTex", texture);
             }
        }
        private Material m_glintShader;

        public override void Pickup(PlayerController player)
		{
			player.GunChanged += this.HandleGunChanged;
			base.Pickup(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.GunChanged += this.HandleGunChanged;
			base.OnDestroy();
		}
	}
}
