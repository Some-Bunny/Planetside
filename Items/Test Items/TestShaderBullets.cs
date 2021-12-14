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

//Garbage Code Incoming
namespace Planetside
{
    public class TestShaderBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "TestPSOGShaderBullets";
            string resourceName = "Planetside/Resources/aurabullets.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TestShaderBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Radiant";
            string longDesc = "Makes bullets deal damage to enemies near them." +
                "\n\nThese bullets contain a very rare and powerful radioactive isotope. Don't lick them!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			//ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 0f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			//ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.PlayerBulletScale, 5f, StatModifier.ModifyMethod.MULTIPLICATIVE);

			item.quality = PickupObject.ItemQuality.EXCLUDED;
			
		}

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{

				TemplateMovementModule mod = new TemplateMovementModule();
				//mod.ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true;
				sourceProjectile.OverrideMotionModule = mod;	

				sourceProjectile.sprite.usesOverrideMaterial = true;
				sourceProjectile.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
				sourceProjectile.sprite.renderer.material.SetFloat("_Fade", 0.2f);
				//sourceProjectile.gameObject.AddComponent<TransShader>();


				//var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");

				//sourceProjectile.sprite.renderer.material.shader = ShaderCache.Acquire("Unlit/SimpleAlphaCutoutBlend");
				//sourceProjectile.sprite.renderer.material.SetFloat("_Fade", 0.33f);
				//sourceProjectile.sprite.renderer.material.SetTexture("_MainTex", sourceProjectile.sprite.renderer.material.mainTexture);
				//sourceProjectile.sprite.renderer.material.SetFloat("_BaseAlphaMax", 1f);
				//sourceProjectile.sprite.renderer.material.SetTexture("_AddlTex", sourceProjectile.sprite.renderer.material.mainTexture);

				//sourceProjectile.sprite.renderer.material.SetTexture("_MaskTex", texture);

				//material.SetTexture("_MainTex", sourceProjectile.sprite.renderer.material.mainTexture);
				//material.SetTexture("_MaskTex", sourceProjectile.sprite.renderer.material.mainTexture);
				//material.SetFloat("_Fade", 0.33f);
				/*
				Material material = new Material(ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit"));
				bool flag5 = sourceProjectile.sprite.spriteAnimator != null;
				if (flag5)
				{
					List<tk2dSpriteAnimationFrame> list = sourceProjectile.sprite.spriteAnimator.GetClipById(sourceProjectile.sprite.spriteAnimator.DefaultClipId).frames.ToList();
					for (int i = 0; i < list.Count; i++)
					{

						if (list[i].spriteCollection)
                        {
							ETGModConsole.Log(list[i].spriteCollection.);
						}
						else
                        {
							ETGModConsole.Log("ITS FUCKING NULL");
						}
						//list[i].spriteCollection.material.mainTexture.ToString();

						//ETGModConsole.Log(list[i].spriteCollection));
						//material.SetTexture("_MainTex", ETGMod.Databases.Items.ProjectileCollection.;
						//material.SetTexture("_MaskTex", list[i].spriteCollection.material.GetTexture("_MainTex"));
						//material.SetFloat("_Fade", 0.33f);
						//list[i].spriteCollection.material = material;

						
						//tk2dBaseSprite sprite = ETGMod.GetAnySprite(sourceProjectile).SetSprite(list[i].spriteCollection, list[i].spriteId);
						ETGModConsole.Log("2");
						material.SetTexture("_MainTex", );
						ETGModConsole.Log("2.1");
						material.SetTexture("_MaskTex", list[i].spriteCollection.material.mainTexture);
						ETGModConsole.Log("2.2");
						material.SetFloat("_Fade", 0.33f);
						ETGModConsole.Log("2.3");
						list[i].spriteCollection.material = material;
						ETGModConsole.Log("3");
						
					}
				}
				*/

				//foreach (tk2dSpriteAnimationFrame frame in sourceProjectile.spriteAnimator.GetClipById(sourceProjectile.spriteAnimator.DefaultClipId).frames)
				//{
				//material.SetTexture("_MainTex", frame.spriteCollection.material.mainTexture);
				//material.SetTexture("_MaskTex", frame.spriteCollection.material.mainTexture);
				//material.SetFloat("_Fade", 0.33f);
				//frame.spriteCollection.material = material;
				//}
				//List<Texture> Txt = sourceProjectile.spriteAnimator.GetClipById(ID).fra;


				//material.SetFloat("_Shininess", 1f);



			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}
		private void PostProcessBeam(BeamController obj)
		{

			try
			{

				TemplateMovementModule mod = new TemplateMovementModule();
				//mod.ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true;
				obj.projectile.OverrideMotionModule = mod;

				ETGModConsole.Log(obj.projectile.sprite.renderer.material.shader.ToString());

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
		protected override void OnDestroy()
		{
			if (Owner != null)
			{
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam += this.PostProcessBeam;
			}
			base.OnDestroy();
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;

		}

	}
	internal class TransShader : MonoBehaviour
	{
		public TransShader()
		{
		}
		public void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			Projectile projectile = this.projectile;
			PlayerController playerController = projectile.Owner as PlayerController;
			Projectile component = base.gameObject.GetComponent<Projectile>();
			bool flag = component != null;
			bool flag2 = flag;
			if (flag2)
			{
				component.sprite.usesOverrideMaterial = true;
				component.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
				component.sprite.renderer.material.SetFloat("_Fade", 0.2f);
			}
		}
		public void Update()
        {
			this.projectile = base.GetComponent<Projectile>();
			Projectile projectile = this.projectile;
			PlayerController playerController = projectile.Owner as PlayerController;
			Projectile component = base.gameObject.GetComponent<Projectile>();
			bool flag = component != null;
			bool flag2 = flag;
			if (flag2)
			{
				component.sprite.usesOverrideMaterial = true;
				component.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
				component.sprite.renderer.material.SetFloat("_Fade", 0.2f);
			}
		}
			
		
		private Projectile projectile;
	}
}


