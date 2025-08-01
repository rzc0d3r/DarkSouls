using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using Terraria;
using Terraria.ModLoader;

namespace DarkSouls.Utils
{
    public static class DarkSoulsResourcePack
    {
        public const string ResourcePackFolderName = "DarkSoulsResourcesPack_0495685";
        public const string OriginalSHA256 = "e4f47acd63ada7a692ab30c46df7b7a55f9f4b02d708d7c779285792d5f89424";
        public const string NoHurtSHA256 = "6fa6493ed341e20faca64f9f448744ebcbb1c7f5bceecdb8ba6aa326d28fe5c9";
        public static int SortingOrder = -1;
        public static bool IsEnabled = false;
        public static bool IsInstalled
        {
            get
            {
                string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);

                if (Directory.Exists(resourcePackPath))
                {
                    string sha256 = GetDirectoryHash(resourcePackPath);
                    return sha256 == OriginalSHA256 || sha256 == NoHurtSHA256;
                }
                return false;
            }
        }

        public static bool IsOverrideHurtSounds
        {
            get
            {
                string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
                if (Directory.Exists(resourcePackPath))
                    return GetDirectoryHash(resourcePackPath) != OriginalSHA256;
                return true;
            }
        }

        public static readonly HashSet<string> HurtSoundFiles = new([
            "Female_Hit_0.xnb", "Female_Hit_1.xnb", "Female_Hit_2.xnb",
            "Player_Hit_0.xnb", "Player_Hit_1.xnb", "Player_Hit_2.xnb",
            "Player_Killed.xnb"
        ]);


        public static void EnableOverrideHurtSound()
        {
            string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
            if (!Directory.Exists(resourcePackPath))
                return;

            string pathToSounds = Path.Combine(resourcePackPath, "Content", "Sounds");
            if (!Directory.Exists(pathToSounds))
                return;

            foreach (string filePath in Directory.GetFiles(pathToSounds))
            {
                string fileName = Path.GetFileName(filePath);

                if (!fileName.StartsWith("_"))
                    continue;

                string targetName = fileName.Substring(1);
                if (!HurtSoundFiles.Contains(targetName))
                    continue;

                string targetPath = Path.Combine(pathToSounds, targetName);

                if (File.Exists(targetPath))
                {
                    ConsoleUtils.WriteLine($"[DarkSouls] Failed to rename {fileName}, file {targetName} already exists!", ConsoleColor.DarkRed);
                    continue;
                }

                File.Move(filePath, targetPath);
                ConsoleUtils.WriteLine($"[DarkSouls] Renamed {fileName} -> {targetName}", ConsoleColor.DarkGreen);
            }
        }

        public static void DisableOverrideHurtSound()
        {
            string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
            if (!Directory.Exists(resourcePackPath))
                return;

            string pathToSounds = Path.Combine(resourcePackPath, "Content", "Sounds");
            if (!Directory.Exists(pathToSounds))
                return;

            foreach (string filePath in Directory.GetFiles(pathToSounds))
            {
                string fileName = Path.GetFileName(filePath);

                if (fileName.StartsWith("_"))
                    continue;

                if (!HurtSoundFiles.Contains(fileName))
                    continue;

                string targetName = "_" + fileName;
                string targetPath = Path.Combine(pathToSounds, targetName);

                if (File.Exists(targetPath))
                {
                    ConsoleUtils.WriteLine($"[DarkSouls] Failed to rename {fileName}, file {targetName} already exists!", ConsoleColor.DarkRed);
                    continue;
                }

                File.Move(filePath, targetPath);
                ConsoleUtils.WriteLine($"[DarkSouls] Renamed {fileName} -> {targetName}", ConsoleColor.DarkGreen);
            }
        }

        public static void GetInfo()
        {
            if (!Main.Configuration.Contains("ResourcePacks"))
                return;

            var data = Main.Configuration.Get<JArray>("ResourcePacks", null);
            if (data == null)
                return;

            foreach (JToken token in data)
            {
                try
                {
                    if ((string)token["FileName"] != ResourcePackFolderName)
                        continue;

                    IsEnabled = (bool)token["Enabled"];
                    SortingOrder = (int)token["SortingOrder"];
                    break;
                }
                catch (Exception ex)
                {
                    IsEnabled = false;
                    SortingOrder = -1;
                    ConsoleUtils.WriteLine($"[DarkSouls] Failed to read resource pack info: {ex}", ConsoleColor.DarkRed);
                }
            }
        }

        public static void Install()
        {
            string sourcePrefix = $"ResourcePack/";
            string destFolder = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);

            if (Directory.Exists(destFolder))
                Directory.Delete(destFolder, true);

            if (!ModLoader.TryGetMod("DarkSouls", out Mod mod))
                return;

            foreach (string fileName in mod.GetFileNames())
            {
                if (!fileName.StartsWith(sourcePrefix))
                    continue;

                string relativePath = fileName.Substring(sourcePrefix.Length);
                string fullDestPath = Path.Combine(destFolder, relativePath).Replace("._png", ".png");

                Directory.CreateDirectory(Path.GetDirectoryName(fullDestPath)!);
                using Stream stream = mod.GetFileStream(fileName);
                using FileStream fs = File.OpenWrite(fullDestPath);
                stream.CopyTo(fs);
            }

            ConsoleUtils.WriteLine("[DarkSouls] Embedded resource pack was successfully extracted!", ConsoleColor.DarkGreen);
        }

        public static string GetDirectoryHash(string directoryPath)
        {
            using var sha256 = SHA256.Create();
            var files = Directory.GetFiles(directoryPath, "*", System.IO.SearchOption.AllDirectories).OrderBy(p => p);

            using var ms = new MemoryStream();

            string rootName = Path.GetFileName(Path.GetFullPath(directoryPath));
            byte[] rootNameBytes = Encoding.UTF8.GetBytes(rootName);
            ms.Write(rootNameBytes, 0, rootNameBytes.Length);

            foreach (string filePath in files)
            {
                byte[] pathBytes = Encoding.UTF8.GetBytes(filePath.Replace(directoryPath, ""));
                ms.Write(pathBytes, 0, pathBytes.Length);

                byte[] contentBytes = File.ReadAllBytes(filePath);
                ms.Write(contentBytes, 0, contentBytes.Length);
            }

            ms.Position = 0;
            byte[] hash = sha256.ComputeHash(ms);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public class DarkSoulsResourcePackInfoUpdater : ModSystem
        {
            public override void OnLocalizationsLoaded() => GetInfo();
        }


        //GetDirectoryHash



        //foreach (string fileName in Directory.GetFiles(pathToSounds))
        //{
        //    FileInfo fileInfo = new FileInfo(fileName);
        //    if (fileInfo.Name.StartsWith('_') && fileInfo.Name )
        //}
    }
}
