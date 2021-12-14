using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGUI;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine.Collections;
using MonoMod.RuntimeDetour;

namespace Planetside
{
	public static class AdvancedLogging
	{
		public static void LogPlain(object msg, Color32? col = null)
		{
			Color color = Color.white;
			if (col != null)
			{
				color = col.Value;
			}
			SLabel label = new SLabel(msg.ToString());
			label.Colors[0] = color;
			ETGModConsole.Instance.GUI[0].Children.Add(label);
		}

		/// <summary>
		/// Used to log messages. messages can have the mod name, mod icon, both, or none in front.
		/// if color is set to null, messege color will be set to white
		/// note that modifiers are applied the sGroup children, not to the sGroup. if you want to change/add modifiers to the sGroup, use the returned sGroup
		/// n order to log an image in your text, you do "@(embedded file path, sizemult)" in your text, sizemult is not required, will default to 1.
		/// </summary>
		/// <param name="msg">the object or string you want to log</param>
		/// <param name="modifiers">an array of Selement modifiers to be added to each element logged</param>
		/// <param name="col">text color in unity Color32 or Color</param>
		/// <param name="HaveModName">whether your log messege will have the mod name at the front</param>
		/// <param name="HaveModIcon">whether your logged messege will have your mod icon at the front</param>
		public static SGroup Log(object msg, Color32? col = null, bool HaveModName = false, bool HaveModIcon = false, SModifier[] modifiers = null)
		{

			//in your module outside of methods you need:
			// public static ETGModuleMetadata metadata = new ETGModuleMetadata(); 
			//public static string ZipFilePath;

			//then in your modules init you need:
			// metadata = this.Metadata;
			//ZipFilePath = this.Metadata.Archive;
			Color color = Color.white;
			if (col != null)
			{
				color = col.Value;
			}
			string message = msg.ToString();

			SGroup sGroup = new SGroup();
			sGroup.AutoGrowDirection = SGroup.EDirection.Vertical;
			sGroup.AutoLayout = (SGroup g) => g.AutoLayoutHorizontal;
			sGroup.OnUpdateStyle = delegate (SElement elem)
			{
				elem.Fill();
			};
			sGroup.AutoLayoutVerticalStretch = false;
			sGroup.AutoLayoutHorizontalStretch = false;
			sGroup.GrowExtra = Vector2.zero;
			sGroup.ContentSize = Vector2.zero;
			sGroup.AutoLayoutPadding = 0;
			sGroup.Background = Color.clear;

			if (File.Exists(PlanetsideModule.metadata.Archive))
			{
				if (HaveModIcon)
				{
					SImage icon = new SImage(PlanetsideModule.metadata.Icon);
					sGroup.Children.Add(icon);
				}
			}
			if (HaveModName)
			{
				LogLabel modname = new LogLabel(PlanetsideModule.metadata.Name + ": ");
				modname.Colors[0] = color;
				sGroup.Children.Add(modname);
			}
			string[] split = Regex.Split(message, "(@\\(.+?\\))");
			foreach (string item in split)
			{

				if (item.StartsWith("@("))
				{
					string image = item.TrimStart('@', '(').TrimEnd(')');
					string[] sizeMult = image.Split(',');
					image = sizeMult[0];
					float SizeMultButForReal = 1;

					if (sizeMult.Length > 1)
					{
						if (sizeMult[1] != null && sizeMult[1] != "" && sizeMult[1] != " ")
							SizeMultButForReal = float.Parse(sizeMult[1]);
					}

					string extension = !image.EndsWith(".png") ? ".png" : "";
					string path = image + extension;
					Texture2D tex = GetTextureFromResource(path);
					TextureScale.Point(tex, Mathf.RoundToInt(tex.width * SizeMultButForReal), Mathf.RoundToInt(tex.height * SizeMultButForReal));

					SImage img = new SImage(tex);
					sGroup.Children.Add(img);
					var idx = sGroup.Children.IndexOf(img);
				}
				else
				{
					LogLabel label = new LogLabel(item);
					label.Colors[0] = color;
					label.Background = Color.clear;
					sGroup.Children.Add(label);
				}
				if (modifiers != null)
				{
					for (int i = 0; i < modifiers.Length; i++)
					{
						sGroup.Children[sGroup.Children.Count - 1].Modifiers.Add(modifiers[i]);
					}
				}
			}
			ETGModConsole.Instance.GUI[0].Children.Add(sGroup);

			ETGModConsole.Instance.GUI[0].UpdateStyle();
			return sGroup;
		}

		/// <summary>
		/// used to log buttons, buttons can run certain code when pressed. 
		/// do SButton button = YourLogCode.
		/// then button.OnClick += myMethod;
		/// </summary>
		/// <param name="msg">the object or string you want to log</param>
		/// <param name="col">text color you want</param>
		/// <param name="HaveModName">whether your log messege will have the mod name at the front</param>
		/// <param name="HaveModIcon">whether your logged messege will have your mod icon at the front</param>
		public static SButton LogButton(object msg, Color32? col = null, bool HaveModName = false, bool HaveModIcon = false)
		{

			SButton img;
			Color color = Color.white;
			if (col != null)
			{
				color = col.Value;
			}
			if (HaveModIcon == false)
			{
				if (HaveModName == false)
				{
					img = new SButton($"{msg}");
					img.Colors[0] = color;
					ETGModConsole.Instance.GUI[0].Children.Add(img);

				}
				else
				{
					img = new SButton($"{PlanetsideModule.metadata.Name}: {msg}");
					img.Colors[0] = color;
					ETGModConsole.Instance.GUI[0].Children.Add(img);
				}
			}
			else
			{
				if (HaveModName == false)
				{
					img = new SButton($"{msg}");
					img.Colors[0] = color;
					if (File.Exists(PlanetsideModule.metadata.Archive))
						img.Icon = PlanetsideModule.metadata.Icon;
					ETGModConsole.Instance.GUI[0].Children.Add(img);
				}
				else
				{
					img = new SButton($"{PlanetsideModule.metadata.Name}: {msg}");
					img.Colors[0] = color;
					if (File.Exists(PlanetsideModule.metadata.Archive))
						img.Icon = PlanetsideModule.metadata.Icon;
					ETGModConsole.Instance.GUI[0].Children.Add(img);
				}
			}
			return img;

		}

		/// <summary>
		/// Used to log images. 
		/// if you want to log something you have in your mod, 
		/// ie. an embedded resource, use log and put in @(filepath)
		/// </summary>
		/// <param name="img">The texture of the item/enemy/image you want to log.</param> 
		/// <returns></returns>
		public static SImage LogImage(Texture img)
		{
			var image = new SImage(img);
			ETGModConsole.Instance.GUI[0].Children.Add(image);
			return image;
		}

		///// <summary>
		///// Used to log animations. 
		///// </summary>
		///// <param name="frames">a list of spritepaths used for your animation *in order*.</param> 
		///// <param name="TimeBetweenFrames"> how many seconds it should wait until it playes the next frame.</param> 
		///// <returns></returns>
		//public static void LogAnim(List<string> frames, float TimeBetweenFrames)
		//{
		//	var img = l(frames[0]);
		//	var imgthang = ETGModConsole.Instance.GUI[0].Children[img] as SImage;
		//	var anim = ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<PlayAnim>();
		//	anim.period = TimeBetweenFrames;
		//	anim.imgToAnim = img;
		//	anim.frames = frames;
		//}

		public static byte[] ExtractEmbeddedResource(String filePath)
		{
			filePath = filePath.Replace("/", ".");
			filePath = filePath.Replace("\\", ".");
			var baseAssembly = Assembly.GetCallingAssembly();
			using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filePath))
			{
				if (resFilestream == null)
				{
					return null;
				}
				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}

		public static Texture2D GetTextureFromResource(string resourceName)
		{
			string file = resourceName;
			byte[] bytes = ExtractEmbeddedResource(file);
			if (bytes == null)
			{
				AdvancedLogging.Log("No bytes found in " + file, Color.red);
				return null;
			}
			Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			ImageConversion.LoadImage(texture, bytes);
			texture.filterMode = FilterMode.Point;

			string name = file.Substring(0, file.LastIndexOf('.'));
			if (name.LastIndexOf('.') >= 0)
			{
				name = name.Substring(name.LastIndexOf('.') + 1);
			}
			texture.name = name;

			return texture;
		}
	}


	public class LogLabel : SElement
	{

		public string Text;

		public TextAnchor Alignment = TextAnchor.MiddleLeft;

		public LogLabel()
			: this("") { }

		public LogLabel(string text)
		{
			Text = text;
		}

		public override void UpdateStyle()
		{
			// This will get called again once this element gets added to the root.
			if (Root == null) return;

			if (UpdateBounds)
			{
				if (Parent == null)
				{
					Size = Backend.MeasureText(Text);
				}
				else
				{
					Size = Backend.MeasureText(Text, Parent.InnerSize, font: Font);
				}
			}

			base.UpdateStyle();
		}

		public override void Render()
		{
			RenderBackground();
			Draw.Text(this, Vector2.zero, Size, Text, Alignment);
		}
	}


}



