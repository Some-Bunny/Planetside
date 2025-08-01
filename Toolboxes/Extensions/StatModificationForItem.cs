using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    public static class StatModificationForItem
    {
        public static StatModifier AddStat(this PassiveItem self, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            if (self.passiveStatModifiers == null)
            {
                self.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
            }
            else
            {
                self.passiveStatModifiers = self.passiveStatModifiers.Concat(new StatModifier[]
                {
                    statModifier
                }).ToArray<StatModifier>();
            }
            return statModifier;
        }
        public static void RemoveStat(this PassiveItem self, StatModifier statModifier)
        {
            if (statModifier == null) { return; }
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < self.passiveStatModifiers.Length; i++)
            {
                if (self.passiveStatModifiers[i] != statModifier)
                {
                    list.Add(self.passiveStatModifiers[i]);
                }
            }
            self.passiveStatModifiers = list.ToArray();
        }

        public static StatModifier AddStat(this PlayerItem self, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            if (self.passiveStatModifiers == null)
            {
                self.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
            }
            else
            {
                self.passiveStatModifiers = self.passiveStatModifiers.Concat(new StatModifier[]
                {
                    statModifier
                }).ToArray<StatModifier>();
            }
            return statModifier;
        }
        public static void RemoveStat(this PlayerItem self, StatModifier statModifier)
        {
            if (statModifier == null) { return; }
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < self.passiveStatModifiers.Length; i++)
            {
                if (self.passiveStatModifiers[i] != statModifier)
                {
                    list.Add(self.passiveStatModifiers[i]);
                }
            }
            self.passiveStatModifiers = list.ToArray();
        }
        public static StatModifier AddStat(this Gun self, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            if (self.passiveStatModifiers == null)
            {
                self.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
            }
            else
            {
                self.passiveStatModifiers = self.passiveStatModifiers.Concat(new StatModifier[]
                {
                    statModifier
                }).ToArray<StatModifier>();
            }
            return statModifier;
        }
        public static void RemoveStat(this Gun self, StatModifier statModifier)
        {
            if (statModifier == null) { return; }
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < self.passiveStatModifiers.Length; i++)
            {
                if (self.passiveStatModifiers[i] != statModifier)
                {
                    list.Add(self.passiveStatModifiers[i]);
                }
            }
            self.passiveStatModifiers = list.ToArray();
        }
    }
}
