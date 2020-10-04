namespace Upgrades
{
    public class SolarPanel : Upgrade
    {
        public override int Level => 1;
        public override string Name => "Solar Panel";
        public override string Description => "Increase your energy generation speed by 1 per 3 seconds";
        public override Cost Cost => new Cost(3, 8, 0, 5);
        
        public override void Apply()
        {
            GameManager.Instance.Player.DefaultEnergyPerSecond += 0.33f;
        }
    }
}