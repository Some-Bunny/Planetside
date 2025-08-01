using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside.Components.Projectile_Components
{
    public class TravelledDistanceComponent : MonoBehaviour
    {
        public void Start()
        {
            projectile = projectile ?? this.GetComponent<Projectile>();
            lastStoredPosition = projectile.sprite.WorldCenter;

        }

        public Action<Projectile, Vector2, int> OnTravelledDistance;

        public void Update()
        {
            if (TriggerAmount == 0) { return; }
            if (projectile == null) { return; }
            DistTick += (int)(Vector2.Distance(projectile.sprite.WorldCenter, lastStoredPosition) / DistanceToTravel);
            for (float i = 0; i < DistTick; i++)
            {
                if (TriggerAmount > 0) { TriggerAmount--; }
                AmountOfProcs++;

                if (OnTravelledDistance != null)
                {
                    float t = (float)i / DistTick;
                    Vector3 vector3 = Vector3.Lerp(projectile.sprite.WorldCenter, lastStoredPosition, t);
                    OnTravelledDistance(projectile, vector3, AmountOfProcs);
                }
                DistTick--;
                if (i == DistTick - 1)
                {
                    lastStoredPosition = projectile.sprite.WorldCenter;
                }

            }
        }
        public Projectile projectile;
        private Vector2 lastStoredPosition;
        private float DistTick = 0;

        public int TriggerAmount = -1;

        public float DistanceToTravel = 5;
        public int AmountOfProcs = 0;
    }
}
