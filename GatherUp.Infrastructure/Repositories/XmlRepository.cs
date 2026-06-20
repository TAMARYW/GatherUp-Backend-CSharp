using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.Core.Exceptions;
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

    /// <summary>
    /// תוקן: הגרסה המקורית לא עשתה כלום כשהשימוש בסריאלייזר מופעל (לא קראה מהדיסק,
    /// לא כתבה לדיסק) - כל עדכון מה-BL היה "נבלע" בלי שגיאה ובלי השפעה. כעת קוראים
    /// את הרשימה המלאה מהדיסק, מאתרים את האיבר לפי Id (FirstOrDefault, ללא לולאה ידנית),
    /// מחליפים אותו ברשימה, וכותבים את הרשימה המעודכנת בחזרה לדיסק.
    /// תוקן בשלב 4: KeyNotFoundException הוחלף ב-EntityNotFoundException, כדי
    /// שה-Middleware הגלובלי ב-API יתרגם אותו אוטומטית לסטטוס 404.
    /// </summary>
    public virtual void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (!_useSerializer) throw new InvalidOperationException("Modification is not allowed.");

        if (entity is IEntity entityWithId)
        {
            List<T> items = XMLSerializer.ReadFromXml<List<T>>(_filePath);
            int index = items.FindIndex(i => i is IEntity e && e.Id == entityWithId.Id);
            if (index < 0)
                throw new EntityNotFoundException(typeof(T).Name, entityWithId.Id);

            items[index] = entity;
            XMLSerializer.WriteToXml(_filePath, items);
        }
    }

    /// <summary>
    /// תוקן באותו אופן כמו Update - היה no-op, כעת קורא, מסנן את האיבר עם ה-Id המבוקש
    /// (Where, ללא לולאה ידנית) וכותב את התוצאה בחזרה.
    /// תוקן בשלב 4: נוסף בדיקת קיום מפורשת לפני המחיקה (ולא רק סינון שקט) - כדי
    /// שמחיקת מזהה שלא קיים תזרוק EntityNotFoundException ותתורגם לסטטוס 404,
    /// ולא "תיבלע" בשקט כהצלחה מדומה.
    /// </summary>
    public virtual void Delete(int id)
    {
        if (!_useSerializer) throw new InvalidOperationException("Deletion is not allowed.");

        List<T> items = XMLSerializer.ReadFromXml<List<T>>(_filePath);
        bool exists = items.Any(i => i is IEntity e && e.Id == id);
        if (!exists)
            throw new EntityNotFoundException(typeof(T).Name, id);

        List<T> remaining = items.Where(i => !(i is IEntity e && e.Id == id)).ToList();
        XMLSerializer.WriteToXml(_filePath, remaining);
    }
}
