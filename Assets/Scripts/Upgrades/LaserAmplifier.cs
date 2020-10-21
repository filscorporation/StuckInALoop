namespace Upgrades
{
    public class LaserAmplifier : Upgrade
    {
        public override int Level => 2;
        public override string Name => "Laser Amplifier";
        public override string Description => "Increse your titan mining speed by 1 per second";
        public override Cost Cost => new Cost(25, 10, 0, 0);
        public override void Apply(Spaceship player)
        {
            player.TitanMiningSpeed += 1;
        }
    }
}