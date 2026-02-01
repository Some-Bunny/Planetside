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
using Planetside.SoundAPI;
using BreakAbleAPI;
using BepInEx;
using HarmonyLib;
using Planetside.Static_Storage;
using Planetside.NPC_Stuff;
using AmmonomiconAPI;
using Planetside.APIs;
using Planetside.Controllers;

namespace Planetside
{
    [BepInDependency("etgmodding.etg.mtgapi")]

    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]

    public class PlanetsideModule : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.planetsideofgunymede";
        public const string NAME = "Planetside Of Gunymede Pre-Release";
        public const string VERSION = "1.3.197";
        //9006FF
        public static readonly string TEXT_COLOR = "#00d0ff";
        //00d0ff
        public static string RoomFilePath;
        public static string FilePathFolder;
        public static string GunFilePath;

        public static AdvancedStringDB Strings;

        public static AssetBundle ModAssets;
        public static AssetBundle TilesetAssets;
        public static AssetBundle SpriteCollectionAssets;
        public static AssetBundle VFXAssets;

        public static Shader InverseGlowShader;



        #region DebugToggles

        public static bool DebugMode = false;

        private static bool RoomsActive = true;
        private static bool ControllersActive = true;
        private static bool EnemiesActive = true;
        private static bool ShrinesActive = true;
        private static bool NPCsActive = true;
        private static bool PlaceablesActive = true;
        private static bool EnemyChangesActive = true;
        public static bool PrisonerDebug = false;

        #endregion



        public void Start()
        {
            new Harmony(GUID).PatchAll();
            ETGModMainBehaviour.WaitForGameManagerStart(GameManagerStart);
        }

        public void GameManagerStart(GameManager gameManager)
        {
            #region Basic Initialization

            //Initialise World-Stuff here
            CrossGameDataStorage.Start();

            FoolMode.StartUp();
            StaticShaders.InitShaders();
            GunFilePath = this.FolderPath() + "/sprites";
            //ETGMod.Assets.SetupSpritesFromFolder(GunFilePath);


            //ZipFilePath = this.Metadata.Archive;
            RoomFilePath = this.FolderPath() + "/rooms";
            FilePathFolder = this.FolderPath();
            //metadata = this.Metadata;




            //Asset bundle stuff
            ModAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidebundle");
            TilesetAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidetilesets");
            VFXAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("specialvfx");
            SpriteCollectionAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidesprites");


            StaticSpriteDefinitions.Init();
            AlphabetController.InitialiseAlphabet();

            InverseGlowShader = PlanetsideModule.ModAssets.LoadAsset<Shader>("InverseGlowShaderUpdated");
            ParticleBase.InitParticleBase();
            StaticVFXStorage.Init();
            StaticInformation.Init();
            EasyGoopDefinitions.DefineDefaultGoops();
            PlanetsideModule.Strings = new AdvancedStringDB();
            PlanetsideCommands.Init();
            ItemIDs.MakeCommand();
            StaticReferences.Init(); //<- Used in GungeonAPI, IMPORTANT to initialise it before DungeonHandler
            if (PlaceablesActive || DebugMode == false)
            {
                InitNewPlaceables.InitPlaceables();
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [PlaceablesActive] off, this should not be on a live build!");
            }
            #endregion

            //initialise Tools classes here
            Tools.Init();
            NpcTools.Init();

            #region Hooks
            Actions.Init();
            PickupHooks.Init();
            ExplosionHooks.Init();
            Hooks.Init();
            MultiActiveReloadManager.SetupHooks();
            FakePrefabHooks.Init();
            TitleDioramaHooks.Init();
            #endregion


            #region Additional APIs
            BulletBuilder.Init();
            SoundManager.Init();
            SoundManager.LoadBankFromModProject("Planetside/PlanetsideBank");
            SoundManager.RegisterStopEvent("Stop_MUS_PrisonerTheme", StopEventType.Music);
            SoundManager.RegisterStopEvent("Stop_WARWITHOUTREASON", StopEventType.Music);
            ItemBuilder.Init();
            ExpandDungeonMusicAPI.InitHooks();
            AmmonomiconModification.InitializeAmmonomiconStuff();



            #endregion




            if (EnemyChangesActive || DebugMode == false)
            {
                ToolsEnemy.Init();
                EnemyHooks.Init();
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [EnemyChangesActive] off, this should not be on a live build!");
            }


            //InfectionReplacement.Init();


            //TestTeleporter.Init();


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


            RandomPiecesOfStuffToInitialise.BuildPrefab();


            SomethingWickedEnemy.Init();
            Flowder.Init();
            HitBoxShower.Init();

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
            ResaultBlue.Add();

            Immateria.Add();

            //Unlocked By Beating Loop 1

            ArmWarmer.Add();
            Oscillato.Add();
            OscillatoSynergyForme.Add();
            //==
            RebarPuncher.Add();
            LaserChainsaw.Add();
            ExecutionersCrossbow.Add();
            ForgiveMePlease.Init();
            ForgiveMePlease.BuildPrefab();
            PortablePylon.Init();

            //LoaderPylonController.Init();
            //LoaderPylonSynergyFormeController.Init();

            DeadKingsDesparation.Init();
            DeadKingsDesparation.BuildPrefab();

            //Debuff Icons

            HolyBlessingEffect.Init();
            ExecuteDebuff.Init();

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
            TestShaderBullets.Init();
            DerpyBullets.Init();
            PrayerAmulet.Init();
            LockOnGun.Add();
            CoinTosser.Init();
            //DivineLight.Add();
            //HellLight.Add();
            //KnuckleBlaster.Init();
            Preigniter.Init();
            AttractorBeam.Add();
            LilPew.Add();

            TarnishedRounds.Init();
            TarnishedAmmolet.Init();
            GunslingersRing.Init();
            LuckyCharm.Init();
            TrapDefusalKit.Init();
            TableTechTelefrag.Init();
            AlchemicalVial.Init();
            SpinningDeath.Init();
            ConnoisseursRobes.Init();
            LostVoidPotential.Init();
            ShopInABox.Init();
            StableVector.Init();
            ShipmentRequestForm.Init();
            //Tracker.Add();

            //ModifierNeedle.Init();
            TatteredRobe.Init();
            OrbOfPower.Init();

            NemesisGun.Add();
            NemesisRailgun.Add();
            NemesisShotgun.Add();


            Whistler.Add();
            ThunderShot.Add();
            HexaPulseCannon.Add();
            PulseCannon.Add();
            ParticleCollapser.Add();
            HeatSink.Add();
            MeasuringTape.Add();
            //Forgotten Rounds
            ForgottenRoundOubliette.Init();
            ForgottenRoundAbbey.Init();
            ForgottenRoundRat.Init();
            ForgottenRoundRNG.Init();
            SporeBullets.Init();
            OrbitalInsertion.Init();
            Autocannon.Add();
            PunctureWound.Add();
            GunClassToken.Init();
            // ShopDiscountItem.Init();
            SawBladeGun.Add();
            CoinShot.Add();
            NeutroniumCore.Init();

            //Perks
            AllSeeingEyeMiniPickup.Init();
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
            Patience.Init();
            CorruptedWealth.Init();
            HollowWalls.Init();

            CandyHeart.Init();
            CanisterLauncher.Add();
            Sawcon.Add();


            LightningController.Init();
            SelfReplicatingBlank.Init();
            LightningMaker.Init();
            SurgeGrenade.Init();
            StormBringer.Add();
            ImmolationPowder.Init();
            WrapaRounds.Init();
            WarpMastersKit.Init();

            PointNull.Add();
            Kunai.Init();
            LeadBaton.Init();
            StaticCharger.Add();

            InitialiseSynergies.DoInitialisation();
            SynergyFormInitialiser.AddSynergyForms();
            InitialiseGTEE.DoInitialisation();
            HoveringGunsAdder.AddHovers();
            HotSwapper.Init();
            CHROMA.Init();
            GunWithNoName.Add();

            RepairNode.Init();
            UmbraController.InitEffect();

            LostAdventurersSword.Init();
            LostAdventurersBackpack.Init();
            LostAdventurersShield.Init();

            //TestActiveItem.Init();

            #region Enemies
            if (EnemiesActive || DebugMode == false)
            {
                Ophanaim.Init();
                Fungannon.Init();
                Coallet.Init();
                Shamber.Init();

                ProperCube.Init(true, true, false, false);
                ProperCube.Init(false, false, true, true, "leftRight", true);


                Detscavator.Init();

                Creationist.Init();
                Observant.Init();
                Stagnant.Init();
                Inquisitor.Init();
                Vessel.Init();
                Unwilling.Init();
                Collective.Init();
                Bloat.Init();
                Tower.Init();
                Oppressor.Init();

                //fuck 
                TrespassTrollRock.Init();


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
                RobotShopkeeperBoss.Init();
                FriendlyHMPrime.Init();
                //Servont.Init();

                THREarthmover.Init();
                Revenant_Enemy.Init(true);
                Revenant_Enemy.Init(false);


                An3sBullet.Init();
                HunterBullet.Init();
                NevernamedBullet.Init();
                SkilotarBullet.Init();
                JuneBullet.Init();
                SpapiBullet.Init();
                GlaurBullet.Init();
                ApacheBullet.Init();
                NeighborinoBullet.Init();
                BleakBullet.Init();
                QueenBullet.Init();
                PandaBullet.Init();
                RetrashBullet.Init();
                KyleBullet.Init();
                BunnyBullet.Init();
                BotBullet.Init();
                WowBullet.Init();
                TurboBullet.Init();
                SpcreatBullet.Init();
                MarcyBullet.Init();
                LordRiceBullet.Init();
                PretzelBullet.Init();

                GoldenRevolverBullet.Init();
                NotSoAIBullet.Init();
                CortifyBullet.Init();
                ShotzerBullet.Init();
                LynceusBullet.Init();
                DallanBullet.Init();

                BulletBankMan.Init();

                Shellrax.Init();
                Wailer.Init();

                CelBullet.Init();
                QadayBullet.Init();

                Nemesis.Init();
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [EnemiesActive] off, this should not be on a live build!");
            }
            #endregion



            //MiniMap.Init();

            #region Shrines
            if (ShrinesActive || DebugMode == false)
            {
                ShrineFactory.Init();
                ShrineFakePrefabHooks.Init();
                GunOrbitShrine.Add();
                NullShrine.Add();
                HolyChamberShrine.Add();
                TooLate.Add();
                PrisonShrine.Add();
                VoidMuncher.Add();
                TrespassChallengeShrine.Add();

                BrokenChamberShrine.Add();
                ShrineOfDarkness.Add();
                ShrineOfCurses.Add();
                ShrineOfPetrification.Add();
                ShrineOfSomething.Add();
                ShrineOfPurity.Add();

                SWMinesShrine.Add();
                BlueShrine.Add();
                RedShrine.Add();

                RoomReader.Init();
                QuestWanderer.Add();

                OuroborousShrine.Add();

                ShrineFactory.PlaceBreachShrines();
                SomethingWickedEnemy.Init();
                SomethingWickedEnemy.InitDummyEnemy();
                Thing.Init();
                RedThing.Init();


            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [ShrinesActive] off, this should not be on a live build!");
            }
            #endregion






            if (NPCsActive || DebugMode == false)
            {
                CustomLootTableInitialiser.InitialiseCustomLootTables();
                CustomShopInitialiser.InitialiseCustomShops();
                Absconditus.Init();
                TrespassStone.Init();
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [NPCsActive] off, this should not be on a live build!");
            }




            if (ControllersActive || DebugMode == false)
            {
                FlowInjectionInitialiser.InitialiseFlows();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<HMPrimeSpawnController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<NevernamedsDarknessHandler>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<MasteryTraderSpawnController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<SomethingWickedEventManager>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<TimeTraderSpawnController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<SpecificUnlockController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<OuroborosController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<CursesController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<ContainmentBreachController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<ChallengeModeExtraChallenges>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<HellShrinesController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<NemesisSpawnController>();
                ETGModMainBehaviour.Instance.gameObject.AddComponent<ResourcefulRatMasteryController>();

                
                PlanetsideQOL.Init();
                PlanetsideBalanceChanges.Init();
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [ControllersActive] off, this should not be on a live build!");
            }


            if (RoomsActive || DebugMode == false)
            {
                DungeonHandler.Init();
                CustomDungeonHooks.InitDungeonHook();
                ModPrefabs.InitCustomPrefabs();
                ModRoomPrefabs.InitCustomRooms();
                AbyssDungeonFlows.InitDungeonFlows();
                AbyssDungeon.InitCustomDungeon();
                new Hook(
                         typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
                         typeof(PlanetsideModule).GetMethod("GameManager_Awake", BindingFlags.NonPublic | BindingFlags.Instance),
                         typeof(GameManager)
                     );
                //Alexandria.DungeonAPI.RoomUtility.EnableDebugLogging = true;

                Alexandria.DungeonAPI.RoomFactory.LoadRoomsFromRoomDirectory("psog", FilePathFolder + "/newRooms");
            }
            else
            {
                Log($"[PSOG] WARNING, Game launched with Debug Setting [RoomsActive] off, this should not be on a live build!");
            }


            if (DebugMode == true)
            {
                ETGModConsole.Commands.AddGroup("psog_floor", args =>
                {
                });
                ETGModConsole.Commands.GetGroup("psog_floor").AddUnit("load", this.LoadFloor);
            }

            PlanetsideModule.Log($"{{{{ {NAME} v{VERSION} started successfully. }}}}" + (DebugMode ? "[DEBUG MODE : NOT FOR LIVE BUILD]" : ""), TEXT_COLOR);
            List<string> RandomFunnys = new List<string>
            {
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
                "Powered by SaveAPI!",
                "Now With 100% Less Nulls!",
                "You Lost The Game.",
                "WEAK.",
                "weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                "If you see this, you owe me 10 bucks you nerd.",
                "Ashes To Ashes, To Ashes (To Ashes)",
                "Deadbolt Is Underrated!",
                "You're Gonna Need A Bigger Gun",
                "oh no",
                "is that supposed to be like that???",
                "I removed an item from the item pool, which one is it? ;)",
                "Peter Griffin!",
                "Hey VSauce!",
                "Yo Mama!",
                "NullReferenceException: Object Reference not set to an instance of an object.",
                "Nuh uh, yr'oue.",
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
                "Another Night...",
                "skibidi bop mm dada",
                "I coded most of this after 1am.",
                "Planetside Supports Trans Rights",
                "bepis",
                "pootis",
                "Frogs are cool!",
                "Poor aim, and a poor Reaper.",
                "The Sun! The Sun! The Sun!",
                "if(player.IsStupid){  }",
                "cultist_comits_tax_evasion.mp3",
                "Stop it, I'm bees!",
                "nice",
                "Powered By AudioBuilder!",
                "Powered By BeamBuilder!",
                "Powered By Friendship!",
                "YOUR PAST IS DEAD",
                "LEAD IS FUEL",
                "BULLET HELL IS FULL",
                "What the dog doin'",
                "Powered by like 7 different APIs",
                "Rock And Stone!",
                "By The Beard!",
                "Powered By BreakableAPI!",
                "I removed Clone from the item pool, thank me later.",
                "Powered By SynergyAPI!",
                "Amogus!",
                "DM me images of pigeons you find on the street.",
                "Powered by MTGAPI!",
                "Now with 100% more Bepinex!",
                "I once sneezed a pasta noodle out through my nose -- back when I had a nose, on Earth.",
                "You shouldn't read too much anyway. It's bad for your teeth.",
                "This cats name is Humongous Honkers, look him up on your work computer.",
                "Check out Astronautilus!",
                "Check out Star Of Providence!",
                "Check Out ZeroRanger!",
                "Check Out Animal Well!",
                "Abandon everything you hold dear.",
                "This is it.",
                "When the fajitas come out sizzlin'",
                "GREEN FRAUD",
                "DESTROY"
            };
            Random r = new Random();
            int index = r.Next(RandomFunnys.Count);
            string randomString = RandomFunnys[index];
            Log(" - " + randomString, TEXT_COLOR);

            Log("Here's a To-Do list for unlocks, hope you have fun!", TEXT_COLOR);
            Log("If you ever need a reminder, the command 'psog to_do_list' to remind yourself.", TEXT_COLOR);
            string a = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED) ? " Done!\n" : " -Defeat The Dragun At 'Full Curse'.\n";
            string b = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED) ? " Done!\n" : " -Defeat The Guardian Of The Holy Chamber.\n";
            string c = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED) ? " Done!\n" : " -Defeat The Failed Demi-Lich\n";
            string d = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED) ? " Done!\n" : " -Defeat The Banker Of Bullets.\n";
            string e = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED) ? " Done!\n" : " -Defeat The Lich With A Broken Remnant In Hand.\n";
            string f = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LOOP_1) ? " Done!\n" : " -Beat The Game On Ouroborous Level 0.\n";
            string g = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND) ? " Done!\n" : " -Kill A Boss After Dealing 500 Damage Or More At Once.\n";
            string h = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON) ? " Done!\n" : " -Defeat The Fungal Beast Of The Sewers.\n";
            string i = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM) ? " Done!\n" : " -Defeat The Observant Aimgel Of The Abbey.\n";
            string j = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER) ? " Done!\n" : " -Defeat A Ravenous, Violent Chamber.\n";
            string k = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK) ? " Done!\n" : " -Remove Each Hell-Bound Curse At Least Once.\n";
            string l = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED) ? " Done!\n" : " -Survive An Encounter With Something Wicked.\n";
            string m = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE) ? " Done!\n" : " -Trespass Into Somewhere Else.\n";

            string n = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.UMBRAL_ENEMIES_KILLED) >= 4 ? " Done!\n" : " -Slay 5 Umbral Enemies.\n";
            string o = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED) >= 14 ? " Done!\n" : " -Defeat 15 Jammed Arch Gunjurers.\n";
            string p = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED) ? " Done!\n" : " -Perform Maintenance On The Damaged Robot.\n";

            string q = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4) ? " Done!\n" : " -Perform the Highest Level Maintenance On The Damaged Robot.\n";
            string r1 = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.INFECTED_FLOOR_COMPLETED) ? " Done!\n" : " -Break The Veil and Survive A Mixed Chamber.\n";
            string s = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.NEMESIS_KILLED) ? " Done!\n" : " -Defeat Your Rival.\n";


            string color1 = "00d0ff";
            OtherTools.PrintNoID("Unlock List:\n" + a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p + q + r1 + s, color1);
            OtherTools.Init();
            //new Hook(typeof(ConversationBarController).GetMethod("ShowBar", BindingFlags.Instance | BindingFlags.Public), typeof(PlanetsideModule).GetMethod("HAHA"));


            //            GameManager.Instance.StartCoroutine(FrameDelay());
        }

        public IEnumerator FrameDelay()
        {
            yield return null;
            yield break;
        }

        public static void HAHA(Action<ConversationBarController, PlayerController, string[]> orig, ConversationBarController self, PlayerController p, string[] r)
        {
            orig(self, p, r);
        }



        private void LoadFloor(string[] obj)
        {
            GameManager.Instance.LoadCustomLevel(AbyssDungeon.AbyssDefinition.dungeonSceneName);
        }
        private void GameManager_Awake(Action<GameManager> orig, GameManager self)
        {
            orig(self);
            AbyssDungeon.InitCustomDungeon();
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
            //SaveAPIManager.Setup("psog");

            //manager.whatever code you need;
            return manager;
        }

        public static void Log(string text, string color = "#00d0ff")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }


        public void Awake()
        {
            SaveAPIManager.Setup("psog");
        }

    }
}






