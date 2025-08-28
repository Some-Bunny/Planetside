using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;

namespace Planetside
{
	public static class ItemTools
	{
        public static PlayerItem GetRandomActiveOfQualities(System.Random usedRandom, List<int> excludedIDs, params PickupObject.ItemQuality[] qualities)
        {
            List<PlayerItem> list = new List<PlayerItem>();
            for (int i = 0; i < PickupObjectDatabase.Instance.Objects.Count; i++)
            {
                if (PickupObjectDatabase.Instance.Objects[i] != null && PickupObjectDatabase.Instance.Objects[i] is PlayerItem)
                {
                    if (PickupObjectDatabase.Instance.Objects[i].quality != PickupObject.ItemQuality.EXCLUDED && PickupObjectDatabase.Instance.Objects[i].quality != PickupObject.ItemQuality.SPECIAL)
                    {
                        if (!(PickupObjectDatabase.Instance.Objects[i] is ContentTeaserItem))
                        {
                            if (Array.IndexOf<PickupObject.ItemQuality>(qualities, PickupObjectDatabase.Instance.Objects[i].quality) != -1)
                            {
                                if (!excludedIDs.Contains(PickupObjectDatabase.Instance.Objects[i].PickupObjectId))
                                {
                                    EncounterTrackable component = PickupObjectDatabase.Instance.Objects[i].GetComponent<EncounterTrackable>();
                                    if (component && component.PrerequisitesMet())
                                    {
                                        list.Add(PickupObjectDatabase.Instance.Objects[i] as PlayerItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            int num = usedRandom.Next(list.Count);
            if (num < 0 || num >= list.Count)
            {
                return null;
            }
            return list[num];
        }

        public static void Init()
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			string text = "assets/data/goops/water goop.asset";
			GoopDefinition goopDefinition;
			try
			{
				GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition = gameObject.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			ItemTools.DefaultWaterGoop = goopDefinition;
			text = "assets/data/goops/poison goop.asset";
			GoopDefinition goopDefinition2;
			try
			{
				GameObject gameObject2 = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition2 = gameObject2.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition2 = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition2.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			ItemTools.DefaultPoisonGoop = goopDefinition2;
			text = "assets/data/goops/napalmgoopquickignite.asset";
			GoopDefinition goopDefinition3;
			try
			{
				GameObject gameObject3 = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition3 = gameObject3.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition3 = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition3.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			ItemTools.DefaultFireGoop = goopDefinition3;
			PickupObject byId = PickupObjectDatabase.GetById(310);
			bool flag = byId == null;
			GoopDefinition defaultCharmGoop;
			if (flag)
			{
				defaultCharmGoop = null;
			}
			else
			{
				WingsItem component = byId.GetComponent<WingsItem>();
				defaultCharmGoop = ((component != null) ? component.RollGoop : null);
			}
			ItemTools.DefaultCharmGoop = defaultCharmGoop;
			ItemTools.DefaultCheeseGoop = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect.CheeseGoop;
			ItemTools.DefaultBlobulonGoop = EnemyDatabase.GetOrLoadByGuid("0239c0680f9f467dbe5c4aab7dd1eca6").GetComponent<GoopDoer>().goopDefinition;
			ItemTools.DefaultPoopulonGoop = EnemyDatabase.GetOrLoadByGuid("116d09c26e624bca8cca09fc69c714b3").GetComponent<GoopDoer>().goopDefinition;
		}




        public static void AddCurrentGunStatModifier(this Gun gun, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod modifyMethod)
		{
			gun.currentGunStatModifiers = gun.currentGunStatModifiers.Concat(new StatModifier[]
			{
				new StatModifier
				{
					statToBoost = statType,
					amount = amount,
					modifyType = modifyMethod
				}
			}).ToArray<StatModifier>();
		}

		public static AIActor SummonAtRandomPosition(string guid, PlayerController owner)
		{
			return AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(guid), new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(1, 1)).Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(1, 1)).Value), true, AIActor.AwakenAnimationType.Awaken, true);
		}

		public static RoomHandler AddCustomRuntimeRoom(PrototypeDungeonRoom prototype, bool addRoomToMinimap = true, bool addTeleporter = true, bool isSecretRatExitRoom = false, Action<RoomHandler> postProcessCellData = null, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.STANDARD)
		{
			Dungeon dungeon = GameManager.Instance.Dungeon;
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
			tk2dTileMap component3 = gameObject3.GetComponent<tk2dTileMap>();
			string str = UnityEngine.Random.Range(10000, 99999).ToString();
			gameObject3.name = "Breach_RuntimeTilemap_" + str;
			component3.renderData.name = "Breach_RuntimeTilemap_" + str + " Render Data";

			component3.Editor__SpriteCollection = dungeon.tileIndices.dungeonCollection;

			TK2DDungeonAssembler.RuntimeResizeTileMap(component3, 8, 8, component3.partitionSizeX, component3.partitionSizeY);

			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
			tk2dTileMap component2 = gameObject2.GetComponent<tk2dTileMap>();
			//creepyRoom.OverrideTilemap = component;
			tk2dTileMap component4 = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
			tk2dTileMap mainTilemap = component4;
			//tk2dTileMap mainTilemap = dungeon.MainTilemap;

			if (mainTilemap == null)
			{
				ETGModConsole.Log("ERROR: TileMap object is null! Something seriously went wrong!", false);
				Debug.Log("ERROR: TileMap object is null! Something seriously went wrong!");
				return null;
			}
			TK2DDungeonAssembler tk2DDungeonAssembler = new TK2DDungeonAssembler();
			tk2DDungeonAssembler.Initialize(dungeon.tileIndices);
			IntVector2 zero = IntVector2.Zero;
			IntVector2 intVector = new IntVector2(50, 50);
			int x = intVector.x;
			int y = intVector.y;
			IntVector2 intVector2 = new IntVector2(int.MaxValue, int.MaxValue);
			IntVector2 lhs = new IntVector2(int.MinValue, int.MinValue);
			intVector2 = IntVector2.Min(intVector2, zero);
			IntVector2 intVector3 = IntVector2.Max(lhs, zero + new IntVector2(prototype.Width, prototype.Height)) - intVector2;
			IntVector2 b = IntVector2.Min(IntVector2.Zero, -1 * intVector2);
			intVector3 += b;
			IntVector2 intVector4 = new IntVector2(dungeon.data.Width + x, x);
			int newWidth = dungeon.data.Width + x * 2 + intVector3.x;
			int newHeight = Mathf.Max(dungeon.data.Height, intVector3.y + x * 2);
			CellData[][] array = BraveUtility.MultidimensionalArrayResize<CellData>(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
			dungeon.data.cellData = array;
			dungeon.data.ClearCachedCellData();
			IntVector2 intVector5 = new IntVector2(prototype.Width, prototype.Height);
			IntVector2 b2 = zero + b;
			IntVector2 intVector6 = intVector4 + b2;
			CellArea cellArea = new CellArea(intVector6, intVector5, 0);
			cellArea.prototypeRoom = prototype;
			RoomHandler roomHandler = new RoomHandler(cellArea);
			for (int i = -x; i < intVector5.x + x; i++)
			{
				for (int j = -x; j < intVector5.y + x; j++)
				{
					IntVector2 intVector7 = new IntVector2(i, j) + intVector6;
					if ((i >= 0 && j >= 0 && i < intVector5.x && j < intVector5.y) || array[intVector7.x][intVector7.y] == null)
					{
						CellData cellData = new CellData(intVector7, CellType.WALL);
						cellData.positionInTilemap = cellData.positionInTilemap - intVector4 + new IntVector2(y, y);
						cellData.parentArea = cellArea;
						cellData.parentRoom = roomHandler;
						cellData.nearestRoom = roomHandler;
						cellData.distanceFromNearestRoom = 0f;
						array[intVector7.x][intVector7.y] = cellData;
					}
				}
			}
			dungeon.data.rooms.Add(roomHandler);
			try
			{
				roomHandler.WriteRoomData(dungeon.data);
			}
			catch (Exception)
			{
				ETGModConsole.Log("WARNING: Exception caused during WriteRoomData step on room: " + roomHandler.GetRoomName(), false);
			}
			try
			{
				dungeon.data.GenerateLightsForRoom(dungeon.decoSettings, roomHandler, GameObject.Find("_Lights").transform, lightStyle);
			}
			catch (Exception)
			{
				ETGModConsole.Log("WARNING: Exception caused during GeernateLightsForRoom step on room: " + roomHandler.GetRoomName(), false);
			}
			if (postProcessCellData != null)
			{
				postProcessCellData(roomHandler);
			}
			if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
			{
				roomHandler.BuildSecretRoomCover();
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
			tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
			string str2 = UnityEngine.Random.Range(10000, 99999).ToString();
			gameObject.name = "Glitch_RuntimeTilemap_" + str;
			component.renderData.name = "Glitch_RuntimeTilemap_" + str + " Render Data";
			component.Editor__SpriteCollection = dungeon.tileIndices.dungeonCollection;
			try
			{
				TK2DDungeonAssembler.RuntimeResizeTileMap(component, intVector3.x + y * 2, intVector3.y + y * 2, mainTilemap.partitionSizeX, mainTilemap.partitionSizeY);
				IntVector2 intVector8 = new IntVector2(prototype.Width, prototype.Height);
				IntVector2 b3 = zero + b;
				IntVector2 intVector9 = intVector4 + b3;
				for (int k = -y; k < intVector8.x + y; k++)
				{
					for (int l = -y; l < intVector8.y + y + 2; l++)
					{
						tk2DDungeonAssembler.BuildTileIndicesForCell(dungeon, component, intVector9.x + k, intVector9.y + l);
					}
				}
				RenderMeshBuilder.CurrentCellXOffset = intVector4.x - y;
				RenderMeshBuilder.CurrentCellYOffset = intVector4.y - y;
				component.ForceBuild();
				RenderMeshBuilder.CurrentCellXOffset = 0;
				RenderMeshBuilder.CurrentCellYOffset = 0;
				component.renderData.transform.position = new Vector3((float)(intVector4.x - y), (float)(intVector4.y - y), (float)(intVector4.y - y));
			}
			catch (Exception exception)
			{
				ETGModConsole.Log("WARNING: Exception occured during RuntimeResizeTileMap / RenderMeshBuilder steps!", false);
				Debug.Log("WARNING: Exception occured during RuntimeResizeTileMap/RenderMeshBuilder steps!");
				Debug.LogException(exception);
			}
			roomHandler.OverrideTilemap = component;
			for (int m = 0; m < roomHandler.area.dimensions.x; m++)
			{
				for (int n = 0; n < roomHandler.area.dimensions.y + 2; n++)
				{
					IntVector2 intVector10 = roomHandler.area.basePosition + new IntVector2(m, n);
					if (dungeon.data.CheckInBoundsAndValid(intVector10))
					{
						CellData currentCell = dungeon.data[intVector10];
						TK2DInteriorDecorator.PlaceLightDecorationForCell(dungeon, component, currentCell, intVector10);
					}
				}
			}
			Pathfinder.Instance.InitializeRegion(dungeon.data, roomHandler.area.basePosition + new IntVector2(-3, -3), roomHandler.area.dimensions + new IntVector2(3, 3));
			if (prototype.usesProceduralDecoration && prototype.allowFloorDecoration)
			{
				new TK2DInteriorDecorator(tk2DDungeonAssembler).HandleRoomDecoration(roomHandler, dungeon, mainTilemap);
			}
			roomHandler.PostGenerationCleanup();
			if (addRoomToMinimap)
			{
				roomHandler.visibility = RoomHandler.VisibilityStatus.VISITED;
				MonoBehaviour mono = new MonoBehaviour();
				mono.StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(roomHandler, true, true, false));
				if (isSecretRatExitRoom)
				{
					roomHandler.visibility = RoomHandler.VisibilityStatus.OBSCURED;
				}
			}
			if (addTeleporter)
			{
				roomHandler.AddProceduralTeleporterToRoom();
			}
			if (addRoomToMinimap)
			{
				Minimap.Instance.InitializeMinimap(dungeon.data);
			}
			DeadlyDeadlyGoopManager.ReinitializeData();
			return roomHandler;
		}


		// Token: 0x06000173 RID: 371 RVA: 0x00013268 File Offset: 0x00011468
		public static GameActorEffect CopyEffectFrom(this GameActorEffect self, GameActorEffect other)
		{
			bool flag = self == null || other == null;
			GameActorEffect result;
			if (flag)
			{
				result = null;
			}
			else
			{
				self.AffectsPlayers = other.AffectsPlayers;
				self.AffectsEnemies = other.AffectsEnemies;
				self.effectIdentifier = other.effectIdentifier;
				self.resistanceType = other.resistanceType;
				self.stackMode = other.stackMode;
				self.duration = other.duration;
				self.maxStackedDuration = other.maxStackedDuration;
				self.AppliesTint = other.AppliesTint;
				self.TintColor = new Color
				{
					r = other.TintColor.r,
					g = other.TintColor.g,
					b = other.TintColor.b
				};
				self.AppliesDeathTint = other.AppliesDeathTint;
				self.DeathTintColor = new Color
				{
					r = other.DeathTintColor.r,
					g = other.DeathTintColor.g,
					b = other.DeathTintColor.b
				};
				self.AppliesOutlineTint = other.AppliesOutlineTint;
				self.OutlineTintColor = new Color
				{
					r = other.OutlineTintColor.r,
					g = other.OutlineTintColor.g,
					b = other.OutlineTintColor.b
				};
				self.OverheadVFX = other.OverheadVFX;
				self.PlaysVFXOnActor = other.PlaysVFXOnActor;
				result = self;
			}
			return result;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000133F0 File Offset: 0x000115F0


		// Token: 0x06000175 RID: 373 RVA: 0x00013490 File Offset: 0x00011690
		public static bool PlayerHasCompletionItem(this PlayerController player)
		{
			bool result = false;
			bool flag = player != null && player.passiveItems != null;
			if (flag)
			{
				foreach (PassiveItem passiveItem in player.passiveItems)
				{
					bool flag2 = passiveItem is SynergyCompletionItem;
					if (flag2)
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00013518 File Offset: 0x00011718
		public static bool PlayerHasActiveSynergy(this PlayerController player, string synergyNameToCheck)
		{
			foreach (int num in player.ActiveExtraSynergies)
			{
				AdvancedSynergyEntry advancedSynergyEntry = GameManager.Instance.SynergyManager.synergies[num];
				bool flag = advancedSynergyEntry.NameKey == synergyNameToCheck;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00013598 File Offset: 0x00011798
		public static tk2dSpriteDefinition CopyDefinitionFrom(this tk2dSpriteDefinition other)
		{
			tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition
			{
				boundsDataCenter = new Vector3
				{
					x = other.boundsDataCenter.x,
					y = other.boundsDataCenter.y,
					z = other.boundsDataCenter.z
				},
				boundsDataExtents = new Vector3
				{
					x = other.boundsDataExtents.x,
					y = other.boundsDataExtents.y,
					z = other.boundsDataExtents.z
				},
				colliderConvex = other.colliderConvex,
				colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions,
				colliderType = other.colliderType,
				colliderVertices = other.colliderVertices,
				collisionLayer = other.collisionLayer,
				complexGeometry = other.complexGeometry,
				extractRegion = other.extractRegion,
				flipped = other.flipped,
				indices = other.indices,
				material = new Material(other.material),
				materialId = other.materialId,
				materialInst = new Material(other.materialInst),
				metadata = other.metadata,
				name = other.name,
				normals = other.normals,
				physicsEngine = other.physicsEngine,
				position0 = new Vector3
				{
					x = other.position0.x,
					y = other.position0.y,
					z = other.position0.z
				},
				position1 = new Vector3
				{
					x = other.position1.x,
					y = other.position1.y,
					z = other.position1.z
				},
				position2 = new Vector3
				{
					x = other.position2.x,
					y = other.position2.y,
					z = other.position2.z
				},
				position3 = new Vector3
				{
					x = other.position3.x,
					y = other.position3.y,
					z = other.position3.z
				},
				regionH = other.regionH,
				regionW = other.regionW,
				regionX = other.regionX,
				regionY = other.regionY,
				tangents = other.tangents,
				texelSize = new Vector2
				{
					x = other.texelSize.x,
					y = other.texelSize.y
				},
				untrimmedBoundsDataCenter = new Vector3
				{
					x = other.untrimmedBoundsDataCenter.x,
					y = other.untrimmedBoundsDataCenter.y,
					z = other.untrimmedBoundsDataCenter.z
				},
				untrimmedBoundsDataExtents = new Vector3
				{
					x = other.untrimmedBoundsDataExtents.x,
					y = other.untrimmedBoundsDataExtents.y,
					z = other.untrimmedBoundsDataExtents.z
				}
			};
			List<Vector2> list = new List<Vector2>();
			foreach (Vector2 vector in other.uvs)
			{
				list.Add(new Vector2
				{
					x = vector.x,
					y = vector.y
				});
			}
			tk2dSpriteDefinition.uvs = list.ToArray();
			List<Vector3> list2 = new List<Vector3>();
			foreach (Vector3 vector2 in other.colliderVertices)
			{
				list2.Add(new Vector3
				{
					x = vector2.x,
					y = vector2.y,
					z = vector2.z
				});
			}
			tk2dSpriteDefinition.colliderVertices = list2.ToArray();
			return tk2dSpriteDefinition;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00013730 File Offset: 0x00011930
		public static Gun GetGunById(this PickupObjectDatabase database, int id)
		{
			return PickupObjectDatabase.GetById(id) as Gun;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00013750 File Offset: 0x00011950
		public static Gun GetGunById(int id)
		{
			return ItemTools.GetGunById((PickupObjectDatabase)null, id);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0001376C File Offset: 0x0001196C
		public static ExplosionData CopyExplosionData(this ExplosionData other)
		{
			return new ExplosionData
			{
				useDefaultExplosion = other.useDefaultExplosion,
				doDamage = other.doDamage,
				forceUseThisRadius = other.forceUseThisRadius,
				damageRadius = other.damageRadius,
				damageToPlayer = other.damageToPlayer,
				damage = other.damage,
				breakSecretWalls = other.breakSecretWalls,
				secretWallsRadius = other.secretWallsRadius,
				forcePreventSecretWallDamage = other.forcePreventSecretWallDamage,
				doDestroyProjectiles = other.doDestroyProjectiles,
				doForce = other.doForce,
				pushRadius = other.pushRadius,
				force = other.force,
				debrisForce = other.debrisForce,
				preventPlayerForce = other.preventPlayerForce,
				explosionDelay = other.explosionDelay,
				usesComprehensiveDelay = other.usesComprehensiveDelay,
				comprehensiveDelay = other.comprehensiveDelay,
				effect = other.effect,
				doScreenShake = other.doScreenShake,
				ss = new ScreenShakeSettings
				{
					magnitude = other.ss.magnitude,
					speed = other.ss.speed,
					time = other.ss.time,
					falloff = other.ss.falloff,
					direction = new Vector2
					{
						x = other.ss.direction.x,
						y = other.ss.direction.y
					},
					vibrationType = other.ss.vibrationType,
					simpleVibrationTime = other.ss.simpleVibrationTime,
					simpleVibrationStrength = other.ss.simpleVibrationStrength
				},
				doStickyFriction = other.doStickyFriction,
				doExplosionRing = other.doExplosionRing,
				isFreezeExplosion = other.isFreezeExplosion,
				freezeRadius = other.freezeRadius,
				freezeEffect = other.freezeEffect,
				playDefaultSFX = other.playDefaultSFX,
				IsChandelierExplosion = other.IsChandelierExplosion,
				rotateEffectToNormal = other.rotateEffectToNormal,
				ignoreList = other.ignoreList,
				overrideRangeIndicatorEffect = other.overrideRangeIndicatorEffect
			};
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000139AC File Offset: 0x00011BAC
		public static void SetProjectileSpriteRight(this Projectile proj, string name, int pixelWidth, int pixelHeight, bool lightened = true, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.LowerLeft, bool anchorChangesCollider = true, int? overrideColliderPixelWidth = null,
	 int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
		{
			try
			{
				proj.GetAnySprite().spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
				tk2dSpriteDefinition def = ItemTools.SetupDefinitionForProjectileSprite(name, proj.GetAnySprite().spriteId, pixelWidth, pixelHeight, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX,
					overrideColliderOffsetY, overrideProjectileToCopyFrom);
				def.ConstructOffsetsFromAnchor(anchor, def.position3);
				proj.GetAnySprite().scale = new Vector3(1f, 1f, 1f);
				proj.transform.localScale = new Vector3(1f, 1f, 1f);
				proj.GetAnySprite().transform.localScale = new Vector3(1f, 1f, 1f);
				proj.AdditionalScaleMultiplier = 1f;
			}
			catch (Exception ex)
			{
				ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:");
				ETGModConsole.Log(ex.ToString());
			}
		}

		public static tk2dSpriteDefinition SetupDefinitionForProjectileSprite(string name, int id, int pixelWidth, int pixelHeight, bool lightened = true, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null,
			int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
		{
			if (overrideColliderPixelWidth == null)
			{
				overrideColliderPixelWidth = pixelWidth;
			}
			if (overrideColliderPixelHeight == null)
			{
				overrideColliderPixelHeight = pixelHeight;
			}
			if (overrideColliderOffsetX == null)
			{
				overrideColliderOffsetX = 0;
			}
			if (overrideColliderOffsetY == null)
			{
				overrideColliderOffsetY = 0;
			}
			float thing = 16;
			float thing2 = 16;
			float trueWidth = (float)pixelWidth / thing;
			float trueHeight = (float)pixelHeight / thing;
			float colliderWidth = (float)overrideColliderPixelWidth.Value / thing2;
			float colliderHeight = (float)overrideColliderPixelHeight.Value / thing2;
			float colliderOffsetX = (float)overrideColliderOffsetX.Value / thing2;
			float colliderOffsetY = (float)overrideColliderOffsetY.Value / thing2;
			tk2dSpriteDefinition def = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[(overrideProjectileToCopyFrom ??
					(PickupObjectDatabase.GetById(lightened ? 47 : 12) as Gun).DefaultModule.projectiles[0]).GetAnySprite().spriteId].CopyDefinitionFrom();
			def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
			def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
			def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
			def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
			def.texelSize = new Vector2(1 / 16f, 1 / 16f);
			def.position0 = new Vector3(0f, 0f, 0f);
			def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
			def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
			def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
			def.colliderVertices[0].x = colliderOffsetX;
			def.colliderVertices[0].y = colliderOffsetY;
			def.colliderVertices[1].x = colliderWidth;
			def.colliderVertices[1].y = colliderHeight;
			def.name = name;
			ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[id] = def;
			return def;
		}

		public static void MakeOffset(this tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
		{
			float xOffset = offset.x;
			float yOffset = offset.y;
			def.position0 += new Vector3(xOffset, yOffset, 0);
			def.position1 += new Vector3(xOffset, yOffset, 0);
			def.position2 += new Vector3(xOffset, yOffset, 0);
			def.position3 += new Vector3(xOffset, yOffset, 0);
			def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
			def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
			def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
			def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
			if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
			{
				def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
			}
		}

		public static void ConstructOffsetsFromAnchor(this tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2 scale, bool changesCollider = true)
		{
			float xOffset = 0;
			if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
			{
				xOffset = -(scale.x / 2f);
			}
			else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
			{
				xOffset = -scale.x;
			}
			float yOffset = 0;
			if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
			{
				yOffset = -(scale.y / 2f);
			}
			else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
			{
				yOffset = -scale.y;
			}
			def.MakeOffset(new Vector2(xOffset, yOffset), changesCollider);
		}


		// Token: 0x0600017C RID: 380 RVA: 0x00013BFC File Offset: 0x00011DFC
		public static StatModifier SetupStatModifier(PlayerStats.StatType statType, float modificationAmount, StatModifier.ModifyMethod modifyMethod = StatModifier.ModifyMethod.ADDITIVE, bool breaksOnDamage = false)
		{
			return new StatModifier
			{
				statToBoost = statType,
				amount = modificationAmount,
				modifyType = modifyMethod,
				isMeatBunBuff = breaksOnDamage
			};
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00013C30 File Offset: 0x00011E30
		public static GameActorCharmEffect CopyCharmFrom(this GameActorCharmEffect self, GameActorCharmEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorCharmEffect();
			}
			return (GameActorCharmEffect)self.CopyEffectFrom(other);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00013C60 File Offset: 0x00011E60
		public static GameActorFireEffect CopyFireFrom(this GameActorFireEffect self, GameActorFireEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorFireEffect();
			}
			bool flag2 = other == null;
			GameActorFireEffect result;
			if (flag2)
			{
				result = self;
			}
			else
			{
				self = (GameActorFireEffect)self.CopyEffectFrom(other);
				self.FlameVfx = new List<GameObject>();
				foreach (GameObject item in other.FlameVfx)
				{
					self.FlameVfx.Add(item);
				}
				self.flameNumPerSquareUnit = other.flameNumPerSquareUnit;
				self.flameBuffer = new Vector2
				{
					x = other.flameBuffer.x,
					y = other.flameBuffer.y
				};
				self.flameFpsVariation = other.flameFpsVariation;
				self.flameMoveChance = other.flameMoveChance;
				self.IsGreenFire = other.IsGreenFire;
				result = self;
			}
			return result;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00013D64 File Offset: 0x00011F64
		public static GameActorHealthEffect CopyPoisonFrom(this GameActorHealthEffect self, GameActorHealthEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorHealthEffect();
			}
			bool flag2 = other == null;
			GameActorHealthEffect result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				self.CopyEffectFrom(other);
				self.DamagePerSecondToEnemies = other.DamagePerSecondToEnemies;
				self.ignitesGoops = other.ignitesGoops;
				result = self;
			}
			return result;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00013DB8 File Offset: 0x00011FB8
		public static GameActorSpeedEffect CopySpeedFrom(this GameActorSpeedEffect self, GameActorSpeedEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorSpeedEffect();
			}
			bool flag2 = other == null;
			GameActorSpeedEffect result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				self.CopyEffectFrom(other);
				self.SpeedMultiplier = other.SpeedMultiplier;
				self.CooldownMultiplier = other.CooldownMultiplier;
				self.OnlyAffectPlayerWhenGrounded = other.OnlyAffectPlayerWhenGrounded;
				result = self;
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00013E18 File Offset: 0x00012018
		public static GameActorFreezeEffect CopyFreezeFrom(this GameActorFreezeEffect self, GameActorFreezeEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorFreezeEffect();
			}
			bool flag2 = other == null;
			GameActorFreezeEffect result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				self.CopyEffectFrom(other);
				self.FreezeAmount = other.FreezeAmount;
				self.UnfreezeDamagePercent = other.UnfreezeDamagePercent;
				self.FreezeCrystals = new List<GameObject>();
				foreach (GameObject item in other.FreezeCrystals)
				{
					self.FreezeCrystals.Add(item);
				}
				self.crystalNum = other.crystalNum;
				self.crystalRot = other.crystalRot;
				self.crystalVariation = new Vector2
				{
					x = other.crystalVariation.x,
					y = other.crystalVariation.y
				};
				self.debrisMinForce = other.debrisMinForce;
				self.debrisMaxForce = other.debrisMaxForce;
				self.debrisAngleVariance = other.debrisAngleVariance;
				self.vfxExplosion = other.vfxExplosion;
				result = self;
			}
			return result;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00013F48 File Offset: 0x00012148
		public static GameActorBleedEffect CopyBleedFrom(this GameActorBleedEffect self, GameActorBleedEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorBleedEffect();
			}
			bool flag2 = other == null;
			GameActorBleedEffect result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				self.CopyEffectFrom(other);
				self.ChargeAmount = other.ChargeAmount;
				self.ChargeDispelFactor = other.ChargeDispelFactor;
				self.vfxChargingReticle = other.vfxChargingReticle;
				self.vfxExplosion = other.vfxExplosion;
				result = self;
			}
			return result;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00013FB4 File Offset: 0x000121B4
		public static GameActorCheeseEffect CopyCheeseFrom(this GameActorCheeseEffect self, GameActorCheeseEffect other)
		{
			bool flag = self == null;
			if (flag)
			{
				self = new GameActorCheeseEffect();
			}
			bool flag2 = other == null;
			GameActorCheeseEffect result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				self.CopyEffectFrom(other);
				self.CheeseAmount = other.CheeseAmount;
				self.CheeseGoop = other.CheeseGoop;
				self.CheeseGoopRadius = other.CheeseGoopRadius;
				self.CheeseCrystals = new List<GameObject>();
				foreach (GameObject item in other.CheeseCrystals)
				{
					self.CheeseCrystals.Add(item);
				}
				self.crystalNum = other.crystalNum;
				self.crystalRot = other.crystalRot;
				self.crystalVariation = new Vector2
				{
					x = other.crystalVariation.x,
					y = other.crystalVariation.y
				};
				self.debrisMinForce = other.debrisMinForce;
				self.debrisMaxForce = other.debrisMaxForce;
				self.debrisAngleVariance = other.debrisAngleVariance;
				self.vfxExplosion = other.vfxExplosion;
				result = self;
			}
			return result;
		}
		/*
		// Token: 0x06000184 RID: 388 RVA: 0x000140F0 File Offset: 0x000122F0
		public static void SetFlag(this GameStatsManager manager, CustomDungeonFlags flag, bool value)
		{
			CustomGungeonFlagsManager.SetCustomDungeonFlag(flag, value);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000140FB File Offset: 0x000122FB
		public static void SetStat(this GameStatsManager manager, CustomTrackedStats stat, float value)
		{
			CustomTrackedStatsManager.SetCustomStat(stat, value);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00014106 File Offset: 0x00012306
		public static void RegisterStatChange(this GameStatsManager manager, CustomTrackedStats stat, float value)
		{
			CustomTrackedStatsManager.RegisterStatChange(stat, value);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00014114 File Offset: 0x00012314
		public static bool GetFlag(this GameStatsManager manager, CustomDungeonFlags flag)
		{
			return CustomGungeonFlagsManager.GetCustomDungeonFlag(flag);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0001412C File Offset: 0x0001232C
		public static float GetPlayerStatValue(this GameStatsManager manager, CustomTrackedStats stat)
		{
			return CustomTrackedStatsManager.GetCustomStat(stat);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00014144 File Offset: 0x00012344
		public static void SetupUnlockOnFlag(this PickupObject self, CustomDungeonFlags flag, bool requiredFlagValue)
		{
			self.SetupUnlockOnFlag(CustomGungeonFlagsManager.GetGungeonFlagForCustomDungeonFlag(flag), requiredFlagValue);
		}
		*/
		// Token: 0x0600018A RID: 394 RVA: 0x00014158 File Offset: 0x00012358
		public static void SetupUnlockOnFlag(this PickupObject self, GungeonFlags flag, bool requiredFlagValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			bool flag2 = encounterTrackable.prerequisites == null;
			if (flag2)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
					requireFlag = requiredFlagValue,
					saveFlagToCheck = flag
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
						requireFlag = requiredFlagValue,
						saveFlagToCheck = flag
					}
				}).ToArray<DungeonPrerequisite>();
			}
			EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			bool flag3 = !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
			if (flag3)
			{
				entry.ProxyEncounterGuid = "";
			}
			bool flag4 = entry.prerequisites == null;
			if (flag4)
			{
				entry.prerequisites = new DungeonPrerequisite[1];
				entry.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
					requireFlag = requiredFlagValue,
					saveFlagToCheck = flag
				};
			}
			else
			{
				entry.prerequisites = entry.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
						requireFlag = requiredFlagValue,
						saveFlagToCheck = flag
					}
				}).ToArray<DungeonPrerequisite>();
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00014290 File Offset: 0x00012490
		public static void SetupUnlockOnStat(this PickupObject self, TrackedStats stat, DungeonPrerequisite.PrerequisiteOperation operation, int comparisonValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			bool flag = encounterTrackable.prerequisites == null;
			if (flag)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
					prerequisiteOperation = operation,
					statToCheck = stat,
					comparisonValue = (float)comparisonValue
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
						prerequisiteOperation = operation,
						statToCheck = stat,
						comparisonValue = (float)comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
			EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			bool flag2 = !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
			if (flag2)
			{
				entry.ProxyEncounterGuid = "";
			}
			bool flag3 = entry.prerequisites == null;
			if (flag3)
			{
				entry.prerequisites = new DungeonPrerequisite[1];
				entry.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
					prerequisiteOperation = operation,
					statToCheck = stat,
					comparisonValue = (float)comparisonValue
				};
			}
			else
			{
				entry.prerequisites = entry.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
						prerequisiteOperation = operation,
						statToCheck = stat,
						comparisonValue = (float)comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000143E8 File Offset: 0x000125E8
		public static ItemTools.ChestType GetChestType(this Chest chest)
		{
			bool flag = chest.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW;
			ItemTools.ChestType result;
			if (flag)
			{
				result = ItemTools.ChestType.SECRET_RAINBOW;
			}
			else
			{
				bool flag2 = chest.ChestIdentifier == Chest.SpecialChestIdentifier.RAT;
				if (flag2)
				{
					result = ItemTools.ChestType.RAT_REWARD;
				}
				else
				{
					bool isRainbowChest = chest.IsRainbowChest;
					if (isRainbowChest)
					{
						result = ItemTools.ChestType.RAINBOW;
					}
					else
					{
						bool isGlitched = chest.IsGlitched;
						if (isGlitched)
						{
							result = ItemTools.ChestType.GLITCHED;
						}
						else
						{
							bool flag3 = chest.breakAnimName.Contains("wood");
							if (flag3)
							{
								result = ItemTools.ChestType.BROWN;
							}
							else
							{
								bool flag4 = chest.breakAnimName.Contains("silver");
								if (flag4)
								{
									result = ItemTools.ChestType.BLUE;
								}
								else
								{
									bool flag5 = chest.breakAnimName.Contains("green");
									if (flag5)
									{
										result = ItemTools.ChestType.GREEN;
									}
									else
									{
										bool flag6 = chest.breakAnimName.Contains("redgold");
										if (flag6)
										{
											result = ItemTools.ChestType.RED;
										}
										else
										{
											bool flag7 = chest.breakAnimName.Contains("black");
											if (flag7)
											{
												result = ItemTools.ChestType.BLACK;
											}
											else
											{
												bool flag8 = chest.breakAnimName.Contains("synergy");
												if (flag8)
												{
													result = ItemTools.ChestType.SYNERGY;
												}
												else
												{
													result = ItemTools.ChestType.UNIDENTIFIED;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000144F5 File Offset: 0x000126F5
		public static void PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
		{
			item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
		}

		// Token: 0x0400028B RID: 651
		public static GoopDefinition DefaultWaterGoop;

		// Token: 0x0400028C RID: 652
		public static GoopDefinition DefaultFireGoop;

		// Token: 0x0400028D RID: 653
		public static GoopDefinition DefaultPoisonGoop;

		// Token: 0x0400028E RID: 654
		public static GoopDefinition DefaultCharmGoop;

		// Token: 0x0400028F RID: 655
		public static GoopDefinition DefaultBlobulonGoop;

		// Token: 0x04000290 RID: 656
		public static GoopDefinition DefaultPoopulonGoop;

		// Token: 0x04000291 RID: 657
		public static GoopDefinition DefaultCheeseGoop;

		// Token: 0x02000093 RID: 147
		public enum ChestType
		{
			// Token: 0x040003C5 RID: 965
			BROWN = 1,
			// Token: 0x040003C6 RID: 966
			BLUE,
			// Token: 0x040003C7 RID: 967
			GREEN,
			// Token: 0x040003C8 RID: 968
			RED,
			// Token: 0x040003C9 RID: 969
			BLACK,
			// Token: 0x040003CA RID: 970
			SYNERGY = -1,
			// Token: 0x040003CB RID: 971
			RAINBOW = -2,
			// Token: 0x040003CC RID: 972
			SECRET_RAINBOW = -3,
			// Token: 0x040003CD RID: 973
			GLITCHED = -4,
			// Token: 0x040003CE RID: 974
			RAT_REWARD = -5,
			// Token: 0x040003CF RID: 975
			UNIDENTIFIED = -50
		}

		public static List<int> CastleWallIDs = new List<int>
		{
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			73,
			74,
			75,
			76,
			80,
			81,
			82,
			83,
			95,
			96,
			97,
			98,
			176,
			332,
			333,
			334,
			354,
			355,
			356
		};


	}
}