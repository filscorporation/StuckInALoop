using UnityEngine;

namespace DataManagement
{
    public abstract class DataObject : MonoBehaviour
    {
        public bool WasLoaded { get; set; } = false;
        public abstract IData ToData();
    }
}