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
using BepInEx;
using HarmonyLib;




namespace Planetside
{
    // [HarmonyLib]

    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]

    public class PlanetsideModule : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.planetsideofgunymede";
        public const string NAME = "Planetside Of Gunymede";
        public const string VERSION = "1.3.0";

        public static readonly string TEXT_COLOR = "#9006FF";

        public static string RoomFilePath;
        public static string FilePathFolder;

        public static AdvancedStringDB Strings;
        public static HellDragZoneController hellDrag;

        public static AssetBundle ModAssets;
        public static AssetBundle TilesetAssets;
        public static AssetBundle SpriteCollectionAssets;

        public static Shader InverseGlowShader;

        public static bool DebugMode = true;

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GameManagerStart);
        }

        public void GameManagerStart(GameManager gameManager)
        {
            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");
            PlanetsideModule.hellDrag = forgeDungeon.PatternSettings.flows[0].AllNodes.Where(node => node.overrideExactRoom != null && node.overrideExactRoom.name.Contains("EndTimes")).First().overrideExactRoom.placedObjects.Where(ppod => ppod != null && ppod.nonenemyBehaviour != null).First().nonenemyBehaviour.gameObject.GetComponentsInChildren<HellDragZoneController>()[0];
            forgeDungeon = null;

            ETGMod.Assets.SetupSpritesFromFolder(this.FolderPath()+"/sprites");

            
            //ZipFilePath = this.Metadata.Archive;
            RoomFilePath = this.FolderPath() + "/rooms";
            FilePathFolder = this.FolderPath();
            //metadata = this.Metadata;

            //Initialise World-Stuff here
            CrossGameDataStorage.Start();

            ChamberGunAPI.Init("PlanetsideOfGunymede");

            //Asset bundle stuff
            PlanetsideModule.ModAssets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidebundle");
            foreach (string str in PlanetsideModule.ModAssets.GetAllAssetNames())
            {
                //ETGModConsole.Log(PlanetsideModule.ModAssets.name + ": " + str, false);
            }
            AssetBundle tilesets = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidetilesets");
            if (tilesets != null) { TilesetAssets = tilesets; }

            AssetBundle spriteCollections = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("planetsidesprites");
            if (spriteCollections != null) { SpriteCollectionAssets = spriteCollections; }

            foreach (string str in PlanetsideModule.TilesetAssets.GetAllAssetNames())
            {
                //ETGModConsole.Log(PlanetsideModule.TilesetAssets.name + ": " + str, false);
            }
            StaticInformation.Init();

            InverseGlowShader = PlanetsideModule.ModAssets.LoadAsset<Shader>("inverseglowshader");

            //Initialise Statically Stored Stuff Here
            StaticVFXStorage.Init();

            EasyGoopDefinitions.DefineDefaultGoops();

            RandomPiecesOfStuffToInitialise.BuildPrefab();

            PlanetsideModule.Strings = new AdvancedStringDB();

            PlanetsideCommands.Init();

            ItemIDs.MakeCommand();

            StaticReferences.Init(); //<- Used in GungeonAPI, IMPORTANT to initialise it before DungeonHandler

            InitNewPlaceables.InitPlaceables();


            //initialise Tools classes here
            Tools.Init();
            NpcTools.Init();

            //Hook stuff here
            Actions.Init();
            PickupHooks.Init();
            ExplosionHooks.Init();
            Hooks.Init();
            MultiActiveReloadManager.SetupHooks();

            FakePrefabHooks.Init();

            TitleDioramaHooks.Init();



            //Initialise API stuff here
            BulletBuilder.Init();

            SoundManager.Init();

            SoundManager.LoadBankFromModProject("Planetside/PlanetsideBank");

            SoundManager.RegisterStopEvent("Stop_MUS_PrisonerTheme", StopEventType.Music);

            ItemBuilder.Init();
            ExpandDungeonMusicAPI.InitHooks();


            //Shrine Initialisation
            //ShrineFactory.Init();
            ShrineFactory.Init();
            ShrineFakePrefabHooks.Init();
            GunOrbitShrine.Add();
            NullShrine.Add();
            HolyChamberShrine.Add();
            TooLate.Add();
            PrisonShrine.Add();
            VoidMuncher.Add();
            TrespassChallengeShrine.Add();

            ToolsEnemy.Init();
            EnemyHooks.Init();

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
            TarnishEffect.Init();
            BrainHostDummyBuff.Init();
            InfectedEnemyEffect.Init();
            InfectedBossEffect.Init();

            DebuffLibrary.Init();

            LeSackPickup.Init();
            NullPickupInteractable.Init();
            RepairNode.Init();

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
            KnuckleBlaster.Init();
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
            Tracker.Add();

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
            ForgottenRoundRNG.Init();
            SporeBullets.Init();
            OrbitalInsertion.Init();
            Autocannon.Add();
            PunctureWound.Add();
            GunClassToken.Init();
            ShopDiscountItem.Init();
            SawBladeGun.Add();
            CoinShot.Add();

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
            Patience.Init();
            CorruptedWealth.Init();

            //VengefulShell.Init();
            //LaserWelder.Add();
            //BoscoDesignator.Add();

            Ophanaim.Init();
            Fungannon.Init();
            Coallet.Init();
            Shamber.Init();
            ProperCube.Init();
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
            CortifyBullet.Init();
            ShotzerBullet.Init();

            BulletBankMan.Init();

            Shellrax.Init();
            Wailer.Init();

            CelBullet.Init();
            Nemesis.Init();


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
            //DungeonHooks.OnPostDungeonGeneration += this.PlaceOtherHellShrines;


            //TestActiveItem.Init();
            OuroborousShrine.Add();

            ShrineFactory.PlaceBreachShrines();
            SomethingWickedEnemy.Init();
            SomethingWickedEnemy.InitDummyEnemy();
            Thing.Init();
            RedThing.Init();

            CustomLootTableInitialiser.InitialiseCustomLootTables();
            CustomShopInitialiser.InitialiseCustomShops();
            TrespassStone.Init();
            //AGM.Register();

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


            //HellShrinesController
            PlanetsideQOL.Init();
            PlanetsideBalanceChanges.Init();

            DungeonHandler.Init();
            MasteryReplacementOub.InitDungeonHook();

            ModPrefabs.InitCustomPrefabs();
            ModRoomPrefabs.InitCustomRooms();
            AbyssDungeonFlows.InitDungeonFlows();
            AbyssDungeon.InitCustomDungeon();
            new Hook(
                     typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
                     typeof(PlanetsideModule).GetMethod("GameManager_Awake", BindingFlags.NonPublic | BindingFlags.Instance),
                     typeof(GameManager)
                 );

            ETGModConsole.Commands.AddGroup("psog_floor", args =>
            {
            });
            ETGModConsole.Commands.GetGroup("psog_floor").AddUnit("load", this.LoadFloor);


            PlanetsideModule.Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
            List<string> RandomFunnys = new List<string>
            {
                "Powered by SaveAPI!",
                "Now With 100% Less Nulls!",
                "You Lost The Game.",
                "WEAK.",
                "Bullet Banks are not to rob!",
                "Art By SirWow!",
                "*Don't Download Some Bunnys Content Pack, you can't even have it on Bepinex anyway :D",
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
                "Up Up Down Down Left Right Left Right B A Start",
                "Another Night...",
                "skibidi bop mm dada",
                "I coded most of this after 1am.",
                "Planetside Supports Trans Rights",
                "bepis",
                "pootis",
                "Frogs are cool!",
                "Poor aim, and a poor Reaper.",
                "egassem sdrawkcab",
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
                "You shouldn't read too much anyway. It's bad for your teeth."
            };
            Random r = new Random();
            int index = r.Next(RandomFunnys.Count);
            string randomString = RandomFunnys[index];
            Log(" - " + randomString, TEXT_COLOR);

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
            string m = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE) ? " Done!\n" : " -Trespass Into Somewhere Else.\n";

            string n = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.UMBRAL_ENEMIES_KILLED) >= 4 ? " Done!\n" : " -Slay 5 Umbral Enemies.\n";
            string o = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED) >= 14 ? " Done!\n" : " -Defeat 15 Jammed Arch Gunjurers.\n";
            string p = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED) ? " Done!\n" : " -Perform Maintenance On The Damaged Robot.\n";

            string q = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4) ? " Done!\n" : " -Perform the Highest Level Maintenance On The Damaged Robot.\n";


            string color1 = "9006FF";
            OtherTools.PrintNoID("Unlock List:\n" + a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p + q, color1);
            OtherTools.Init();
        }

       // public override void Start(){}


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

        public static void Log(string text, string color= "#9006FF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }


        public void Awake() 
        {
            //new Harmony(GUID).PatchAll();
            SaveAPIManager.Setup("psog");
        }
        /*
        [HarmonyPatch(typeof(Minimap), "AddIconToRoomList")]
        [HarmonyPrefix]
        public static void MIniMapPatch(RoomHandler room, GameObject instanceIcon)
        {
            bool active =instanceIcon.activeSelf;
            instanceIcon.SetActive(true);
            instanceIcon.SetActive(active);
        }
        */
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






