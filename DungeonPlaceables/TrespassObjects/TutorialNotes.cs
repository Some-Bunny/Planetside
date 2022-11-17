using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using System.Collections;
using SaveAPI;

namespace Planetside
{

    public class NoteTriggerEnemiesController : MonoBehaviour
    {
        public MajorBreakable self;
        public RoomHandler parentRoom;

        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            if (self != null)
            {
                IntVector2 intVec2 = new IntVector2((int)self.transform.position.x, (int)self.transform.position.y);
                parentRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(intVec2);
            }
        }
        public void OnDestroy()
        {
            if (parentRoom != null)
            {
                this.parentRoom.TriggerNextReinforcementLayer();
                List<IPlayerInteractable> interactables = PlanetsideReflectionHelper.ReflectGetField<List<IPlayerInteractable>>(typeof(RoomHandler), "interactableObjects", parentRoom);
                foreach (BraveBehaviour obj in interactables)
                {
                    TrespassReturnPortalController controller = obj.gameObject.GetComponent<TrespassReturnPortalController>();
                    if (controller != null)
                    {
                        if (controller.m_room == parentRoom)
                        {
                            controller.Invoke("DoCheckCloseIfEnemies", 0f);
                        }
                    }
                }
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER, true);
            }
        }
    }
    public class TutorialNotes
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TutorialNote/";
            string[] idlePaths = new string[]
            {
                defaultPath+"tatterednote.png",
            };

            ETGMod.Databases.Strings.Core.Set("#TUTORIAL_NODODGE_NOTE_1", 
                "I'm writing these in case someone stumbles upon by remains, either by chance or searching, so they don't suffer the name fate I did." +
                "\n\nThrough my many run-throughs of the Gungeon, I stumbled onto these mysterious blue portals like the one you most likely found.");
            
            ETGMod.Databases.Strings.Core.Set("#TUTORIAL_NODODGE_NOTE_2", 
                "Out of curiosity, I always entered to see what was on the other side." +
                "\n\nBut on this occasion, I seemed to have found some residents of this plane who clearly didn't take kindly to my intrusion." +
                "\n\nI tried to fend them off, yet their bullets are unlike anything I've ever seen. I don't think I can handle another encounter with *them*.");

            ETGMod.Databases.Strings.Core.Set("#TUTORIAL_NODODGE_NOTE_3", 
                "Abandon all of Ser Manuels teachings, you who entered here. It is of no use here, whatever this place may be." +
                "\n\nMay Kaliber be with you.");


            MajorBreakable note1 = BreakableAPIToolbox.GenerateMajorBreakable("tutorialNote_1", idlePaths, 1, idlePaths, 1, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            NoteDoer finishedNote1 = BreakableAPIToolbox.GenerateNoteDoer(note1, BreakableAPIToolbox.GenerateTransformObject(note1.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#TUTORIAL_NODODGE_NOTE_1");

            MajorBreakable note2 = BreakableAPIToolbox.GenerateMajorBreakable("tutorialNote_2", idlePaths, 1, idlePaths, 1, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            NoteDoer finishedNote2 = BreakableAPIToolbox.GenerateNoteDoer(note2, BreakableAPIToolbox.GenerateTransformObject(note2.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#TUTORIAL_NODODGE_NOTE_2");

            MajorBreakable note3 = BreakableAPIToolbox.GenerateMajorBreakable("tutorialNote_3", idlePaths, 1, idlePaths, 1, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            NoteDoer finishedNote3 = BreakableAPIToolbox.GenerateNoteDoer(note3, BreakableAPIToolbox.GenerateTransformObject(note3.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#TUTORIAL_NODODGE_NOTE_3", true);
            
            
            finishedNote3.gameObject.AddComponent<NoteTriggerEnemiesController>();


            ETGMod.Databases.Strings.Core.Set("#PRERELEASE_NOTE_1",
                "There will be something cool here during the full release, I promise!");

            MajorBreakable note4 = BreakableAPIToolbox.GenerateMajorBreakable("prisonerNote_1", idlePaths, 1, idlePaths, 1, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            NoteDoer PrisonerNote1 = BreakableAPIToolbox.GenerateNoteDoer(note4, BreakableAPIToolbox.GenerateTransformObject(note4.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#PRERELEASE_NOTE_1");


            StaticReferences.StoredRoomObjects.Add("note1", finishedNote1.gameObject);
            StaticReferences.StoredRoomObjects.Add("note2", finishedNote2.gameObject);
            StaticReferences.StoredRoomObjects.Add("note3", finishedNote3.gameObject);
            StaticReferences.StoredRoomObjects.Add("prisonerNote", PrisonerNote1.gameObject);


        }
    }
}
