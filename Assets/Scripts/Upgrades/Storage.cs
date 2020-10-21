namespace Upgrades
{
    public class Storage : Upgrade
    {
        public override int Level => 1;
        public override string Name => nameof(Storage);
        public override string Description => "Increase maximum amount of titan by 20 and crystals by 3";
        public override Cost Cost => new Cost(2, 10, 0, 5);
        public override void Apply(Spaceship player)
        {
            player.TitanMax += 20;
            player.CrystalsMax += 3;
            player.Storage.SetActive(true);
        }
    }
}