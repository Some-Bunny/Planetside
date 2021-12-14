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

namespace Planetside
{
    public class SelfDestructComponent : MonoBehaviour
    {
        public SelfDestructComponent()
        {

            this.maxDuration = 10;
            this.PoofVFX = true;
        }
        public void Start()
        {
            if (PoofVFX == true)
            {
                LootEngine.DoDefaultPurplePoof(base.gameObject.transform.position, false);
            }
            //base.StartCoroutine(this.HandleTimedDestroy());
        }
        public void Update()
        {
            this.elapsed += BraveTime.DeltaTime;
            if (elapsed >= maxDuration)
            {
                if (PoofVFX == true)
                {
                    LootEngine.DoDefaultPurplePoof(base.gameObject.transform.position, false);
                }
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
        private float elapsed;

        private IEnumerator HandleTimedDestroy()
        {
            yield return new WaitForSeconds(this.maxDuration);
           
            yield break;
        }
        public bool PoofVFX;
        public float maxDuration = 10f;
    }
}
