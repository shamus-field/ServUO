using System;
using Server.Items;

namespace Server.Items
{
	public class BoltVendStone : Item
	{
		[Constructable]
		public BoltVendStone() : base( 0xEDC )
		{
			Movable = false;
			Hue = 0x83E;
			Name = "Bolt Stone";
		}

		public override void OnDoubleClick( Mobile from )
		{
		   	if ( from.Backpack.ConsumeTotal( typeof( Gold ), 1000 ) )
            {
                from.AddToBackpack(new Bolt(200));
                from.SendMessage( "1000 gold has been removed from your pack." );
		}
		   	else
		      {
		   		from.SendMessage( "You do not have 1000 gold for that." );
		   	}
					
		}

		public BoltVendStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}