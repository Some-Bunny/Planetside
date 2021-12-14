namespace Planetside
{
    public class DebuffStatics
    {
        //---------------------------------------BASEGAME STATUS EFFECTS
        //Fires
        public static GameActorFireEffect hotLeadEffect = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect;
        public static GameActorFireEffect greenFireEffect = PickupObjectDatabase.GetById(706).GetComponent<Gun>().DefaultModule.projectiles[0].fireEffect;


        //Freezes
        public static GameActorFreezeEffect frostBulletsEffect = PickupObjectDatabase.GetById(278).GetComponent<BulletStatusEffectItem>().FreezeModifierEffect;
        public static GameActorFreezeEffect chaosBulletsFreeze = PickupObjectDatabase.GetById(569).GetComponent<ChaosBulletsItem>().FreezeModifierEffect;

        //Poisons
        public static GameActorHealthEffect irradiatedLeadEffect = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>().HealthModifierEffect;

        //Charms
        public static GameActorCharmEffect charmingRoundsEffect = PickupObjectDatabase.GetById(527).GetComponent<BulletStatusEffectItem>().CharmModifierEffect;

        //Cheeses
        public static GameActorCheeseEffect cheeseeffect = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect;
        //Speed Changes
        public static GameActorSpeedEffect tripleCrossbowSlowEffect = (PickupObjectDatabase.GetById(381) as Gun).DefaultModule.projectiles[0].speedEffect;        
    }
}