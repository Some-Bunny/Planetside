using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;
using MonoMod.Utils;
using Dungeonator;
using Brave.BulletScript;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using GungeonAPI;
using SaveAPI;
using Planetside;
using EnemyBulletBuilder;
using NpcApi;
using SoundAPI;
using BreakAbleAPI;

namespace Planetside
{
    public class PlanetsideModule : ETGModule
    {
        public static readonly string MOD_NAME = "Planetside Of Gunymede";
        public static readonly string VERSION = "1.3 Beta";
        public static readonly string TEXT_COLOR = "#9006FF";

        public static string ZipFilePath;
        public static string RoomFilePath;
        public static string FilePathFolder;

        public static ETGModuleMetadata metadata = new ETGModuleMetadata(); 

        public static AdvancedStringDB Strings;
        public static HellDragZoneController hellDrag;

        public static AssetBundle ModAssets;

        public override void Start()
        {
            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");
            PlanetsideModule.hellDrag = forgeDungeon.PatternSettings.flows[0].AllNodes.Where(node => node.overrideExactRoom != null && node.overrideExactRoom.name.Contains("EndTimes")).First().overrideExactRoom.placedObjects.Where(ppod => ppod != null && ppod.nonenemyBehaviour != null).First().nonenemyBehaviour.gameObject.GetComponentsInChildren<HellDragZoneController>()[0];
            forgeDungeon = null;

            ZipFilePath = this.Metadata.Archive;
            RoomFilePath = this.Metadata.Directory + "/rooms";   
            FilePathFolder = this.Metadata.Directory;
            metadata = this.Metadata;
            //Initialise World-Stuff here
            StaticInformation.Init();
            ETGModMainBehaviour.Instance.gameObject.AddComponent<NevernamedsDarknessHandler>();

            //Asset bundle stuff
            PlanetsideModule.ModAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidebundle");
            foreach (string str in PlanetsideModule.ModAssets.GetAllAssetNames())
            {
                ETGModConsole.Log(PlanetsideModule.ModAssets.name + ": " + str, false);
            }

            //Initialise Statically Stored Stuff Here
            StaticVFXStorage.Init();
            EasyGoopDefinitions.DefineDefaultGoops();
            RandomPiecesOfStuffToInitialise.BuildPrefab();
            PlanetsideModule.Strings = new AdvancedStringDB();
            ItemIDs.MakeCommand();
            StaticReferences.Init(); //<- Used in GungeonAPI, IMPORTANT to initialise it before DungeonHandler
            //Targets.Init();
            HolyChamberStatue.Init();
            TrespassLight.Init();

            //initialise Tools classes here
            Tools.Init();
            NpcTools.Init();


            //Hook stuff here
            PickupHooks.Init();
            ExplosionHooks.Init();
            Hooks.Init();
            MultiActiveReloadManager.SetupHooks();
            FakePrefabHooks.Init();

            //Initialise API stuff here
            BulletBuilder.Init();
            SoundManager.Init();
            SoundManager.LoadBankFromModProject("Planetside/PlanetsideBank");
            SoundManager.RegisterStopEvent("Stop_MUS_PrisonerTheme", StopEventType.Music);
            ItemBuilder.Init();

            //Shrine Initialisation
            ShrineFactory.Init();
            OldShrineFactory.Init();
            ShrineFakePrefabHooks.Init();
            GunOrbitShrine.Add();
            NullShrine.Add();
            HolyChamberShrine.Add();
            TooLate.Add();



            EnemyBuilder.Init();
            BossBuilder.Init();
            CustomClipAmmoTypeToolbox.Init();
            ETGModConsole.Commands.AddUnit("planetsideflow", (args) =>
            {
                DungeonHandler.debugFlow = !DungeonHandler.debugFlow;
                string status = DungeonHandler.debugFlow ? "enabled" : "disabled";
                string color = DungeonHandler.debugFlow ? "00FF00" : "FF0000";
                ETGModConsole.Log($"Planetside flow {status}", false);
            });




            SomethingWickedEnemy.Init();


            Unstabullets.Init();
            HullBreakerBullets.Init();
            //Unlocked By Killing Jammed Guard
            ShelltansBlessing.Init();
            EcholocationAmmolet.Init();
            GildedPots.Init();
            GunPrinter.Init();
            ElectrostaticGuonStone.Init();
            AoEBullets.Init();
            PsychicBlank.Init();
            LDCBullets.Init();
            BlastShower.Init();
            BrokenChamber.Init();
            //Unlocked by beating Lich with Broken Chamber
            DiamondChamber.Init();
            NetheriteChamber.Init();
            TableTechNullReferenceException.Init();
            WispInABottle.Init();
            //Unlocked by killing Shellrax
            Shellheart.Init();
            SoulboundSkull.Init();
            DiasukesPolymorphine.Init();
            //Unlocked by beating Dragun at 15+ Curse
            FrailtyRounds.Init();
            FrailtyAmmolet.Init();
            OffWorldMedicine.Init();
            BabyGoodCandleKin.Init();
            ShellsnakeOil.Init();
            TableTechDevour.Init();
            UnstableTeslaCoil.Init();
            DeathWarrant.Init();
            OscilaltingBullets.Init();
            TinyPlanetBullets.Init();

            WitherLance.Add();
            SwanOff.Add();
            UglyDuckling.Add();
            HardlightNailgun.Add();
            BurningSun.Add();
            StatiBlast.Add();
            Polarity.Add();
            PolarityForme.Add();
            Revenant.Add();
            //Unlocked By Beating Bullet Banker
            SoulLantern.Add();
            VeteranShotgun.Add();
            VeteranerShotgun.Add();
            GTEE.Add();
            ShockChain.Add();
            Resault.Add();
            Immateria.Add();
            //Unlocked By Beating Loop 1
            ArmWarmer.Add();
            Oscillato.Add();
            OscillatoSynergyForme.Add();
            RebarPuncher.Add();
            LaserChainsaw.Add();
            ExecutionersCrossbow.Add();
            ExecutionersCrossbowSpecial.Init();
            ForgiveMePlease.Init();
            ForgiveMePlease.BuildPrefab();
            PortablePylon.Init();
            LoaderPylonController.Init();
            LoaderPylonSynergyFormeController.Init();
            DeadKingsDesparation.Init();
            DeadKingsDesparation.BuildPrefab();

            //Debuff Icons
            BrokenArmorEffect.Init();
            FrailtyHealthEffect.Init();
            PossessedEffect.Init();
            HolyBlessingEffect.Init();
            HeatStrokeEffect.Init();

            DebuffLibrary.Init();

            LeSackPickup.Init();
            NullPickupInteractable.Init();

            //=================      
            KineticStrike.Init();
            ShellsOfTheMountain.Init();
            InjectorRounds.Init();
            Capactior.Add();
            TatteredRobes.Init();
            BulletGuonMaker.Init();
            JammedJar.Init();
            DamnedGuonStone.Init();
            Petrifier.Add();
            Funcannon.Add();
            GunWarrant.Init();
            SirenSynergyForme.Add();
            BanditsRevolver.Add();
            Mop.Add();
            ParasiticHeart.Add();
            EyeOfAnnihilation.Add();
            UnknownGun.Add();
            EnergyShield.Init();
            BloodIdol.Init();
            Riftaker.Add();
            HeresyHammer.Init();
            PerfectedColossus.Add();
            Colossus.Add();
            ResourceGuonMaker.Init();     
            ChargerGun.Add();
            //FIX THIS SWORD TO NOT CAUSE MASSIVE EXCEPTIONS WITH EXPAND ON LOAD
            PlanetBlade.Add();
            //TestShaderBullets.Init();
            DerpyBullets.Init();
            PrayerAmulet.Init();
            LockOnGun.Add();
            CoinTosser.Init();
            DivineLight.Add();
            HellLight.Add();
            KnuckleBlaster.Init();
            Preigniter.Init();
            AttractorBeam.Add();
            LilPew.Add();

            GunslingersRing.Init();
            LuckyCharm.Init();
            TrapDefusalKit.Init();
            TableTechTelefrag.Init();
            AlchemicalVial.Init();
            SpinningDeath.Init();
            ConnoisseursRobes.Init();
            LostVoidPotential.Init();
            TrespassStone.Init();

            ModifierNeedle.Init();

            Whistler.Add();
            ThunderShot.Add();

            //Forgotten Rounds
            ForgottenRoundOubliette.Init();
            ForgottenRoundAbbey.Init();
            ForgottenRoundRNG.Init();

            //Perks
            AllStatsUp.Init();
            Greedy.Init();
            AllSeeingEye.Init();
            BlastProjectiles.Init();
            Glass.Init();
            Contract.Init();
            ChaoticShift.Init();
            PitLordsPact.Init();
            UnbreakableSpirit.Init();
            Gunslinger.Init();

            //VengefulShell.Init();
            //LaserWelder.Add();
            //BoscoDesignator.Add();

            Ophanaim.Init();
            Fungannon.Init();
            Coallet.Init();
            Shamber.Init();
            ProperCube.Init();
            Detscavator.Init();

            DeTurretRight.Init();
            DeTurretLeft.Init();
            Barretina.Init();
            Glockulus.Init();
            Cursebulon.Init();
            ArchGunjurer.Init();
            RevolverSkull.Init();
            FodderEnemy.Init();
            JammedGuard.Init();
            AnnihiChamber.Init();
            PrisonerPhaseOne.Init();

            An3sBullet.Init();
            HunterBullet.Init();
            NevernamedBullet.Init();
            SkilotarBullet.Init();
            BlazeyBullet.Init();
            SpapiBullet.Init();
            GlaurBullet.Init();
            ApacheBullet.Init();
            NeighborinoBullet.Init();
            BleakBullet.Init();
            KingBullet.Init();
            PandaBullet.Init();
            RetrashBullet.Init();
            KyleBullet.Init();
            BunnyBullet.Init();
            BotBullet.Init();
            WowBullet.Init();
            TurboBullet.Init();
            SpcreatBullet.Init();
            
            GoldenRevolverBullet.Init();
            NotSoAIBullet.Init();

            BulletBankMan.Init();

            Shellrax.Init();
            Wailer.Init();

            CelBullet.Init();
                        
            InitialiseSynergies.DoInitialisation();
            SynergyFormInitialiser.AddSynergyForms();
            InitialiseGTEE.DoInitialisation();
            HoveringGunsAdder.AddHovers();
            

            
            BrokenChamberShrine.Add();
            //ShrineOfEvil.Add();
            ShrineOfDarkness.Add();
            ShrineOfCurses.Add();
            ShrineOfPetrification.Add();
            ShrineOfSomething.Add();
            ShrineOfPurity.Add();

            SWMinesShrine.Add();
            BlueShrine.Add();
            RedShrine.Add();

            //TestShaderItem.Init();


            RoomReader.Init();
            QuestWanderer.Add();
            DungeonHooks.OnPostDungeonGeneration += this.PlaceHellShrines;
            DungeonHooks.OnPostDungeonGeneration += this.PlaceOtherHellShrines;


            //TestActiveItem.Init();
            OuroborousShrine.Add();

            ShrineFactory.PlaceBreachShrines();
            SomethingWickedEnemy.Init();
            SomethingWickedEnemy.InitDummyEnemy();
            Thing.Init();
            RedThing.Init();

            CustomLootTableInitialiser.InitialiseCustomLootTables();
            CustomShopInitialiser.InitialiseCustomShops();


            FlowInjectionInitialiser.InitialiseFlows();

            //RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShrineOfEvilShrineRoomHell.room").room)
            //GenericRoomTable table = RoomTableTools.CreateRoomTable();
            //WeightedRoom roomer =  RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources2/smileBossRoom.room").room);

            //table.includedRooms.Add(roomer);

            //new Hook(typeof(GameManager).GetProperty("BossManager", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(), typeof(PlanetsideModule).GetMethod("BossManagerHook"));

            //RoomTableTools.AddBossTableToManager(RoomTableTools.GenerateIndividualBossFloorEntry(table, null, "FUCK"));
            DungeonHandler.Init();
            MasteryReplacementOub.InitDungeonHook();

            //AdvancedLogging.Log($"{MOD_NAME} v{VERSION} started successfully.", new Color(144, 6, 255, 255), false, true, null);
            PlanetsideModule.Log($"{MOD_NAME} v{VERSION} started successfully.", TEXT_COLOR);
            List<string> RandomFunnys = new List<string>
            {
                "Powered by SaveAPI!",
                "Now With 100% Less Nulls!",
                "You Lost The Game.",
                "WEAK.",
                "Bullet Banks are not to rob!",
                "Art By SirWow!",
                "*Don't* Download Some Bunnys Content Pack",
                "weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                "If you see this, you owe me 10 bucks you nerd.",
                "https://www.youtube.com/watch?v=qn0YdT_pQF8",
                "https://www.youtube.com/watch?v=EGXPAoyP_cg",
                "Ashes To Ashes, To Ashes (To Ashes)",
                "Deadbolt Is Underrated!",
                "You're Gonna Need A Bigger Gun",
                "Sai Sinut Nayttamaan",
                "oh no",
                "is that supposed to be like that???",
                "I'm so lonely.",
                "I removed an item from the item pool, which one is it? ;)",
                "Peter Griffin!",
                "Hey VSauce!",
                "Yo Mama!",
                "NullReferenceException: Object Reference not set to an instance of an object.",
                "I stole this one from Nevernamed.",
                "poggers",
                "Nuh uh, yr'oue.",
                "eg",
                "._.",
                "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm",
                "Warning! Level 5 Quarantine In Action! Do Not Land!",
                "Dusa",
                "Theres something in the walls...",
                "<3",
                "Did I ever fix that bug?",
                "I didn't get enough sleep last night.",
                "bonk",
                "This is not a GMod Server.",
                "oops",
                "Can I offer you a nice egg in this trying time?",
                "Up Up Down Down Left Right Left Right B A Start",
                "Another Night...",
                "skibidi bop mm dada",
                "I coded most of this after 1am.",
                "Planetside Supports Trans Rights",
                "bepis",
                "pootis",
                "Frogs are cool!",
                "It's... so... warm...",
                "Poor aim, and a poor Reaper.",
                "egassem sdrawkcab",
                "The Sun! The Sun! The Sun!",
                "if(player.IsStupid){  }",
                "cultist_comits_tax_evasion.mp3",
                "Stop it, I'm bees!",
                "bonk",
                "fwendship",
                "nice",
                "Powered By AudioBuilder!",
                "Powered By BeamBuilder!",
                "Powered By Friendship!",
                "YOUR PAST IS DEAD",
                "LEAD IS FUEL",
                "BULLET HELL IS FULL",
                "bzaazzz",
                "What the dog doin'",
                "Powered by like 7 different APIs",
                "Rock And Stone!",
                "By The Beard!",
                "Powered By BreakableAPI!"

            };
            Random r = new Random();
            int index = r.Next(RandomFunnys.Count);
            string randomString = RandomFunnys[index];
            Log(" - "+randomString, TEXT_COLOR);

            Log("Here's a To-Do list for unlocks, hope you have fun!", TEXT_COLOR);
            Log("If you ever need a reminder, the command 'psog to_do_list' to remind yourself.", TEXT_COLOR);
            string a = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED) ? " Done!\n" : " -Defeat The Dragun At A Higher Curse.\n";
            string b = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED) ? " Done!\n" : " -Defeat The Guardian Of The Holy Chamber.\n";
            string c = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED) ? " Done!\n" : " -Defeat The Failed Demi-Lich\n";
            string d = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED) ? " Done!\n" : " -Defeat The Banker Of Bullets.\n";
            string e = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED) ? " Done!\n" : " -Defeat The Lich With A Broken Remnant In Hand.\n";
            string f = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LOOP_1) ? " Done!\n" : " -Beat The Game On Ouroborous Level 0.\n";
            string g = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND) ? " Done!\n" : " -Kill A Boss After Dealing 500 Damage Or More At Once.\n";
            string h = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON) ? " Done!\n" : " -Defeat The Fungal Beast Of The Sewers.\n";
            string i = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM) ? " Done!\n" : " -Defeat The Eternal Eye Of The Abbey.\n";
            string j = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER) ? " Done!\n" : " -Defeat A Ravenous, Violent Chamber.\n";
            string k = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK) ? " Done!\n" : " -Remove Each Hell-Bound Curse At Least Once.\n";
            string l = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED) ? " Done!\n" : " -Survive An Encounter With Something Wicked.\n";

            string color1 = "9006FF";
            OtherTools.PrintNoID("Unlock List:\n" + a + b + c + d + e + f + g +h+i+j+k+l, color1);
            OtherTools.Init();


            /*
            ETGModConsole.Commands.GetGroup("psog").AddUnit("dumpPlayerSprites", delegate (string[] args)
            {
                OtherTools.DumpCollection(GameManager.Instance.PrimaryPlayer.sprite.Collection);
            });
            */
        }
        public static BossManager BossManagerHook(Func<GameManager, BossManager> orig, GameManager self)
        {
            var manager = orig(self);

            foreach (BossFloorEntry cum in manager.BossFloorData)
            {

                if (cum.AssociatedTilesets == GlobalDungeonData.ValidTilesets.CASTLEGEON)
                {
                    GenericRoomTable table = RoomTableTools.CreateRoomTable();
                    WeightedRoom roomer = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources2/smileBossRoom.room").room, 1);



                    table.includedRoomTables = new List<GenericRoomTable>();
                    table.includedRooms.Add(roomer);
                    table.name = "greg";

                    IndividualBossFloorEntry ent = RoomTableTools.GenerateIndividualBossFloorEntry(table, null, "FUCK");
                    ent.BossWeight = 1;
                    ent.GlobalBossPrerequisites = new DungeonPrerequisite[0];
                    cum.Bosses.Add(ent);
                }
                foreach (IndividualBossFloorEntry fycjyou in cum.Bosses)
                {
                    ETGModConsole.Log(fycjyou.TargetRoomTable.name);
                }
            }

            //manager.whatever code you need;
            return manager;
        }

        public static void Log(string text, string color= "#9006FF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }

        public static void RunStartHook(Action<PlayerController, float> orig, PlayerController self, float invisibleDelay)
        {
            orig(self, invisibleDelay);
            float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
            bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
            if (LoopOn == true)
            {
                if (Loop == 69)
                {
                    TextMaker text = self.gameObject.AddComponent<TextMaker>();
                    text.TextSize = 5;
                    text.Color = Color.red;
                    text.ExistTime = 3;
                    text.FadeInTime = 0.75f;
                    text.FadeOutTime = 1.25f;
                    text.Text = "Ouroborous Level: " + Loop.ToString() +"? Nice";
                    text.Opacity = 1;
                    text.anchor = dfPivotPoint.TopCenter;
                    text.offset = new Vector3(-3, 2f);
                    text.GameObjectToAttachTo = self.gameObject;
                }
                else
                {
                    TextMaker text = self.gameObject.AddComponent<TextMaker>();
                    text.TextSize = 5;
                    text.Color = Color.red;
                    text.ExistTime = 3;
                    text.FadeInTime = 0.75f;
                    text.FadeOutTime = 1.25f;
                    text.Text = "Ouroborous Level: " + Loop.ToString();
                    text.Opacity = 1;
                    text.anchor = dfPivotPoint.TopCenter;
                    text.offset = new Vector3(-2, 2f);
                    text.GameObjectToAttachTo = self.gameObject;
                }
            }
        }


        private void PlaceHellShrines()
        {
            bool flag = GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON;
            if (flag)
            {
                bool flag2 = false;
                try
                {
                    for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
                    {
                        RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
                        string name =roomHandler.GetRoomName();
                        bool flag3 = !flag2 && name == "Hell Entrance";
                        if (flag3)
                        {
                            IntVector2 randomVisibleClearSpot = roomHandler.GetCenterCell();
                            bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                            if (flag4)
                            {
                                GameObject original;
                                OldShrineFactory.builtShrines.TryGetValue("psog:shrineofdarkness", out original);
                                GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x + 5.0625f, (float)randomVisibleClearSpot.y+ 5.0625f), Quaternion.identity);
                                gObj.AddComponent<HellShrineController>();
                                IPlayerInteractable[] interfaces = gObj.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces2 = gObj.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler2 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler2.RegisterInteractable(interfaces[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces2[k].ConfigureOnPlacement(roomHandler2);
                                }
                                GameObject original1;
                                OldShrineFactory.builtShrines.TryGetValue("psog:shrineofcurses", out original1);
                                GameObject gObj1 = UnityEngine.Object.Instantiate<GameObject>(original1, new Vector3((float)randomVisibleClearSpot.x - 6.0625f, (float)randomVisibleClearSpot.y - 6.0625f), Quaternion.identity);
                                gObj1.AddComponent<HellShrineController>();

                                IPlayerInteractable[] interfaces1 = gObj1.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces21 = gObj1.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler21 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler2.RegisterInteractable(interfaces1[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces21[k].ConfigureOnPlacement(roomHandler21);
                                }

                                GameObject original11;
                                OldShrineFactory.builtShrines.TryGetValue("psog:shrineofpetrification", out original11);
                                GameObject gObj11 = UnityEngine.Object.Instantiate<GameObject>(original11, new Vector3((float)randomVisibleClearSpot.x + 5.0625f, (float)randomVisibleClearSpot.y - 6.0625f), Quaternion.identity);
                                gObj11.AddComponent<HellShrineController>();

                                IPlayerInteractable[] interfaces11 = gObj11.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces211 = gObj11.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler211 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler2.RegisterInteractable(interfaces11[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces21[k].ConfigureOnPlacement(roomHandler21);
                                }

                                GameObject original2;
                                OldShrineFactory.builtShrines.TryGetValue("psog:shrineofsomething", out original2);
                                GameObject gObj2 = UnityEngine.Object.Instantiate<GameObject>(original2, new Vector3((float)randomVisibleClearSpot.x -6.25f , (float)randomVisibleClearSpot.y + 5.0625f), Quaternion.identity);
                                HellShrineController deez = gObj2.AddComponent<HellShrineController>();

                                IPlayerInteractable[] interfaces221 = gObj2.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces22 = gObj2.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler22 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler22.RegisterInteractable(interfaces221[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces2[k].ConfigureOnPlacement(roomHandler22);
                                }
                                flag2 = true;
                            }
                        }
                        
                    }
                }
                catch
                {
                    ETGModConsole.Log("Catastrophic Failure In Placing Curse Shrines! Send A Screenshot of this and associated error in F3 Console.");
                }
            }
        }
        private void PlaceOtherHellShrines()
        {
            bool flag = GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON;
            if (flag)
            {
                try
                {
                    List<int> list = Enumerable.Range(0, GameManager.Instance.Dungeon.data.rooms.Count).ToList<int>().Shuffle<int>();
                    List<int> list2 = list;
                    for (int i = 0; i < 3; i++)
                    {
                        RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[list2[i]];
                        string name = roomHandler.GetRoomName();
                        bool flag3 = roomHandler.IsStandardRoom && name != "Hell Entrance" && name != "Boss Foyer" && name != "LichRoom1" && name != "LichRoom2" && name != "LichRoom3" && name != "BigDumbIdiotBossRoom1.room";
                        if (flag3)
                        {
                            IntVector2 randomVisibleClearSpot = roomHandler.GetRandomVisibleClearSpot(6, 6);
                            bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                            if (flag4)
                            {
                                GameObject original;
                                OldShrineFactory.builtShrines.TryGetValue("psog:shrineofpurity", out original);
                                GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x, (float)randomVisibleClearSpot.y), Quaternion.identity);

                                gObj.gameObject.AddComponent<PurityShrineController>();

                                IPlayerInteractable[] interfaces = gObj.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces2 = gObj.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler2 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler2.RegisterInteractable(interfaces[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces2[k].ConfigureOnPlacement(roomHandler2);
                                }
                                this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(roomHandler, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
                            }
                            list2.Remove(i);
                        }

                    }
                }
                catch
                {
                    ETGModConsole.Log("Catastrophic Failure In Placing Purity Shrines! Send A Screenshot of this and associated error in F3 Console.");
                }
            }
        }

        private void PlaceBrokenChamberShrine()
        {
            /*
            List<int> piss = Enumerable.Range(0, GameManager.Instance.Dungeon.data.rooms.Count).ToList<int>();
            for (int i = 0; i < piss.Count; i++)
            {
                RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[piss[i]];
                string name = roomHandler.GetRoomName();
                if (name != null)
                {
                    ETGModConsole.Log(name);
                }
            }
            */
                bool flag = GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON;
            if (flag)
            {
                try
                {
                    List<int> list = Enumerable.Range(0, GameManager.Instance.Dungeon.data.rooms.Count).ToList<int>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[list[i]];
                        string name = roomHandler.GetRoomName();
                        //if (name != null)
                        //{
                            //ETGModConsole.Log(name);
                        //}
                        bool flag3 = name == "BrokenChamberRoom.room" && roomHandler.IsSecretRoom;
                        if (flag3)
                        {
                            IntVector2 randomVisibleClearSpot = roomHandler.GetCenterCell();
                            bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                            if (flag4)
                            {
                                GameObject original;
                                OldShrineFactory.builtShrines.TryGetValue("psog:brokenchambershrine", out original);
                                GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x, (float)randomVisibleClearSpot.y), Quaternion.identity);

                                BrokenChamberShrineController broken = gObj.gameObject.AddComponent<BrokenChamberShrineController>();
                                broken.obj = gObj;

                                IPlayerInteractable[] interfaces = gObj.GetInterfaces<IPlayerInteractable>();
                                IPlaceConfigurable[] interfaces2 = gObj.GetInterfaces<IPlaceConfigurable>();
                                RoomHandler roomHandler2 = roomHandler;
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    roomHandler2.RegisterInteractable(interfaces[j]);
                                }
                                for (int k = 0; k < interfaces2.Length; k++)
                                {
                                    interfaces2[k].ConfigureOnPlacement(roomHandler2);
                                }
                                Minimap.Instance.RegisterRoomIcon(roomHandler, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
                            }
                        }
                    }
                }
                catch
                {
                    ETGModConsole.Log("Catastrophic Failure In Placing Broken Chamber Shrine! Send A Screenshot of this and associated error in F3 Console.");
                }
            }
        }


        private GameObject m_instanceMinimapIcon;

        public class BrokenChamberShrineController : BraveBehaviour
        {

            public BrokenChamberShrineController()
            {
                obj = base.gameObject;
            }
            private int ID;
            public GameObject obj;
            public void Start()
            {
                ID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/EOEShrine", obj.GetComponent<tk2dBaseSprite>().Collection);
                bool Shrine = SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED);
                if (Shrine == true)
                {
                    tk2dSprite sprite = obj.GetComponent<tk2dSprite>();
                    sprite.GetComponent<tk2dBaseSprite>().SetSprite(ID);
                    try
                    {
                        SimpleShrine shrine = obj.GetComponent<SimpleShrine>();
                        shrine.text = "A shrine with 4 engravings carved onto it. Although the engravings shift, you can slightly make out what they are...";
                        shrine.OnAccept = Accept;
                        shrine.OnDecline = null;
                        shrine.acceptText = "Kneel.";
                        shrine.declineText = "Leave.";
                        //shrine.CanUse = CanUse;
                    }
                    catch
                    {
                        ETGModConsole.Log("Failure in modifying shrines (1)");
                    }
                }
            }
            public static void Accept(PlayerController player, GameObject shrine)
            {
                Gun gun = PickupObjectDatabase.GetByEncounterName(InitialiseGTEE.GunIDForEOE) as Gun;
                int StoredGunID = gun.PickupObjectId;

                //PickupObject Item1 = PickupObjectDatabase.GetByName(InitialiseGTEE.HOneToFireIt);
                int Item1ID = Game.Items[InitialiseGTEE.HOneToFireIt].PickupObjectId;
                int Item2ID = Game.Items[InitialiseGTEE.HOneToPrimeIt].PickupObjectId;
                int Item3ID = Game.Items[InitialiseGTEE.HOneToHoldIt].PickupObjectId;

                string encounterNameOrDisplayName1 = (PickupObjectDatabase.GetById(StoredGunID) as Gun).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName2 = (PickupObjectDatabase.GetById(Item1ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName3 = (PickupObjectDatabase.GetById(Item2ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName4 = (PickupObjectDatabase.GetById(Item3ID)).EncounterNameOrDisplayName;

                string header;
                string text;

                header = encounterNameOrDisplayName1 + " / " + encounterNameOrDisplayName2;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);


                header = encounterNameOrDisplayName3 + " / " + encounterNameOrDisplayName4;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);


                shrine.GetComponent<OldShrineFactory.CustomShrineController>().numUses++;
                shrine.GetComponent<OldShrineFactory.CustomShrineController>().GetRidOfMinimapIcon();
            }
            private static void Notify(string header, string text)
            {
                tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
                int spriteIdByName = encounterIconCollection.GetSpriteIdByName("Planetside/Resources/shellheart");
                GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, null, spriteIdByName, UINotificationController.NotificationColor.PURPLE, true, true);
            }
        }

        public class PurityShrineController : BraveBehaviour
        {
            public void Start()
            {
                Trigger = false;
                player = GameManager.Instance.PrimaryPlayer;
                obj = base.gameObject;
                base.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

                Vector2 vector = new Vector2(base.specRigidbody.UnitCenter.x, base.specRigidbody.UnitCenter.y);
                //Vector3 vector2 = new Vector3(vector.x, vector.y, 0f);
                //PurityShrineController component = base.gameObject.GetComponent<PurityShrineController>();
                //SpawnObjectPlayerItem HoleObject = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>();
                //component.synergyobject = PurityShrineController.HoleObject.objectToSpawn;
                //BlackHoleDoer component2 = PurityShrineController.HoleObject.objectToSpawn.GetComponent<BlackHoleDoer>();
                GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, new Vector3(base.transform.position.x + 1f, base.transform.position.y, base.transform.position.z + 5f), Quaternion.Euler(0f, 0f, 0f));
               
                
                MeshRenderer component3 = gameObject1.GetComponent<MeshRenderer>();
                base.StartCoroutine(this.HoldPortalOpen(component3, vector, gameObject1));


                var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
                component3.material.SetTexture("_PortalTex", texture);
                hole = component3;

                tk2dSprite sprite = obj.GetComponent<tk2dSprite>();
                Material material = sprite.sprite.renderer.material;
                material.shader = Shader.Find("Brave/GoopShader");
                material.SetFloat("_OilGoop", 1f);
                material.SetFloat("_OpaquenessMultiply", 0.5f);
                material.SetTexture("_WorldTex", texture);
            }
            private IEnumerator HoldPortalOpen(MeshRenderer component, Vector2 vector, GameObject gameObject1)
            {
                float elapsed = 0f;
                while (component != null)
                {
                    elapsed += BraveTime.DeltaTime;
                    float t = Mathf.Clamp01(elapsed / 0.25f);
                    component.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0f, 0.30f, t));
                    yield return null;
                }

                yield break;
            }
            private IEnumerator ClosePortal(MeshRenderer portal, tk2dSprite self) // this be closing coroutine
            {
                AkSoundEngine.PostEvent("Play_BOSS_spacebaby_charge_01", self.gameObject);
                var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
                Material material = self.sprite.renderer.material;
                material.shader = Shader.Find("Brave/GoopShader");
                material.SetFloat("_OilGoop", 1f);
                material.SetFloat("_OpaquenessMultiply", 0.5f);
                material.SetTexture("_WorldTex", texture);
                portal.material.SetFloat("_UVDistCutoff", 0.30f);
                yield return new WaitForSeconds(1);
                float elapsed = 0f;
                float duration = 2;
                float t = 0f;
                while (elapsed < duration)
                {
                    material.SetFloat("_OilGoop", 1 - (elapsed/5));
                    material.SetFloat("_OpaquenessMultiply", 0.5f + (elapsed/10));
                    elapsed += BraveTime.DeltaTime;
                    t = Mathf.Clamp01(elapsed / 1.25f);
                    portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.30f-(elapsed/30f), 0f, t));
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", self.gameObject);
                for (int i = 0; i < 15; i++)
                {
                    SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, self.sprite.WorldCenter.ToVector3ZisY(0f) + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 100), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
                }
                material.shader = Shader.Find("Brave/PlayerShader");
                UnityEngine.Object.Destroy(portal.gameObject);

                yield break;
            }

            public void Update()
            {
                RoomHandler absoluteRoom = obj.transform.position.GetAbsoluteRoom();
                if (absoluteRoom == player.CurrentRoom && !player.IsInCombat && Trigger == false)
                {
                    tk2dSprite sprite = obj.GetComponent<tk2dSprite>();
                    base.StartCoroutine(this.ClosePortal(hole, sprite));
                    Trigger = true;
                    base.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                    CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));
                }
            }
            MeshRenderer hole;
            //private GameObject synergyobject;
            //private static SpawnObjectPlayerItem HoleObject;
            //private GameObject gameObject1;
            bool Trigger;
            GameObject obj;
            PlayerController player;
        }
        public class HellShrineController : BraveBehaviour
        {

            public HellShrineController()
            {
                shrine = base.gameObject;
            }
            public void Start()
            {
                Trigger = false;
                player = GameManager.Instance.PrimaryPlayer;
                obj = base.gameObject;

                ID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/HellShrines/shrinebroken", obj.GetComponent<tk2dBaseSprite>().Collection);

            }
            public void Update()
            {

                RoomHandler absoluteRoom = obj.transform.position.GetAbsoluteRoom();
                if (absoluteRoom != player.CurrentRoom && player.IsInCombat && Trigger == false)
                {
                    Trigger = true;
                    LootEngine.DoDefaultPurplePoof(obj.transform.position, false);
                    tk2dSprite sprite = obj.GetComponent<tk2dSprite>();

                    sprite.GetComponent<tk2dBaseSprite>().SetSprite(ID);

                    try
                    {
                        SimpleShrine shrine = base.gameObject.GetComponent<SimpleShrine>();
                        shrine.text = "The spirits that have once inhabited the shrine have departed.";
                        shrine.OnAccept = null;
                        shrine.OnDecline = null;
                        shrine.acceptText = "Leave, with style.";
                        shrine.declineText = "Leave.";
                        shrine.CanUse = CanUse;
                    }
                    catch
                    {
                        ETGModConsole.Log("Failure in modifying shrines (1)");
                    }
                }
            }
            public static bool CanUse(PlayerController player, GameObject shrine)
            {
                return false;
            }
            int ID;
            GameObject shrine;
            bool Trigger;
            GameObject obj;
            PlayerController player;
        }

        public override void Exit() { }
        public override void Init() 
        {
            SaveAPIManager.Setup("psog");
        }
        public static void ReloadBreachShrinesPSOG(Action<Foyer> orig, Foyer self1)
        {
            orig(self1);
            /*
            if (!PlanetsideModule.hasInitialized)
            {
                OuroborousShrine.Add();
                ShrineFactory.PlaceBreachShrines();
                PlanetsideModule.hasInitialized = true;
            }
            ShrineFactory.PlaceBreachShrines();
            */
        }
        //private static bool hasInitialized;
    }
}






