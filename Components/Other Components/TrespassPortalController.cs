using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

namespace Planetside
{
    public class TrespassPortalController : BraveBehaviour, IPlayerInteractable
    {
        public void Start()
        {
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.zero, Vector3.one * 2f, 1f));
            base.Invoke("ReregisterInteractable", 1f);
            Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 1, 0.2f, 3, 0.4f);
            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
            partObj.transform.position = gameObject.transform.position;
            partObj.transform.parent = gameObject.transform;
            partObj.transform.localScale *= 0.33f;
            Destroy(partObj, 1);
        }




        public RoomHandler m_room;
        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.gameObject) return float.MaxValue;
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.gameObject.transform.position - new Vector3(1.25f,1.25f), new Vector3(2.5f, 2.5f));
            float result = Vector2.Distance(point, v) / 1.5f;
            return result;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            //A method that runs whenever the player enters the interaction range of the interactable. This is what outlines it in white to show that it can be interacted with
            GameManager.Instance.StartCoroutine(LerpShaderValue(0.05f, 0.1f, 0.33f, "_OutlineWidth"));
            GameManager.Instance.StartCoroutine(LerpShaderValue(10f, 45f, 0.33f, "_OutlinePower"));
        }

        public void YoureFreakingDead()
        {
            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
            partObj.transform.position = gameObject.transform.position;
            partObj.transform.parent = gameObject.transform;
            Destroy(partObj, 1);
            base.Invoke("DeregisterInteractable", 0f);
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.one * 2, Vector3.zero, 0.9f));
            base.StartCoroutine(LerpShaderValue(0.1f, 0.00f, 0.8f, "_OutlineWidth"));
            base.StartCoroutine(LerpShaderValue(45f, 6f, 0.3f, "_OutlinePower"));
            Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 1, 0.2f, 3, 0.4f);
            Destroy(gameObject, 1);
        }


        public void OnExitRange(PlayerController interactor)
        {
            base.StartCoroutine(LerpShaderValue(0.1f, 0.05f, 0.33f, "_OutlineWidth"));
            base.StartCoroutine(LerpShaderValue(45f, 10f, 0.33f, "_OutlinePower"));
        }
        public void Interact(PlayerController interactor)
        {
            AkSoundEngine.PostEvent("Play_ENM_critter_poof_01", interactor.gameObject);
            base.Invoke("DeregisterInteractable", 0f);
            Dungeon d = GameManager.Instance.Dungeon;
            WeightedRoom newRoom = TrespassStone.trespassTable.SelectByWeight();
            Dungeon floor = DungeonDatabase.GetOrLoadByName("Base_Forge");
            RoomHandler room = DungeonGenToolbox.AddCustomRuntimeRoomWithTileSet(floor, newRoom.room, false, false, false, null, DungeonData.LightGenerationStyle.STANDARD, false, true, 0);
            floor = null;
            if (room != null)
            {
                GameManager.Instance.StartCoroutine(this.TransportToRoom(interactor, room));
            }
            else
            {
               
                //room.RegisterInteractable
                GameManager.Instance.StartCoroutine(LerpToSize(Vector3.one * 2, Vector3.zero, 0.33f));
                var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
                partObj.transform.position = gameObject.transform.position;
                partObj.transform.parent = gameObject.transform;
                Destroy(partObj, 1);
                DebrisObject debrisSpawned = LootEngine.SpawnItem(PickupObjectDatabase.GetById(LostVoidPotential.LostVoidPotentialID).gameObject, gameObject.transform.position, Vector2.zero, 0).GetComponent<DebrisObject>();
                Destroy(gameObject);
            }
        }

        private IEnumerator TransportToRoom(PlayerController player, RoomHandler room)
        {
            Pixelator.Instance.FadeToColor(1f, Color.black, false, 0f);
            float elaWait = 0f;
            float duraWait = 1f;
            //Pixelator.Instance.FadeToColor(1f, Color.black, true, 1f);
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].SetInputOverride("Trespassing");
                }
            }
            while (elaWait < duraWait)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Pixelator.Instance.FadeToColor(1f, Color.black, true, 0f);
            Vector2 newPlayerPosition = room.area.Center;
            List<IPlayerInteractable> interactables = ReflectionHelper.ReflectGetField<List<IPlayerInteractable>>(typeof(RoomHandler), "interactableObjects", room);
            foreach (BraveBehaviour obj in interactables)
            {
                TrespassReturnPortalController controller = obj.gameObject.GetComponent<TrespassReturnPortalController>();
                if (controller != null)
                {
                    if (controller.m_room == room)
                    {
                        newPlayerPosition = obj.transform.position;
                        controller.ReturnPosition = base.gameObject.transform.position;
                        controller.PortalToDestroy = base.gameObject;
                    }
                }
            }
               
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].WarpToPoint(newPlayerPosition, false, false);
                    GameManager.Instance.AllPlayers[j].ClearInputOverride("Trespassing");

                }
            }
            //Pixelator.Instance.FadeToColor(1f, Color.black, false, 0f);

            Minimap.Instance.TemporarilyPreventMinimap = true;

            yield break;
        }


        private void ReregisterInteractable()
        {
            this.m_room.RegisterInteractable(this);
        }
        private void DeregisterInteractable()
        {
            this.m_room.DeregisterInteractable(this);
        }
        public float GetOverrideMaxDistance()
        {
            return 1f;
        }
        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private IEnumerator LerpShaderValue(float prevSize, float afterSize, float duration, string KeyWord)
        {
            float elaWait = 0f;
            float duraWait = duration;
            while (elaWait < duraWait)
            {
                elaWait += BraveTime.DeltaTime;
                float t = elaWait / duraWait;
                if (base.gameObject == null) { yield break; }
                if (base.gameObject != null)
                {
                    base.gameObject.GetComponent<MeshRenderer>().material.SetFloat(KeyWord, Mathf.Lerp(prevSize, afterSize, t));                
                }
                yield return null;
            }
            yield break;
        }

        private IEnumerator LerpToSize(Vector3 prevSize, Vector3 afterSize, float duration)
        {
            if (base.gameObject != null)
            {
                base.gameObject.transform.localScale = prevSize;
                float elaWait = 0f;
                float duraWait = duration;
                while (elaWait < duraWait)
                {
                    elaWait += BraveTime.DeltaTime;
                    float t = elaWait / duraWait;
                    if (base.gameObject == null) { yield break; }
                    if (base.gameObject != null)
                    {
                        base.gameObject.transform.localScale = Vector3.Lerp(prevSize, afterSize, t);
                    }
                    yield return null;
                }
            }
            
            yield break;
        }

    }
}
