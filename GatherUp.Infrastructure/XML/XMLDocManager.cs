using System;
using System.IO;
using System.Xml.Linq;

namespace GatherUp.Infrastructure.XML;

public static class XMLDocManager
{
    public static XDocument LoadDocument(string filePath, string rootName = "Root")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (File.Exists(filePath))
        {
            return XDocument.Load(filePath);
        }

        return new XDocument(new XElement(rootName));
    }

    public static void SaveDocument(string filePath, XDocument document)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (document == null)
            throw new ArgumentNullException(nameof(document));

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        document.Save(filePath);
    }

    public static XElement? GetElementById(XDocument document, string elementName, string idValue)
    {
        if (document == null || document.Root == null) return null;

        foreach (XElement element in document.Root.Descendants(elementName))
        {
            XAttribute? idAttr = element.Attribute("Id");
            if (idAttr != null && idAttr.Value == idValue)
            {
                return element;
            }
        }

        return null;
    }

    public static void AddElement(string filePath, XElement newElement, string rootName = "Root")
    {
        XDocument doc = LoadDocument(filePath, rootName);
        if (doc.Root != null)
        {
            doc.Root.Add(newElement);
            SaveDocument(filePath, doc);
        }
    }

    public static bool DeleteElementById(string filePath, string elementName, string idValue)
    {
        XDocument doc = LoadDocument(filePath);
        XElement? elementToDelete = GetElementById(doc, elementName, idValue);

        if (elementToDelete != null)
        {
            elementToDelete.Remove();
            SaveDocument(filePath, doc);
            return true;
        }

        return false;
    }
}