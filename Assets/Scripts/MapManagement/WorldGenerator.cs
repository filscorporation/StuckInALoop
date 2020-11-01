using UnityEngine;

namespace MapManagement
{
    public class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Generate(WorldParams param)
        {
            WorldGraphNode root = new WorldGraphNode(param.Root);
            root.Generate(param);
        }
    }
}