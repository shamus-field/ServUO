using System;
using Server.Factions;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dark summoner's corpse")]
    public class DarkSummoner : BaseCreature
    {
        [Constructable]
        public DarkSummoner()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a dark summoner";
            this.Body = 400;

            this.SetStr(100, 124);
            this.SetDex(66, 75);
            this.SetInt(345, 354);

            this.SetHits(140, 180);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 70.1, 120.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 120.0);
            this.SetSkill(SkillName.EvalInt, 90.1, 120.0);
            this.SetSkill(SkillName.Spellweaving, 90.1, 120.0);
            this.SetSkill(SkillName.Mysticism, 90.1, 120.0);
            this.SetSkill(SkillName.Imbuing, 90.1, 120.0);
            this.SetSkill(SkillName.Necromancy, 90.1, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 90.1, 120.0);

            this.Fame = 200;
            this.Karma = -5000;

            this.FollowersMax = 10;
            GoldEarrings earrings = new GoldEarrings();
            earrings.Attributes.CastSpeed = 2;
            earrings.Attributes.CastRecovery = 2;
            earrings.Attributes.LowerManaCost = 10;
            earrings.Movable = false;
            this.AddItem(earrings);
            
            HoodedShroudOfShadows shroud = new HoodedShroudOfShadows();
            shroud.Movable = false;
            this.AddItem(shroud);
            
            this.AddItem(new Boots(Utility.RandomNeutralHue()));            
        }

        public DarkSummoner(Serial serial)
            : base(serial)
        {
        }
        
        protected override BaseAI ForcedAI
        {
            get
            {
                return new SummonerAI(this, false, false);
            }
        }
        
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 4);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
