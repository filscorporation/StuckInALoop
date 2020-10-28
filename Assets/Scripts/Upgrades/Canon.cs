namespace Upgrades
{
    public class Canon : Upgrade
    {
        public override int Level => 1;
        public override string Name => "Canon";
        public override string Description => "Enables canon: 1 energy per shot, right click to shot";
        public override Cost Cost => new Cost(5, 10, 0, 5);
        
        public override void Apply(Spaceship player)
        {
            ShowComponent(player);
        }

        public override void ShowComponent(Spaceship player)
        {
            base.ShowComponent(player);
            
            player.EnableCanon();
        }
    }
}