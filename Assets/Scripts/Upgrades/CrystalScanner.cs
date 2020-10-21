namespace Upgrades
{
    public class CrystalScanner : Upgrade
    {
        public override int Level => 3;
        public override string Name => "Crystal Scanner";
        public override string Description => "Shows you direction to your home planet";
        public override Cost Cost => new Cost(20, 30, 5, 20);
        public override void Apply(Spaceship player)
        {
            GameManager.Instance.Scanner.gameObject.SetActive(true);
        }
    }
}