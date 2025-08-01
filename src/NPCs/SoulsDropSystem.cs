using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using System.Collections.Generic;

namespace DarkSouls.NPCs
{
    public class SoulsDropSystem : GlobalNPC
    {
        public static readonly HashSet<int> NPCIDBlackList = new()
        {
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            NPCID.EaterofWorldsHead,

            NPCID.BrainofCthulhu,
            NPCID.Creeper,

            NPCID.KingSlime,
            NPCID.SlimeSpiked,

            NPCID.EyeofCthulhu,
            NPCID.ServantofCthulhu,
            NPCID.QueenBee,

            NPCID.SkeletronHand,
            NPCID.SkeletronHead,

            NPCID.WallofFlesh,
            NPCID.WallofFleshEye,
            NPCID.TheHungry,
            NPCID.TheHungryII,
            NPCID.LeechBody,
            NPCID.LeechHead,
            NPCID.LeechTail,

            NPCID.QueenSlimeBoss,
            NPCID.QueenSlimeMinionBlue,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple,

            NPCID.Retinazer,
            NPCID.Spazmatism,

            NPCID.TheDestroyer,
            NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail,
            NPCID.Probe,

            NPCID.SkeletronPrime,
            NPCID.PrimeCannon,
            NPCID.PrimeLaser,
            NPCID.PrimeSaw,
            NPCID.PrimeVice,

            NPCID.Plantera,
            NPCID.PlanterasTentacle,

            NPCID.Golem,
            NPCID.GolemFistLeft,
            NPCID.GolemFistRight,
            NPCID.GolemHead,
            NPCID.GolemHeadFree,

            NPCID.DukeFishron,
            NPCID.Sharkron,
            NPCID.Sharkron2,

            NPCID.HallowBoss,

            NPCID.CultistBoss,
            NPCID.CultistBossClone,
            NPCID.CultistDragonBody1,
            NPCID.CultistDragonBody2,
            NPCID.CultistDragonBody3,
            NPCID.CultistDragonBody4,
            NPCID.CultistDragonHead,
            NPCID.CultistDragonTail,

            NPCID.LunarTowerSolar,
            NPCID.SolarCrawltipedeBody,
            NPCID.SolarCrawltipedeBody,
            NPCID.SolarCrawltipedeTail,

            NPCID.LunarTowerVortex,
            
            NPCID.LunarTowerNebula,

            NPCID.LunarTowerStardust,
            NPCID.StardustCellSmall,

            NPCID.MoonLordCore,
            NPCID.MoonLordFreeEye,
            NPCID.MoonLordHand,
            NPCID.MoonLordHead,
            NPCID.MoonLordLeechBlob

        };
       
        public int GetSoulsByNPC(NPC npc, out bool boss)
        {
            boss = false;
            int npcID = npc.type;
            int souls = 0;

            int golemHP;
            NPC golem = new(); golem.SetDefaults(NPCID.Golem);
            NPC golemFist = new(); golemFist.SetDefaults(NPCID.GolemFistLeft);
            NPC golemHead = new(); golemHead.SetDefaults(NPCID.GolemHead);
            golemHP = golem.lifeMax + 2 * golemFist.lifeMax + golemHead.lifeMax;

            // Pre-Hardmode Bosses
            if (npcID == NPCID.KingSlime) // King Slime
            {
                NPC kingSlime = new(); kingSlime.SetDefaults(NPCID.KingSlime);
                souls = kingSlime.lifeMax;
                if (NPC.downedSlimeKing)
                    souls = (int)(kingSlime.lifeMax * 0.15f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.EyeofCthulhu) // Eye of Cthulhu
            {
                NPC eyeOfCthulhu = new(); eyeOfCthulhu.SetDefaults(NPCID.EyeofCthulhu);
                souls = eyeOfCthulhu.lifeMax;
                if (NPC.downedBoss1)
                    souls = (int)(eyeOfCthulhu.lifeMax * 0.15f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.EaterofWorldsHead) // Eater of Worlds
            {
                NPC eaterOfWorlds = new(); eaterOfWorlds.SetDefaults(NPCID.EaterofWorldsHead);
                souls = eaterOfWorlds.lifeMax;
                if (NPC.downedBoss2)
                    souls = (int)(eaterOfWorlds.lifeMax * 0.1f);
                else
                    souls = (int)(eaterOfWorlds.lifeMax * 0.5f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.BrainofCthulhu) // Brain of Cthulhu
            {
                NPC brainOfCthulhu = new(); brainOfCthulhu.SetDefaults(NPCID.BrainofCthulhu);
                souls = brainOfCthulhu.lifeMax;
                if (NPC.downedBoss2)
                    souls = (int)(souls * 0.5f); // 50% of HP (only Boss)
                else
                    souls = (int)(souls * 2.6f); // 100% of HP (Boss + Creepers = lifeMax * 2.6)
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.QueenBee) // QueenBee
            {
                NPC queenBee = new(); queenBee.SetDefaults(NPCID.QueenBee);
                souls = queenBee.lifeMax;
                if (NPC.downedQueenBee)
                    souls = (int)(souls * 0.4f);
                else
                    souls = (int)(souls * 1.2f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.Deerclops) // Deerclops
            {
                NPC deerclops = new(); deerclops.SetDefaults(NPCID.Deerclops);
                souls = deerclops.lifeMax;
                if (NPC.downedDeerclops)
                    souls = (int)(souls * 0.25f);
                else
                    souls = (int)(souls * 0.8f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.SkeletronHead) // Skeletron
            {
                NPC skeletron = new(); skeletron.SetDefaults(NPCID.SkeletronHead);
                NPC skeletronHand = new(); skeletron.SetDefaults(NPCID.SkeletronHand);
                souls = skeletron.lifeMax + 2 * skeletronHand.lifeMax;
                if (NPC.downedBoss3)
                    souls = (int)(souls * 0.4f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.WallofFleshEye || npcID == NPCID.WallofFlesh) // Wall of Flesh
            {
                NPC wallOfFlesh = new(); wallOfFlesh.SetDefaults(NPCID.WallofFlesh);
                souls = wallOfFlesh.lifeMax;
                if (Main.hardMode)
                    souls = (int)(souls * 0.4f);
                boss = true;
                return souls;
            }
            // Hardmode Bosses
            else if (npcID == NPCID.QueenSlimeBoss) // Queen Slime
            {
                NPC queenSlime = new(); queenSlime.SetDefaults(NPCID.QueenSlimeBoss);
                souls = queenSlime.lifeMax;
                if (NPC.downedQueenSlime)
                    souls = (int)(souls * 0.3f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.TheDestroyer) // Destroyer
            {
                NPC destroyer = new(); destroyer.SetDefaults(NPCID.TheDestroyer);
                souls = destroyer.lifeMax;
                if (NPC.downedMechBoss1)
                    souls = (int)(souls * 0.1f);
                else
                    souls = (int)(souls * 0.65f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.Retinazer || npcID == NPCID.Spazmatism) // Twins
            {
                if (!NPC.AnyNPCs(npcID == NPCID.Retinazer ? NPCID.Spazmatism : NPCID.Retinazer)) // Second is not alive
                {
                    NPC retinazer = new(); retinazer.SetDefaults(NPCID.Retinazer);
                    NPC spazmatism = new(); spazmatism.SetDefaults(NPCID.Spazmatism);
                    souls = retinazer.lifeMax + spazmatism.lifeMax;
                    if (NPC.downedMechBoss2)
                        souls = (int)(souls * 0.25f);
                    boss = true;
                }
                return souls;
            }
            else if (npcID == NPCID.SkeletronPrime) // Skeletron Prime
            {
                NPC primeCannon = new(); primeCannon.SetDefaults(NPCID.PrimeCannon);
                NPC primeLaser = new(); primeLaser.SetDefaults(NPCID.PrimeLaser);
                NPC primeSaw = new(); primeSaw.SetDefaults(NPCID.PrimeSaw);
                NPC primeVice = new(); primeVice.SetDefaults(NPCID.PrimeVice);
                souls = primeCannon.lifeMax + primeLaser.lifeMax + primeSaw.lifeMax + primeVice.lifeMax;
                if (NPC.downedMechBoss3)
                    souls = (int)(souls * 0.25f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.Plantera) // Plantera
            {
                NPC plantera = new(); plantera.SetDefaults(NPCID.Plantera);
                souls = (int)(plantera.lifeMax * 2.5f);
                if (NPC.downedPlantBoss)
                    souls = (int)(plantera.lifeMax * 0.5f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.Golem) // Golem
            {
                souls = (int)(golemHP * 1.5f); // 90K
                if (NPC.downedGolemBoss)
                    souls = (int)(golemHP * 0.3f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.DukeFishron) // Duke Fishron
            {
                souls = golemHP; // souls in relation to Golem
                if (NPC.downedFishron)
                    souls = (int)(souls * 0.35f); // 21K
                else
                    souls = (int)(souls * 1.5f + 15000); // 105K
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.HallowBoss) // Empress of Light
            {
                NPC empressOfLight = new(); empressOfLight.SetDefaults(NPCID.HallowBoss);
                souls = (int)(empressOfLight.lifeMax * 1.5f) + 15000;  // 120K
                if (NPC.downedEmpressOfLight)
                    souls = (int)(empressOfLight.lifeMax * 0.35f); // 24.5K
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.CultistBoss) // Cultist
            {
                NPC empressOfLight = new(); empressOfLight.SetDefaults(NPCID.HallowBoss); // souls in relation to Empress of Light
                souls = (int)(empressOfLight.lifeMax * 1.5f) + 30000; // 135K
                if (NPC.downedAncientCultist)
                    souls = (int)(empressOfLight.lifeMax * 0.4f); // 28K
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.LunarTowerNebula || npcID == NPCID.LunarTowerSolar ||
                npcID == NPCID.LunarTowerStardust || npcID == NPCID.LunarTowerVortex) // Lunar Towers
            {
                NPC tower = new(); tower.SetDefaults(npcID);
                souls = (int)(tower.lifeMax * 1.5f);
                if ((NPC.downedTowerSolar && npcID == NPCID.LunarTowerSolar) ||
                    (NPC.downedTowerNebula && npcID == NPCID.LunarTowerNebula) ||
                    (NPC.downedTowerVortex && npcID == NPCID.LunarTowerVortex) ||
                    (NPC.downedTowerStardust && npcID == NPCID.LunarTowerStardust)
                )
                    souls = (int)(souls * 0.5f);
                boss = true;
                return souls;
            }
            else if (npcID == NPCID.MoonLordCore) // Moon Lord
            {
                NPC moonLordCore = new(); moonLordCore.SetDefaults(NPCID.MoonLordCore);
                NPC moonLordHead = new(); moonLordHead.SetDefaults(NPCID.MoonLordHead);
                NPC moonLordHand = new(); moonLordHand.SetDefaults(NPCID.MoonLordHand);
                souls = moonLordCore.lifeMax + moonLordHead.lifeMax + 2 * moonLordHand.lifeMax;
                if (NPC.downedMoonlord)
                    souls = (int)(souls * 0.5f); // 72.5K
                else
                    souls = (int)(souls * 1.25f); // 181250
                boss = true;
                return souls;
            }
            else // bosses that have not been manually handled.
                boss = npc.boss;

            // Blacklist
            if (NPCIDBlackList.Contains(npcID))
                return 0;

            // Other NPCs
            int statLife, maxStatLife;
            npc.GetLifeStats(out statLife, out maxStatLife);
            return maxStatLife;
        }

        public override void OnKill(NPC npc)
        {
            int playerIndex = npc.lastInteraction;
            if (playerIndex == 255)
                return;

            int npcID = npc.type;
            if (!npc.boss && npcID != NPCID.LunarTowerNebula && npcID != NPCID.LunarTowerSolar && npcID != NPCID.LunarTowerStardust && npcID != NPCID.LunarTowerVortex)
                if (npc.SpawnedFromStatue || npc.friendly || npc.townNPC || npc.lifeMax <= 5 || npc.damage == 0) // NPC hasn't been damaged by any Player + Souls farming with Statues and friendly NPCs and Boss parts disabled :)
                    return;

            bool boss;
            DarkSoulsPlayer dsPlayer = Main.player[playerIndex].GetModPlayer<DarkSoulsPlayer>();
            int souls = GetSoulsByNPC(npc, out boss);

            if (Main.dedServ)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DarkSouls.NetMessageTypes.GetSouls);
                packet.Write(souls);
                if (boss) // server sends souls to all clients (if NPC is downed boss)
                    packet.Send();
                else // 
                    packet.Send(playerIndex); // if the client (specific player) kills someone other than boss
            }
            else // single player
                dsPlayer.AddSouls(souls);
        }
    }
}
