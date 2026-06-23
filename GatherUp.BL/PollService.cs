using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

public class PollService
{
    private readonly IRepository<Poll> _pollRepo;
    private readonly IRepository<Event> _eventRepo;
    private readonly IEventPublisher _eventPublisher;

    public PollService(
        IRepository<Poll> pollRepo,
        IRepository<Event> eventRepo,
        IEventPublisher eventPublisher)
    {
        _pollRepo = pollRepo ?? throw new ArgumentNullException(nameof(pollRepo));
        _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    private Event GetExistingEvent(int eventId)
    {
        Event? ev = _eventRepo.GetById(eventId);
        if (ev == null) throw new EntityNotFoundException("האירוע", eventId);
        return ev;
    }

    private Poll GetExistingPoll(int pollId)
    {
        Poll? poll = _pollRepo.GetById(pollId);
        if (poll == null) throw new EntityNotFoundException("הסקר", pollId);
        return poll;
    }

    private Event GetEventOfPoll(int pollId)
    {
        Event? ev = _eventRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
        if (ev == null) throw new EntityNotFoundException("האירוע של הסקר", pollId);
        return ev;
    }

    public Poll CreatePoll(int eventId, string name, string? description, DateTime closingDate, List<PollQuestion> questions)
    {
        if (questions == null) throw new ArgumentNullException(nameof(questions));
        Event ev = GetExistingEvent(eventId);

        var poll = new Poll
        {
            Name = name,
            Description = description,
            ClosingDate = closingDate
        };
        poll.Questions.AddRange(questions);

        _pollRepo.Add(poll);
        ev.PollIds.Add(poll.Id);
        _eventRepo.Update(ev);

        _eventPublisher.RaisePollCreated(poll.Id, eventId, poll.Name);

        return poll;
    }

    public IEnumerable<Poll> GetEventPolls(int eventId)
    {
        Event ev = GetExistingEvent(eventId);
        return _pollRepo.GetAll().Where(p => ev.PollIds.Contains(p.Id));
    }

    public bool IsPollOpen(int pollId)
    {
        Poll poll = GetExistingPoll(pollId);
        return !poll.ClosingDate.HasValue || poll.ClosingDate.Value > DateTime.Now;
    }

    public void CastVote(int pollId, int questionId, int participantId, string chosenOption)
    {
        Poll poll = GetExistingPoll(pollId);

        PollQuestion? question = poll.Questions.FirstOrDefault(q => q.Id == questionId);
        if (question == null) throw new EntityNotFoundException("השאלה", questionId);

        if (!question.Options.Contains(chosenOption))
            throw new BusinessValidationException("האפשרות שנבחרה אינה קיימת ברשימת האפשרויות של השאלה.");

        ParticipantVote? previousVote = question.Votes.FirstOrDefault(v => v.ParticipantId == participantId);
        bool optionActuallyChanged = previousVote == null || previousVote.ChosenOption != chosenOption;

        if (previousVote != null)
        {
            question.Votes.RemoveAll(v => v.ParticipantId == participantId);
        }
        question.Votes.Add(new ParticipantVote { ParticipantId = participantId, ChosenOption = chosenOption });

        _pollRepo.Update(poll);

        if (optionActuallyChanged)
        {
            Event ev = GetEventOfPoll(pollId);
            _eventPublisher.RaisePollAnswered(pollId, questionId, participantId, chosenOption, ev.Id, ev.EventManagerId);
        }
    }

    public (Poll Poll, Dictionary<int, Dictionary<string, double>> ResultsByQuestion) GetPollWithResults(int pollId)
    {
        Poll poll = GetExistingPoll(pollId);

        Dictionary<int, Dictionary<string, double>> results = poll.Questions.ToDictionary(
            q => q.Id,
            q =>
            {
                int totalVotes = q.Votes.Count;
                return q.Votes
                    .GroupBy(v => v.ChosenOption)
                    .ToDictionary(g => g.Key, g => totalVotes == 0 ? 0 : Math.Round(100.0 * g.Count() / totalVotes, 1));
            });

        return (poll, results);
    }
}