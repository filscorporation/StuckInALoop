namespace Upgrades
{
    public class Reactor : Upgrade
    {
        public override int Level => 2;
        public override string Name => "Reactor";
        public override string Description => "Increase your energy generation speed by 1 per 2 seconds";
        public override Cost Cost => new Cost(8, 15, 0, 10);
        
        public override void Apply(Spaceship player)
        {
            player.DefaultEnergyPerSecond += 0.5f;
        }
    }
}