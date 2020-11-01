using System.Collections.Generic;
using UnityEngine;

namespace MapManagement
{
    public class WorldGraphNode
    {
        public WorldGraphNode(WorldLevel level)
        {
            Level = level;
        }

        public WorldGraphNode Parent;
        public List<WorldGraphNode> Children;

        public WorldLevel Level;

        public void Generate(WorldParams param)
        {
            int childrenCount = Random.Range(1, param.Size * 2 - 1);
            Children = new List<WorldGraphNode>(childrenCount);
            
            if (Level == WorldLevel.PlanetarySystem)
            {
                // TODO: Planetary system types
                
                
                return;
            }
            
            for (int i = 0; i < childrenCount; i++)
            {
                Children[i] = new WorldGraphNode(Level - 1);
                Children[i].Generate(param);
            }
        }
    }
}