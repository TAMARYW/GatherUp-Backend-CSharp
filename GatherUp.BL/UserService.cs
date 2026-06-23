using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

public class UserService
{
    private readonly IRepository<Person> _personRepo;

    public UserService(IRepository<Person> personRepo)
    {
        _personRepo = personRepo ?? throw new ArgumentNullException(nameof(personRepo));
    }

    public Person? AuthenticateUser(string email, string idCard)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(idCard))
            return null;

        return _personRepo.GetAll()
            .FirstOrDefault(p =>
                string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase)
                && p.Id.ToString() == idCard);
    }

    public void RegisterNewUser(Person newUser)
    {
        if (newUser == null) throw new ArgumentNullException(nameof(newUser));

        if (newUser.Id <= 0)
            throw new BusinessValidationException("מספר ת.ז (Id) חייב להיות מספר חיובי.");

        bool idExists = _personRepo.GetAll().Any(p => p.Id == newUser.Id);
        if (idExists)
            throw new BusinessValidationException("מספר ת.ז (Id) זה כבר רשום במערכת.");

        bool emailExists = _personRepo.GetAll()
            .Any(p => string.Equals(p.Email, newUser.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExists)
            throw new BusinessValidationException("משתמש עם כתובת אימייל זו כבר קיים במערכת.");

        _personRepo.Add(newUser);
    }

    public Person? GetUserById(int id) => _personRepo.GetById(id);

    public Person UpdateUserDetails(int id, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessValidationException("שם המשתמש לא יכול להיות ריק.");
        if (string.IsNullOrWhiteSpace(email)) throw new BusinessValidationException("כתובת האימייל לא יכולה להיות ריקה.");

        Person? person = _personRepo.GetById(id);
        if (person == null) throw new EntityNotFoundException("משתמש", id);

        bool emailTakenByOther = _personRepo.GetAll()
            .Any(p => p.Id != id && string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));
        if (emailTakenByOther)
            throw new BusinessValidationException("כתובת האימייל הזו כבר תפוסה על ידי משתמש אחר.");

        person.Name = name;
        person.Email = email;
        _personRepo.Update(person);
        return person;
    }

    public void UpdateNotificationPreferences(int personId, IEnumerable<NotificationType> preferences)
    {
        if (preferences == null) throw new ArgumentNullException(nameof(preferences));

        Person? person = _personRepo.GetById(personId);
        if (person == null) throw new EntityNotFoundException("משתמש", personId);

        person.NotificationPreferences = preferences.ToList();
        _personRepo.Update(person);
    }

    public Person? FindByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return _personRepo.GetAll()
            .FirstOrDefault(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Person> SearchByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Enumerable.Empty<Person>();
        return _personRepo.GetAll()
            .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
}