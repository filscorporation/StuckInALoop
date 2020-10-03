namespace Upgrades
{
    public class Drill : Upgrade
    {
        public override string Name => nameof(Drill);
        public override string Description => "Additionally mines 1 titan per 3 seconds from planet";
        public override Cost Cost => new Cost(5, 3, 0, 5);
        
        public override void Apply()
        {
            GameManager.Instance.Player.TitanMiningSpeed += 0.33f;
        }
    }
}