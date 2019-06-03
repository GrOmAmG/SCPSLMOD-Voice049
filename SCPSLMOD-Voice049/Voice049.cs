using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using Unity;

namespace SCPSLMOD_Voice049
{
    public class Mod
    {
        public static bool VoiceCond(GameObject obj)
        {
            return obj.GetComponent<Scp049PlayerScript>() != null && obj.GetComponent<Scp049PlayerScript>().iAm049;
        }

        public static bool VoiceCond2()
        {
            return Radio.localRadio.transform.GetComponent<Scp049PlayerScript>() != null && Radio.localRadio.transform.GetComponent<Scp049PlayerScript>().iAm049;
        }

        public static void OnLoad()
        {
            GameConsole.Console.singleton.AddLog("Voice049 activated, thanks for azgard", Color.green, false);
            HarmonyInstance.Create("com.grom_amg.voice049").PatchAll(Assembly.GetExecutingAssembly());
            Mod._customNetworkManager = GameObject.FindObjectOfType<CustomNetworkManager>();
        }

        private static CustomNetworkManager _customNetworkManager;
    }
    [HarmonyPatch(typeof(Radio))]
    [HarmonyPatch("SetRelationship")]
    public static class RadioPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo right = AccessTools.Field(typeof(Radio), "voiceInfo");
            FieldInfo right2 = AccessTools.Field(typeof(Radio.VoiceInfo), "isAliveHuman");
            FieldInfo right3 = AccessTools.Field(typeof(Radio.VoiceInfo), "isSCP");
            MethodInfo getMethod = AccessTools.Property(typeof(Radio), "gameObject").GetGetMethod();
            MethodInfo methodInfo = AccessTools.Method(typeof(Mod), "VoiceCond", null, null);
            MethodInfo methodInfo2 = AccessTools.Method(typeof(Mod), "VoiceCond2", null, null);
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldarg_0 && list[i + 1].opcode == OpCodes.Ldfld && (FieldInfo)list[i + 1].operand == right && list[i + 2].opcode == OpCodes.Ldfld && (FieldInfo)list[i + 2].operand == right2)
                {
                    List<CodeInstruction> collection = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Ldarg_0, null),
                        new CodeInstruction(OpCodes.Call, getMethod),
                        new CodeInstruction(OpCodes.Call, methodInfo),
                        new CodeInstruction(OpCodes.Brtrue, list[i + 3].operand)
                    };
                    list.InsertRange(i + 4, collection);
                }
                else if (list[i].opcode == OpCodes.Ldarg_0 && list[i + 1].opcode == OpCodes.Ldfld && (FieldInfo)list[i + 1].operand == right && list[i + 2].opcode == OpCodes.Ldfld && (FieldInfo)list[i + 2].operand == right3)
                {
                    List<CodeInstruction> collection2 = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Call, methodInfo2),
                        new CodeInstruction(OpCodes.Brtrue, list[i + 3].operand),
                        new CodeInstruction(OpCodes.Ldarg_0, null),
                        new CodeInstruction(OpCodes.Call, getMethod),
                        new CodeInstruction(OpCodes.Call, methodInfo),
                        new CodeInstruction(OpCodes.Brtrue, list[i + 3].operand)
                    };
                    list.InsertRange(i + 8, collection2);
                    break;
                }
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }
}
