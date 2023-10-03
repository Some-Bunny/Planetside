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
using GungeonAPI;

namespace Planetside
{
    public class TrespassPortalReturn
    {       
        public static void Init()
        {
            var partObj = PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal");
            GameObject portal =  FakePrefab.Clone(partObj);
            MeshRenderer rend = portal.GetComponent<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            portal.name = "ReturnPortal";
            portal.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            TrespassPortalReturnController romas = portal.AddComponent<TrespassPortalReturnController>();
            StaticReferences.StoredRoomObjects.Add("returnPortal_hell", portal);
        }
    }

    public class TrespassPortalReturnController : BraveBehaviour, IPlayerInteractable
    {
        public TrespassPortalReturnController()
        {
        }

        public void Start()
        {
            base.gameObject.SetActive(true);
            m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(new IntVector2((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.y));
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.zero, Vector3.one * 6f, 0.2f));
            base.Invoke("ReregisterInteractable", 0f);
            base.transform.position += new Vector3(4, 4);
        }

      

        public RoomHandler m_room;
        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.gameObject) return float.MaxValue;
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.gameObject.transform.position - new Vector3(1.25f, 1.25f), new Vector3(2.5f, 2.5f));
            float result = Vector2.Distance(point, v) / 1.5f;
            return result;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            //A method that runs whenever the player enters the interaction range of the interactable. This is what outlines it in white to show that it can be interacted with
            GameManager.Instance.StartCoroutine(LerpShaderValue(0.05f, 0.1f, 0.33f, "_OutlineWidth"));
            GameManager.Instance.StartCoroutine(LerpShaderValue(10f, 45f, 0.33f, "_OutlinePower"));
        }



        public void OnExitRange(PlayerController interactor)
        {
            base.StartCoroutine(LerpShaderValue(0.1f, 0.05f, 0.33f, "_OutlineWidth"));
            base.StartCoroutine(LerpShaderValue(45f, 10f, 0.33f, "_OutlinePower"));
        }
        public void Interact(PlayerController interactor)
        {
            base.Invoke("DeregisterInteractable", 0f);
            AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", interactor.gameObject);
            GameManager.Instance.StartCoroutine(TransportToRoom());
        }


        private IEnumerator TransportToRoom()
        {
            Pixelator.Instance.FadeToColor(0.7f, Color.black, false, 0f);
            float elaWait = 0f;
            float duraWait = 0.7f;
            GameUIRoot.Instance.HideCoreUI("Trespassing");
            GameUIRoot.Instance.ForceHideGunPanel = true;
            GameUIRoot.Instance.ForceHideItemPanel = true;
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
            GameManager.DoMidgameSave(GlobalDungeonData.ValidTilesets.HELLGEON);
            GameManager.Instance.DelayedLoadNextLevel(0.5f);
            GameManager.Instance.LoadCustomLevel("tt_bullethell");


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

        public GameObject PortalToDestroy;
        public Vector2 ReturnPosition;
        private IEnumerator LerpToSize(Vector3 prevSize, Vector3 afterSize, float duration)
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
            yield break;
        }
    }
}
