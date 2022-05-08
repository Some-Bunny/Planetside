using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Planetside;
using UnityEngine;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Collections;
using System.Collections.ObjectModel;

namespace ItemAPI
{
    public static class Tools
    {
        public static bool verbose = false;
        private static string defaultLog = Path.Combine(ETGMod.ResourcesDirectory, "planetside.txt");
        public static string modID = "PSOG";

        public static AssetBundle sharedAuto1 = ResourceManager.LoadAssetBundle("shared_auto_001");
        public static AssetBundle sharedAuto2 = ResourceManager.LoadAssetBundle("shared_auto_002");

        public static void Init()
        {
            if (File.Exists(defaultLog)) File.Delete(defaultLog);
        }

        public static void Print<T>(T obj, string color = "FFFFFF", bool force = false)
        {
            if (verbose || force)
                ETGModConsole.Log($"<color=#{color}>{modID}: {obj.ToString()}</color>");

            Log(obj.ToString());
        }

        public static void PrintRaw<T>(T obj, bool force = false)
        {
            if (verbose || force)
                ETGModConsole.Log(obj.ToString());

            Log(obj.ToString());
        }

        public static void PrintError<T>(T obj, string color = "FF0000")
        {
            ETGModConsole.Log($"<color=#{color}>{modID}: {obj.ToString()}</color>");

            Log(obj.ToString());
        }

        public static void PrintException(Exception e, string color = "FF0000")
        {
            ETGModConsole.Log($"<color=#{color}>{modID}: {e.Message}</color>");
            ETGModConsole.Log(e.StackTrace);

            Log(e.Message);
            Log("\t" + e.StackTrace);
        }

        public static void Log<T>(T obj)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(ETGMod.ResourcesDirectory, defaultLog), true))
            {
                writer.WriteLine(obj.ToString());
            }
        }

        public static void Log<T>(T obj, string fileName)
        {
            if (!verbose) return;
            using (StreamWriter writer = new StreamWriter(Path.Combine(ETGMod.ResourcesDirectory, fileName), true))
            {
                writer.WriteLine(obj.ToString());
            }
        }

        public static void Dissect(this GameObject obj)
        {
            Tools.Print(obj.name + " Components:");
            foreach (var comp in obj.GetComponents<Component>())
            {
                Tools.Print("    " + comp.GetType());
            }
        }

        public static void ShowHitBox(this SpeculativeRigidbody body)
        {
            PixelCollider hitbox = body.HitboxPixelCollider;
            GameObject hitboxDisplay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hitboxDisplay.name = "HitboxDisplay";
            hitboxDisplay.transform.SetParent(body.transform);

            Tools.Print($"{body.name}");
            Tools.Print($"    Offset: {hitbox.Offset}, Dimesions: {hitbox.Dimensions}");
            hitboxDisplay.transform.localScale = new Vector3((float)hitbox.Dimensions.x / 16f, (float)hitbox.Dimensions.y / 16f, 1f);
            Vector3 localPosition = new Vector3((float)hitbox.Offset.x + (float)hitbox.Dimensions.x * 0.5f, (float)hitbox.Offset.y + (float)hitbox.Dimensions.y * 0.5f, -16f) / 16f;
            hitboxDisplay.transform.localPosition = localPosition;
        }

        public static void HideHitBox(this SpeculativeRigidbody body)
        {
            var display = body.transform.Find("HitboxDisplay");
            if (display)
                GameObject.Destroy(display);
        }

        public static void ExportTexture(Texture texture)
        {
            File.WriteAllBytes(Path.Combine(ETGMod.ResourcesDirectory, texture.name + ".png"), ((Texture2D)texture).EncodeToPNG());
        }

        public static void LogPropertiesAndFields<T>(this T obj, string header = "", bool debug = false)
        {

            ETGModConsole.Log(header, debug);
            ETGModConsole.Log("=======================", debug);
            if (obj == null) { ETGModConsole.Log("LogPropertiesAndFields: Null object", debug); return; }
            Type type = obj.GetType();
            ETGModConsole.Log($"Type: {type}", debug);
            PropertyInfo[] pinfos = type.GetProperties();
            ETGModConsole.Log($"{typeof(T)} Properties: ", debug);
            foreach (var pinfo in pinfos)
            {
                try
                {
                    var value = pinfo.GetValue(obj, null);
                    string valueString = value.ToString();
                    bool isList = obj?.GetType().GetGenericTypeDefinition() == typeof(List<>);
                    if (isList)
                    {
                        var list = value as List<object>;
                        valueString = $"List[{list.Count}]";
                        foreach (var subval in list)
                        {
                            valueString += "\n\t\t" + subval.ToString();
                        }
                    }
                    ETGModConsole.Log($"\t{pinfo.Name}: {valueString}", debug);
                }
                catch { }
            }
            ETGModConsole.Log($"{typeof(T)} Fields: ", debug);
            FieldInfo[] finfos = type.GetFields();
            foreach (var finfo in finfos)
            {
                ETGModConsole.Log($"\t{finfo.Name}: {finfo.GetValue(obj)}", debug);
            }
        }
    }
}
