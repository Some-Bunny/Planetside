using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside.Components.Other_Components
{
    public class CustomDragunBoulderController : BraveBehaviour
    {
        public static CustomDragunBoulderController customDragunBoulderController;
        public static CustomDragunBoulderController Instantiate(Vector3 vector3, float Duration, float Scale = 1)
        {
            var _ = Instantiate(customDragunBoulderController.gameObject, vector3, Quaternion.identity).GetComponent<CustomDragunBoulderController>();
            _.LifeTime = Duration;
            _.UpdateScale(Scale);
            return _;
        }

        public static void Init()
        {
            GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
            foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
            {
                if (item is SkyRocket laser)
                {
                    var obj = FakePrefab.Clone(laser.SpawnObject);
                    DraGunBoulderController b = null;
                    foreach (Component item2 in obj.GetComponentsInChildren(typeof(Component)))
                    {
                        if (item2 is DraGunBoulderController laser2)
                        {
                            b = laser2;
                            break;
                        }
                    }
                    if (b)
                    {
                        var c = obj.AddComponent<CustomDragunBoulderController>();
                        c.CircleSprite = b.CircleSprite;
                        c.specRigidbody = b.specRigidbody;
                        c.CircleSprite.gameObject.layer = Layers.FG_Nonsense;


                        List<PixelCollider> colliders = c.specRigidbody.PixelColliders.ToList();
                        foreach (PixelCollider collider in colliders)
                        {




                            c.DefSizes.Add(collider.ManualOffsetX);// = (int)(collider.ManualOffsetX * 0.5f);
                            c.DefSizes.Add(collider.ManualOffsetY);// = (int)(collider.ManualOffsetY * 0.5f);
                            c.DefSizes.Add(collider.ManualDiameter);// = (int)(collider.ManualDiameter * 0.5f);
                            c.DefSizes.Add(collider.ManualHeight);// = (int)(collider.ManualHeight * 0.5f);
                        }
                        c.specRigidbody.UpdateCollidersOnScale = true;

                        Destroy(b);
                        customDragunBoulderController = c;
                    }
                }
            }
        }

        public List<int> DefSizes = new List<int>();

        public void Start()
        {
            base.specRigidbody.OnEnterTrigger += HandleTriggerEntered;
            base.specRigidbody.OnExitTrigger += this.HandleTriggerExited;
            if (this.CircleSprite)
            {
                tk2dSpriteDefinition currentSpriteDef = this.CircleSprite.GetCurrentSpriteDef();
                Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
                for (int i = 0; i < currentSpriteDef.uvs.Length; i++)
                {
                    vector = Vector2.Min(vector, currentSpriteDef.uvs[i]);
                    vector2 = Vector2.Max(vector2, currentSpriteDef.uvs[i]);
                }
                Vector2 vector3 = (vector + vector2) / 2f;
                this.CircleSprite.renderer.material.SetVector("_WorldCenter", new Vector4(vector3.x, vector3.y, vector3.x - vector.x, vector3.y - vector.y));
            }
            this.m_lifeTime = 0f;
        }



        private IEnumerator IncreaseInSize(float SizeMultiplier = 1, float T = 0.75f)
        {
            var v = Scale;
            float elapsed = 0f;
            while (elapsed < T)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / T);
                if (CircleSprite != null && CircleSprite.gameObject != null)
                {
                    Scale = Mathf.Lerp(v, SizeMultiplier, t);
                    this.transform.localScale = new Vector3(Scale, Scale, Scale);
                    specRigidbody.Reinitialize();
                }
                yield return null;
            }
            __ = null;
            yield break;
        }


        public void UpdateScale(float _Scale, float T = 0)
        {
            if (T <= 0)
            {
                //Scale = _Scale;

                //this.transform.localScale = new Vector3(Scale, Scale, Scale);
                //specRigidbody.Reinitialize();
                Scale = _Scale;// Mathf.Lerp(v, SizeMultiplier, t);
                this.transform.localScale = new Vector3(Scale, Scale, Scale);
                specRigidbody.Reinitialize();
                return;
            }
            if (__ != null)
            {
                this.StopCoroutine(__);
                __ = null;
            }
            __ = this.StartCoroutine(IncreaseInSize(_Scale, T));
        }
        private Coroutine __;

        public void UpdatePosition(Vector3 newPosition)
        {
            this.transform.position = newPosition;
            this.specRigidbody.Reinitialize();
        }


        private void UpdateScaleBodies()
        {
            List<PixelCollider> colliders = this.specRigidbody.PixelColliders.ToList();
            for (int i = 0; i < colliders.Count; i++)
            {
                var collider = colliders[i];
                var _ = i * 4;
                /*
                Debug.Log($"{DefSizes.Count} " +
                    $"| {((float)DefSizes[_]) * Scale} " +
                    $"| {((float)DefSizes[_ + 1]) * Scale} " +
                    $"| {((float)DefSizes[_ + 2]) * Scale} " +
                    $"| {((float)DefSizes[_ + 3]) * Scale}");
                */
                
                //collider.ManualOffsetX = Mathf.RoundToInt(((float)DefSizes[_]) * Scale);
                //collider.ManualOffsetY = Mathf.RoundToInt(((float)DefSizes[_+1]) * Scale);
                //collider.ManualDiameter = Mathf.RoundToInt(((float)DefSizes[_+2]) * Scale);
                //collider.ManualHeight = Mathf.RoundToInt(((float)DefSizes[_ + 3]) * Scale);
                
                collider.Sprite =  CircleSprite; // new Vector2(Scale, Scale);

                collider.Regenerate(this.specRigidbody.transform);
            }
        }

        private void Update()
        {
            this.m_lifeTime += BraveTime.DeltaTime;
            if (this.m_lifeTime >= this.LifeTime)
            {
                this.m_lifeTime = 0f;
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleBreakCR());
            }
            for (int i = 0; i < this.m_cursedPlayers.Count; i++)
            {
                this.DoCurse(this.m_cursedPlayers[i]);
            }
        }

        private IEnumerator HandleBreakCR()
        {
            float elapsed = 0f;
            float duration = 0.3f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                //this.CircleSprite.scale = Vector3.Lerp(new Vector3(Scale, Scale, Scale), Vector3.zero, t);
                this.transform.localScale = Vector3.Lerp(new Vector3(Scale, Scale, Scale), Vector3.zero, t);
                specRigidbody.Reinitialize();
                yield return null;
            }
            UnityEngine.Object.Destroy(base.gameObject);
            yield break;
        }

        private void DoCurse(PlayerController targetPlayer)
        {
            if (targetPlayer.IsGhost)
            {
                return;
            }
            targetPlayer.CurrentStoneGunTimer = Mathf.Max(targetPlayer.CurrentStoneGunTimer, 0.3f);
        }

        private void HandleTriggerExited(SpeculativeRigidbody exitRigidbody, SpeculativeRigidbody sourceSpecRigidbody)
        {
            if (exitRigidbody && exitRigidbody.gameActor && exitRigidbody.gameActor is PlayerController && this.m_cursedPlayers.Contains(exitRigidbody.gameActor as PlayerController))
            {
                this.m_cursedPlayers.Remove(exitRigidbody.gameActor as PlayerController);
            }
        }

        private void HandleTriggerEntered(SpeculativeRigidbody enteredRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
            if (GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Round)) != GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(enteredRigidbody.UnitCenter.ToIntVector2(VectorConversions.Round)))
            {
                return;
            }
            if (enteredRigidbody.gameActor != null && enteredRigidbody.gameActor is PlayerController)
            {
                this.m_cursedPlayers.Add(enteredRigidbody.gameActor as PlayerController);
            }
        }

        public float LifeTime = 1f;

        public float Scale = 1;

        public tk2dSprite CircleSprite;
        private float m_lifeTime;
        private List<PlayerController> m_cursedPlayers = new List<PlayerController>();
    }
}
