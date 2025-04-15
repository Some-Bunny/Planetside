using Alexandria.ItemAPI;
using Alexandria.PrefabAPI;
using GungeonAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace Planetside.DungeonPlaceables
{
    public class Gooper : BraveBehaviour
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("GoopPlumber");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("fwoomp"));
            tk2d.sprite.usesOverrideMaterial = true;
            var body = obj.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.PlayerBlocker);
            obj.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.BeamBlocker);
            obj.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.BulletBlocker);
            obj.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.EnemyBlocker);

            var body_1 = obj.CreateFastBody(new IntVector2(16, 64), new IntVector2(0, 16), CollisionLayer.Projectile, true, true);
            var body_2 = obj.CreateFastBody(new IntVector2(16, 64), new IntVector2(0, -64), CollisionLayer.Projectile, true, true);
            var body_3 = obj.CreateFastBody(new IntVector2(64, 16), new IntVector2(-64, 0), CollisionLayer.Projectile, true, true);
            var body_4 = obj.CreateFastBody(new IntVector2(64, 16), new IntVector2(16, 0), CollisionLayer.Projectile, true, true);

            var goopie = obj.AddComponent<Gooper>();
            goopie.sprite = tk2d;
            goopie.bodies = new SpeculativeRigidbody[] { body_1, body_2, body_3, body_4 };
            StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteDefinition("fwoomp").AddOffset(new Vector2(-0.0625f, -0.0625f));

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_Gooper", obj);
            StaticReferences.StoredRoomObjects.Add("PSOG_Gooper", obj.gameObject);
        }

        public void Start()
        {
            foreach (var entry in bodies)
            {
                entry.OnEnterTrigger += OnEnterTrigger;
            }
        }
        private void OnEnterTrigger(SpeculativeRigidbody obj, SpeculativeRigidbody source, CollisionData collisionData)
        {
            if (obj.gameActor && obj.gameActor is PlayerController && Active == true)
            {
                Active = false;
                this.Invoke("In", 5);
                var v = this.transform.position + new Vector3(0.5f, 0.5f);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.PoisonDef).TimedAddGoopLine(v, v + new Vector3(0, 5),0.75f , 0.5f);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.PoisonDef).TimedAddGoopLine(v, v + new Vector3(0, -5), 0.75f, 0.5f);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.PoisonDef).TimedAddGoopLine(v, v + new Vector3(5, 0), 0.75f, 0.5f);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.PoisonDef).TimedAddGoopLine(v, v + new Vector3(-5, 0), 0.75f, 0.5f);
            }
        }

        private void In()
        { Active = true; }
        private bool Active = true;
        public SpeculativeRigidbody[] bodies;
         
    }
}
