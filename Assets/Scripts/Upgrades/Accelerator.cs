namespace Upgrades
{
    public class Accelerator : Upgrade
    {
        public override int Level => 2;
        public override string Name => nameof(Accelerator);
        public override string Description => "Lowers travel cost by 25% and increase your speed";
        public override Cost Cost => new Cost(15, 14, 0, 7);
        public override void Apply()
        {
            GameManager.Instance.Player.Speed += 3;
            GameManager.Instance.Player.TravelCostDiscount += 0.25f;
            GameManager.Instance.Player.Accelerator.SetActive(true);
        }
    }
}