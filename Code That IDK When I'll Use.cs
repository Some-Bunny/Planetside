using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *if (proj1.GetComponentInChildren<tk2dTiledSprite>() != null)
            {
                proj1.GetComponentInChildren<tk2dTiledSprite>().usesOverrideMaterial = true;
                if (false)
                {
                    
                    proj1.GetComponentInChildren<tk2dTiledSprite>().renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
                    proj1.GetComponentInChildren<tk2dTiledSprite>().sprite.renderer.material.SetFloat("_HueTestValue", 0);
                }
                else
                {
                    proj1.GetComponentInChildren<tk2dTiledSprite>().renderer.material.SetFloat("_VertexColor", 1);
                    proj1.GetComponentInChildren<tk2dTiledSprite>().color = Color.cyan;
                }
            }

 *
 *
 *

 * TextBoxManager.ShowTextBox(baseTransform.position + offset, baseTransform, duration, stringKey, string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
 * 
 * 
 * 
 * 
 else if (user.CurrentRoom != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) is Chest )
{
                Chest chest = (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) as Chest);
                List<PickupObject> contents = chest.PredictContents(user);
}
 * 
 * 
 * USING THE RAT TELL LASERS
float angle = yourAngle;
CrimsonChamberController component = this.BulletBank.GetComponent<CrimsonChamberController>();
float num2 = 20f;
Vector2 zero = Vector2.zero;
if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(angle, 60f), area.UnitBottomLeft, area.UnitTopRight - new Vector2(0f, 6f), ref zero))
{
  num2 = (zero - this.Position).magnitude;
}
GameObject gameObject = SpawnManager.SpawnVFX(component.tellLaser, false);
tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
component2.transform.position = new Vector3(this.Position.x, this.Position.y, this.Position.y) + BraveMathCollege.DegreesToVector(angle, 2f).ToVector3ZUp(0);
component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
component2.dimensions = new Vector2((num2 - 3f) * 16f, 5f);
component2.UpdateZDepth();
 * 
 SpawnManager.Despawn(tellLaser);
 * 
 crimsonChamberController.tellLaser = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;  
 * 
 * 
 * animation stuff
 
 
 *tk2dSpriteAnimationClip chargeClip = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation);
            foreach (tk2dSpriteAnimationFrame frame in chargeClip.frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    def.MakeOffset(new Vector2(-0.56f, -2.31f));
                }
            }
            tk2dSpriteAnimationClip fireClip = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation);
            foreach (tk2dSpriteAnimationFrame frame in fireClip.frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    def.MakeOffset(new Vector2(-0.56f, -2.31f));
                }
            }
 * 
 * 
 * 
 * public class PierceDeadActors : MonoBehaviour
        {
            public PierceDeadActors()
            {
            }
            private void Start()
            {
                this.m_projectile = base.GetComponent<Projectile>();
                this.m_projectile.specRigidbody.OnPreRigidbodyCollision += this.PreCollision;
            }
            private void PreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
            {
                if (myRigidbody != null && otherRigidbody != null)
                {
                    if (otherRigidbody.healthHaver != null && otherRigidbody.healthHaver.IsDead)
                    {
                        PhysicsEngine.SkipCollision = true;
                    }
                }
            }
            private Projectile m_projectile;
        }
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * GameObject gameObject = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam"); there's only one really irc

 * 
 *Reverse Distortion Wave 
 *
private IEnumerator DoDistortionWaveLocal(Vector2 center, float distortionIntensity, float distortionRadius, float maxRadius, float duration)
        {
            Material distMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
            Vector4 distortionSettings = this.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
            distMaterial.SetVector("_WaveCenter", distortionSettings);
            Pixelator.Instance.RegisterAdditionalRenderPass(distMaterial);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                    elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                t = BraveMathCollege.LinearToSmoothStepInterpolate(0f, 1f, t);
                distortionSettings = this.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
                distortionSettings.w = Mathf.Lerp(distortionSettings.w, 0f, t);
                distMaterial.SetVector("_WaveCenter", distortionSettings);
                float currentRadius = Mathf.Lerp(maxRadius, 0f, t);
                distMaterial.SetFloat("_DistortProgress", (currentRadius / (maxRadius * 5)));
                yield return null;
            }
            Pixelator.Instance.DeregisterAdditionalRenderPass(distMaterial);
            UnityEngine.Object.Destroy(distMaterial);

*
*
* Adding A pickup to the pickup shop pool & room clear pool
item.CustomCost = 30;
            item.UsesCustomCost = true;

            WeightedGameObject weightedObject = new WeightedGameObject();
            weightedObject.SetGameObject(gameObject);
            weightedObject.weight = 1;
            weightedObject.rawGameObject = gameObject;
            weightedObject.pickupId = item.PickupObjectId;
            weightedObject.forceDuplicatesPossible = true;
            weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];

            foreach (FloorRewardData rewardData in GameManager.Instance.RewardManager.FloorRewardData)
            {
                ETGModConsole.Log($"{rewardData.AssociatedTilesets}:");
                if (rewardData.SingleItemRewardTable.defaultItemDrops.elements != null && rewardData.SingleItemRewardTable.defaultItemDrops.elements.Count != 0 && !rewardData.SingleItemRewardTable.defaultItemDrops.elements.Contains(weightedObject));
                {
                    rewardData.SingleItemRewardTable.defaultItemDrops.Add(weightedObject);
                    foreach (WeightedGameObject shrug in rewardData.SingleItemRewardTable.defaultItemDrops.elements)
                    {
                        if (shrug.gameObject != null)
                        {
                            ETGModConsole.Log($"{shrug.gameObject.name} : {shrug.weight}");
                        }
                    }
                }
                ETGModConsole.Log("------");
            }

            GenericLootTable thanksbotluvya = ItemBuilder.LoadShopTable("Shop_Gungeon_Cheap_Items_01");
            thanksbotluvya.defaultItemDrops.elements.Add(weightedObject);


		public static int SackPickupID;

		public override void Pickup(PlayerController player)
		{
			this.random = UnityEngine.Random.Range(0.0f, 1.0f);
			if (random < 0.7)
			{
				int id = BraveUtility.RandomElement<int>(SackPickup.GenericPool);
				float itemsToSpawn = UnityEngine.Random.Range(2, 4);
				for (int i = 0; i < itemsToSpawn; i++)
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2(0, 0), 2.2f, false, true, false);
				}
			}
			else if (random > 0.7 && random < 0.95)
			{
				int id = BraveUtility.RandomElement<int>(SackPickup.HPPool);
				float itemsToSpawn = UnityEngine.Random.Range(2, 4);
				for (int i = 0; i < itemsToSpawn; i++)
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2(0, 0), 2.2f, false, true, false);
				}
			}
			if (random > 0.95)
			{
				int id = BraveUtility.RandomElement<int>(SackPickup.Lootdrops);
				float itemsToSpawn = UnityEngine.Random.Range(3, 5);
				for (int i = 0; i < itemsToSpawn; i++)
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2(0, 0), 2.2f, false, true, false);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		private float random;

		protected void Start()
		{
			try
			{
				this.storedBody = base.gameObject.GetComponent<SpeculativeRigidbody>();
				SpeculativeRigidbody speculativeRigidbody = this.storedBody;
				SpeculativeRigidbody speculativeRigidbody2 = speculativeRigidbody;
				speculativeRigidbody2.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(speculativeRigidbody2.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}

		private void OnPreCollision(SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody source, CollisionData collisionData)
		{
			bool hasBeenPickedUp = this.m_hasBeenPickedUp;
			if (!hasBeenPickedUp)
			{
				PlayerController component = otherRigidbody.GetComponent<PlayerController>();
				bool flag = component != null;
				if (flag)
				{
					this.m_hasBeenPickedUp = true;
					this.Pickup(component);
				}
			}
		}
		public SpeculativeRigidbody storedBody;
		private bool m_hasBeenPickedUp;



		public static List<int> GenericPool = new List<int>
		{
			67,//key
			224,//blank
			600,//partial-ammo
			78,//ammo
			565//glass guon stone
		};
		public static List<int> HPPool = new List<int>
		{
			73,//half-heart
			85,//full-heart
			120//armor

		};
		public static List<int> MoneyPool = new List<int>
		{
			68, //1 casing

		};

		public static List<int> Lootdrops = new List<int>
		{
			73,//half-heart
			85,//full-heart
			120,//armor
			67,//key
			224,//blank
			600,//partial-ammo
			78,//ammo
			565//glass guon stone

		};
*/



/*
 * Transform clockhairTransform = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("Clockhair", ".prefab"))).transform;
		ClockhairController clockhair = clockhairTransform.GetComponent<ClockhairController>();
		elapsed = 0f;
		duration = clockhair.ClockhairInDuration;
		Vector3 clockhairTargetPosition = this.CenterPosition;
		Vector3 clockhairStartPosition = clockhairTargetPosition + new Vector3(-20f, 5f, 0f);
		clockhair.renderer.enabled = false;
		clockhair.spriteAnimator.Play("clockhair_intro");
		clockhair.hourAnimator.Play("hour_hand_intro");
		clockhair.minuteAnimator.Play("minute_hand_intro");
		clockhair.secondAnimator.Play("second_hand_intro");
		if (!isDoingPigSave && (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || GameManager.Instance.GetOtherPlayer(this).IsGhost) && this.OnRealPlayerDeath != null)
		{
			this.OnRealPlayerDeath(this);
		}
		bool hasWobbled = false;
		while (elapsed < duration)
		{
			if (GameManager.INVARIANT_DELTA_TIME == 0f)
			{
				elapsed += 0.05f;
			}
			elapsed += GameManager.INVARIANT_DELTA_TIME;
			float t2 = elapsed / duration;
			float smoothT = Mathf.SmoothStep(0f, 1f, t2);
			Vector3 currentPosition = Vector3.Slerp(clockhairStartPosition, clockhairTargetPosition, smoothT);
			clockhairTransform.position = currentPosition.WithZ(0f);
			if (t2 > 0.5f)
			{
				clockhair.renderer.enabled = true;
				clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
			}
			if (t2 > 0.75f)
			{
				clockhair.hourAnimator.GetComponent<Renderer>().enabled = true;
				clockhair.minuteAnimator.GetComponent<Renderer>().enabled = true;
				clockhair.secondAnimator.GetComponent<Renderer>().enabled = true;
				clockhair.hourAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
				clockhair.minuteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
				clockhair.secondAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
			}
			if (!hasWobbled && clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1)
			{
				clockhair.spriteAnimator.Play("clockhair_wobble");
				hasWobbled = true;
			}
			clockhair.sprite.UpdateZDepth();
			this.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
			yield return null;
		}
		if (!hasWobbled)
		{
			clockhair.spriteAnimator.Play("clockhair_wobble");
		}
		clockhair.SpinToSessionStart(clockhair.ClockhairSpinDuration);
		elapsed = 0f;
		duration = clockhair.ClockhairSpinDuration + clockhair.ClockhairPauseBeforeShot;
		while (elapsed < duration)
		{
			if (GameManager.INVARIANT_DELTA_TIME == 0f)
			{
				elapsed += 0.05f;
			}
			elapsed += GameManager.INVARIANT_DELTA_TIME;
			clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
			yield return null;
		}
		if (isDoingPigSave)
		{
			elapsed = 0f;
			duration = 2f;
			Vector2 targetPosition = clockhairTargetPosition;
			Vector2 startPosition = targetPosition + new Vector2(-18f, 0f);
			Vector2 pigOffset = pigVFX.sprite.WorldCenter - pigVFX.transform.position.XY();
			pigVFX.Play(pigMoveAnim);
			while (elapsed < duration)
			{
				Vector2 lerpPosition = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
				pigVFX.transform.position = (lerpPosition - pigOffset).ToVector3ZisY(0f);
				pigVFX.sprite.UpdateZDepth();
				if (duration - elapsed < 0.1f && !pigVFX.IsPlaying(pigSaveAnim))
				{
					pigVFX.Play(pigSaveAnim);
				}
				pigVFX.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
				if (GameManager.INVARIANT_DELTA_TIME == 0f)
				{
					elapsed += 0.05f;
				}
				elapsed += GameManager.INVARIANT_DELTA_TIME;
				yield return null;
			}
		}
		elapsed = 0f;
		duration = 0.1f;
		clockhairStartPosition = clockhairTransform.position;
		clockhairTargetPosition = clockhairStartPosition + new Vector3(0f, 12f, 0f);
		clockhair.spriteAnimator.Play("clockhair_fire");
		clockhair.hourAnimator.GetComponent<Renderer>().enabled = false;
		clockhair.minuteAnimator.GetComponent<Renderer>().enabled = false;
		clockhair.secondAnimator.GetComponent<Renderer>().enabled = false;
		this.DoVibration(Vibration.Time.Normal, Vibration.Strength.Hard);
 */