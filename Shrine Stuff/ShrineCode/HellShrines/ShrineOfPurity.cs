
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using SaveAPI;
using BreakAbleAPI;
using System.Collections;
using DaikonForge.Tween;

namespace Planetside
{
	public class ShrineOfPurity : DungeonPlaceableBehaviour, IPlayerInteractable
    {

		public static void Add()
		{
            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Shrine Of Purity");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "shrineofpurity");
            tearObject.layer = Layers.FG_Nonsense;
            sprite.HeightOffGround = -1.35f;
            sprite.SortingOrder = 1;
            sprite.IsPerpendicular = false;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0f);
            mat.SetFloat("_EmissivePower", 0);
            sprite.renderer.material = mat;


            //this.modID + ":" + this.name

            var shrine = tearObject.gameObject.AddComponent<ShrineOfPurity>();
            shrine.talkPoint = BreakableAPI_Bundled.GenerateTransformObject(shrine.gameObject, new Vector2(2f, 2f), "TalkTuah").transform;
            shrine.sprite = sprite;

            //majorBreakable.gameObject.AddComponent<DungeonPlaceableBehaviour>();
            //ShrineFactory.registeredShrines.Add("psog:ShrineOfPurity", tearObject);

            var _  = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Pretty");
            sprite = _.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "shrineofpurityoutside");
            sprite.gameObject.layer = Layers.FG_Nonsense;
            sprite.HeightOffGround = -1;
            sprite.SortingOrder = 1;
            sprite.IsPerpendicular = false;

            sprite.transform.SetParent(tearObject.transform, false);

            mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0f);
            mat.SetFloat("_EmissivePower", 0);
            sprite.renderer.material = mat;

            StaticReferences.StoredRoomObjects.Add("psog:shrineofpurity", tearObject);

            SpriteBuilder.AddSpriteToCollection(spriteDefinition1, SpriteBuilder.ammonomiconCollection);

		}
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/PurityIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			if( CursesController.CheckIfAnyCurseActive() == true && !player.IsInCombat)//player.gameObject.GetComponent<ShrineOfDarkness.DarknessTime>() != null || player.gameObject.GetComponent<ShrineOfCurses.JamTime>() != null || player.gameObject.GetComponent<ShrineOfPetrification.PetrifyTime>() != null || player.gameObject.GetComponent<ShrineOfSomething.SomethingTime>() != null)
            {
				return shrine.GetComponent<CustomShrineController>().numUses <= 0;
			}
			else
            {
				return false;
            }
		}



		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", shrine.gameObject);
			for (int i = 0; i < 4; i++)
			{
				SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, player.sprite.WorldCenter.ToVector3ZisY(0f) + new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f), 100), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
			}
			List<string> list = new List<string> { };
			if (CursesController.DarknessCurseState == CursesController.DarknessCurseStates.ENABLED)
            {
				list.Add("Darkness");
            }
			if (CursesController.JamnationCurseState == CursesController.JamnationCurseStates.ENABLED)
			{
				list.Add("Jam");
			}
			if (CursesController.PetrifyCurseState == CursesController.PetrifyCurseStates.ENABLED)
			{
				list.Add("Petrify");
			}
			if (CursesController.BolsterCurseState == CursesController.BolsterCurseStates.ENABLED)
			{
				list.Add("Bolster");
			}
			string ChosenCurse = BraveUtility.RandomElement<string>(list);
			if (ChosenCurse != null)
			{
				if (ChosenCurse == "Darkness")
				{
					OtherTools.Notify("Curse Of Darkness chosen", "Prove your worth.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.DarknessCurseState = CursesController.DarknessCurseStates.UPGRADED_AND_ONEROOMLEFT;
					//ShrineOfDarkness.DarknessTime comp = player.GetComponent<ShrineOfDarkness.DarknessTime>();
					//comp.RemoveSelf();
					//UltraDarkness darkness = player.gameObject.AddComponent<UltraDarkness>();
					//darkness.playeroue = player;
				}
				if (ChosenCurse == "Jam")
				{
					OtherTools.Notify("Curse Of Jamnation chosen", "Prove your worth.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.JamnationCurseState = CursesController.JamnationCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfCurses.JamTime comp = player.GetComponent<ShrineOfCurses.JamTime>();
					//comp.RemoveSelf();
					//UltraJammed jammed = player.gameObject.AddComponent<UltraJammed>();
					//jammed.playeroue = player;
				}
				if (ChosenCurse == "Petrify")
				{
					OtherTools.Notify("Curse Of Petrification chosen", "Prove your worth.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.PetrifyCurseState = CursesController.PetrifyCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfPetrification.PetrifyTime comp = player.GetComponent<ShrineOfPetrification.PetrifyTime>();
					//comp.RemoveSelf();
					//UltraPetrify petrify = player.gameObject.AddComponent<UltraPetrify>();
					//petrify.playeroue = player;
				}
				if (ChosenCurse == "Bolster")
				{
					OtherTools.Notify("Curse Of Bolstering chosen", "Prove your worth.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.BolsterCurseState = CursesController.BolsterCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfSomething.SomethingTime comp = player.GetComponent<ShrineOfSomething.SomethingTime>();
					//comp.RemoveSelf();
					//UltraBolster bolster = player.gameObject.AddComponent<UltraBolster>();
					//bolster.playeroue = player;
				}
			}
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = player.transform.position,
                startSize = 12,
                rotation = 0,
                startLifetime = 0.25f,
                startColor = Color.white.WithAlpha(0.333f)
            });
        }

        private void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            instanceRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
            if (instanceRoom == null) { return; }
            instanceRoom.OnEnemiesCleared += () =>
            {
                instanceRoom.RegisterInteractable(this);
                this.StartCoroutine(DoCandles());
            };

            this.instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(instanceRoom, MinimapIconprefab ?? (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
        }
        public RoomHandler instanceRoom;
        public GameObject instanceMinimapIcon;
        public GameObject MinimapIconprefab;
        public Transform talkPoint;
        public bool Used = false;


        public List<Vector2> V = new List<Vector2>();

        public IEnumerator DoCandles()
        {
            var attachPoints = sprite.collection.spriteDefinitions[sprite.spriteId].GetAttachPoints(sprite.collection, sprite.spriteId).ToList();
            attachPoints = attachPoints.Shuffle();
            foreach (var entry in attachPoints)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.15f));
                V.Add(entry.position);
                var p = this.transform.position + entry.position;
                AkSoundEngine.PostEvent("Play_Immolate", this.gameObject);
                GlobalSparksDoer.DoRadialParticleBurst(4, p, p, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = p,
                    startSize = 4,
                    rotation = 0,
                    startLifetime = 0.1f,
                    startColor = Color.red.WithAlpha(0.333f)
                });
            }
            yield return null;
        }


        public void FixedUpdate()
        {
            if (Used) { return; }
            foreach (var entry in V)
            {
                var p = this.transform.position + entry.ToVector3ZisY() + new Vector3(-0.125f, 0.125f);
                if (UnityEngine.Random.value < 0.05f)
                {
                    GlobalSparksDoer.DoRadialParticleBurst(1, p, p, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                }
                if (UnityEngine.Random.value < 0.1f)
                {
                    GlobalSparksDoer.DoRadialParticleBurst(1, p, p, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }
            }
        }


        public void Interact(PlayerController interactor)
        {
            if (!TextBoxManager.HasTextBox(this.talkPoint))
            {
                base.StartCoroutine(this.HandleConversation(interactor));
            }
        }

        private IEnumerator HandleConversation(PlayerController interactor)
        {
            if (this.talkPoint == null) { ETGModConsole.Log("talkPoint is NULL"); }
            if (this.talkPoint.position == null) { ETGModConsole.Log("talkPoint.position is NULL"); }
            //if (this.text == null) { ETGModConsole.Log("text is NULL"); }

            string Text = "";
            bool b = CursesController.CheckIfAnyCurseActive() == true && !interactor.IsInCombat;
            if (b)//player.gameObject.GetComponent<ShrineOfDarkness.DarknessTime>() != null || player.gameObject.GetComponent<ShrineOfCurses.JamTime>() != null || player.gameObject.GetComponent<ShrineOfPetrification.PetrifyTime>() != null || player.gameObject.GetComponent<ShrineOfSomething.SomethingTime>() != null)
            {
                Text = "The silver pedestal vibrates at your presense.";
            }
            else
            {
                Text = "A ritual circle, with a silver pedestal at its center.\nYou have nothing to offer.";
            }


            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, Text, true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            if (Used)
            {
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "The cleansing spirits that have once resided here have left.", string.Empty);
            }
            else
            {
                if (b)
                {
                    GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "Give up a random curse.", "Leave.");
                }
                else
                {
                    GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "Leave.", "");
                }
            }
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            if (Used == true)
            {
                yield break;
            }
            if (selectedResponse == 0 && b)
            {
                Accept(interactor, this.gameObject);
            }
            else
            {

            }
            yield break;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            float result;
            if (base.sprite == null)
            {
                result = 100f;
            }
            else
            {
                result = Vector2.Distance(point, base.sprite.WorldCenter) / 1.5f;
            }
            return result;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }




        public void ConfigureOnPlacement(RoomHandler room)
        {
            instanceRoom = room;
            this.RegisterMinimapIcon();

        }

        public void RegisterMinimapIcon()
        {
            this.instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.instanceRoom, this.MinimapIconprefab, false);
        }

        public void GetRidOfMinimapIcon()
        {
            if (this.instanceMinimapIcon != null)
            {
                Minimap.Instance.DeregisterRoomIcon(this.instanceRoom, this.instanceMinimapIcon);
                this.instanceMinimapIcon = null;
            }
        }



    }
}



