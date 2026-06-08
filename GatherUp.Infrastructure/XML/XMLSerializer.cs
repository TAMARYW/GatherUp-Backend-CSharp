using System;
using System.IO;
using System.Xml.Serialization;

namespace GatherUp.Infrastructure.XML;

public static class XMLSerializer
{
    public static void WriteToXml<T>(string filePath, T data) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, data);
        }
    }

    public static T ReadFromXml<T>(string filePath) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            return new T();

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        using (StreamReader reader = new StreamReader(filePath))
        {
            var result = serializer.Deserialize(reader);
            return (result as T) ?? new T();
        }
    }
}