 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 01:46*/
 
using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x14F5, 0x14F6)]
    public class OracleOfTheSea : BaseOracle
    {		
        [Constructable]
        public OracleOfTheSea() : base(0x14F5)
        {
			this.Hue = 0x5C;
            this.Weight = 3.0;
			this.UsesRemaining = 5;
			this.Attuned = "[Attuned to: Charybdis]";
        }
		
        public OracleOfTheSea(Serial serial)
            : base(serial)
        {
        }
		
		public override int LabelNumber
        {
            get
            {
                return 1150184;
            }
        }// Oracle of the Sea
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
        }

		public override void OnDoubleClick( Mobile from )
		{

			if ( !IsChildOf (from.Backpack))
			{
				from.SendMessage( "Este objeto precisa estar em sua mochila para que possa utiliza-lo!" );
			}
			else
			{
				int Chance = Utility.Random(100); // Chance of spawning
				
				int x = 0, y = 0, z = 0; // Cordinates variables.
				Map map = this.Map;
				
				#region Spawn Area and Map definitions :
				/*If you want, you can also set Random values to the x axis to make him spawn randomicaly in other places.
				So, if you do this, remember to set the y and z axis before create the creature.
				I have setted 3 default regions of spawn : 1 on felucca, 2 on Ilshenar and 3 on Tokuno Islands.*/
				if ( map == null )
					return;
				else if(map.ToString() == "Felucca"){
					x = 1672;y = 1909;z = -5;}
				else if(map.ToString() == "Ilshenar"){
					x = 1727;y = 825;z = -33;}
				else if(map.ToString() == "Tokuno"){
					x = 1251;y = 1089;z = -5;}
				#endregion
				
				Point3D loc = new Point3D(x, y, z);	
				
				if(Chance > 50)
				{
					if( this.UsesRemaining > 0 && this.UsesRemaining < 6 )
					{
						this.UsesRemaining--;
						from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150190); // You peer into the spyglass, images swirl in your mind as the magic device searches.

						if(x == 1672)
						{
							BaseCreature Charybdis = new Charybdis();
							loc = new Point3D( x, y, z );
							Charybdis.MoveToWorld(loc, map);
							from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150191, loc.ToString()); // The location you seek is: ~1_val~
							//Console.WriteLine("Spawed in {0}, {1}, {2}, Map : {3}", x, y, z, map);
						}
						
						else if(x == 1727)
						{
							BaseCreature Charybdis = new Charybdis();
							loc = new Point3D( x, y, z );
							Charybdis.MoveToWorld(loc, map);
							from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150191, loc.ToString()); // The location you seek is: ~1_val~
							//Console.WriteLine("Spawned in {0}, {1}, {2}, Map : {3}", x, y, z, map);
						}
						
						else if(x == 1251)
						{
							BaseCreature Charybdis = new Charybdis();
							loc = new Point3D( x, y, z );
							Charybdis.MoveToWorld(loc, map);
							from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150191, loc.ToString()); // The location you seek is: ~1_val~
							//Console.WriteLine("Spawned in {0}, {1}, {2}, Map : {3}", x, y, z, map);
						}
						else
							return;
					}
					else
						from.SendMessage("You don't have more charges on the Oracle. To recharge use a Fish Oil on it.");
				}
				else
				{
					if( this.UsesRemaining > 0 && this.UsesRemaining < 6 )
					{
						this.UsesRemaining--;
						from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150198); // The spyglass goes dark, it has failed to find what you seek.
					}
					else
						from.SendMessage("You don't have more charges on the Oracle. To recharge use a Fish Oil on it.");
				}
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