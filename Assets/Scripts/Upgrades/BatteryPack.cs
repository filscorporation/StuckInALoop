﻿namespace Upgrades
{
    public class BatteryPack : Upgrade
    {
        public override string Name => nameof(BatteryPack);
        public override string Description => "Increase max energy to be stored by 20";
        public override Cost Cost => new Cost(20, 10, 0, 15);
        
        public override void Apply()
        {
            GameManager.Instance.Player.EnergyMax += 20;
        }
    }
}