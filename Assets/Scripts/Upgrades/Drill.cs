namespace Upgrades
{
    public class Drill : Upgrade
    {
        public override int Level => 1;
        public override string Name => nameof(Drill);
        public override string Description => "Mines 1 per 3 seconds from planet";
        public override Cost Cost => new Cost(5, 3, 0, 5);
        
        public override void Apply(Spaceship player)
        {
            player.TitanMiningSpeed += 0.33f;
            player.CrystalMiningSpeed += 0.33f;
            player.EnableDrill();
        }
    }
}