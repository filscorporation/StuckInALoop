namespace Upgrades
{
    public class Armour : Upgrade
    {
        public override int Level => 2;
        public override string Name => nameof(Armour);
        public override string Description => "Gives your ship +20 max health";
        public override Cost Cost => new Cost(3, 14, 0, 8);
        
        public override void Apply()
        {
            GameManager.Instance.Player.HealthMax += 20;
        }
    }
}