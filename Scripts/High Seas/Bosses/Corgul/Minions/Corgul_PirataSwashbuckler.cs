 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_PirataSwashbuckler : BaseCreature
    {
        [Constructable]
        public Corgul_PirataSwashbuckler()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Swashbuckler";
            this.Body = 400;

            this.SetStr(250);
            this.SetDex(280);
            this.SetInt(68);
			
			this.SetHits( 500 );
			this.SetStam( 250 );
			this.SetMana( 200 );

            this.SetDamage(20, 25);

            this.SetSkill(SkillName.MagicResist, 99.6);
			this.SetSkill(SkillName.Swords, 120.0);
			this.SetSkill(SkillName.Anatomy, 100.0);
            this.SetSkill(SkillName.Tactics, 77.1);
            this.SetSkill(SkillName.Wrestling, 45.7);

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 30);
			SetDamageType( ResistanceType.Poison, 80);
			SetDamageType( ResistanceType.Energy, 10);
			

			SetResistance( ResistanceType.Physical, 70 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 10 );
			SetResistance( ResistanceType.Energy, 20 );
			
            this.Fame = 2500;
            this.Karma = -1000;
			
			this.AddItem(new SkullCap(Hue = 837));
            this.AddItem(new LeatherChest());
			this.AddItem(new LeatherArms());
			this.AddItem(new LeatherGloves());
            this.AddItem(new ThighBoots());
			this.AddItem(new Katana());

        }

        public Corgul_PirataSwashbuckler(Serial serial)
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