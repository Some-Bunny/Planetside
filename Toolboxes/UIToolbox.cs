using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;
using ItemAPI;
using FullInspector;

using Gungeon;

//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;

using Brave.BulletScript;
using GungeonAPI;

using System.Text;
using System.IO;
using System.Reflection;
using SaveAPI;

using MonoMod.RuntimeDetour;
using DaikonForge;


namespace Planetside
{

    public class UIToolbox : TimeInvariantMonoBehaviour
    {
		public IEnumerator WriteTextABovePlayer(Color color, string customKey, GameObject attachobject, dfPivotPoint abchor, Vector3 offset, float customDuration = -1f, float TextScale = 1, float FadeInTime = 0, float FadeOutTime = 0, float DelayOnDisplay = 0)
		{
			yield return new WaitForSeconds(DelayOnDisplay);
			UIToolbox foo = new UIToolbox();
			foo.SpecialTextBox(color, customKey, attachobject, abchor, offset, customDuration, TextScale, FadeInTime, FadeOutTime);
			yield break;
		}
		public static void TextBox(Color color, string customKey, GameObject attachobject, dfPivotPoint abchor , Vector3 offset, float customDuration = -1f, float TextScale = 1, float FadeInTime = 0, float FadeOutTime = 0, float DelayDisplay = 0)
		{
			UIToolbox foo = new UIToolbox();
			GameManager.Instance.StartCoroutine(foo.WriteTextABovePlayer(color, customKey, attachobject, abchor, offset, customDuration, TextScale, FadeInTime, FadeOutTime, DelayDisplay));
		}

		public void SpecialTextBox(Color color, string customKey, GameObject attachobject, dfPivotPoint anchor , Vector3 offset, float customDuration = -1f, float SizeMultiplier = 1, float FadeInTime = 0, float FadeOutTime = 0)
		
		{
			GameUIRoot UIRoot = GameUIRoot.Instance;
			
			Type type = typeof(GameUIRoot); FieldInfo _property = type.GetField("m_displayingReloadNeeded", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(UIRoot);
			List<bool> DisRelNeed = (List<bool>)_property.GetValue(UIRoot);

			Type type1 = typeof(GameUIRoot); FieldInfo _property1 = type1.GetField("m_extantReloadLabels", BindingFlags.NonPublic | BindingFlags.Instance); _property1.GetValue(UIRoot);
			List<dfLabel> ListDF = (List<dfLabel>)_property1.GetValue(UIRoot);
			if (!attachobject)
			{
				return;
			}
			int num = 0;
			if (DisRelNeed == null || num >= DisRelNeed.Count)
			{
				return;
			}
			if (ListDF == null || num >= ListDF.Count)
			{
				return;
			}
			if (DisRelNeed[num])
			{
				return;
			}
			dfLabel dfLabel = ListDF[num];
			if (dfLabel == null || dfLabel.IsVisible)
			{
				return;
			}
			dfFollowObject component = dfLabel.GetComponent<dfFollowObject>();
			dfLabel.IsVisible = true;
			if (component)
			{
				component.enabled = true;
			}
			GameManager.Instance.StartCoroutine(DisplayTextLabel(color, dfLabel, attachobject, anchor, offset, customDuration, customKey, SizeMultiplier, FadeInTime, FadeOutTime));
		}

		float CalculateCenterXoffset(dfLabel label)
		{
			return label.GetCenter().x - label.transform.position.x;
		}
		public IEnumerator DisplayTextLabel(Color color, dfControl target, GameObject attachobject, dfPivotPoint anchor, Vector3 offset, float customDuration, string customStringKey = "", float SizeMultiplier = 1, float FadeInTime = 0, float FadeOutTime = 0)
		{
			GameUIRoot UIRoot = GameUIRoot.Instance;
			Type type = typeof(GameUIRoot); FieldInfo _property = type.GetField("m_displayingReloadNeeded", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(UIRoot);
			List<bool> DisRelNeed = (List<bool>)_property.GetValue(UIRoot);

			Type type1 = typeof(GameUIRoot); FieldInfo _property1 = type1.GetField("m_isDisplayingCustomReload", BindingFlags.NonPublic | BindingFlags.Instance); _property1.GetValue(UIRoot);
			bool Displaying = (bool)_property1.GetValue(UIRoot);

			PlayerController player = GameManager.Instance.BestActivePlayer;
			int targetIndex = (!player.IsPrimaryPlayer) ? 1 : 0;
			DisRelNeed[targetIndex] = true;
			target.transform.localScale = Vector3.one / GameUIRoot.GameUIScalar;


			dfLabel targetLabel = target as dfLabel;
			string customString = string.Empty;

			dfFollowObject component = targetLabel.gameObject.AddComponent<dfFollowObject>();
			component.attach = attachobject.gameObject;
			component.enabled = true;
			component.mainCamera = GameManager.Instance.MainCameraController.Camera;
			component.anchor = anchor;
			component.offset = new Vector3(CalculateCenterXoffset(targetLabel), 0) + offset;

			customDuration += FadeInTime;
			customDuration += FadeOutTime;

			if (!string.IsNullOrEmpty(customStringKey))
			{
				customString = customStringKey;
			}
			if (customDuration > 0f)
			{
				Displaying = true;
				float outerElapsed = 0f;
				float ExitFade = FadeOutTime;
				while (outerElapsed < customDuration && !GameManager.Instance.IsPaused)
				{
					
					if (outerElapsed < FadeInTime && FadeInTime != 0)
                    {
						float t = outerElapsed / FadeInTime;
						target.IsVisible = true;
						targetLabel.Text = customString;
						targetLabel.Color = color;
						targetLabel.TextScale = SizeMultiplier;
						targetLabel.Opacity = t;
						outerElapsed += BraveTime.DeltaTime;
						yield return null;
					}
				
					else if (outerElapsed > customDuration - FadeOutTime && FadeOutTime != 0)
                    {
						float t = ExitFade / FadeOutTime;
						target.IsVisible = true;
						targetLabel.Text = customString;
						targetLabel.Color = color;
						targetLabel.TextScale = SizeMultiplier;
						targetLabel.Opacity = t;
						outerElapsed += BraveTime.DeltaTime;
						ExitFade -= BraveTime.DeltaTime;
						yield return null;
					}
					else if (outerElapsed > FadeInTime && outerElapsed < customDuration - FadeOutTime)
					{
						target.IsVisible = true;
						targetLabel.Text = customString;
						targetLabel.Color = color;
						targetLabel.TextScale = SizeMultiplier;
						targetLabel.Opacity = 1;
						outerElapsed += BraveTime.DeltaTime;
						yield return null;
					}
					else if (outerElapsed < customDuration && !GameManager.Instance.IsPaused)
					{
						target.IsVisible = true;
						targetLabel.Text = customString;
						targetLabel.Color = color;
						targetLabel.TextScale = SizeMultiplier;
						targetLabel.Opacity = 1;
						outerElapsed += BraveTime.DeltaTime;
					}
					yield return null;
				}
				Displaying = false;
			}
			else
			{
				while (DisRelNeed[targetIndex] && !GameManager.Instance.IsPaused)
				{
					target.IsVisible = true;
					if (!string.IsNullOrEmpty(customString))
					{
						targetLabel.Text = customString;
						targetLabel.Color = Color.white;
					}
					else
					{
						targetLabel.Text = "no";
						targetLabel.Color = Color.red;
					}
					bool shouldShowEver = customDuration > 0f;
					float elapsed = 0f;
					while (elapsed < 0.6f)
					{
						elapsed += this.m_deltaTime;
						if (!DisRelNeed[targetIndex])
						{
							target.IsVisible = false;
							yield break;
						}
						if (!shouldShowEver)
						{
							target.IsVisible = false;
						}
						if (GameManager.Instance.IsPaused)
						{
							target.IsVisible = false;
						}
						yield return null;
					}
					target.IsVisible = false;
					elapsed = 0f;
					while (elapsed < 0.6f)
					{
						elapsed += this.m_deltaTime;
						if (!DisRelNeed[targetIndex])
						{
							target.IsVisible = false;
							yield break;
						}
						yield return null;
					}
				}
			}
			DisRelNeed[targetIndex] = false;
			target.IsVisible = false;
			yield break;
		}
	}
}



namespace Planetside
{
	class TextMaker : MonoBehaviour
	{
		dfControl nameLabel;
		tk2dSprite sprite = null;

		public string Text;
		public Color Color;
		public float Opacity;
		public float TextSize;
		//============================//
		public dfPivotPoint anchor;
		public Vector3 offset;
		public GameObject GameObjectToAttachTo;
		//============================//
		public float FadeInTime;
		public float FadeOutTime;
		public float ExistTime;
		public float Delay;
		public bool TextDisappearsEver;
		//============================//
		private float Elapsed = 0;
		private float ExitFade;
		//============================//

		public TextMaker()
		{
			this.Text = "test";
			this.Color = Color.white;
			this.Opacity = 1;
			this.TextSize = 3;
			this.TextDisappearsEver = true;
			//============================//
			this.anchor = dfPivotPoint.TopCenter;
			this.offset = new Vector3(0, 0);
			//this.GameObjectToAttachTo = GameManager.Instance.PrimaryPlayer.gameObject;
			//============================//
			this.FadeInTime = 0;
			this.FadeOutTime = 0;
			this.ExistTime = 1;
			this.Delay = 0;

		}

		void Start()
		{

			ExistTime += FadeInTime;
			ExistTime += FadeOutTime;
			ExitFade = FadeOutTime;
			sprite = this.gameObject.GetComponent<tk2dSprite>();
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DamagePopupLabel", ".prefab"), GameUIRoot.Instance.transform);
			Vector3 worldPosition = base.transform.position;
			dfLabel Label = gameObject.GetComponent<dfLabel>();
			
			dfLabel targetLabel = Label as dfLabel;

			targetLabel.gameObject.SetActive(true);

			targetLabel.Text = this.Text;
			targetLabel.Color = this.Color;
			if (FadeInTime > 0)
			{
				targetLabel.Opacity = 0;
			}
			else
			{
				targetLabel.Opacity = this.Opacity;
			}
			targetLabel.TextScale = this.TextSize;

			targetLabel.transform.position = dfFollowObject.ConvertWorldSpaces(worldPosition, GameManager.Instance.MainCameraController.Camera, GameUIRoot.Instance.Manager.RenderCamera).WithZ(0f);
			targetLabel.transform.position = targetLabel.transform.position.QuantizeFloor(targetLabel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
			xOffSet = CalculateCenterXoffset(targetLabel);
			
			dfFollowObject component = targetLabel.gameObject.AddComponent<dfFollowObject>();
			component.attach = base.transform.gameObject;
			component.enabled = true;
			component.mainCamera = GameManager.Instance.MainCameraController.Camera;
			component.anchor = this.anchor;
			component.offset = new Vector3(CalculateCenterXoffset(targetLabel), 0) + this.offset;
			nameLabel = targetLabel;
		}

		float CalculateCenterXoffset(dfLabel label)
		{
			return label.GetCenter().x - label.transform.position.x;
		}
		float xOffSet = 0;

		public void Update()
		{
			dfLabel targetLabel = nameLabel as dfLabel;
			if (nameLabel != null)
            {
				if (GameManager.Instance.IsPaused)
				{

					targetLabel.IsVisible = false;
				}
				else
				{
					if (TextDisappearsEver == true && IsForceFading == false)
                    {
						targetLabel.IsVisible = true;
						this.Elapsed += BraveTime.DeltaTime;
						if (Elapsed < FadeInTime && FadeInTime != 0)
						{
							float t = Elapsed / FadeInTime;
							targetLabel.Opacity = t * Opacity;
						}
						else if (Elapsed > ExistTime - FadeOutTime && FadeOutTime != 0)
						{
							float t = ExitFade / FadeOutTime;
							targetLabel.Opacity = t * Opacity;
							ExitFade -= BraveTime.DeltaTime;
						}
						else if (Elapsed > FadeInTime && Elapsed < ExistTime - FadeOutTime)
						{
							targetLabel.Opacity = Opacity;
						}
						else if (Elapsed < ExistTime && !GameManager.Instance.IsPaused)
						{
							targetLabel.Opacity = Opacity;
						}
						else if (Elapsed > ExistTime)
						{
							UnityEngine.Object.Destroy(targetLabel.gameObject);
						}
					}
					else if (IsForceFading == false)
					{
						targetLabel.IsVisible = true;
						this.Elapsed += BraveTime.DeltaTime;
						if (Elapsed < FadeInTime && FadeInTime != 0)
						{
							float t = Elapsed / FadeInTime;
							targetLabel.Opacity = t * Opacity;
						}
						else if (Elapsed > FadeInTime && Elapsed < ExistTime - FadeOutTime)
						{
							targetLabel.Opacity = Opacity;
						}
						else if (Elapsed < ExistTime && !GameManager.Instance.IsPaused)
						{
							targetLabel.Opacity = Opacity;
						}
					}
				}
			}
		}
		public void ChangeText(string text)
		{
			dfLabel targetLabel = nameLabel as dfLabel;
			if (nameLabel != null)
            {
				targetLabel.Text = text;
			}
		}
		public void ChangeColor(Color color)
		{
			dfLabel targetLabel = nameLabel as dfLabel;
			if (nameLabel != null)
			{
				targetLabel.Color = color;
			}
		}
		public void ChangeSize(float size)
		{
			dfLabel targetLabel = nameLabel as dfLabel;
			if (nameLabel != null)
			{
				targetLabel.TextScale = size;
			}
		}
		public void ChangeOffset(Vector3 vector)
		{
			dfFollowObject targetLabel = nameLabel.GetComponent<dfFollowObject>();
			if (targetLabel != null && nameLabel != null)
			{
				targetLabel.offset = vector;
			}
		}

		public void ForceFadeOut(float Length)
		{
			dfFollowObject targetLabel = nameLabel.GetComponent<dfFollowObject>();
			if (targetLabel != null && nameLabel != null)
			{
				GameManager.Instance.StartCoroutine(ForceFadeMethod(Length));
			}
		}
		private bool IsForceFading;
		private IEnumerator ForceFadeMethod(float Length)
        {
			float ExitFadeForce = Length;
			IsForceFading = true;
			dfLabel targetLabel = nameLabel as dfLabel;
			if (targetLabel != null && nameLabel != null)
			{
				this.Elapsed += BraveTime.DeltaTime;
				{
					float t = ExitFadeForce / Length;
					targetLabel.Opacity = t * Opacity;
					ExitFadeForce -= BraveTime.DeltaTime;
				}
			}
			
			yield break;
        }

		private void OnDestroy()
		{
			if (nameLabel != null)
			{
                if (nameLabel.gameObject != null)
                {
                    UnityEngine.Object.Destroy(nameLabel.gameObject);
                }
            }		
		}
	}
}