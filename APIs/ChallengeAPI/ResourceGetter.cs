using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChallengeAPI
{
    /// <summary>
    /// A class that has the methods to load resources from your project.
    /// </summary>
    public static class ResourceGetter
    {
        /// <summary>
        /// Initializes <see cref="ResourceGetter"/>.
        /// </summary>
        public static void Init()
        {
            if (m_initialized)
            {
                return;
            }
            CurrentAssembly = Assembly.GetCallingAssembly();
            m_initialized = true;
        }

        /// <summary>
        /// Unloads <see cref="ResourceGetter"/>.
        /// </summary>
        public static void Unload()
        {
            if (!m_initialized)
            {
                return;
            }
            CurrentAssembly = null;
            m_initialized = false;
        }

        /// <summary>
        /// Gets the bytes of an embedded resource in your project.
        /// </summary>
        /// <param name="resourcePath">The filepath to the embedded resource in your project.</param>
        /// <returns>The bytes of the embedded resource.</returns>
        public static byte[] GetBytesFromResource(string resourcePath)
        {
            resourcePath = resourcePath.Replace("/", ".");
            resourcePath = resourcePath.Replace("\\", ".");
            if (CurrentAssembly == null)
            {
                Init();
            }
            using (Stream stream = CurrentAssembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    return null;
                }
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Gets a <see cref="Texture2D"/> from an embedded image in your project.
        /// </summary>
        /// <param name="resourcePath">The filepath to the embedded image in your project.</param>
        /// <returns>The loaded <see cref="Texture2D"/></returns>
        public static Texture2D GetTextureFromResource(string resourcePath)
        {
            if (!resourcePath.ToLower().EndsWith(".png"))
            {
                resourcePath += ".png";
            }
            byte[] bytes = GetBytesFromResource(resourcePath);
            if(bytes == null)
            {
                ETGModConsole.Log("Resource Getter couldn't find anything using the filepath " + resourcePath + ". Maybe you got the filepath wrong, forgot to add the file or forgot to embed it?");
                return null;
            }
            Texture2D result = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            ImageConversion.LoadImage(result, bytes);
            result.filterMode = FilterMode.Point;
            string name = resourcePath.Substring(0, resourcePath.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            result.name = name;
            return result;
        }

        private static bool m_initialized;
        /// <summary>
        /// The assembly that your mod uses.
        /// </summary>
        public static Assembly CurrentAssembly;
    }
}
