namespace Upgrades
{
    public class Storage : Upgrade
    {
        public override string Name => nameof(Storage);
        public override string Description => "Increase maximum amount of titan by 20 and crystals by 5";
        public override Cost Cost => new Cost(2, 10, 0, 5);
        public override void Apply()
        {
            GameManager.Instance.Player.TitanMax += 20;
            GameManager.Instance.Player.CrystalsMax += 5;
        }
    }
}