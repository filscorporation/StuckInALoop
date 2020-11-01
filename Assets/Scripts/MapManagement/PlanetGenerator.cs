using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapManagement
{
    public class PlanetGenerator : MonoBehaviour
    {
        public static PlanetGenerator Instance;
    
        [SerializeField] private List<GameObject> planetPrefabs;

        private void Awake()
        {
            Instance = this;
        }

        public void ClearPlanets()
        {
            foreach (Planet planet in FindObjectsOfType<Planet>())
            {
                Destroy(planet.gameObject);
            }
        }

        public Planet GetPlanetByIndex(int index)
        {
            // TODO: optimization
            return FindObjectsOfType<Planet>().First(p => p.Index == index);
        }

        public int GetPlanetPrefabIndex(Planet planet)
        {
            // for premade maps
            return planetPrefabs
                .First(p => p.GetComponent<SpriteRenderer>().sprite == planet.GetComponent<SpriteRenderer>().sprite)
                .GetComponent<Planet>().PrefabIndex;
        }

        public Planet SpawnPlanet(int prefabIndex, Vector2 position)
        {
            GameObject go = Instantiate(planetPrefabs[prefabIndex], position, Quaternion.identity);
            go.transform.SetParent(transform);

            return go.GetComponent<Planet>();
        }
    }
}