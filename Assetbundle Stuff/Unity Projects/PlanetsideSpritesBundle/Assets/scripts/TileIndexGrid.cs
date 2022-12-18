using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TileIndexGrid", menuName = "ScriptableObject", order = 1)]
public class TileIndexGrid : ScriptableObject {

	// Token: 0x04004C6E RID: 19566
	public int roomTypeRestriction;

	// Token: 0x04004C6F RID: 19567

	public TileIndexList topLeftIndices;

	// Token: 0x04004C70 RID: 19568

	public TileIndexList topIndices;

	// Token: 0x04004C71 RID: 19569

	public TileIndexList topRightIndices;

	// Token: 0x04004C72 RID: 19570

	public TileIndexList leftIndices;

	// Token: 0x04004C73 RID: 19571

	public TileIndexList centerIndices;

	// Token: 0x04004C74 RID: 19572

	public TileIndexList rightIndices;

	// Token: 0x04004C75 RID: 19573

	public TileIndexList bottomLeftIndices;

	// Token: 0x04004C76 RID: 19574

	public TileIndexList bottomIndices;

	// Token: 0x04004C77 RID: 19575

	public TileIndexList bottomRightIndices;

	// Token: 0x04004C78 RID: 19576

	public TileIndexList horizontalIndices;

	// Token: 0x04004C79 RID: 19577

	public TileIndexList verticalIndices;

	// Token: 0x04004C7A RID: 19578

	public TileIndexList topCapIndices;

	// Token: 0x04004C7B RID: 19579

	public TileIndexList rightCapIndices;

	// Token: 0x04004C7C RID: 19580

	public TileIndexList bottomCapIndices;

	// Token: 0x04004C7D RID: 19581

	public TileIndexList leftCapIndices;

	// Token: 0x04004C7E RID: 19582

	public TileIndexList allSidesIndices;

	// Token: 0x04004C7F RID: 19583

	public TileIndexList topLeftNubIndices;

	// Token: 0x04004C80 RID: 19584

	public TileIndexList topRightNubIndices;

	// Token: 0x04004C81 RID: 19585

	public TileIndexList bottomLeftNubIndices;

	// Token: 0x04004C82 RID: 19586

	public TileIndexList bottomRightNubIndices;

	// Token: 0x04004C83 RID: 19587
	public bool extendedSet;

	// Token: 0x04004C84 RID: 19588

	[Header("Extended Set")]
	public TileIndexList topCenterLeftIndices;

	// Token: 0x04004C85 RID: 19589

	public TileIndexList topCenterIndices;

	// Token: 0x04004C86 RID: 19590

	public TileIndexList topCenterRightIndices;

	// Token: 0x04004C87 RID: 19591

	public TileIndexList thirdTopRowLeftIndices;

	// Token: 0x04004C88 RID: 19592

	public TileIndexList thirdTopRowCenterIndices;

	// Token: 0x04004C89 RID: 19593

	public TileIndexList thirdTopRowRightIndices;

	// Token: 0x04004C8A RID: 19594

	public TileIndexList internalBottomLeftCenterIndices;

	// Token: 0x04004C8B RID: 19595

	public TileIndexList internalBottomCenterIndices;

	// Token: 0x04004C8C RID: 19596

	public TileIndexList internalBottomRightCenterIndices;

	// Token: 0x04004C8D RID: 19597

	[Header("Additional Borders")]
	public TileIndexList borderTopNubLeftIndices;

	// Token: 0x04004C8E RID: 19598

	public TileIndexList borderTopNubRightIndices;

	// Token: 0x04004C8F RID: 19599

	public TileIndexList borderTopNubBothIndices;

	// Token: 0x04004C90 RID: 19600

	public TileIndexList borderRightNubTopIndices;

	// Token: 0x04004C91 RID: 19601

	public TileIndexList borderRightNubBottomIndices;

	// Token: 0x04004C92 RID: 19602

	public TileIndexList borderRightNubBothIndices;

	// Token: 0x04004C93 RID: 19603

	public TileIndexList borderBottomNubLeftIndices;

	// Token: 0x04004C94 RID: 19604

	public TileIndexList borderBottomNubRightIndices;

	// Token: 0x04004C95 RID: 19605

	public TileIndexList borderBottomNubBothIndices;

	// Token: 0x04004C96 RID: 19606

	public TileIndexList borderLeftNubTopIndices;

	// Token: 0x04004C97 RID: 19607

	public TileIndexList borderLeftNubBottomIndices;

	// Token: 0x04004C98 RID: 19608

	public TileIndexList borderLeftNubBothIndices;

	// Token: 0x04004C99 RID: 19609

	public TileIndexList diagonalNubsTopLeftBottomRight;

	// Token: 0x04004C9A RID: 19610

	public TileIndexList diagonalNubsTopRightBottomLeft;

	// Token: 0x04004C9B RID: 19611

	public TileIndexList doubleNubsTop;

	// Token: 0x04004C9C RID: 19612

	public TileIndexList doubleNubsRight;

	// Token: 0x04004C9D RID: 19613

	public TileIndexList doubleNubsBottom;

	// Token: 0x04004C9E RID: 19614

	public TileIndexList doubleNubsLeft;

	// Token: 0x04004C9F RID: 19615

	public TileIndexList quadNubs;

	// Token: 0x04004CA0 RID: 19616

	public TileIndexList topRightWithNub;

	// Token: 0x04004CA1 RID: 19617

	public TileIndexList topLeftWithNub;

	// Token: 0x04004CA2 RID: 19618

	public TileIndexList bottomRightWithNub;

	// Token: 0x04004CA3 RID: 19619

	public TileIndexList bottomLeftWithNub;

	// Token: 0x04004CA4 RID: 19620
	[Header("Diagonals--For Borders Only")]

	public TileIndexList diagonalBorderNE;

	// Token: 0x04004CA5 RID: 19621

	public TileIndexList diagonalBorderSE;

	// Token: 0x04004CA6 RID: 19622

	public TileIndexList diagonalBorderSW;

	// Token: 0x04004CA7 RID: 19623

	public TileIndexList diagonalBorderNW;

	// Token: 0x04004CA8 RID: 19624

	public TileIndexList diagonalCeilingNE;

	// Token: 0x04004CA9 RID: 19625

	public TileIndexList diagonalCeilingSE;

	// Token: 0x04004CAA RID: 19626

	public TileIndexList diagonalCeilingSW;

	// Token: 0x04004CAB RID: 19627

	public TileIndexList diagonalCeilingNW;

	// Token: 0x04004CAC RID: 19628
	[Header("Carpet Options")]
	public bool CenterCheckerboard;

	// Token: 0x04004CAD RID: 19629
	public int CheckerboardDimension;

	// Token: 0x04004CAE RID: 19630
	public bool CenterIndicesAreStrata;

	// Token: 0x04004CAF RID: 19631
	[Header("Weirdo Options")]
	[Space(5f)]
	public List<TileIndexGrid> PitInternalSquareGrids;

	// Token: 0x04004CB0 RID: 19632
	[Space(5f)]
	public PitSquarePlacementOptions PitInternalSquareOptions;

	// Token: 0x04004CB1 RID: 19633
	[Space(5f)]
	public bool PitBorderIsInternal;

	// Token: 0x04004CB2 RID: 19634
	[Space(5f)]
	public bool PitBorderOverridesFloorTile;

	// Token: 0x04004CB3 RID: 19635
	[Space(5f)]
	public bool CeilingBorderUsesDistancedCenters;

	// Token: 0x04004CB4 RID: 19636
	[Space(5f)]
	[Header("For Rat Chunk Borders")]
	public bool UsesRatChunkBorders;

	// Token: 0x04004CB5 RID: 19637

	public TileIndexList RatChunkNormalSet;

	// Token: 0x04004CB6 RID: 19638

	public TileIndexList RatChunkBottomSet;

	// Token: 0x04004CB7 RID: 19639
	[Header("Path Options")]
	[Space(5f)]
	public GameObject PathFacewallStamp;

	// Token: 0x04004CB8 RID: 19640
	public GameObject PathSidewallStamp;

	// Token: 0x04004CB9 RID: 19641
	[Space(5f)]

	public TileIndexList PathPitPosts;

	// Token: 0x04004CBA RID: 19642

	public TileIndexList PathPitPostsBL;

	// Token: 0x04004CBB RID: 19643

	public TileIndexList PathPitPostsBR;

	// Token: 0x04004CBC RID: 19644
	[Space(5f)]
	public GameObject PathStubNorth;

	// Token: 0x04004CBD RID: 19645
	public GameObject PathStubEast;

	// Token: 0x04004CBE RID: 19646
	public GameObject PathStubSouth;

	// Token: 0x04004CBF RID: 19647
	public GameObject PathStubWest;

	// Token: 0x02000F3E RID: 3902
	public enum RatChunkResult
	{
		// Token: 0x04004CC1 RID: 19649
		NONE,
		// Token: 0x04004CC2 RID: 19650
		NORMAL,
		// Token: 0x04004CC3 RID: 19651
		BOTTOM,
		// Token: 0x04004CC4 RID: 19652
		CORNER
	}
}
