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
using UnityEngine.Playables;

namespace Planetside
{
    public class TrespassPortalBack
    {       
        public static void Init()
        {
            var partObj = PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal");
            GameObject portal =  FakePrefab.Clone(partObj);
            MeshRenderer rend = portal.GetComponent<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            portal.name = "ReturnPortal";
            portal.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            TrespassReturnPortalController romas = portal.AddComponent<TrespassReturnPortalController>();
            StaticReferences.StoredRoomObjects.Add("returnPortal", portal);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_returnPortal", portal);

        }
    }

    public class TrespassReturnPortalController : BraveBehaviour, IPlayerInteractable
    {
        public TrespassReturnPortalController()
        {
            this.WillForceTriggerReinforcements = false;
        }

        public bool WillForceTriggerReinforcements;
        public void Start()
        {
            base.gameObject.SetActive(true);
            m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(new IntVector2((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.y));
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.zero, Vector3.one * 2f, 0.2f));
            base.Invoke("ReregisterInteractable", 0f);
            Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 1, 0.2f, 3, 0.4f);
            base.transform.position += new Vector3(1, 1);
        }

        public void DoCheckCloseIfEnemies()
        {
            if (m_room != null)
            {
                if (m_room.GetEnemiesInReinforcementLayer(0) != 0 || m_room.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) == true)
                {
                    if (WillForceTriggerReinforcements == true) { this.m_room.TriggerNextReinforcementLayer(); }
                    base.Invoke("DeregisterInteractable", 0f);
                    base.StartCoroutine(LerpShaderValue(0.1f, 0.00f, 2f, "_OutlineWidth"));
                    base.StartCoroutine(LerpShaderValue(45f, 0f, 2f, "_OutlinePower"));
                    GameManager.Instance.StartCoroutine(LerpToSize(Vector3.one * 2, Vector3.zero, 2f));
                    m_room.OnEnemiesCleared += OnEnemiesCleared;
                }
            }
        }
        public void OnEnemiesCleared()
        {
            base.Invoke("ReregisterInteractable", 1f);
            base.StartCoroutine(LerpShaderValue(0f, 0.1f, 1f, "_OutlineWidth"));
            base.StartCoroutine(LerpShaderValue(0f,45f, 1f, "_OutlinePower"));
            GameManager.Instance.StartCoroutine(LerpToSize(Vector3.zero, Vector3.one * 2, 1f));

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
            SuperReaperController.PreventShooting = true;
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].healthHaver.TriggerInvulnerabilityPeriod(2.5f);
                    GameManager.Instance.AllPlayers[j].SetInputOverride("Trespassing");
                }
            }
            while (elaWait < duraWait)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Pixelator.Instance.FadeToColor(0.5f, Color.black, true, 0f);

            AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
            Minimap.Instance.TemporarilyPreventMinimap = false;
            this.ResetMusic(GameManager.Instance.Dungeon);
            AkSoundEngine.PostEvent(this.m_cachedMusicEventCore, GameManager.Instance.gameObject);
            if (PortalToDestroy != null) { PortalToDestroy.GetComponent<TrespassPortalController>().Invoke("YoureFreakingDead", 0); }

            if (ReturnPosition == null | ReturnPosition == new Vector2(0,0)) { Debug.Log("FAILSAFE TRIGGERED FOR RETURN PORTAL"); ReturnPosition = GameManager.Instance.Dungeon.data.Exit.area.Center - new Vector2(0 ,-2); }
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].WarpToPoint(ReturnPosition - new Vector2(1f, 1f), false, false);
                    GameManager.Instance.AllPlayers[j].ClearInputOverride("Trespassing");

                }
            }
            GameUIRoot.Instance.ForceHideGunPanel = false;
            GameUIRoot.Instance.ForceHideItemPanel = false;
            GameUIRoot.Instance.ShowCoreUI("Trespassing");
            Minimap.Instance.TemporarilyPreventMinimap = false;
            SuperReaperController.PreventShooting = false;

            yield break;
        }

        public void ResetMusic(Dungeon d)
        {
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            if (flag)
            {
                this.m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                this.m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
        }
        private string m_cachedMusicEventCore;


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
