using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections;
using Brave.BulletScript;
using EnemyBulletBuilder;

namespace Planetside
{
    class CustomEnemyBulletsInitialiser
    {
        public static void Init()
        {
            BulletBuilder.CreateBulletPrefab("Planetside/Resources/EnemyBulletSprites/bigspore.png", "BigAssSpore", false, true);
            BulletBuilder.CreateBulletPrefab("Planetside/Resources/EnemyBulletSprites/hammberbullet.png", "HammerOfTheMoon", true,true);
            //Ophanaim Bullets
            BulletBuilder.CreateBulletPrefab("Planetside/Resources/EnemyBulletSprites/eyeneedle.png", "EyeNeedle", true, true);
            BulletBuilder.CreateBulletPrefab("Planetside/Resources/EnemyBulletSprites/eyeball.png", "BigEye", false, true);
        }
    }
}
