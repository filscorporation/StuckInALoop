using System.Linq;
using MapManagement;
using UnityEngine;

namespace UIManagement
{
    public class UIScanner : MonoBehaviour
    {
        [SerializeField] private Transform arrow;
        private Transform homePlanet;
    
        private void Start()
        {
            homePlanet = FindObjectsOfType<Planet>().FirstOrDefault(p => p.IsHomePlanet)?.transform;
        }
    
        private void Update()
        {
            Vector2 p1 = homePlanet.transform.position;
            Vector2 p2 = GameManager.Instance.Player.transform.position;
            arrow.eulerAngles = new Vector3(0, 0, Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg + 90);
        }
    }
}