namespace MapManagement
{
    public enum WorldLevel
    {
        Universe = 0,
        Galaxy = 1,
        StarCluster = 2,
        PlanetarySystem = 3,
        AstronomicalObject = 4,
    }
    
    public class WorldParams
    {
        public WorldParams(WorldLevel root, int size, float difficulty)
        {
            Root = root;
            Size = size;
            Difficulty = difficulty;
        }

        public WorldLevel Root;
        public int Size;
        public float Difficulty;
    }
}