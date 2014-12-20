 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_MagoGuerreiro : BaseCreature
    {
        [Constructable]
        public Corgul_MagoGuerreiro()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Battle Mage";
            this.Body = 124;

            this.SetStr(156);
            this.SetDex(101);
            this.SetInt(181);
			
			this.SetHits( 419 );
			this.SetStam( 101 );
			this.SetMana( 619 );

            this.SetDamage(12, 17);

            this.SetSkill(SkillName.MagicResist, 83.6);
			this.SetSkill(SkillName.Magery, 91.7);
			this.SetSkill(SkillName.EvalInt, 81.9);
            this.SetSkill(SkillName.Tactics, 77.1);
            this.SetSkill(SkillName.Wrestling, 45.7);

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20);
			SetDamageType( ResistanceType.Poison, 20);
			SetDamageType( ResistanceType.Energy, 20);
			

			SetResistance( ResistanceType.Physical, 55 );
			SetResistance( ResistanceType.Fire, 54 );
			SetResistance( ResistanceType.Cold, 51 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 52 );
			
            this.Fame = 125;
            this.Karma = -1000;

        }

        public Corgul_MagoGuerreiro(Serial serial)
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