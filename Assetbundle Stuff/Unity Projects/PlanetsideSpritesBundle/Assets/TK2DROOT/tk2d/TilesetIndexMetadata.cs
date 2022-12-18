using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E7C RID: 3708
[Serializable]
public class TilesetIndexMetadata
{
	// Token: 0x040045AB RID: 17835
	public TilesetIndexMetadata.TilesetFlagType type;

	// Token: 0x040045AC RID: 17836
	public float weight;

	// Token: 0x040045AD RID: 17837
	public int dungeonRoomSubType;

	// Token: 0x040045AE RID: 17838
	public int secondRoomSubType;

	// Token: 0x040045AF RID: 17839
	public int thirdRoomSubType;

	// Token: 0x040045B0 RID: 17840
	public bool preventWallStamping;

	// Token: 0x040045B1 RID: 17841
	public bool usesAnimSequence;

	// Token: 0x040045B2 RID: 17842
	public bool usesNeighborDependencies;

	// Token: 0x040045B3 RID: 17843
	public bool usesPerTileVFX;

	// Token: 0x040045B4 RID: 17844
	public TilesetIndexMetadata.VFXPlaystyle tileVFXPlaystyle;

	// Token: 0x040045B5 RID: 17845
	public float tileVFXChance;

	// Token: 0x040045B6 RID: 17846
	public GameObject tileVFXPrefab;

	// Token: 0x040045B7 RID: 17847
	public Vector2 tileVFXOffset;

	// Token: 0x040045B8 RID: 17848
	public float tileVFXDelayTime;

	// Token: 0x040045B9 RID: 17849
	public float tileVFXDelayVariance;

	// Token: 0x040045BA RID: 17850
	public int tileVFXAnimFrame;

	// Token: 0x02000E7D RID: 3709
	[Flags]
	public enum TilesetFlagType
	{
		NONE = 0,
		// Token: 0x040045BC RID: 17852
		FACEWALL_UPPER = 1,
		// Token: 0x040045BD RID: 17853
		FACEWALL_LOWER = 2,
		// Token: 0x040045BE RID: 17854
		FLOOR_TILE = 4,
		// Token: 0x040045BF RID: 17855
		CHEST_HIGH_WALL = 8,
		// Token: 0x040045C0 RID: 17856
		DECAL_TILE = 16,
		// Token: 0x040045C1 RID: 17857
		PATTERN_TILE = 32,
		// Token: 0x040045C2 RID: 17858
		DOOR_FEET_NS = 64,
		// Token: 0x040045C3 RID: 17859
		DOOR_FEET_EW = 128,
		// Token: 0x040045C4 RID: 17860
		DIAGONAL_FACEWALL_UPPER_NE = 256,
		// Token: 0x040045C5 RID: 17861
		DIAGONAL_FACEWALL_UPPER_NW = 512,
		// Token: 0x040045C6 RID: 17862
		DIAGONAL_FACEWALL_LOWER_NE = 1024,
		// Token: 0x040045C7 RID: 17863
		DIAGONAL_FACEWALL_LOWER_NW = 2048,
		// Token: 0x040045C8 RID: 17864
		DIAGONAL_FACEWALL_TOP_NE = 4096,
		// Token: 0x040045C9 RID: 17865
		DIAGONAL_FACEWALL_TOP_NW = 8192,
		// Token: 0x040045CA RID: 17866
		FACEWALL_LOWER_LEFTCORNER = 16384,
		// Token: 0x040045CB RID: 17867
		FACEWALL_LOWER_RIGHTCORNER = 32768,
		// Token: 0x040045CC RID: 17868
		FACEWALL_UPPER_LEFTCORNER = 65536,
		// Token: 0x040045CD RID: 17869
		FACEWALL_UPPER_RIGHTCORNER = 131072,
		// Token: 0x040045CE RID: 17870
		FACEWALL_LOWER_LEFTEDGE = 262144,
		// Token: 0x040045CF RID: 17871
		FACEWALL_LOWER_RIGHTEDGE = 524288,
		// Token: 0x040045D0 RID: 17872
		FACEWALL_UPPER_LEFTEDGE = 1048576,
		// Token: 0x040045D1 RID: 17873
		FACEWALL_UPPER_RIGHTEDGE = 2097152
	}

	// Token: 0x02000E7E RID: 3710
	public enum VFXPlaystyle
	{
		// Token: 0x040045D3 RID: 17875
		CONTINUOUS,
		// Token: 0x040045D4 RID: 17876
		TIMED_REPEAT,
		// Token: 0x040045D5 RID: 17877
		ON_ANIMATION_FRAME
	}
}
