namespace Upgrades
{
    public class Armour : Upgrade
    {
        public override string Name => nameof(Armour);
        public override string Description => "Gives your ship +30 max health";
        public override Cost Cost => new Cost(3, 10, 0, 8);
        
        public override void Apply()
        {
            GameManager.Instance.Player.Health += 30;
            GameManager.Instance.Player.HealthMax += 30;
        }
    }
}