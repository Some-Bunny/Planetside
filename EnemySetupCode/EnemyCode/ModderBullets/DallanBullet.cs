using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using JetBrains.Annotations;

namespace Planetside
{
	public class DallanBullet : AIActor
	{

		public static void Init()
		{
			var enm = EnemyToolbox.CreateNewBulletBankerEnemy("dallan_bullet", "Dallan", 16, 16, new List<int> { 243, 244, 245, 246 }, new List<int> { 247, 248, 249, 250, 251, 252 }, null, new SkellScript(), 2f);
            enm.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
            enm.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));


        }

        public static List<DallanArtMaker> arts = new List<DallanArtMaker>()
        {
            new DallanArtMaker(),//square
			new DallanArtMaker() //eye
			{
                Layer_1 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_2 = new DallanArtMaker.DallanArtLayer()
                {
                    P_2 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                },
                Layer_3 = new DallanArtMaker.DallanArtLayer()
                {
                    P_2 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(true, "bigBullet"),
                    P_4 = new Tuple<bool, string>(false, null),
                },
                Layer_4 = new DallanArtMaker.DallanArtLayer()
                {
                    P_2 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                },
                Layer_5 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                }
            },
            new DallanArtMaker()//gun
            {
                Layer_1 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_2 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_2 = new DallanArtMaker.DallanArtLayer()
                {
                    P_3 = new Tuple<bool, string>(false, null),
                },
                Layer_3 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                },
                Layer_4 = new DallanArtMaker.DallanArtLayer()
                {
                    P_4 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_5 = new DallanArtMaker.DallanArtLayer()
                {
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
            },
            new DallanArtMaker()//smile
            {
                Layer_1 = new DallanArtMaker.DallanArtLayer()
                {
                    P_3 = new Tuple<bool, string>(false, null),
                },
                Layer_2 = new DallanArtMaker.DallanArtLayer()
                {
                    P_3 = new Tuple<bool, string>(false, null),
                },
                Layer_3 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_2 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_4 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_5 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
            },
            new DallanArtMaker()//sus
            {
                Weight = 0.1f,
                Layer_1 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_2 = new DallanArtMaker.DallanArtLayer()
                {
                    P_3 = new Tuple<bool, string>(false, null),
                    P_4 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),

                },
                Layer_3 = new DallanArtMaker.DallanArtLayer()
                {
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_4 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
                Layer_5 = new DallanArtMaker.DallanArtLayer()
                {
                    P_1 = new Tuple<bool, string>(false, null),
                    P_3 = new Tuple<bool, string>(false, null),
                    P_5 = new Tuple<bool, string>(false, null),
                },
            },
        };


		public class DallanArtMaker
		{
            public float Weight = 1;
			public DallanArtLayer Layer_1 = new DallanArtLayer();
            public DallanArtLayer Layer_2 = new DallanArtLayer();
            public DallanArtLayer Layer_3 = new DallanArtLayer();
            public DallanArtLayer Layer_4 = new DallanArtLayer();
            public DallanArtLayer Layer_5 = new DallanArtLayer();
            public class DallanArtLayer 
			{ 
				public Tuple<bool, string> P_1 = new Tuple<bool, string>(true, "default");
                public Tuple<bool, string> P_2 = new Tuple<bool, string>(true, "default");
                public Tuple<bool, string> P_3 = new Tuple<bool, string>(true, "default");
                public Tuple<bool, string> P_4 = new Tuple<bool, string>(true, "default");
                public Tuple<bool, string> P_5 = new Tuple<bool, string>(true, "default");
            }
        }

		public class SkellScript : Script 
		{
            public Tuple<bool, string> returnDetails(int h, int h2, DallanArtMaker maker)
            {
                if (h == 2)
                {
                    return returnLayer(h2, maker.Layer_1);
                }
                if (h == 1)
                {
                    return returnLayer(h2, maker.Layer_2);
                }
                if (h == 0)
                {
                    return returnLayer(h2, maker.Layer_3);
                }
                if (h == -1)
                {
                    return returnLayer(h2, maker.Layer_4);
                }
                if (h == -2)
                {
                    return returnLayer(h2, maker.Layer_5);
                }
                return new Tuple<bool, string>(false, null);
            }
            private Tuple<bool, string> returnLayer(int h2, DallanArtMaker.DallanArtLayer layer)
            {
                if (h2 == 2)
                {
                    return layer.P_1;
                }
                if (h2 == 1)
                {
                    return layer.P_2;
                }
                if (h2 == 0)
                {
                    return layer.P_3;
                }
                if (h2 == -1)
                {
                    return layer.P_4;
                }
                if (h2 == -2)
                {
                    return layer.P_5;
                }
                return new Tuple<bool, string>(false, null);
            }

            public override IEnumerator Top() 
			{
                Active = false;
                var Art = BraveUtility.RandomElement<DallanArtMaker>(arts);
                Vector2 po = this.Position;
                for (int ijj = 2; ijj > -3; ijj--)
                {
                    for (int i = 2; i > -3; i--)
                    {
                        var thing = returnDetails(ijj, i, Art);
                        if (thing.First == true)
                        {
                            base.Fire(Offset.OverridePosition(po + new Vector2(0.75f * i, 0.75f * ijj)), new Direction(0, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new WallBullet(thing.Second, this));
                            yield return this.Wait(4);
                        }
                    }
                }
                yield return this.Wait(40);
                Angle = this.AimDirection;
                yield return this.Wait(5);
                Active = true;
                yield break;
			}
            public bool Active;
            public float Angle = 0;

        }




        public class WallBullet : Bullet
		{
			public WallBullet(string bullet, SkellScript SCRIPT) : base(bullet, false, false, false)
			{
                script = SCRIPT;

            }
            public override IEnumerator Top()
            {
                while (script.Active == false)
                {
                    yield return this.Wait(1);

                }
                base.ChangeDirection(new Brave.BulletScript.Direction(script.Angle, DirectionType.Absolute), 0);
                base.ChangeSpeed(new Speed(15, SpeedType.Absolute), 150);
            }

            private SkellScript script;
        }
	}
}








