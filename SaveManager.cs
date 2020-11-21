using System.IO;
using UnityEngine;
using System.Xml.Serialization;

public static class SaveManager
{
    /// <summary>
    /// Load XML file
    /// </summary>
    /// <typeparam name="T">Datatype to load</typeparam>
    /// <param name="filename">Full filename</param>
    /// <returns>If file exists returns saved T otherwise default T</returns>
    public static T XmlLoad<T>(string filename)
    {
        string filepath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(filepath))
        {
            using (FileStream file = File.Open(filepath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                TextReader reader = new StreamReader(file);
                T information = (T)serializer.Deserialize(reader);
                reader.Close();
                return information;
            }
        }
        return default;
    }

    /// <summary>
    /// Save with XML
    /// </summary>
    /// <typeparam name="T">Datatype to save</typeparam>
    /// <param name="information">The information to save</param>
    /// <param name="filename">Full filename</param>
    public static void XmlSave<T>(T information, string filename)
    {
        string filepath = Application.persistentDataPath + "/" + filename;

        //XML doesn't like opening existing files for some reason
        if (File.Exists(filepath))
            File.Delete(filepath);

        using (FileStream file = File.Open(filepath, FileMode.OpenOrCreate))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, information);
            writer.Close();
        }
    }
}
