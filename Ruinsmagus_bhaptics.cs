using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

using App.Players.BattleSystems.Magics;
using App.Equipments.Gauntlets.Animations;

namespace Ruinsmagus_bhaptics
{
    public class Ruinsmagus_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool rightHanded = true;

        [Obsolete]
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }


        [HarmonyPatch(typeof(HandedInputSelector), "SetActiveController", new Type[] { typeof(OVRInput.Controller) })]
        public class bhaptics_ChangeHandedness
        {
            [HarmonyPostfix]
            public static void Postfix(OVRInput.Controller c)
            {
                rightHanded = ((c == OVRInput.Controller.RHand) || (c == OVRInput.Controller.RTouch));
            }
        }


        [HarmonyPatch(typeof(PlayerMagicEmittersManager), "OnEmitAtEmitter", new Type[] { typeof(App.Magics.MasterData.MagicMasterData) })]
        public class bhaptics_CastPrimarySpell
        {
            [HarmonyPostfix]
            public static void Postfix(App.Magics.MasterData.MagicMasterData magicData)
            {
                string magicPattern = "Fire";
                switch (magicData.magicType)
                {
                    case App.Magics.MasterData.MagicType.Default:
                        magicPattern = "Fire";
                        break;
                    case App.Magics.MasterData.MagicType.Skill:
                        magicPattern = "Fire";
                        break;
                    default:
                        magicPattern = "Fire";
                        break;
                }
                tactsuitVr.CastSpell(magicPattern, rightHanded);
            }
        }
        

        [HarmonyPatch(typeof(App.Items.ConsumableItems.BaseConsumableItem), "Consume", new Type[] { })]
        public class bhaptics_ConsumeItem
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Drinking");
            }
        }

        [HarmonyPatch(typeof(App.Players.PlayerCore), "OnDie", new Type[] { })]
        public class bhaptics_PlayerDies
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        
        [HarmonyPatch(typeof(App.Equipments.Shields.Views.ShieldBodyView), "OnGuardSucceed", new Type[] {  })]
        public class bhaptics_ShieldAttacked
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.Block(!rightHanded);
            }
        }

        [HarmonyPatch(typeof(App.Equipments.Shields.Views.ShieldBodyView), "OnParrySucceed", new Type[] { })]
        public class bhaptics_ShieldParries
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.Block(!rightHanded);
            }
        }

        [HarmonyPatch(typeof(App.Players.BattleSystems.PlayerDamageManager), "CalculatePlayerDamageStateAndPlayEffect", new Type[] { typeof(int), typeof(bool) })]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(App.Players.BattleSystems.PlayerDamageManager __instance, int currentHealth, bool isIncrease)
            {
                if (currentHealth <= 66) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
                //tactsuitVr.LOG("Health: " + currentHealth.ToString());
                if (isIncrease) return;
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }

    }
}
