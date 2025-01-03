﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SGUI;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;

namespace ItemAPI
{
    public static class ResourceExtractor
    {
        private static string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");
        /// <summary>
        /// Converts all png's in a folder to a list of Texture2D objects
        /// </summary>
        public static List<Texture2D> GetTexturesFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Tools.PrintError(directoryPath + " not found.");
                return null;
            }

            List<Texture2D> textures = new List<Texture2D>();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (!filePath.EndsWith(".png")) continue;

                Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), Path.GetFileName(filePath).Replace(".png", ""));
                textures.Add(texture);
            }
            return textures;
        }

        /// <summary>
        /// Creates a Texture2D from a file in the sprites directory
        /// </summary>
        public static Texture2D GetTextureFromFile(string fileName, string extension = ".png")
        {
            fileName = fileName.Replace(extension, "");
            string filePath = Path.Combine(spritesDirectory, fileName + extension);
            if (!File.Exists(filePath))
            {
                Tools.PrintError(filePath + " not found.");
                return null;
            }
            Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), fileName);
            return texture;
        }

        /// <summary>
        /// Retuns a list of sprite collections in the sprite folder
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCollectionFiles()
        {
            List<string> collectionNames = new List<string>();
            foreach (string filePath in Directory.GetFiles(spritesDirectory))
            {
                if (filePath.EndsWith(".png"))
                {
                    collectionNames.Add(Path.GetFileName(filePath).Replace(".png", ""));
                }
            }
            return collectionNames;
        }

        /// <summary>
        /// Converts a byte array into a Texture2D
        /// </summary>
        public static Texture2D BytesToTexture(byte[] bytes, string resourceName)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;
            texture.name = resourceName;
            return texture;
        }

        public static string[] GetLinesFromEmbeddedResource(string filePath)
        {
            string allLines = BytesToString(ExtractEmbeddedResource(filePath));
            return allLines.Split('\n');
        }

        public static string[] GetLinesFromFile(string filePath)
        {
            string allLines = BytesToString(File.ReadAllBytes(filePath));
            return allLines.Split('\n');
        }

        public static string BytesToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns a list of folders in the ETG resources directory
        /// </summary>
        public static List<String> GetResourceFolders()
        {
            List<String> dirs = new List<String>();
            string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");

            if (Directory.Exists(spritesDirectory))
            {
                foreach (String directory in Directory.GetDirectories(spritesDirectory))
                {
                    dirs.Add(Path.GetFileName(directory));
                }
            }
            return dirs;
        }

        /// <summary>
        /// Converts an embedded resource to a byte array
        /// </summary>
        public static byte[] ExtractEmbeddedResource(String filePath)
        {
            filePath = filePath.Replace("/", ".");
            filePath = filePath.Replace("\\", ".");
            var baseAssembly = Assembly.GetCallingAssembly();
            using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filePath))
            {
                if (resFilestream == null)
                {
                    return null;
                }
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        /// <summary>
        /// Converts an embedded resource to a Texture2D object
        /// </summary>
        public static Texture2D GetTextureFromResource(string resourceName)
        {
            string file = resourceName;
            byte[] bytes = ExtractEmbeddedResource(file);
            if (bytes == null)
            {
                Tools.PrintError("No bytes found in " + file);
                return null;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            //FlipTexture(ref texture);

            texture.filterMode = FilterMode.Point;

            string name = file.Substring(0, file.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            texture.name = name;

            return texture;
        }


        public static void FlipTexture(ref Texture2D original)
        {
            int textureWidth = original.width;
            int textureHeight = original.height;

            Color[] colorArray = original.GetPixels();

            

            for (int j = 0; j < textureHeight; j++)
            {
                //ETGModConsole.Log($"H: {textureHeight} : {textureWidth}");

                int rowStart = 0;
                int rowEnd = textureWidth - 1;

                
                while (rowStart < rowEnd)
                {
                    Color hold = colorArray[(j * textureWidth) + (rowStart)];
                    colorArray[(j * textureWidth) + (rowStart)] = colorArray[(j * textureWidth) + (rowEnd)];
                    colorArray[(j * textureWidth) + (rowEnd)] = hold;
                    rowStart++;
                    rowEnd--;
                }
                
            }

            //Texture2D finalFlippedTexture = new Texture2D(original.width, original.height);
            original.SetPixels(colorArray);
        }


        public static Texture2D AnotherGetTextureFromResource(string resourceName)
        {
            byte[] array = ResourceExtractor.ExtractEmbeddedResource(resourceName);
            bool flag = array == null;
            Texture2D result;
            if (flag)
            {
                Tools.PrintError<string>("No bytes found in " + resourceName, "FF0000");
                result = null;
            }
            else
            {
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                texture2D.LoadImage(array);
                texture2D.filterMode = FilterMode.Point;
                string text = resourceName.Substring(0, resourceName.LastIndexOf('.'));
                bool flag2 = text.LastIndexOf('.') >= 0;
                if (flag2)
                {
                    text = text.Substring(text.LastIndexOf('.') + 1);
                }
                texture2D.name = text;
                result = texture2D;
            }
            return result;
        }


        /// <summary>
        /// Returns a list of the names of all embedded resources
        /// </summary>
        public static string[] GetResourceNames()
        {
            var baseAssembly = Assembly.GetCallingAssembly();
            string[] names = baseAssembly.GetManifestResourceNames();
            if (names == null)
            {
                ETGModConsole.Log("No manifest resources found.");
                return null;
            }
            return names;
        }
    }
}
