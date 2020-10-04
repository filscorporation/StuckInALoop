namespace Upgrades
{
    public class GravitationalGenerator : Upgrade
    {
        public override string Name => "Gravitational Generator";
        public override string Description => "Restores half of energy you spent to travel to planet";
        public override Cost Cost => new Cost(25, 10, 0, 8);
        public override void Apply()
        {
            GameManager.Instance.Player.MoveEnergyToRestore += 0.5f;
        }
    }
}