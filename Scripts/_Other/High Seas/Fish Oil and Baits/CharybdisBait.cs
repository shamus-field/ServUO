 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 31/01/2014
 * Hour: 20:51*/
 
using System;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
    public class CharybdisBait : Item
    {	
        [Constructable]
        public CharybdisBait()
            : base(0x0997)
        {
			this.Name = "Charybdis Bait";
			this.Hue = 1170;
            this.Weight = 1.0;
        }

        public CharybdisBait(Serial serial)
            : base(serial)
        {
        }
		
		public override void OnDoubleClick(Mobile from)
		{
			if ( !IsChildOf (from.Backpack)) // If the object is not on your backpack the doubleclick will not work.
			{
				//from.SendMessage( "Este objeto precisa estar em sua mochila para que possa utiliza-lo!" );
				from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
			}
			else
			{
				from.SendLocalizedMessage(1154219); // Where do you wish to use this?
				from.BeginTarget(2, false, TargetFlags.None, new TargetCallback( OnTargetFishingPole )); // Begins the targeting.
			}
		}
		
		// OnTarget method that is used for the targeting purpose.
		private void OnTargetFishingPole(Mobile from, object targeted)
		{
			// if the target is the Fishing Pole, does the magic :
			if(targeted is FishingPole)
			{ 
				// The Fishing Pole settings for check.
				((FishingPole)targeted).Baited = true;
				((FishingPole)targeted).Hue = 1170;
				((FishingPole)targeted).Charges = 10;
				((FishingPole)targeted).BaitedMob = "Charybids";
			 
				from.SendMessage("You have successfully baited your Fishing Pole !"); // Confirmation message.
				//Delete the Charybids Bait
				this.Delete();
			}
			else
			{
				// If the targeted item is not a Fishing Pole, the bait does nothing.
				from.SendMessage("This item does not have any effect on this.");
			}
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