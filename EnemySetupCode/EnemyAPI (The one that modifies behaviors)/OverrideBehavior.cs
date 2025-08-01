﻿using Brave.BulletScript;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    public abstract class OverrideBehavior
    {
        public abstract string OverrideAIActorGUID { get; }

        public AIActor actor;
        public BehaviorSpeculator behaviorSpec;
        public AIBulletBank bulletBank;
        public HealthHaver healthHaver;



        public void SetupOB(AIActor actor)
        {
            this.actor = actor;
            this.behaviorSpec = actor.behaviorSpeculator;
            this.bulletBank = actor.bulletBank;
            this.healthHaver = actor.healthHaver;
        }

        public virtual bool ShouldOverride()
        {
            if (SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER) == false) { return false; }
            if (ContainmentBreachController.CurrentState == ContainmentBreachController.States.ALLOWED) { return true; }
            return false;
            //return true;
        }

        public abstract void DoOverride();

        public void SetupBehavior(AttackBehaviorBase behavior)
        {
            behavior.Init(behaviorSpec.gameObject, actor, behaviorSpec.aiShooter);
            behaviorSpec.AttackBehaviors.Add(behavior);
            behavior.Start();
        }

        public void SetupBehaviorABG(AttackBehaviorBase behavior, string name = "N/A", int probability = 1)
        {
            behavior.Init(behaviorSpec.gameObject, actor, behaviorSpec.aiShooter);
            behaviorSpec.AttackBehaviorGroup.AttackBehaviors.Add(new AttackBehaviorGroup.AttackGroupItem
            {
                Behavior = behavior,
                NickName = name,
                Probability = probability
            });
            behavior.Start();
        }
    }

    public class CustomBulletScriptSelector : BulletScriptSelector
    {
        public Type bulletType;

        public CustomBulletScriptSelector(Type _bulletType)
        {
            bulletType = _bulletType;
            this.scriptTypeName = bulletType.AssemblyQualifiedName;
        }

        public new Bullet CreateInstance()
        {
            if (bulletType == null)
            {
                ETGModConsole.Log("Unknown type! " + this.scriptTypeName);
                return null;
            }
            return (Bullet)Activator.CreateInstance(bulletType);
        }

        public new bool IsNull
        {
            get
            {
                return string.IsNullOrEmpty(this.scriptTypeName) || this.scriptTypeName == "null";
            }
        }
        
        public new BulletScriptSelector Clone()
        {
            return new CustomBulletScriptSelector(bulletType);
        }
    }
}
