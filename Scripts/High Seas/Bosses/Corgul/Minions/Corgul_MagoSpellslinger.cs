 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_MagoSpellslinger : BaseCreature
    {
        [Constructable]
        public Corgul_MagoSpellslinger()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Spellslinger";
            this.Body = 124;

            this.SetStr(129);
            this.SetDex(96);
            this.SetInt(127);
			
			this.SetHits( 192 );
			this.SetStam( 96 );
			this.SetMana( 440 );

            this.SetDamage(8, 12);

            this.SetSkill(SkillName.MagicResist, 97.0);
			this.SetSkill(SkillName.Magery, 100.3);
			this.SetSkill(SkillName.EvalInt, 83.3);
            this.SetSkill(SkillName.Tactics, 86.3);
            this.SetSkill(SkillName.Wrestling, 95.6);

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 32 );
			SetResistance( ResistanceType.Fire, 34 );
			SetResistance( ResistanceType.Cold, 32 );
			SetResistance( ResistanceType.Poison, 32 );
			SetResistance( ResistanceType.Energy, 32 );
			
            this.Fame = 2540;
            this.Karma = -1000;

        }

        public Corgul_MagoSpellslinger(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
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
            this.AddLoot(LootPack.Average);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}