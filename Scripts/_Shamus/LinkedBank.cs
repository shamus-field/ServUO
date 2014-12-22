using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class LinkedBank : Item
	{
		[Constructable]
		public LinkedBank() : base( 0xE79 )
		{
            Weight = 1.0;
			Name = "Linked Bank";
			Hue = 1161 ;
		}
        
        public override void OnDoubleClick( Mobile from )
		{

			PlayerMobile pm = from as PlayerMobile;
			
			if ( from.Criminal )
				from.SendMessage( "Thou art a criminal and cannot access thy bank box." );
			else if ( IsChildOf(from.Backpack) )
			{
				BankBox box = from.BankBox;

				if ( box != null )
					box.Open();
			}
			else
				from.SendMessage( "This must be in your backpack." );
		}
        
        public LinkedBank(Serial serial): base(serial)
        {
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
