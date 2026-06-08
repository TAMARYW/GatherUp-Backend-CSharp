using System;
using System.Collections.Generic;
using System.Linq;
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

    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        T existingItem = GetById(entity.Id);
        if (existingItem != null)
        {
            int index = _items.IndexOf(existingItem);
            _items[index] = entity;
        }
        else
        {
            throw new KeyNotFoundException($"Entity with Id {entity.Id} was not found for update.");
        }
    }

    public void Delete(int id)
    {
        var item = GetById(id);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}