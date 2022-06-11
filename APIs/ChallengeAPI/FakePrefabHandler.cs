using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChallengeAPI
{
    /// <summary>
    /// Class used to control fake prefabs.
    /// </summary>
    public static class FakePrefabHandler
    {
        /// <summary>
        /// Initializes <see cref="FakePrefabHandler"/>.
        /// </summary>
        public static void Init()
        {
            if (m_initialized)
            {
                return;
            }
            fakePrefabs = new HashSet<GameObject>();
            instantiateOPR = new Hook(
                typeof(UnityEngine.Object).GetMethod("Instantiate", new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion) }),
                typeof(FakePrefabHandler).GetMethod("InstantiateOPR")
            );
            instantiateOPRP = new Hook(
                typeof(UnityEngine.Object).GetMethod("Instantiate", new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion), typeof(Transform) }),
                typeof(FakePrefabHandler).GetMethod("InstantiateOPRP")
            );
            instantiateO = new Hook(
                typeof(UnityEngine.Object).GetMethod("orig_Instantiate", new Type[] { typeof(UnityEngine.Object) }),
                typeof(FakePrefabHandler).GetMethod("InstantiateO")
            );
            instantiateOP = new Hook(
                typeof(UnityEngine.Object).GetMethod("Instantiate", new Type[] { typeof(UnityEngine.Object), typeof(Transform) }),
                typeof(FakePrefabHandler).GetMethod("InstantiateOP")
            );
            instantiateOPI = new Hook(
                typeof(UnityEngine.Object).GetMethod("Instantiate", new Type[] { typeof(UnityEngine.Object), typeof(Transform), typeof(bool) }),
                typeof(FakePrefabHandler).GetMethod("InstantiateOPI")
            );
            m_initialized = true;
        }

        /// <summary>
        /// Unloads <see cref="FakePrefabHandler"/>.
        /// </summary>
        public static void Unload()
        {
            if (!m_initialized)
            {
                return;
            }
            fakePrefabs?.Clear();
            fakePrefabs = null;
            instantiateOPR?.Dispose();
            instantiateOPRP?.Dispose();
            instantiateO?.Dispose();
            instantiateOP?.Dispose();
            instantiateOPI?.Dispose();
            m_initialized = false;
        }

        /// <summary>
        /// Marks <paramref name="component"/>'s <see cref="GameObject"/> as a fake prefab. 
        /// </summary>
        /// <param name="component">The <see cref="Component"/> the object of which will get marked as a fake prefab.</param>
        public static void MarkAsFakePrefab(this Component component)
        {
            if(component.gameObject != null)
            {
                component.gameObject.MarkAsFakePrefab();
            }
        }

        /// <summary>
        /// Marks <paramref name="go"/> as a fake prefab.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> that will get marked as a fake prefab.</param>
        public static void MarkAsFakePrefab(this GameObject go)
        {
            if(fakePrefabs == null)
            {
                Init();
            }
            fakePrefabs.Add(go);
        }

        /// <summary>
        /// Unmarks <paramref name="component"/>'s <see cref="GameObject"/> as a fake prefab. 
        /// </summary>
        /// <param name="component">The <see cref="Component"/> the object of which will get unmarked as a fake prefab.</param>
        public static void UnmarkAsFakePrefab(this Component component)
        {
            if (component.gameObject != null)
            {
                component.gameObject.UnmarkAsFakePrefab();
            }
        }

        /// <summary>
        /// Unmarks <paramref name="go"/> as a fake prefab.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> that will get unmarked as a fake prefab.</param>
        public static void UnmarkAsFakePrefab(this GameObject go)
        {
            if(fakePrefabs != null)
            {
                if (fakePrefabs.Contains(go))
                {
                    fakePrefabs.Remove(go);
                }
            }
        }

        /// <summary>
        /// Checks if <paramref name="component"/>'s <see cref="GameObject"/> is a fake prefab.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> the object of which will get checked.</param>
        /// <returns><see langword="true"/> if the <see cref="GameObject"/> of <paramref name="component"/> is a fake prefab, <see langword="false"/> otherwise.</returns>
        public static bool IsFakePrefab(this Component component)
        {
            return component.gameObject.IsFakePrefab();
        }

        /// <summary>
        /// Checks if <paramref name="go"/> is a fake prefab.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> that will get checked.</param>
        /// <returns><see langword="true"/> if <paramref name="go"/> is a fake prefab, <see langword="false"/> otherwise.</returns>
        public static bool IsFakePrefab(this GameObject go)
        {
            return fakePrefabs.Contains(go);
        }

        public static UnityEngine.Object MaybeInstantiateFakePrefab(UnityEngine.Object origO, UnityEngine.Object newO)
        {
            if(origO != null && newO != null && fakePrefabs != null)
            {
                if(origO is GameObject && fakePrefabs.Contains(origO as GameObject))
                {
                    (newO as GameObject).SetActive(true);
                }
                else if(origO is Component && (origO as Component).gameObject != null && fakePrefabs.Contains((origO as Component).gameObject))
                {
                    (newO as Component).gameObject.SetActive(true);
                }
            }
            return newO;
        }

        public static UnityEngine.Object InstantiateOPR(Func<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object> orig, UnityEngine.Object origO, Vector3 pos, Quaternion rotation)
        {
            return MaybeInstantiateFakePrefab(origO, orig(origO, pos, rotation));
        }

        public static UnityEngine.Object InstantiateOPRP(Func<UnityEngine.Object, Vector3, Quaternion, Transform, UnityEngine.Object> orig, UnityEngine.Object origO, Vector3 pos, Quaternion rotation, Transform parent)
        {
            return MaybeInstantiateFakePrefab(origO, orig(origO, pos, rotation, parent));
        }

        public static UnityEngine.Object InstantiateO(Func<UnityEngine.Object, UnityEngine.Object> orig, UnityEngine.Object origO)
        {
            return MaybeInstantiateFakePrefab(origO, orig(origO));
        }

        public static UnityEngine.Object InstantiateOP(Func<UnityEngine.Object, Transform, UnityEngine.Object> orig, UnityEngine.Object origO, Transform parent)
        {
            return MaybeInstantiateFakePrefab(origO, orig(origO, parent));
        }

        public static UnityEngine.Object InstantiateOPI(Func<UnityEngine.Object, Transform, bool, UnityEngine.Object> orig, UnityEngine.Object origO, Transform parent, bool b)
        {
            return MaybeInstantiateFakePrefab(origO, orig(origO, parent, b));
        }

        public static Hook instantiateOPR;
        public static Hook instantiateOPRP;
        public static Hook instantiateO;
        public static Hook instantiateOP;
        public static Hook instantiateOPI;
        public static HashSet<GameObject> fakePrefabs;
        private static bool m_initialized;
    }
}
