 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_MagoAprendiz : BaseCreature
    {
        [Constructable]
        public Corgul_MagoAprendiz()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Apprentice Mage";
            this.Body = 124;

            this.SetStr(172);
            this.SetDex(136);
            this.SetInt(73);
			
			this.SetHits( 320 );
			this.SetStam( 101 );
			this.SetMana( 619 );

            this.SetDamage(10, 15);

            this.SetSkill(SkillName.MagicResist, 83.6);
			this.SetSkill(SkillName.Magery, 91.7);
			this.SetSkill(SkillName.EvalInt, 81.9);
            this.SetSkill(SkillName.Tactics, 77.1);
            this.SetSkill(SkillName.Wrestling, 45.7);

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Cold, 30 );
			SetDamageType( ResistanceType.Poison, 30);
			SetDamageType( ResistanceType.Energy, 30);
			

			SetResistance( ResistanceType.Physical, 94 );
			SetResistance( ResistanceType.Cold, 30 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 30 );
			
            this.Fame = 1000;
            this.Karma = -1000;

        }

        public Corgul_MagoAprendiz(Serial serial)
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