using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/*
namespace Planetside
{
    public static class AddItemsToChamberGunIndex
    {
      

        public static void AddAsChamberGunForme(this Gun gun, string modName, int targetFloorTilesetID, List<int> viableMasterRounds, float index)
        {
            Debug.Log($"ChamberGunAPI ({modName}): Adding custom Chamber Gun Forme with ID {gun.PickupObjectId} for Chamber ID {targetFloorTilesetID}.");
            CustomChamberGunForm custom = gun.gameObject.AddComponent<CustomChamberGunForm>();
            custom.modName = modName;
            custom.floorTilesetID = targetFloorTilesetID;
            custom.correspondingFormeID = gun.PickupObjectId;
            custom.indexValue = index;
            if (custom.viableMasterRounds == null) custom.viableMasterRounds = new List<int>();
            if (viableMasterRounds != null && viableMasterRounds.Count > 0) custom.viableMasterRounds.AddRange(viableMasterRounds);

            EtGModChamberGunDeliveryComponent delivery = ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<EtGModChamberGunDeliveryComponent>();
            if (delivery != null)
            {
                if (delivery.modName == "unset")
                {
                    delivery.modName = modName; Debug.Log($"ChamberGunAPI ({modName}): Initialising first added item from mod {modName} with ID {gun.PickupObjectId}");
                }
                if (delivery.chamberGunFormIDs == null || delivery.chamberGunFormIDs.Count <= 0) { delivery.chamberGunFormIDs = new List<int>(); }
                delivery.chamberGunFormIDs.Add(gun.PickupObjectId);
            }
        }
        public static void AddAsChamberGunMastery(this PickupObject item, string modName, int targetFloorTilesetID)
        {
            Debug.Log($"ChamberGunAPI ({modName}): Adding custom Chamber Gun Master with ID {item.PickupObjectId} for Chamber ID {targetFloorTilesetID}.");
            CustomChamberGunMasterRound custom = item.gameObject.AddComponent<CustomChamberGunMasterRound>();
            custom.modName = modName;
            custom.floorTilesetID = targetFloorTilesetID;

            EtGModChamberGunDeliveryComponent delivery = ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<EtGModChamberGunDeliveryComponent>();
            if (delivery != null)
            {
                if (delivery.modName == "unset")
                {
                    delivery.modName = modName; Debug.Log($"ChamberGunAPI ({modName}): Initialising first added item from mod {modName} with ID {item.PickupObjectId}");
                }
                if (delivery.chamberGunMasteryIDs == null || delivery.chamberGunMasteryIDs.Count <= 0) { delivery.chamberGunMasteryIDs = new List<int>(); }
                delivery.chamberGunMasteryIDs.Add(item.PickupObjectId);
            }
        }
    }
    public class CustomChamberGunForm : MonoBehaviour
    {
        public string modName;
        public int floorTilesetID;
        public List<int> viableMasterRounds;
        public int correspondingFormeID;
        public float indexValue;
    }
    public class CustomChamberGunMasterRound : MonoBehaviour
    {
        public string modName;
        public int floorTilesetID;
    }
    public class EtGModChamberGunDeliveryComponent : MonoBehaviour
    {
        public string modName = "unset";
        public List<int> chamberGunFormIDs = new List<int>();
        public List<int> chamberGunMasteryIDs = new List<int>();
    }
    class ChamberGunAPI
    {
        public static void Init(string modName)
        {
            Gun ChamberGun = PickupObjectDatabase.GetById(647) as Gun;
            ChamberGunProcessor extantProcessor = ChamberGun.GetComponent<ChamberGunProcessor>();
            if (extantProcessor)
            {

                AdvancedChamberGunController newProcessor = ChamberGun.gameObject.AddComponent<AdvancedChamberGunController>();
                newProcessor.RefillsOnFloorChange = true;
                newProcessor.primeHandlerModName = modName;
                //newProcessor.hyperDebugMode = true;

                if (AdvancedChamberGunController.floorFormeDatas == null)
                {
                    AdvancedChamberGunController.floorFormeDatas = new List<AdvancedChamberGunController.ChamberGunData>();
                }

                #region SetupVanillaFloors
                //KEEP
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 2,
                    indexValue = 1,
                    correspondingFormeID = 647,
                    viableMasterRounds = new List<int>()
                    {
                        469,
                    }
                });
                //OUBLIETTE
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 4,
                    indexValue = 1.5f,
                    correspondingFormeID = 657,
                    viableMasterRounds = new List<int>()
                    {

                    }
                });
                //GUNGEON PROPER
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 1,
                    indexValue = 2,
                    correspondingFormeID = 660,
                    viableMasterRounds = new List<int>()
                    {
                        471,
                    }
                });
                //ABBEY
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 8,
                    indexValue = 2.5f,
                    correspondingFormeID = 806,
                    viableMasterRounds = new List<int>()
                    {
                    }
                });
                //MINES
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 16,
                    indexValue = 3,
                    correspondingFormeID = 807,
                    viableMasterRounds = new List<int>()
                    {
                        468,
                    }
                });
                //RAT FLOOR
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 32768,
                    indexValue = 3.5f,
                    correspondingFormeID = 808,
                    viableMasterRounds = new List<int>()
                    {
                    }
                });
                //HOLLOW
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 32,
                    indexValue = 4,
                    correspondingFormeID = 659,
                    viableMasterRounds = new List<int>()
                    {
                        470,
                    }
                });
                //R&G DEPT
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 2048,
                    indexValue = 4.5f,
                    correspondingFormeID = 823,
                    viableMasterRounds = new List<int>()
                    {
                    }
                });
                //FORGE
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 64,
                    indexValue = 5,
                    correspondingFormeID = 658,
                    viableMasterRounds = new List<int>()
                    {
                        467,
                    }
                });
                //BULLET HELL
                AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                {
                    modName = "Vanilla Gungeon",
                    floorTilesetID = 128,
                    indexValue = 6,
                    correspondingFormeID = 763,
                    viableMasterRounds = new List<int>()
                    {
                    }
                });
                #endregion

                ETGMod.StartGlobalCoroutine(postStart(newProcessor, modName));

                UnityEngine.Object.Destroy(extantProcessor);
                Debug.Log($"Mod '{modName}' correctly initialised the ChamberGunAPI and is the prime handler.");
            }
            else
            {
                Debug.Log($"Mod '{modName}' did not alter the Chamber Gun, as the Chamber Gun was already altered.");
            }
        }
        public static IEnumerator postStart(AdvancedChamberGunController newProcessor, string modName)
        {
            yield return null;
            //These variables are for debugging to keep track of how many things were added to the gun
            int newFormesAdded = 0;
            int masterRoundsAdded = 0;

            //Define a master list
            List<int> FormestoCheck = new List<int>();
            List<int> MasteriesToCheck = new List<int>();

            //Nullchecks 4-ever
            if (ETGModMainBehaviour.Instance == null || ETGModMainBehaviour.Instance.gameObject == null)
            {
                Debug.LogError($"ChamberGunAPI ({modName}): ETGModMainBehaviour.Instance OR ETGModMainBehaviour.Instance.gameObject was NULL.");
                yield break;
            }
            //Here we check the ETGMod main object for any components with the name of the delivery component.
            foreach (Component component in ETGModMainBehaviour.Instance.gameObject.GetComponents<Component>())
            {
                if (component.GetType().ToString().ToLower().Contains("etgmodchambergundeliverycomponent"))
                {
                    //If the component is present we use reflection to get the lists it contains and adds them to the master list we defined earlier
                    List<int> formeIDsInComp = (List<int>)ReflectionHelper.GetValue(component.GetType().GetField("chamberGunFormIDs"), component);
                    List<int> masteryIDsInComp = (List<int>)ReflectionHelper.GetValue(component.GetType().GetField("chamberGunMasteryIDs"), component);
                    string compModName = (string)ReflectionHelper.GetValue(component.GetType().GetField("modName"), component);
                    if (formeIDsInComp != null && formeIDsInComp.Count > 0)
                    {
                        //Add the list of Forme IDs to the master list
                        FormestoCheck.AddRange(formeIDsInComp);
                    }
                    if (masteryIDsInComp != null && masteryIDsInComp.Count > 0)
                    {
                        //Add the list of Mastery IDs to the master list.
                        MasteriesToCheck.AddRange(masteryIDsInComp);
                    }
                    Debug.Log($"ChamberGunAPI ({modName}): Detected Delivery Component from Mod '{compModName}' with {formeIDsInComp.Count} formes and {masteryIDsInComp.Count} additional masteries.");
                }
            }

            //If the formes to check master list is not empty or null, we iterate through it
            if (FormestoCheck != null && FormestoCheck.Count > 0)
            {
                foreach (int id in FormestoCheck)
                {
                    //Get the pickupobject corresponding to the ID
                    PickupObject gunPickObj = PickupObjectDatabase.GetById(id);
                    if (gunPickObj == null) Debug.LogError($"ChamberGunAPI ({modName}): A mod attempted to add item ID {id} as a forme, but that ID does not exist!");
                    if (!(gunPickObj is Gun)) Debug.LogError($"ChamberGunAPI ({modName}): A mod attempted to add item ID {id} as a forme, but that ID does not correspond to a Gun.");

                    //Iterate through each component in the pickupobject looking for the one containing all the actual info on the forme
                    bool foundCompOnThisID = false;
                    foreach (Component component in gunPickObj.GetComponents<Component>())
                    {
                        if (component.GetType().ToString().ToLower().Contains("customchambergunform"))
                        {
                            foundCompOnThisID = true;

                            //Use reflection to get all the necessary values off the component
                            string modname = (string)ReflectionHelper.GetValue(component.GetType().GetField("modName"), component);
                            int desiredTileset = (int)ReflectionHelper.GetValue(component.GetType().GetField("floorTilesetID"), component);
                            List<int> validMasterRounds = (List<int>)ReflectionHelper.GetValue(component.GetType().GetField("viableMasterRounds"), component);
                            int correspondingForm = (int)ReflectionHelper.GetValue(component.GetType().GetField("correspondingFormeID"), component);
                            float index = (float)ReflectionHelper.GetValue(component.GetType().GetField("indexValue"), component);

                            //Some debugging messages for when shit inevitably goes wrong
                            Debug.Log($"ChamberGunAPI ({modName}): Adding cross mod chamber gun form with the following criteria -> ModName({modName}), TilesetID({desiredTileset}), GunID({correspondingForm}), Index({index})");
                            if (string.IsNullOrEmpty(modname) || modname == "Unset") Debug.LogWarning($"ChamberGunAPI ({modName}): Trying to add a form with no modname set, this may make things difficult to debug!");

                            if (validMasterRounds == null) { validMasterRounds = new List<int>() { }; }

                            //Actually Add forms to the processor
                            AdvancedChamberGunController.floorFormeDatas.Add(new AdvancedChamberGunController.ChamberGunData()
                            {
                                modName = modname,
                                floorTilesetID = desiredTileset,
                                indexValue = index,
                                correspondingFormeID = correspondingForm,
                                viableMasterRounds = validMasterRounds,
                            });
                            newFormesAdded += 1;
                        }
                    }
                    if (foundCompOnThisID == false)
                    {
                        //If iterating through every component on the gun didn't find a proper component, we shit out an error here
                        Debug.LogError($"ChamberGunAPI ({modName}): A mod attempted to add item ID {id} as a forme, but that ID did not possess a data component?");
                    }
                }
            }
            //Do the same with the Masteries list
            if (MasteriesToCheck != null && MasteriesToCheck.Count > 0)
            {
                foreach (int id in MasteriesToCheck)
                {
                    //Get the pickupobject corresponding to the ID
                    PickupObject masteryObj = PickupObjectDatabase.GetById(id);
                    if (masteryObj == null) Debug.LogError($"ChamberGunAPI ({modName}): A mod attempted to add item ID {id} as a custom mastery, but that ID does not exist!");

                    //Iterate through each component on the item
                    bool foundCompOnThisID = false;
                    foreach (Component component in masteryObj.GetComponents<Component>())
                    {
                        if (component.GetType().ToString().ToLower().Contains("customchambergunmasterround"))
                        {
                            foundCompOnThisID = true;
                            string modname = (string)ReflectionHelper.GetValue(component.GetType().GetField("modName"), component);
                            int tilesetID = (int)ReflectionHelper.GetValue(component.GetType().GetField("floorTilesetID"), component);
                            bool foundTileset = false;

                            //Iterate through each form data in the list to see if there's a form that matches the set tileset id of the mastery
                            foreach (AdvancedChamberGunController.ChamberGunData data in AdvancedChamberGunController.floorFormeDatas)
                            {
                                if (data.floorTilesetID == tilesetID)
                                {
                                    //If the ids match, add the mastery id to that form's list of valid mastery IDs
                                    if (data.viableMasterRounds == null) data.viableMasterRounds = new List<int>() { };
                                    data.viableMasterRounds.Add(masteryObj.PickupObjectId);
                                    Debug.Log($"ChamberGunAPI ({modName}): Mod '{modname}' added a viable Master round with the Id {masteryObj.PickupObjectId} to the viable master rounds of floor tileset id {tilesetID}.");
                                    masterRoundsAdded++;
                                    foundTileset = true;
                                }
                            }
                            if (!foundTileset)
                            {
                                //If the code is unable to find a valid id in the form list, spit out this error
                                Debug.LogError($"ChamberGunAPI ({modName}): Mod {modname} failed to add viable Master Round with ID {masteryObj.PickupObjectId} because the target floor id ({tilesetID}) does not exist in the custom forme list.");
                            }
                        }
                    }
                    if (!foundCompOnThisID)
                    {
                        //Shit out an error if we didn't find the component
                        Debug.LogError($"ChamberGunAPI ({modName}): A mod attempted to add item ID {id} as a custom mastery, but the item at that ID does not have a data component?");
                    }
                }
            }

            Debug.Log($"Mod '{modName}' correctly completed postStart, adding {newFormesAdded} new formes and {masterRoundsAdded} new valid master rounds.");
            yield break;
        }
    }
    public class AdvancedChamberGunController : MonoBehaviour
    {
        public AdvancedChamberGunController()
        {
            hyperDebugMode = false;
        }
        public string primeHandlerModName;
        public bool hyperDebugMode;
        private Gun gun;
        private int currentTileset;
        private void Awake()
        {
            if (string.IsNullOrEmpty(primeHandlerModName)) Debug.LogError("ChamberGunAPI: The modname of the mod that replaced the Chamber Gun component is null?");
            currentTileset = 2; //Keep Tileset ID
            gun = base.GetComponent<Gun>();
            gun.OnReloadPressed += this.HandleReloadPressed;
        }
        private GlobalDungeonData.ValidTilesets GetFloorTileset()
        {
            if (GameManager.Instance.IsLoadingLevel || !GameManager.Instance.Dungeon)
            {
                Debug.LogError($"ChamberGunAPI ({primeHandlerModName}): Tried to get the tileset on a dungeon that was still loading, or null, so the Keep was chosen as a fallback.");
                return GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
            if (GameManager.Instance.Dungeon.tileIndices == null)
            {
                Debug.LogError($"ChamberGunAPI ({primeHandlerModName}): Tried to get the tileset on a dungeon with NULL tile indeces (like a past), so the Keep was chosen as a fallback.");
                return GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
            return GameManager.Instance.Dungeon.tileIndices.tilesetId;
        } //Returns the Tileset of the current floor
        private bool PlayerHasValidMasterRoundForTileset(PlayerController player, int t)
        {
            foreach (ChamberGunData data in floorFormeDatas)
            {
                if (data.floorTilesetID == t && data.viableMasterRounds != null && data.viableMasterRounds.Count() > 0)
                {
                    foreach (int id in data.viableMasterRounds)
                    {
                        if (player.HasPickupID(id)) return true;
                    }
                }
            }
            return false;
        }
        private bool IsValidTileset(GlobalDungeonData.ValidTilesets t)
        {
            return IsValidTileset((int)t);
        }
        private bool IsValidTileset(int t)
        {
            if (t == (int)GetFloorTileset()) return true;
            if (gun.CurrentOwner && gun.CurrentOwner is PlayerController)
            {
                PlayerController playerController = gun.CurrentOwner as PlayerController;
                foreach (ChamberGunData data in floorFormeDatas)
                {
                    if (data.correspondingFormeID != -1 && data.floorTilesetID == t && PlayerHasValidMasterRoundForTileset(playerController, t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void ChangeToTileset(int t)
        {
            if (hyperDebugMode) ETGModConsole.Log($"Attempting to Change to Tileset {t}. FloorFormeDatas is at a count of {floorFormeDatas.Count}.");

            int targetID = -1;
            foreach (ChamberGunData data in floorFormeDatas)
            {
                if (hyperDebugMode) ETGModConsole.Log($"Checking ChamberGunData with tileset id {data.floorTilesetID}.");
                if (data.floorTilesetID == t)
                {
                    if (hyperDebugMode) ETGModConsole.Log($"ChamberGunData with tileset id {data.floorTilesetID} MATCHES {t}, and targetID has been set to it.");
                    targetID = data.correspondingFormeID;
                    break;
                }
                else
                {
                    if (hyperDebugMode) ETGModConsole.Log($"ChamberGunData with tileset id {data.floorTilesetID} does NOT match with {t}.");
                }
            }
            if (targetID != -1)
            {
                ChangeForme(targetID); currentTileset = t;
            }
            else Debug.LogWarning($"ChamberGunAPI ({primeHandlerModName}): Attempted to change form to a tileset that wasn't valid ({t}).");
        }
        private void ChangeForme(int targetID)
        {
            Gun targetGun = PickupObjectDatabase.GetById(targetID) as Gun;
            if (targetGun == null) Debug.LogError($"ChamberGunAPI ({primeHandlerModName}): Attempted to change form to an id that is either null or not a gun! ({targetID}).");

            if (hyperDebugMode) ETGModConsole.Log($"Changing to gun id {targetID}, with a gunhandedness of: {targetGun.gunHandedness}.");

            gun.TransformToTargetGun(targetGun);
            gun.gunHandedness = targetGun.gunHandedness;
        }
        private void Update()
        {
            if (Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
            {
                return;
            }
            if (gun && (!gun.CurrentOwner || !this.IsValidTileset(currentTileset)))
            {
                GlobalDungeonData.ValidTilesets validTilesets = this.GetFloorTileset();
                if (!gun.CurrentOwner) validTilesets = GlobalDungeonData.ValidTilesets.CASTLEGEON;
                if (currentTileset != (int)validTilesets) this.ChangeToTileset((int)validTilesets);
            }
            this.JustActiveReloaded = false;
        }
        private List<ChamberGunData> ReOrderList(List<ChamberGunData> extantList)
        {
            List<ChamberGunData> tempDatas = new List<ChamberGunData>();
            tempDatas.AddRange(extantList);

            List<ChamberGunData> orderedDatas = new List<ChamberGunData>();
            foreach (ChamberGunData data in tempDatas)
            {
                if (orderedDatas.Count == 0) { orderedDatas.Add(data); }
                else
                {
                    bool placeFound = false;
                    for (int i = 0; i < orderedDatas.Count(); i++)
                    {
                        ChamberGunData orderedItem = orderedDatas[i];
                        if (data.indexValue <= orderedItem.indexValue)
                        {
                            orderedDatas.Insert(i, data);
                            placeFound = true;
                            break;
                        }
                    }
                    if (!placeFound) orderedDatas.Add(data);
                }
            }
            return orderedDatas;
        }
        private List<ChamberGunData> RemoveInvalidEntriesFromList(List<ChamberGunData> extantList)
        {
            if (hyperDebugMode) ETGModConsole.Log($"Attempting to remove invalid forme entries from the list of extant data, counting {extantList.Count}.");

            List<ChamberGunData> newDatas = new List<ChamberGunData>();
            foreach (ChamberGunData data in extantList)
            {
                if (hyperDebugMode) ETGModConsole.Log($"Checking tileset id {data.floorTilesetID}...");
                if (IsValidTileset(data.floorTilesetID))
                {
                    if (hyperDebugMode) ETGModConsole.Log("Tileset was Valid!");
                    newDatas.Add(data);
                }
            }
            return newDatas;
        }
        private int FetchNextValidTilesetID(int currentTilesetID)
        {
            if (hyperDebugMode) ETGModConsole.Log("Fetching next forme id");
            List<ChamberGunData> rawData = RemoveInvalidEntriesFromList(floorFormeDatas);
            if (hyperDebugMode) ETGModConsole.Log($"RawData count: {rawData.Count}");

            List<ChamberGunData> validSortedData = ReOrderList(rawData);
            if (hyperDebugMode) ETGModConsole.Log($"Valid Sorted Data count: {validSortedData.Count}");

            //Determines what form the gun is currently in by iterating through all the valid forms and seeing which ones line up
            int selectedIteration = -1;
            for (int i = 0; i < validSortedData.Count(); i++)
            {
                if (validSortedData[i].floorTilesetID == currentTilesetID)
                {
                    selectedIteration = i;
                    if (hyperDebugMode) ETGModConsole.Log($"Gun has been determined to be in position {i} of the heirarchy with form {currentTilesetID}.");
                }

            }
            //If the gun is actually in a form on the list, proceed
            if (selectedIteration > -1)
            {
                if (selectedIteration < (validSortedData.Count - 1))
                {
                    //If the current positioning on the list is less than the sum of all positions (-1 to compensate for the Count/index inconsistency), then we can
                    //   proceed to the next position, if it's not less than, meaning it's either at the highest value or above, we fallback to the first gun.
                    return validSortedData[selectedIteration + 1].floorTilesetID;
                }
                else return validSortedData[0].floorTilesetID; //Fallback to first gun in the heirarchy
            }
            else return validSortedData[0].floorTilesetID; //Return position 0 on the form list as a fallback if the gun's not in a valid position
        }
        private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
        {
            if (this.JustActiveReloaded)
            {
                return;
            }
            if (manual && !sourceGun.IsReloading)
            {
                int nextValidTilesetID = FetchNextValidTilesetID((int)currentTileset);
                if ((int)currentTileset != nextValidTilesetID)
                {
                    this.ChangeToTileset(nextValidTilesetID);
                }
            }
        }
        public void BraveOnLevelWasLoaded()
        {
            if (this.RefillsOnFloorChange && gun && gun.CurrentOwner)
            {
                gun.StartCoroutine(this.DelayedRegainAmmo());
            }
        }
        private IEnumerator DelayedRegainAmmo()
        {
            yield return null;
            while (Dungeon.IsGenerating)
            {
                yield return null;
            }
            if (this.RefillsOnFloorChange && gun && gun.CurrentOwner)
            {
                gun.GainAmmo(gun.AdjustedMaxAmmo);
            }
            yield break;
        }

        public bool RefillsOnFloorChange = true;
        public bool JustActiveReloaded;

        public static List<ChamberGunData> floorFormeDatas;
        public class ChamberGunData
        {
            public string modName = "Unset";
            public int floorTilesetID = -1;
            public float indexValue = 0;
            public int correspondingFormeID = -1;
            public List<int> viableMasterRounds;
        }
    }
}


*/