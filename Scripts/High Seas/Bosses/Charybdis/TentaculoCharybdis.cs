 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:45*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class CharybdisTentacles : BaseCreature
    {
        [Constructable]
        public CharybdisTentacles()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Giant Tentacle";
            this.Body = 1245;
			this.BaseSoundID = 0x669;

            this.SetStr(127, 155);
            this.SetDex(65, 85);
            this.SetInt(102, 123);
			
			this.SetHits( 105, 113 );
			this.SetStam( 66, 85 );
			this.SetMana( 102, 123 );

            this.SetDamage(10, 15);

            this.SetSkill(SkillName.MagicResist, 100.4, 113.5);
			this.SetSkill(SkillName.Magery, 60.2, 72.4);
			this.SetSkill( SkillName.EvalInt, 60.1, 73.4 );
            this.SetSkill(SkillName.Wrestling, 52.1, 70.0);

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50);	

			SetResistance( ResistanceType.Physical, 32, 45 );
			SetResistance( ResistanceType.Fire, 10, 25 );
			SetResistance( ResistanceType.Cold, 10, 25 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 5, 10 );
			
            this.Fame = 2000;
            this.Karma = -1000;

        }

        public CharybdisTentacles(Serial serial)
            : base(serial)
        {
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