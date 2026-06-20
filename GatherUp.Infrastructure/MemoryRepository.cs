using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure;

public class MemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new List<T>();

    private int _nextId = 1;

    public void Add(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.Id = _nextId++;

        _items.Add(entity);
    }

    public T? GetById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return _items;
    }

    /// <summary>
    /// תוקן בשלב 4: KeyNotFoundException הוחלף ב-EntityNotFoundException, לעקביות
    /// עם XmlRepository ועם ה-Middleware הגלובלי ב-API (סטטוס 404).
    /// </summary>
    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        T? existingItem = GetById(entity.Id);
        if (existingItem == null)
            throw new EntityNotFoundException(typeof(T).Name, entity.Id);

        int index = _items.IndexOf(existingItem);
        _items[index] = entity;
    }

    /// <summary>
    /// תוקן בשלב 4: נוסף בדיקת קיום מפורשת - מחיקת מזהה שלא קיים זורקת
    /// EntityNotFoundException, לעקביות עם XmlRepository.
    /// </summary>
    public void Delete(int id)
    {
        var item = GetById(id);
        if (item == null)
            throw new EntityNotFoundException(typeof(T).Name, id);

        _items.Remove(item);
    }
}
