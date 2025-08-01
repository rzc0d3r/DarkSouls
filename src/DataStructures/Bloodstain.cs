using Terraria;
using Terraria.ModLoader.IO;

using Microsoft.Xna.Framework;

namespace DarkSouls.DataStructures
{
    public sealed class Bloodstain
    {
        public Vector2 position;
        public int humanity;
        public long souls;
        public string worldGUID;

        public Bloodstain(Vector2 position, int lostHumanity, long lostSouls, Player player)
        {
            this.position = position;
            humanity = lostHumanity;
            souls = lostSouls;
            worldGUID = Main.ActiveWorldFileData.UniqueId.ToString();
        }

        public Bloodstain()
        {
            position = Vector2.Zero;
            humanity = -1;
            souls = -1;
            worldGUID = "";
        }

        public TagCompound ToTag()
        {
            return new TagCompound
            {
                ["position"] = position,
                ["humanity"] = humanity,
                ["souls"] = souls,
                ["worldGUID"] = worldGUID
            };
        }

        public static Bloodstain FromTag(TagCompound tag)
        {
            long souls;
            object rawSouls = tag["souls"];
            if (rawSouls is long l)
                souls = l;
            else if (rawSouls is int i)
                souls = i;
            else
                souls = 0;

            return new Bloodstain
            {
                position = tag.Get<Vector2>("position"),
                humanity = tag.GetInt("humanity"),
                souls = souls,
                worldGUID = tag.GetString("worldGUID")
            };
        }

        public override string ToString()
        {
            return "Souls:\n" +
                $"    position: {position}\n" +
                $"    humanity: {humanity}\n" +
                $"    souls: {souls}\n" +
                $"    worldGUID: {worldGUID}\n";
        }
    }
}
