using System;
using Server.Items;

namespace Server.Items
{
	public class ArcheryStone : Item
	{
		[Constructable]
		public ArcheryStone() : base( 0xEDC )
		{
			Movable = false;
			Hue = 0x4EA;
			Name = "Reagent Stone";
		}

		public override void OnDoubleClick( Mobile from )
		{
		   	if ( from.Backpack.ConsumeTotal( typeof( Gold ), 400 ) )
            {
                from.AddToBackpack(new Arrow(200));
                from.SendMessage( "400 gold has been removed from your pack." );
		}
		   	else
		      {
		   		from.SendMessage( "You do not have 400 gold for that." );
		   	}
					
		}

		public ArcheryStone( Serial serial ) : base( serial )
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