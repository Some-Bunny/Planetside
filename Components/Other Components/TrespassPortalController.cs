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
using SaveAPI;

namespace Planetside
{
    public class TrespassPortalController : BraveBehaviour, IPlayerInteractable
    {
        public void Start()
        {
            AkSoundEngine.PostEvent("Play_PortalOpen", base.gameObject);
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.zero, Vector3.one * 2f, 1f));
            base.Invoke("ReregisterInteractable", 1f);
            Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 1.5f, 0.25f, 50, 5f);
            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
            partObj.transform.position = gameObject.transform.position;
            //partObj.transform.parent = gameObject.transform;
            partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            partObj.transform.localScale *= 6f;
            ParticleSystem particleSystem = partObj.GetComponent<ParticleSystem>();
            Destroy(partObj, 3.4f);
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
            //partObj.transform.parent = gameObject.transform;
            partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            Destroy(partObj, 3.4f);
            partObj.transform.localScale *= 5f;
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
            base.Invoke("DeregisterInteractable", 0f);
            WeightedRoom newRoom =  AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER) ? TrespassStone.trespassDeeperTable.SelectByWeight() : TrespassStone.trespassTable.SelectByWeight();
            var floor = DungeonDatabase.GetOrLoadByName("Base_Abyss");
            int num = 5;
            RoomHandler room;
            do
            {
                room = DungeonGenToolbox.AddCustomRuntimeRoomWithTileSet(floor, newRoom.room, false, false, false, null, DungeonData.LightGenerationStyle.STANDARD, false, false, 1, false);
                num--;
            }
            while (num > 0 && room == null);
            bool flag = num == 0;
            if (flag)
            {
                AkSoundEngine.PostEvent("Play_PortalOpen", base.gameObject);
                GameManager.Instance.StartCoroutine(LerpToSize(Vector3.one * 2, Vector3.zero, 0.33f));
                var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
                partObj.transform.position = gameObject.transform.position;
                partObj.transform.localScale *= 7f;
                partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

                Destroy(partObj, 3.4f);
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(LostVoidPotential.LostVoidPotentialID).gameObject, gameObject.transform.position, Vector2.zero, 0).GetComponent<DebrisObject>();
                Destroy(gameObject);
            }
            else
            {

                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, true);
                AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", interactor.gameObject);
                room.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.SHRINE_WAVE_C, false);
                room.CompletelyPreventLeaving = true;
                GameManager.Instance.StartCoroutine(this.TransportToRoom(interactor, room));
            }
            floor = null;
        }

        private IEnumerator TransportToRoom(PlayerController player, RoomHandler room)
        {
            GameUIRoot.Instance.ForceHideGunPanel = true;
            GameUIRoot.Instance.ForceHideItemPanel = true;
            Pixelator.Instance.FadeToColor(1f, Color.black, false, 0f);
            float elaWait = 0f;
            float duraWait = 1f;
            GameUIRoot.Instance.HideCoreUI("Trespassing");
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
            List<IPlayerInteractable> interactables = PlanetsideReflectionHelper.ReflectGetField<List<IPlayerInteractable>>(typeof(RoomHandler), "interactableObjects", room);
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
                        controller.Invoke("DoCheckCloseIfEnemies", 0.5f);
                        controller.WillForceTriggerReinforcements = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER);
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
            GameUIRoot.Instance.ForceHideGunPanel = false;
            GameUIRoot.Instance.ForceHideItemPanel = false;
            GameUIRoot.Instance.ShowCoreUI("Trespassing");
            //Pixelator.Instance.FadeToColor(1f, Color.black, false, 0f);

            Minimap.Instance.TemporarilyPreventMinimap = true;

            yield break;
        }


        private void ReregisterInteractable()
        {
            if (!m_room.IsRegistered(this))
            {
                this.m_room.RegisterInteractable(this);
            }
        }
        private void DeregisterInteractable()
        {
            if (m_room.IsRegistered(this))
            {
                this.m_room.DeregisterInteractable(this);
            }
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

        protected override void OnDestroy()
        {
            if (m_room.IsRegistered(this))
            {
                this.m_room.DeregisterInteractable(this);
            }
            base.OnDestroy();
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
