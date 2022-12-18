using System;
using UnityEngine;

// Token: 0x020012B4 RID: 4788
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Natural Bloom and Dirty Lens")]
[ExecuteInEditMode]
public class SENaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x06006B25 RID: 27429 RVA: 0x0003F961 File Offset: 0x0003DB61
	public SENaturalBloomAndDirtyLens()
	{
		this.bloomIntensity = 0.05f;
		this.lensDirtIntensity = 0.05f;
		this.blurSize = 4f;		
	}

	// Token: 0x06006B26 RID: 27430 RVA: 0x002B0120 File Offset: 0x002AE320
	private void Start()
	{
		this.isSupported = true;
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			this.isSupported = false;
		}
	}

	// Token: 0x06006B27 RID: 27431 RVA: 0x0003F98A File Offset: 0x0003DB8A
	private void OnDisable()
	{
		if (this.material)
		{
			UnityEngine.Object.DestroyImmediate(this.material);
		}
	}

	// Token: 0x17000FE4 RID: 4068
	// (get) Token: 0x06006B28 RID: 27432 RVA: 0x0003F9A7 File Offset: 0x0003DBA7
	protected int IterationCount
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 1;
			}
			return 2;
		}
	}

	// Token: 0x06006B29 RID: 27433 RVA: 0x002B017C File Offset: 0x002AE37C
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.overrideDisable)
		{
			return;
		}
		if (!this.isSupported)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		this.material.hideFlags = HideFlags.HideAndDontSave;
		this.material.SetFloat("_BloomIntensity", Mathf.Exp(this.bloomIntensity) - 1f);
		this.material.SetFloat("_LensDirtIntensity", Mathf.Exp(this.lensDirtIntensity) - 1f);
		source.filterMode = FilterMode.Bilinear;
		int num = source.width / 2;
		int num2 = source.height / 2;
		RenderTexture source2 = source;
		int iterationCount = this.IterationCount;
		for (int i = 0; i < 6; i++)
		{
			RenderTexture renderTexture = RenderTexture.GetTemporary(num, num2, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source2, renderTexture, this.material, 1);
			source2 = renderTexture;
			float num3;
			if (i > 1)
			{
				num3 = 1f;
			}
			else
			{
				num3 = 0.5f;
			}
			if (i == 2)
			{
				num3 = 0.75f;
			}
			for (int j = 0; j < iterationCount; j++)
			{
				this.material.SetFloat("_BlurSize", (this.blurSize * 0.5f + (float)j) * num3);
				RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.material, 2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(num, num2, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.material, 3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			switch (i)
			{
				case 0:
					this.material.SetTexture("_Bloom0", renderTexture);
					break;
				case 1:
					this.material.SetTexture("_Bloom1", renderTexture);
					break;
				case 2:
					this.material.SetTexture("_Bloom2", renderTexture);
					break;
				case 3:
					this.material.SetTexture("_Bloom3", renderTexture);
					break;
				case 4:
					this.material.SetTexture("_Bloom4", renderTexture);
					break;
				case 5:
					this.material.SetTexture("_Bloom5", renderTexture);
					break;
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			num /= 2;
			num2 /= 2;
		}
		this.material.SetTexture("_LensDirt", this.lensDirtTexture);
		Graphics.Blit(source, destination, this.material, 0);
	}

	// Token: 0x04006807 RID: 26631
	[Range(0f, 0.4f)]
	public float bloomIntensity;

	// Token: 0x04006808 RID: 26632
	public Shader shader;

	// Token: 0x04006809 RID: 26633
	private Material material;

	// Token: 0x0400680A RID: 26634
	public Texture2D lensDirtTexture;

	// Token: 0x0400680B RID: 26635
	[Range(0f, 0.95f)]
	public float lensDirtIntensity;

	// Token: 0x0400680C RID: 26636
	private bool isSupported;

	// Token: 0x0400680D RID: 26637
	private float blurSize;

	// Token: 0x0400680E RID: 26638
	public bool inputIsHDR;

	// Token: 0x0400680F RID: 26639
	[HideInInspector]
	public bool overrideDisable;
}
