using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

public record AccountSummaryResult(
    IEnumerable<ParticipantWithDetails> PaidParticipants,
    decimal TotalIncome,
    IEnumerable<VendorAllocation> Vendors,
    decimal TotalOutgoing,
    decimal NetBalance);

public record ReceiptReportEntry(string ReceiptNumber, decimal Amount);

public class FinanceService
{
    private readonly IRepository<VendorAllocation> _vendorRepo;
    private readonly IRepository<Person> _personRepo;
    private readonly IRepository<Event> _eventRepo;
    private readonly IReceiptRepository _receiptRepo;
    private readonly IEmailService _emailService;
    private readonly IEventPublisher _eventPublisher;

    public FinanceService(
        IRepository<VendorAllocation> vendorRepo,
        IRepository<Person> personRepo,
        IRepository<Event> eventRepo,
        IReceiptRepository receiptRepo,
        IEmailService emailService,
        IEventPublisher eventPublisher)
    {
        _vendorRepo = vendorRepo ?? throw new ArgumentNullException(nameof(vendorRepo));
        _personRepo = personRepo ?? throw new ArgumentNullException(nameof(personRepo));
        _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
        _receiptRepo = receiptRepo ?? throw new ArgumentNullException(nameof(receiptRepo));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    private Event GetExistingEvent(int eventId)
    {
        Event? ev = _eventRepo.GetById(eventId);
        if (ev == null) throw new EntityNotFoundException("האירוע", eventId);
        return ev;
    }

    private VendorAllocation GetExistingVendor(int vendorId)
    {
        VendorAllocation? v = _vendorRepo.GetById(vendorId);
        if (v == null) throw new EntityNotFoundException("הספק", vendorId);
        return v;
    }

    private IEnumerable<ParticipantWithDetails> GetEventParticipantsWithDetails(int eventId)
    {
        Event ev = GetExistingEvent(eventId);
        return ev.Participants
            .Select(p =>
            {
                Person? person = _personRepo.GetById(p.PersonId);
                return new ParticipantWithDetails(
                    p.PersonId,
                    person?.Name ?? "לא ידוע",
                    person?.Email ?? "",
                    p.IsAttending,
                    p.HasPaid,
                    p.AmountContributed);
            });
    }

    private IEnumerable<VendorAllocation> GetEventVendors(int eventId)
    {
        Event ev = GetExistingEvent(eventId);
        return _vendorRepo.GetAll().Where(v => ev.VendorIds.Contains(v.Id));
    }

    public void RegisterPayment(int eventId, int personId, decimal amountPaid)
    {
        Event ev = GetExistingEvent(eventId);

        Participant? participant = ev.Participants.FirstOrDefault(p => p.PersonId == personId);
        if (participant == null) throw new EntityNotFoundException("המשתתף", personId);

        participant.AmountContributed += amountPaid;
        participant.HasPaid = true;
        _eventRepo.Update(ev);

        _eventPublisher.RaisePaymentMade(personId, amountPaid, ev.Id, ev.EventManagerId);
    }

    public VendorAllocation RegisterOrAddVendorDebt(int eventId, int vendorId, string name, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessValidationException("שם הספק הוא שדה חובה.");
        if (amount <= 0)
            throw new BusinessValidationException("סכום החוב חייב להיות חיובי.");

        Event ev = GetExistingEvent(eventId);
        VendorAllocation? vendor = _vendorRepo.GetById(vendorId);

        if (vendor == null)
        {
            vendor = new VendorAllocation { Id = vendorId, Name = name, AmountOwed = amount };
            _vendorRepo.Add(vendor);

            if (!ev.VendorIds.Contains(vendorId))
            {
                ev.VendorIds.Add(vendorId);
                _eventRepo.Update(ev);
            }
        }
        else
        {
            vendor.AmountOwed += amount;
            _vendorRepo.Update(vendor);
        }

        return vendor;
    }

    public void SendPaymentReminders(int eventId, string message)
    {
        Event ev = GetExistingEvent(eventId);

        ev.Participants
            .Where(p => !p.HasPaid)
            .Select(p => _personRepo.GetById(p.PersonId))
            .Where(person => person != null)
            .ToList()
            .ForEach(person => _emailService.SendEmail(
                person!.Email,
                "תזכורת תשלום לאירוע",
                $"שלום {person.Name}, {message}"));
    }

    public decimal CalculateNetBudget(int eventId)
    {
        Event ev = GetExistingEvent(eventId);

        return ev.Participants
                .Where(p => p.IsAttending == true && p.HasPaid)
                .Sum(p => p.AmountContributed)
            - _vendorRepo.GetAll()
                .Where(v => ev.VendorIds.Contains(v.Id))
                .Sum(v => v.AmountOwed);
    }

    public IEnumerable<ReceiptReportEntry> GetFlattenedReceiptsReport(int eventId)
    {
        return GetEventVendors(eventId)
            .SelectMany(v => v.Receipts)
            .OrderByDescending(r => r.Date)
            .Select(r => new ReceiptReportEntry(r.ReceiptNumber, r.Amount));
    }

    public AccountSummaryResult GetAccountSummary(int eventId)
    {
        IEnumerable<ParticipantWithDetails> paidParticipants =
            GetEventParticipantsWithDetails(eventId).Where(p => p.HasPaid);
        decimal totalIncome = paidParticipants.Sum(p => p.AmountContributed);

        IEnumerable<VendorAllocation> vendors = GetEventVendors(eventId);
        decimal totalOutgoing = vendors.Sum(v => v.AmountOwed);

        return new AccountSummaryResult(paidParticipants, totalIncome, vendors, totalOutgoing, totalIncome - totalOutgoing);
    }

    public ReceiptDetails UploadVendorReceipt(int vendorId, string receiptNumber, decimal amount, DateTime date, string sourceFilePath)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber))
            throw new BusinessValidationException("מספר קבלה הוא שדה חובה.");
        if (amount <= 0)
            throw new BusinessValidationException("סכום הקבלה חייב להיות חיובי.");

        VendorAllocation vendor = GetExistingVendor(vendorId);

        if (amount > vendor.AmountOwed)
            throw new BusinessValidationException(
                $"סכום הקבלה (₪{amount}) גדול מהחוב הנותר לספק זה (₪{vendor.AmountOwed}).");

        var receipt = new ReceiptDetails(receiptNumber, amount, date);
        _receiptRepo.AddReceipt(receipt, sourceFilePath);

        vendor.Receipts.Add(receipt);
        vendor.HasReceipt = true;
        vendor.AmountOwed -= amount;
        _vendorRepo.Update(vendor);

        return receipt;
    }
}