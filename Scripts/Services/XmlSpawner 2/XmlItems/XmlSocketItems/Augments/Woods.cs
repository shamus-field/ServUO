using System;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    // ---------------------------------------------------
    // Mythic wood
    // ---------------------------------------------------
    public class MythicWood : BaseSocketAugmentation, IMythicAugment
    {
        [Constructable]
        public MythicWood()
            : base(0x1bdd)
        {
            this.Name = "Mythic wood";
            this.Hue = 11;
        }

        public MythicWood(Serial serial)
            : base(serial)
        {
        }

        public override int SocketsRequired
        {
            get
            {
                return 1;
            }
        }
        public override int IconXOffset
        {
            get
            {
                return 5;
            }
        }
        public override int IconYOffset
        {
            get
            {
                return 20;
            }
        }
        public override string OnIdentify(Mobile from)
        {
            return "Armor: +40 Lumberjacking";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for (int i = 0; i < 5; i++)
                {
                    if (a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues(i, SkillName.Lumberjacking, 40.0);
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        public override bool CanAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                return true;
            }

            return false;
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

    // ---------------------------------------------------
    // Legendary wood
    // ---------------------------------------------------
    public class LegendaryWood : BaseSocketAugmentation, ILegendaryAugment
    {
        [Constructable]
        public LegendaryWood()
            : base(0x1bdd)
        {
            this.Name = "Legendary wood";
            this.Hue = 12;
        }

        public LegendaryWood(Serial serial)
            : base(serial)
        {
        }

        public override int SocketsRequired
        {
            get
            {
                return 1;
            }
        }
        public override int IconXOffset
        {
            get
            {
                return 5;
            }
        }
        public override int IconYOffset
        {
            get
            {
                return 20;
            }
        }
        public override string OnIdentify(Mobile from)
        {
            return "Armor: +25 Lumberjacking";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for (int i = 0; i < 5; i++)
                {
                    if (a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues(i, SkillName.Lumberjacking, 25.0);
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        public override bool CanAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                return true;
            }

            return false;
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

    // ---------------------------------------------------
    // Ancient wood
    // ---------------------------------------------------
    public class AncientWood : BaseSocketAugmentation, IAncientAugment
    {
        [Constructable]
        public AncientWood()
            : base(0x1bdd)
        {
            this.Name = "Ancient wood";
            this.Hue = 15;
        }

        public AncientWood(Serial serial)
            : base(serial)
        {
        }

        public override int IconXOffset
        {
            get
            {
                return 5;
            }
        }
        public override int IconYOffset
        {
            get
            {
                return 20;
            }
        }
        public override string OnIdentify(Mobile from)
        {
            return "Armor: +10 Lumberjacking";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for (int i = 0; i < 5; i++)
                {
                    if (a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues(i, SkillName.Lumberjacking, 10.0);
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        public override bool CanAugment(Mobile from, object target)
        {
            if (target is BaseArmor)
            {
                return true;
            }

            return false;
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