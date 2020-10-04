namespace Upgrades
{
    public class Recycler : Upgrade
    {
        public override int Level => 2;
        public override string Name => nameof(Recycler);
        public override string Description => "Gain titan for asteroids you shot";
        public override Cost Cost => new Cost(25, 8, 0, 8);
        public override void Apply()
        {
            GameManager.Instance.Player.CanRecycle = true;
        }
    }
}