using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

namespace DarkSouls.Utils
{
    public static class SoundUtils
    {
        public static SoundStyle GetRandomDamageSound(out int soundIndex, bool male = true, int excludeSoundIndex = -1)
        {
            List<SoundStyle> list = male ? DarkSouls.dsMaleDamageSounds : DarkSouls.dsFemaleDamageSounds;
            int index;
            do
            {
                index = Main.rand.Next(list.Count);
            } while (index == excludeSoundIndex);
            soundIndex = index;
            return list[index];
        }
    }
}
