using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using UnityEngine;
using ItemAPI;


namespace Planetside
{
    public static class MultiActiveReloadManager
    {
        public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public static void SetupHooks()
        {
            Hook hook = new Hook(
                typeof(GameUIReloadBarController).GetMethod("TriggerReload", BindingFlags.Public | BindingFlags.Instance),
                typeof(MultiActiveReloadManager).GetMethod("TriggerReloadHook")
            );
            /*Hook hook2 = new Hook(
                typeof(GameUIReloadBarController).GetMethod("AttemptActiveReload", BindingFlags.Public | BindingFlags.Instance),
                typeof(MultiActiveReloadManager).GetMethod("AttemptActiveReloadHook")
            );*/
            Hook hook3 = new Hook(
                typeof(Gun).GetMethod("OnActiveReloadPressed", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(MultiActiveReloadManager).GetMethod("OnActiveReloadPressedHook")
            );
            Hook hook4 = new Hook(
                typeof(Gun).GetMethod("Reload", BindingFlags.Public | BindingFlags.Instance),
                typeof(MultiActiveReloadManager).GetMethod("ReloadHook")
            );
        }

        public static bool ReloadHook(Func<Gun, bool> orig, Gun self)
        {
            bool result = orig(self);
            if (result && self.GetComponent<MultiActiveReloadController>() != null)
            {
                self.GetComponent<MultiActiveReloadController>().canAttemptActiveReload = true;
                self.GetComponent<MultiActiveReloadController>().damageMult = 1f;
            }
            return result;
        }

        public static void TriggerReloadHook(Action<GameUIReloadBarController, PlayerController, Vector3, float, float, int> orig, GameUIReloadBarController self, PlayerController attachParent, Vector3 offset, float duration, float activeReloadStartPercent,
            int pixelWidth)
        {
            
            if (tempraryActiveReloads.ContainsKey(self))
            {
                foreach (MultiActiveReload multiactivereload in tempraryActiveReloads[self])
                {
                    if (multiactivereload.sprite != null && multiactivereload.sprite.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(multiactivereload.sprite.gameObject);
                    }
                    if (multiactivereload.celebrationSprite != null && multiactivereload.celebrationSprite.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(multiactivereload.celebrationSprite.gameObject);
                    }
                }
                tempraryActiveReloads[self].Clear();
            }
            orig(self, attachParent, offset, duration, activeReloadStartPercent, pixelWidth);
            if (attachParent != null && attachParent.CurrentGun != null && attachParent.CurrentGun.GetComponent<MultiActiveReloadController>() != null)
            {
                foreach (MultiActiveReloadData data in attachParent.CurrentGun.GetComponent<MultiActiveReloadController>().reloads)
                {
                    dfSprite sprite = UnityEngine.Object.Instantiate(self.activeReloadSprite);
                    self.activeReloadSprite.Parent.AddControl(sprite);
                    sprite.enabled = true;
                    float width = self.progressSlider.Width;
                    float maxValue = self.progressSlider.MaxValue;
                    float num = data.startValue / maxValue * width;
                    float num2 = data.endValue / maxValue * width;
                    float x = num + (num2 - num) * data.activeReloadStartPercentage;
                    float width2 = (float)pixelWidth * Pixelator.Instance.CurrentTileScale;
                    sprite.RelativePosition = self.activeReloadSprite.RelativePosition;
                    sprite.RelativePosition = GameUIUtility.QuantizeUIPosition(sprite.RelativePosition.WithX(x));
                    sprite.Width = width2;
                    sprite.IsVisible = true;
                    dfSprite celebrationSprite = UnityEngine.Object.Instantiate(self.celebrationSprite);
                    self.activeReloadSprite.Parent.AddControl(celebrationSprite);
                    celebrationSprite.enabled = true;
                    dfSpriteAnimation component = celebrationSprite.GetComponent<dfSpriteAnimation>();
                    component.Stop();
                    component.SetFrameExternal(0);
                    celebrationSprite.enabled = false;
                    celebrationSprite.RelativePosition = sprite.RelativePosition + new Vector3(Pixelator.Instance.CurrentTileScale * -1f, Pixelator.Instance.CurrentTileScale * -2f, 0f);
                    int activeReloadStartValue = Mathf.RoundToInt((float)(data.endValue - data.startValue) * data.activeReloadStartPercentage) + data.startValue - data.activeReloadLastTime / 2;
                    MultiActiveReload reload = new MultiActiveReload
                    {
                        sprite = sprite,
                        celebrationSprite = celebrationSprite,
                        startValue = activeReloadStartValue,
                        endValue = activeReloadStartValue + data.activeReloadLastTime,
                        stopsReload = data.stopsReload,
                        canAttemptActiveReloadAfterwards = data.canAttemptActiveReloadAfterwards,
                        reloadData = data.reloadData,
                        usesActiveReloadData = data.usesActiveReloadData,
                        Name = data.Name
                    };
                    if (tempraryActiveReloads.ContainsKey(self))
                    {
                        tempraryActiveReloads[self].Add(reload);
                    }
                    else
                    {
                        tempraryActiveReloads.Add(self, new List<MultiActiveReload> { reload });
                    }
                }
            }
            
        }

        public static bool AttemptActiveReloadHook(Func<GameUIReloadBarController, bool> orig, GameUIReloadBarController self)
        {
            if (!self.ReloadIsActive)
            {
                return false;
            }
            if (tempraryActiveReloads.ContainsKey(self))
            {
                foreach (MultiActiveReload reload in tempraryActiveReloads[self])
                {
                    if (self.progressSlider.Value >= (float)reload.startValue && self.progressSlider.Value <= (float)reload.endValue)
                    {
                        self.progressSlider.Color = Color.green;
                        AkSoundEngine.PostEvent("Play_WPN_active_reload_01", self.gameObject);
                        reload.celebrationSprite.enabled = true;
                        reload.sprite.enabled = false;
                        if (reload.stopsReload)
                        {
                            self.progressSlider.Thumb.enabled = false;
                            info.SetValue(self, false);
                        }
                        reload.celebrationSprite.GetComponent<dfSpriteAnimation>().Play();
                        return true;
                    }
                }
            }
            bool result = orig(self);
            return result;
        }

        public static bool AttemptActiveReloadOnlyMultireload(this GameUIRoot self, PlayerController targetPlayer)
        {
            int index = (!targetPlayer.IsPrimaryPlayer) ? 1 : 0;
            bool flag = ((List<GameUIReloadBarController>)info3.GetValue(self))[index].AttemptActiveReloadOnlyMultireload();
            return flag;
        }

        public static bool AttemptActiveReloadOnlyMultireload(this GameUIReloadBarController self)
        {
            if (!self.ReloadIsActive)
            {
                return false;
            }
            if (tempraryActiveReloads.ContainsKey(self))
            {
                foreach (MultiActiveReload reload in tempraryActiveReloads[self])
                {
                    if (self.progressSlider.Value >= (float)reload.startValue && self.progressSlider.Value <= (float)reload.endValue)
                    {
                        self.progressSlider.Color = Color.green;
                        AkSoundEngine.PostEvent("Play_WPN_active_reload_01", self.gameObject);
                        reload.celebrationSprite.enabled = true;
                        reload.sprite.enabled = false;
                        if (reload.stopsReload)
                        {
                            self.progressSlider.Thumb.enabled = false;
                            info.SetValue(self, false);
                        }
                        reload.celebrationSprite.GetComponent<dfSpriteAnimation>().Play();
                        return true;
                    }
                }
            }
            self.progressSlider.Color = Color.red;
            return false;
        }

        public static void OnActiveReloadPressedHook(Action<Gun, PlayerController, Gun, bool> orig, Gun self, PlayerController p, Gun g, bool actualPress)
        {
            orig(self, p, g, actualPress);
            if (self.IsReloading || self.reloadTime < 0f)
            {
                PlayerController playerController = self.CurrentOwner as PlayerController;
                if (playerController && (actualPress || true))
                {
                    MultiActiveReloadController controller = self.GetComponent<MultiActiveReloadController>();
                    if (controller != null && controller.activeReloadEnabled && controller.canAttemptActiveReload && !GameUIRoot.Instance.GetReloadBarForPlayer(self.CurrentOwner as PlayerController).IsActiveReloadGracePeriod())
                    {
                        bool flag2 = GameUIRoot.Instance.AttemptActiveReloadOnlyMultireload(self.CurrentOwner as PlayerController);
                        MultiActiveReload reload = GameUIRoot.Instance.GetReloadBarForPlayer(self.CurrentOwner as PlayerController).GetMultiActiveReloadForController();
                        if (flag2)
                        {
                            controller.OnActiveReloadSuccess(reload);
                            GunFormeSynergyProcessor component = self.GetComponent<GunFormeSynergyProcessor>();
                            if (component)
                            {
                                component.JustActiveReloaded = true;
                            }
                            ChamberGunProcessor component2 = self.GetComponent<ChamberGunProcessor>();
                            if (component2)
                            {
                                component2.JustActiveReloaded = true;
                            }
                        }
                        else
                        {
                            controller.OnActiveReloadFailure(reload);
                        }
                        if (reload == null || !reload.canAttemptActiveReloadAfterwards)
                        {
                            ETGModConsole.Log("yes");
                            controller.canAttemptActiveReload = false;
                            Action<PlayerController, Gun, bool> act = (Action<PlayerController, Gun, bool>)info2.CreateDelegate<Action<PlayerController, Gun, bool>>();
                            self.OnReloadPressed -= act;
                        }
                    }
                }
            }
        }

        public static MultiActiveReload GetMultiActiveReloadForController(this GameUIReloadBarController controller)
        {
            MultiActiveReload result = null;
            if (tempraryActiveReloads.ContainsKey(controller))
            {
                foreach (MultiActiveReload reload in tempraryActiveReloads[controller])
                {
                    if (controller.progressSlider.Value >= (float)reload.startValue && controller.progressSlider.Value <= (float)reload.endValue)
                    {
                        result = reload;
                        break;
                    }
                }
            }
            return result;
        }

        public static Dictionary<GameUIReloadBarController, List<MultiActiveReload>> tempraryActiveReloads = new Dictionary<GameUIReloadBarController, List<MultiActiveReload>>();
        public static FieldInfo info = typeof(GameUIReloadBarController).GetField("m_reloadIsActive", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo info2 = typeof(Gun).GetMethod("OnActiveReloadPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo info3 = typeof(GameUIRoot).GetField("m_extantReloadBars", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public class MultiActiveReload
    {
        public dfSprite sprite;
        public dfSprite celebrationSprite;
        public int startValue;
        public int endValue;
        public bool stopsReload;
        public bool canAttemptActiveReloadAfterwards;
        public ActiveReloadData reloadData;
        public bool usesActiveReloadData;
        public string Name;
    }

    public struct MultiActiveReloadData
    {
        public MultiActiveReloadData(float activeReloadStartPercentage, int startValue, int endValue, int pixelWidth, int activeReloadLastTime, bool stopsReload, bool canAttemptActiveReloadAfterwards, ActiveReloadData reloadData, bool usesActiveReloadData, string Name)
        {
            this.activeReloadStartPercentage = activeReloadStartPercentage;
            this.startValue = startValue;
            this.endValue = endValue;
            this.pixelWidth = pixelWidth;
            this.activeReloadLastTime = activeReloadLastTime;
            this.stopsReload = stopsReload;
            this.canAttemptActiveReloadAfterwards = canAttemptActiveReloadAfterwards;
            this.reloadData = reloadData;
            this.usesActiveReloadData = usesActiveReloadData;
            this.Name = Name;
        }

        public float activeReloadStartPercentage;
        public int startValue;
        public int endValue;
        public int pixelWidth;
        public int activeReloadLastTime;
        public bool stopsReload;
        public bool canAttemptActiveReloadAfterwards;
        public ActiveReloadData reloadData;
        public bool usesActiveReloadData;
        public string Name;
    }

    class MultiActiveReloadController : AdvancedGunBehaviourMultiActive
    {
        public virtual void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            if (reload == null || reload.stopsReload)
            {
                info.Invoke(base.gun, new object[] { true, false, false });
            }
            float num = 1f;
            if (Gun.ActiveReloadActivated && this.PickedUpByPlayer && this.Player.IsPrimaryPlayer)
            {
                num *= CogOfBattleItem.ACTIVE_RELOAD_DAMAGE_MULTIPLIER;
            }
            if (Gun.ActiveReloadActivatedPlayerTwo && this.PickedUpByPlayer && !this.Player.IsPrimaryPlayer)
            {
                num *= CogOfBattleItem.ACTIVE_RELOAD_DAMAGE_MULTIPLIER;
            }
            if (reload == null || reload.usesActiveReloadData)
            {
                if (base.gun.LocalActiveReload && (reload == null || reload.reloadData == null))
                {
                    num *= Mathf.Pow(this.gun.activeReloadData.damageMultiply, (float)((int)info2.GetValue(base.gun) + 1));
                }
                else if (reload != null && reload.reloadData != null)
                {
                    num *= Mathf.Pow(reload.reloadData.damageMultiply, reload.reloadData.ActiveReloadStacks ? (float)((int)info2.GetValue(base.gun) + 1) : 1);
                }
            }
            this.damageMult = num;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.baseData.damage *= this.damageMult;
        }

        public virtual void OnActiveReloadFailure(MultiActiveReload reload)
        {
            this.damageMult = 1f;
        }

        public override void MidGameDeserialize(List<object> data, ref int dataIndex)
        {
            base.MidGameDeserialize(data, ref dataIndex);
            this.reloads = (List<MultiActiveReloadData>)data[dataIndex];
            this.activeReloadEnabled = (bool)data[dataIndex + 1];
            dataIndex += 2;
        }

        public override void MidGameSerialize(List<object> data, int dataIndex)
        {
            base.MidGameSerialize(data, dataIndex);
            data.Add(this.reloads);
            data.Add(this.activeReloadEnabled);
        }

        public override void InheritData(Gun source)
        {
            base.InheritData(source);
            MultiActiveReloadController component = source.GetComponent<MultiActiveReloadController>();
            if (component)
            {
                this.reloads = component.reloads;
                this.activeReloadEnabled = component.activeReloadEnabled;
            }
        }

        public static MethodInfo info = typeof(Gun).GetMethod("FinishReload", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo info2 = typeof(Gun).GetField("SequentialActiveReloads", BindingFlags.NonPublic | BindingFlags.Instance);
        public List<MultiActiveReloadData> reloads = new List<MultiActiveReloadData>();
        public bool canAttemptActiveReload;
        public bool activeReloadEnabled;
        public float damageMult = 1f;
    }
}