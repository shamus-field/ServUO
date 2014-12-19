using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
	public class RewardStone : Item
	{
        [Constructable]
		public RewardStone() : base( 0xED4 )
		{
			Movable = false;
			Name = "Reward Stone";
		}

		public override void OnDoubleClick( Mobile from )
		{
            Container pack = from.Backpack;
            if (pack != null)
            {
                if (pack.ConsumeTotal(typeof(Gold), 5000)) 
                {
                    from.SendMessage("You have received a random reward.");
                    this.ChooseReward(from);
                }
                else
                {
                    from.SendMessage("You need 5000 gold to activate this stone.");
                }
            }
		}
        
        public void ChooseReward(Mobile from)
        {
            switch ( Utility.Random(1) )
            {
                default:
                case 0:
                {
                    break;
                }
                case 1:
                {
                    from.AddToBackpack(ScrollofTranscendence.CreateRandom(5, 10));
                    from.AddToBackpack(ScrollofTranscendence.CreateRandom(5, 10));
                    from.AddToBackpack(ScrollofTranscendence.CreateRandom(5, 10));
                    from.AddToBackpack(ScrollofTranscendence.CreateRandom(5, 10));
                    from.AddToBackpack(ScrollofTranscendence.CreateRandom(5, 10));
                    break;
                }
                case 2:
                {
                    from.AddToBackpack(new ItemBlessDeed());
                    from.AddToBackpack(new ItemBlessDeed());
                    from.AddToBackpack(new ItemBlessDeed());
                    from.AddToBackpack(new ItemBlessDeed());
                    break;
                }
                case 3:
                {
                    int val = Utility.Random(5) + 1;
                    from.AddToBackpack(new StatCapScroll(250 + val * 5));
                    val = Utility.Random(5) + 1;
                    from.AddToBackpack(new StatCapScroll(250 + val * 5));
                    break;
                }
                case 4:
                {
                    from.AddToBackpack(new SturdyPickaxe());
                    from.AddToBackpack(new SturdyPickaxe());
                    from.AddToBackpack(new SturdyPickaxe());
                    from.AddToBackpack(new SturdyShovel());
                    from.AddToBackpack(new SturdyShovel());
                    from.AddToBackpack(new SturdyShovel());
                    break;
                }
                case 5:
                {
                    int val = Utility.Random(3);
                    if (val == 0)
                        from.AddToBackpack(new LeatherGlovesOfMining(2));
                    else if (val == 1)
                        from.AddToBackpack(new StuddedGlovesOfMining(6));
                    else
                        from.AddToBackpack(new RingmailGlovesOfMining(10));
                    
                    from.AddToBackpack(new GargoylesPickaxe());
                    from.AddToBackpack(new ProspectorsTool());
                    from.AddToBackpack(new PowderOfTemperament());
                    break;
                }
                case 6:
                {
                    from.AddToBackpack(PowerScroll.CreateRandom(5, 20));
                    from.AddToBackpack(PowerScroll.CreateRandom(5, 20));
                    from.AddToBackpack(PowerScroll.CreateRandom(5, 20));
                    from.AddToBackpack(PowerScroll.CreateRandom(5, 20));
                    from.AddToBackpack(PowerScroll.CreateRandom(5, 20));
                    break;
                }
                case 7:
                {
                    int val = Utility.Random(8) + 1;
                    from.AddToBackpack(new RunicHammer(CraftResource.Iron + val, Core.AOS ? (55 - (val * 5)) : 50));
                    val = Utility.Random(8) + 1;
                    from.AddToBackpack(new RunicHammer(CraftResource.Iron + val, Core.AOS ? (55 - (val * 5)) : 50));
                    val = Utility.Random(8) + 1;
                    from.AddToBackpack(new RunicHammer(CraftResource.Iron + val, Core.AOS ? (55 - (val * 5)) : 50));
                    break;
                }
                case 8:
                {
                    int val = Utility.Random(4);
                    if (val == 0)
                        from.AddToBackpack(new AncientSmithyHammer(10));
                    else if (val == 1)
                        from.AddToBackpack(new AncientSmithyHammer(15));
                    else if (val == 2)
                        from.AddToBackpack(new AncientSmithyHammer(30));
                    else
                        from.AddToBackpack(new AncientSmithyHammer(60));
                        
                    val = Utility.Random(4);
                    if (val == 0)
                        from.AddToBackpack(new AncientSmithyHammer(10));
                    else if (val == 1)
                        from.AddToBackpack(new AncientSmithyHammer(15));
                    else if (val == 2)
                        from.AddToBackpack(new AncientSmithyHammer(30));
                    else
                        from.AddToBackpack(new AncientSmithyHammer(60));
                        
                    from.AddToBackpack(new ColoredAnvil());
                    from.AddToBackpack(new ColoredAnvil());
                    break;
                }
                case 9:
                {
                    from.AddToBackpack(new Gold(10000));
                    from.SendMessage("You have been given 10000 gold!");
                    break;
                }
                case 10:
                {
                    int val = Utility.Random(3) + 1;
                    from.AddToBackpack(new RunicSewingKit(CraftResource.RegularLeather + val, 60 - (val * 15)));
                    val = Utility.Random(3) + 1;
                    from.AddToBackpack(new RunicSewingKit(CraftResource.RegularLeather + val, 60 - (val * 15)));
                    val = Utility.Random(3) + 1;
                    from.AddToBackpack(new RunicSewingKit(CraftResource.RegularLeather + val, 60 - (val * 15)));
                    break;
                }
                case 11:
                {
                    int val = Utility.Random(4);
                    if (val == 0)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.OakWood, 45));
                    else if (val == 1)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.AshWood, 35));
                    else if (val == 2)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.YewWood, 25));
                    else
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.Heartwood, 15));
                
                    val = Utility.Random(4);
                    if (val == 0)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.OakWood, 45));
                    else if (val == 1)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.AshWood, 35));
                    else if (val == 2)
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.YewWood, 25));
                    else
                        from.AddToBackpack(new RunicFletcherTool(CraftResource.Heartwood, 15));
                
                    break;
                }
                case 12:
                {
                    from.AddToBackpack(Reward.FletcherRecipe());
                    from.AddToBackpack(Reward.TailorRecipe());
                    from.AddToBackpack(Reward.SmithRecipe());
                    from.AddToBackpack(Reward.TinkerRecipe());
                    from.AddToBackpack(Reward.CarpRecipe());
                    from.AddToBackpack(Reward.CookRecipe());
                    break;
                }
            }
        }

		public RewardStone( Serial serial ) : base( serial )
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

    