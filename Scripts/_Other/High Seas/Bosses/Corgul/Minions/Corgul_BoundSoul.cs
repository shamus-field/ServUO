 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_BoundSoul : BaseCreature
    {
        [Constructable]
        public Corgul_BoundSoul()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Bound Soul";
            this.Hue = 0x4001;
            this.Body = 970;

            this.SetStr(172);
            this.SetDex(136);
            this.SetInt(73);
			
			this.SetHits( 419 );
			this.SetStam( 101 );
			this.SetMana( 100 );

            this.SetDamage(17, 22);

            this.SetSkill(SkillName.MagicResist, 102.0);
            this.SetSkill(SkillName.Tactics, 110.0);
            this.SetSkill(SkillName.Wrestling, 101.7);

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

        public Corgul_BoundSoul(Serial serial)
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