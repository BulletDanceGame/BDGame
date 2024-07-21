using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;



public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private SaveData data;
    private BinaryFormatter formatter;
    string path;

    private void Awake()
    {
        Instance = this;


        SetUp();
    }

    private void SetUp()
    {
        data = new SaveData();
        path = Application.persistentDataPath + " /saveData.heh";
        formatter = new BinaryFormatter();

        if (File.Exists(path) == false)
        {
            //Doesnt exist, so create it
            FileStream stream = new FileStream(path, FileMode.CreateNew);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            if (stream.Length > 0)
            {
                data = formatter.Deserialize(stream) as SaveData;
            }
            else
            {
                formatter.Serialize(stream, data);
            }

            stream.Close();
        }
    }



    public void Save()
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public SaveData GetData()
    {
        return data;
    }
}
