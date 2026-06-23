using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

public class EventDashboardService
{
    private readonly IRepository<Event> _eventRepo;
    private readonly IRepository<Person> _personRepo;
    private readonly IEventPublisher _eventPublisher;

    public EventDashboardService(
        IRepository<Event> eventRepo,
        IRepository<Person> personRepo,
        IEventPublisher eventPublisher)
    {
        _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
        _personRepo = personRepo ?? throw new ArgumentNullException(nameof(personRepo));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    public IEnumerable<Event> GetEventsAsOrganizer(int managerId) =>
        _eventRepo.GetAll().Where(e => e.EventManagerId == managerId);

    public IEnumerable<Event> GetEventsAsOwner(int hostId) =>
        _eventRepo.GetAll().Where(e => e.EventHostId == hostId);

    public IEnumerable<Event> GetEventsAsParticipant(int personId) =>
        _eventRepo.GetAll()
            .Where(e => e.Participants.Any(p => p.PersonId == personId));

    public Event? GetEventDetails(int eventId) => _eventRepo.GetById(eventId);

    public void CreateEvent(Event newEvent)
    {
        if (newEvent == null) throw new ArgumentNullException(nameof(newEvent));

        if (_personRepo.GetById(newEvent.EventHostId) == null)
            throw new EntityNotFoundException("בעל האירוע", newEvent.EventHostId);

        _eventRepo.Add(newEvent);
    }

    public void UpdateEventDetails(Event updatedEvent)
    {
        if (updatedEvent == null) throw new ArgumentNullException(nameof(updatedEvent));
        _eventRepo.Update(updatedEvent);
        _eventPublisher.RaiseEventDetailsChanged(updatedEvent.Id);
    }
}