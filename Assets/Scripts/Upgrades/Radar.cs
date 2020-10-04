using UnityEngine;

namespace Upgrades
{
    public class Radar : Upgrade
    {
        public override string Name => nameof(Radar);
        public override string Description => "Increase your maximum view range";
        public override Cost Cost => new Cost(10f, 8, 1, 10);
        public override void Apply()
        {
            Camera.main.GetComponent<CameraController>().MaxSize += 5;
        }
    }
}