using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DataManagement
{
    public class DataManager : MonoBehaviour
    {
        private const string FILE_NAME = "game_save";
        private static string FileName => Path.Combine(Application.persistentDataPath, FILE_NAME);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Save();
            
            if (Input.GetKeyDown(KeyCode.L))
                Load();
        }

        public static void Save()
        {
            List<IData> objects = ((DataObject[]) FindObjectsOfType(typeof(DataObject))).Select(p => p.ToData()).ToList();
            File.WriteAllBytes(FileName, ObjectToByteArray(objects));
            
            Debug.LogFormat("Saved {0} objects", objects.Count);
        }
        
        public static void Load()
        {
            if (!File.Exists(FileName))
            {
                Debug.LogError("No file " + FileName);
                return;
            }
            
            ((List<IData>) ByteArrayToObject(File.ReadAllBytes(FileName))).ForEach(d => d.ToObject());
            
            Debug.LogFormat("Loaded");
        }
        
        private static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        
        private static object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}