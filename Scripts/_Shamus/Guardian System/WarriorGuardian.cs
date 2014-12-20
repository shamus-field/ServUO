using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a warrior guardian's corpse")]
    public class WarriorGuardian: BaseCreature
    {
        [Constructable]
        public WarriorGuardian(int level): base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else 
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the warrior";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);
        
            
            this.SetStr(level * 35, level * 35);
            this.SetDex(level * 25, level * 25);
            this.SetInt(level * 15, level * 15);
            this.SetHits(level * (20 * level), level * (25 * level));

            this.SetDamage(level * (5 * level), level * (5 * level));
            
            this.SetSkill(SkillName.Tactics, 30 * level, 30 * level);
            this.SetSkill(SkillName.Magery, 10 * level, 10 * level);
            this.SetSkill(SkillName.Swords, 30 * level, 30 * level);
            this.SetSkill(SkillName.Parry, 30 * level, 30 * level);
            this.SetSkill(SkillName.Macing, 30 * level, 30 * level);
            this.SetSkill(SkillName.Focus, 30 * level, 30 * level);
            this.SetSkill(SkillName.Wrestling, 30 * level, 30 * level);
            this.SetSkill(SkillName.MagicResist, 20 * level, 20 * level);
            
            this.SetResistance(ResistanceType.Physical, 15 * level, 15 * level);
            this.SetResistance(ResistanceType.Fire, 15 * level, 15 * level);
            this.SetResistance(ResistanceType.Cold, 15 * level, 15 * level);
            this.SetResistance(ResistanceType.Poison, 15 * level, 15 * level);
            this.SetResistance(ResistanceType.Energy, 15 * level, 15 * level);
            
            this.Fame = 100 * level;
            this.Karma = 100 * level;
            
            switch ( Utility.Random(2)) 
            {
                case 0:
                    this.AddItem(new Shoes(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    this.AddItem(new Boots(Utility.RandomNeutralHue()));
                    break;
            }
			
            this.AddItem(new Shirt());

            // Pick a random sword
            switch ( Utility.Random(5)) 
            {
                case 0:
                    this.AddItem(new Longsword());
                    break;
                case 1:
                    this.AddItem(new Broadsword());
                    break;
                case 2:
                    this.AddItem(new VikingSword());
                    break;
                case 3:
                    this.AddItem(new BattleAxe());
                    break;
                case 4:
                    this.AddItem(new Mace());
                    break;
            }

            // Pick a random shield
            switch ( Utility.Random(8)) 
            {
                case 0:
                    this.AddItem(new BronzeShield());
                    break;
                case 1:
                    this.AddItem(new HeaterShield());
                    break;
                case 2:
                    this.AddItem(new MetalKiteShield());
                    break;
                case 3:
                    this.AddItem(new MetalShield());
                    break;
                case 4:
                    this.AddItem(new WoodenKiteShield());
                    break;
                case 5:
                    this.AddItem(new WoodenShield());
                    break;
                case 6:
                    this.AddItem(new OrderShield());
                    break;
                case 7:
                    this.AddItem(new ChaosShield());
                    break;
            }
		  
            switch( Utility.Random(5) )
            {
                case 0:
                    break;
                case 1:
                    this.AddItem(new Bascinet());
                    break;
                case 2:
                    this.AddItem(new CloseHelm());
                    break;
                case 3:
                    this.AddItem(new NorseHelm());
                    break;
                case 4:
                    this.AddItem(new Helmet());
                    break;
            }
            // Pick some armour
            switch( level )
            {
                case 1: // Leather
                    this.AddItem(new LeatherChest());
                    this.AddItem(new LeatherArms());
                    this.AddItem(new LeatherGloves());
                    this.AddItem(new LeatherGorget());
                    this.AddItem(new LeatherLegs());
                    break;
                case 2: // Studded Leather
                    this.AddItem(new StuddedChest());
                    this.AddItem(new StuddedArms());
                    this.AddItem(new StuddedGloves());
                    this.AddItem(new StuddedGorget());
                    this.AddItem(new StuddedLegs());
                    break;
                case 3: // Ringmail
                    this.AddItem(new RingmailChest());
                    this.AddItem(new RingmailArms());
                    this.AddItem(new RingmailGloves());
                    this.AddItem(new RingmailLegs());
                    break;
                case 4: // Chain
                    this.AddItem(new PlateChest());
                    this.AddItem(new PlateLegs());
                    this.AddItem(new PlateArms());
                    this.AddItem(new PlateGloves());
                    this.AddItem(new PlateGorget());
                    break;
            }
            
        }     
        
        public WarriorGuardian(Serial serial): base(serial){}
    
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
    
    public class WarriorGuardian1: WarriorGuardian
    {
        [Constructable]
        public WarriorGuardian1(): base(1)
        {
        }    
        
        public WarriorGuardian1(Serial serial): base(serial){}
    
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
    
    public class WarriorGuardian2: WarriorGuardian
    {
        [Constructable]
        public WarriorGuardian2(): base(2)
        {
        }    
        
        public WarriorGuardian2(Serial serial): base(serial){}
    
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
    
    public class WarriorGuardian3: WarriorGuardian
    {
        [Constructable]
        public WarriorGuardian3(): base(3)
        {
        }    
        
        public WarriorGuardian3(Serial serial): base(serial){}
    
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
    
    public class WarriorGuardian4: WarriorGuardian
    {
        [Constructable]
        public WarriorGuardian4(): base(4)
        {
        }

        public WarriorGuardian4(Serial serial): base(serial){}        
    
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