namespace Upgrades
{
    public class CrystalFocus : Upgrade
    {
        public override int Level => 3;
        public override string Name => "Crystal Focus";
        public override string Description => "Increase your energy generation speed by 1 per second";
        public override Cost Cost => new Cost(15, 15, 2, 12);
        
        public override void Apply()
        {
            GameManager.Instance.Player.DefaultEnergyPerSecond += 1f;
        }
    }
}