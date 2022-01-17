using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    #region PauseSettings
    public static void SaveSettings (PauseMenu pauseMenu)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/pausesettings.sm";
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(pauseMenu);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SettingsData LoadPauseSettings()
    {
        string path = Application.persistentDataPath + "/pausesettings.sm";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
    #endregion
}
