using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.XML;

namespace GatherUp.Infrastructure.Repositories;

public class XmlRepository<T> : IRepository<T> where T : class
{
    protected readonly string _filePath;
    protected readonly bool _useSerializer;

    public XmlRepository()
    {
        string typeName = typeof(T).Name;
        _filePath = Path.Combine("XML", $"{typeName}.xml");
        _useSerializer = true;
    }

    public XmlRepository(string customFilePath, bool useSerializer)
    {
        _filePath = customFilePath;
        _useSerializer = useSerializer;
    }

    public virtual void Add(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        if (!_useSerializer) throw new NotSupportedException("Automatic serialization is disabled for this repository.");

        if (entity is IEntity entityWithId)
        {
            List<T> items = XMLSerializer.ReadFromXml<List<T>>(_filePath);
            if (entityWithId.Id == 0)
            {
                entityWithId.Id = items.Count > 0 ? items.Max(i => ((IEntity)i).Id) + 1 : 1;
            }
            items.Add(entity);
            XMLSerializer.WriteToXml(_filePath, items);
        }
    }

    public virtual T? GetById(int id)
    {
        if (!_useSerializer) throw new NotSupportedException("Use manual retrieval methods.");
        List<T> items = XMLSerializer.ReadFromXml<List<T>>(_filePath);
        return items.FirstOrDefault(i => i is IEntity e && e.Id == id);
    }

    public virtual T? GetById(string id)
    {
        if (_useSerializer) throw new NotSupportedException("This method is for manual non-integer ID repositories.");
        return null; 
    }

    public virtual IEnumerable<T> GetAll()
    {
        if (!_useSerializer) return new List<T>();
        return XMLSerializer.ReadFromXml<List<T>>(_filePath);
    }

    public virtual void Update(T entity)
    {
        if (!_useSerializer) throw new InvalidOperationException("Modification is not allowed.");
    }

    public virtual void Delete(int id)
    {
        if (!_useSerializer) throw new InvalidOperationException("Deletion is not allowed.");
    }
}