namespace Upgrades
{
    public class SolarPanel : Upgrade
    {
        public override string Name => nameof(SolarPanel);
        public override string Description => "Increase your energy generation speed by 1 per 2 seconds";
        public override Cost Cost => new Cost(3, 5, 1, 5);
        
        public override void Apply()
        {
            GameManager.Instance.Player.DefaultEnergyPerSecond += 0.5f;
        }
    }
}