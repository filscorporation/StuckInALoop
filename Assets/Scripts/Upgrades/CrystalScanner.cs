namespace Upgrades
{
    public class CrystalScanner : Upgrade
    {
        public override string Name => "Crystal Scanner";
        public override string Description => "Shows you direction of your home planet";
        public override Cost Cost => new Cost(20, 30, 5, 20);
        public override void Apply()
        {
            GameManager.Instance.Scanner.gameObject.SetActive(true);
        }
    }
}