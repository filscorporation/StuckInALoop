using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public struct Cost
{
    public Cost(float energy, float titan, float crystals, float time)
    {
        Energy = energy;
        Titan = titan;
        Crystals = crystals;
        Time = time;
    }

    public float Energy;
    public float Titan;
    public float Crystals;
    public float Time;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        if (!Mathf.Approximately(Energy, 0))
            sb.Append($"Energy: {Energy}, ");
        if (!Mathf.Approximately(Titan, 0))
            sb.Append($"Titan: {Titan}, ");
        if (!Mathf.Approximately(Crystals, 0))
            sb.Append($"Crystals: {Crystals}, ");
        if (!Mathf.Approximately(Time, 0))
            sb.Append($"Time: {Time}, ");
        if (sb.Length == 0)
            return "Nothing";
        sb.Remove(sb.Length - 2, 2);

        return sb.ToString();
    }
}

public abstract class Upgrade
{
    public static Dictionary<string, Upgrade> UpgradeDictionary = new Dictionary<string, Upgrade>();

    public static void LoadUpgradeDictionary()
    {
        foreach (Type type in Assembly.GetAssembly(typeof(Upgrade)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Upgrade))))
        {
            Upgrade skill = (Upgrade)Activator.CreateInstance(type);
            Upgrade.UpgradeDictionary[skill.Name] = skill;
        }
    }
    
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Cost Cost { get; }
    public abstract void Apply();

    public bool Installed = false;
}