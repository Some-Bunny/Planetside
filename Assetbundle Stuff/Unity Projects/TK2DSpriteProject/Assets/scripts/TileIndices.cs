using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileIndices {

	public ValidTilesets tilesetId;

	// Token: 0x04004798 RID: 18328
	public tk2dSpriteCollectionData dungeonCollection;

	// Token: 0x04004799 RID: 18329
	public bool dungeonCollectionSupportsDiagonalWalls;

	// Token: 0x0400479A RID: 18330
	public AOTileIndices aoTileIndices;

	// Token: 0x0400479B RID: 18331
	public bool placeBorders;

	// Token: 0x0400479C RID: 18332
	public bool placePits;

	// Token: 0x0400479D RID: 18333
	public List<TileIndexVariant> chestHighWallIndices;

	// Token: 0x0400479E RID: 18334
	public TileIndexGrid decalIndexGrid;

	// Token: 0x0400479F RID: 18335
	public TileIndexGrid patternIndexGrid;

	// Token: 0x040047A0 RID: 18336
	public List<int> globalSecondBorderTiles;

	// Token: 0x040047A1 RID: 18337
	public TileIndexGrid edgeDecorationTiles;

	public enum ValidTilesets
	{
		// Token: 0x040046A2 RID: 18082
		GUNGEON = 1,
		// Token: 0x040046A3 RID: 18083
		CASTLEGEON = 2,
		// Token: 0x040046A4 RID: 18084
		SEWERGEON = 4,
		// Token: 0x040046A5 RID: 18085
		CATHEDRALGEON = 8,
		// Token: 0x040046A6 RID: 18086
		MINEGEON = 16,
		// Token: 0x040046A7 RID: 18087
		CATACOMBGEON = 32,
		// Token: 0x040046A8 RID: 18088
		FORGEGEON = 64,
		// Token: 0x040046A9 RID: 18089
		HELLGEON = 128,
		// Token: 0x040046AA RID: 18090
		SPACEGEON = 256,
		// Token: 0x040046AB RID: 18091
		PHOBOSGEON = 512,
		// Token: 0x040046AC RID: 18092
		WESTGEON = 1024,
		// Token: 0x040046AD RID: 18093
		OFFICEGEON = 2048,
		// Token: 0x040046AE RID: 18094
		BELLYGEON = 4096,
		// Token: 0x040046AF RID: 18095
		JUNGLEGEON = 8192,
		// Token: 0x040046B0 RID: 18096
		FINALGEON = 16384,
		// Token: 0x040046B1 RID: 18097
		RATGEON = 32768
	}
}
