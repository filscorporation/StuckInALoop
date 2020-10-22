using UnityEngine;

namespace DataManagement
{
    public abstract class DataObject : MonoBehaviour
    {
        public abstract IData ToData();
    }
}