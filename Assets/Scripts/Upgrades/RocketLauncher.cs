namespace Upgrades
{
    public class RocketLauncher : Upgrade
    {
        public override int Level => 2;
        public override string Name => "Rocket Launcher";
        public override string Description => "Allows to shoot self guided rockets: 1 titan, 2 energy per shot. Press Q to shoot";
        public override Cost Cost => new Cost(15, 20, 0, 10);
        
        public override void Apply(Spaceship player)
        {
            ShowComponent(player);
        }

        public override void ShowComponent(Spaceship player)
        {
            base.ShowComponent(player);
            
            player.EnableRocketLauncher();
        }
    }
}