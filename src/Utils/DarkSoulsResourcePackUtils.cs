
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Terraria;

using Terraria.ModLoader;

namespace DarkSouls.Utils
{
    public static class DarkSoulsResourcePack
    {
        public const string ResourcePackFolderName = "DarkSoulsResourcesPack_0495685";

        public const string ImagesSHA256 = "5e468a89ba95605c7e8422514b091c5fe84f8032894b5c83fd0579f9d862e1b5";
        public const string SoundsSHA256 = "577473590678037affc4b5f396d8d8967458d1fff5de62439aa82aafd63b6149";
        public const string NoHurtSoundsSHA256 = "5bb20c00949883297e04e1cab0253ae4e38d973ff82950ac2113fd25fb56b3b8";
        public const string MusicSHA256 = "baed8df6e36970de5c8768fb441ef90c93d34ad92fed3d5fcaac4fba7d8a4861";
        public const string NoMusicSHA256 = "cc3ed8ef723758155e139a09761731b7d0486fc9b7c2dfc6e00777ce0809b178";
        public const string IconSHA256 = "56764c2dace5ce5d5a07ff12b378325f166c6b07753716cb871f6dd1fc98fd54";
        public const string JsonSHA256 = "76183e1429c7fccdff10bd19dfd61bf5fec9a06110c1af222d426798e862405c";

        public static int SortingOrder = -1;
        public static bool IsEnabled = false;
        public static bool IsInstalled
        {
            get
            {
                string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
                string soundsSHA256 = GetDirectoryHash(Path.Combine(resourcePackPath, "Content", "Sounds"));
                string musicSHA256 = GetDirectoryHash(Path.Combine(resourcePackPath, "Content", "Music"));

                if (Directory.Exists(resourcePackPath))
                {
                    return
                        GetFileHash(Path.Combine(resourcePackPath, "icon.png")) == IconSHA256 &&
                        GetFileHash(Path.Combine(resourcePackPath, "pack.json")) == JsonSHA256 &&
                        GetDirectoryHash(Path.Combine(resourcePackPath, "Content", "Images")) == ImagesSHA256 &&
                        (soundsSHA256 == SoundsSHA256 || soundsSHA256 == NoHurtSoundsSHA256) &&
                        (musicSHA256 == MusicSHA256 || musicSHA256 == NoMusicSHA256);
                }
                return false;
            }
        }

        public static bool IsOverrideHurtSounds
        {
            get
            {
                string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
                string soundsSHA256 = GetDirectoryHash(Path.Combine(resourcePackPath, "Content", "Sounds"));
                if (Directory.Exists(resourcePackPath))
                    return soundsSHA256 == SoundsSHA256;
                return true;
            }
        }

        public static readonly HashSet<string> HurtSoundFiles = new([
            "Female_Hit_0.xnb", "Female_Hit_1.xnb", "Female_Hit_2.xnb",
            "Player_Hit_0.xnb", "Player_Hit_1.xnb", "Player_Hit_2.xnb",
            "Player_Killed.xnb"
        ]);

        public static readonly HashSet<string> MusicFiles = new([
            "Music_5.ogg", "Music_12.ogg", "Music_13.ogg",
            "Music_17.ogg", "Music_24.ogg", "Music_25.ogg",
            "Music_38.ogg", "Music_57.ogg", "Music_58.ogg",
            "Music_89.ogg", "Music_90.ogg"
        ]);


        public static void EnableOverrideResources(bool hurtSounds = false, bool music = false)
        {
            string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
            if (!Directory.Exists(resourcePackPath))
                return;

            string path = "";
            if (hurtSounds)
                path = Path.Combine(resourcePackPath, "Content", "Sounds");
            else if (music)
                path = Path.Combine(resourcePackPath, "Content", "Music");

            if (!Directory.Exists(path))
                return;

            foreach (string filePath in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileName(filePath);

                if (!fileName.StartsWith("_"))
                    continue;

                string targetName = fileName.Substring(1);
                if ((hurtSounds && !HurtSoundFiles.Contains(targetName)) || (music && !MusicFiles.Contains(targetName)))
                    continue;

                string targetPath = Path.Combine(path, targetName);

                if (File.Exists(targetPath))
                {
                    ConsoleUtils.WriteLine($"[DarkSouls] Failed to rename {fileName}, file {targetName} already exists!", ConsoleColor.DarkRed);
                    continue;
                }

                File.Move(filePath, targetPath);
                ConsoleUtils.WriteLine($"[DarkSouls] Renamed {fileName} -> {targetName}", ConsoleColor.DarkGreen);
            }
        }

        public static void DisableOverrideResources(bool hurtSounds = false, bool music = false)
        {
            string resourcePackPath = Path.Combine(Main.SavePath, "ResourcePacks", ResourcePackFolderName);
            if (!Directory.Exists(resourcePackPath))
                return;

            string path = "";
            if (hurtSounds)
                path = Path.Combine(resourcePackPath, "Content", "Sounds");
            else if (music)
                path = Path.Combine(resourcePackPath, "Content", "Music");

            if (!Directory.Exists(path))
                return;

            foreach (string filePath in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileName(filePath);

                if (fileName.StartsWith("_"))
                    continue;

                if ((hurtSounds && !HurtSoundFiles.Contains(fileName)) || (music && !MusicFiles.Contains(fileName)))
                    continue;

                string targetName = "_" + fileName;
                string targetPath = Path.Combine(path, targetName);

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
            
            try
            {
                if (Directory.Exists(destFolder))
                    Directory.Delete(destFolder, true);
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLine($"[DarkSouls] Failed to extract the resource pack: {ex}", ConsoleColor.DarkRed);
                return;
            }


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
            try
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
            catch (Exception ex)
            {
                ConsoleUtils.WriteLine($"[DarkSouls] Failed to compute SHA256 hash for directory at path: {directoryPath} [{ex}]", ConsoleColor.DarkRed);
                return "";
            }
        }

        public static string GetFileHash(string filePath)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLine($"[DarkSouls] Failed to compute SHA256 hash for file at path: {filePath} [{ex}]", ConsoleColor.DarkRed);
                return "";
            }

        }

        public class DarkSoulsResourcePackInfoUpdater : ModSystem
        {
            public override void OnLocalizationsLoaded() => GetInfo();
        }
    }
}
