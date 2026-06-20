using System;
using System.IO;
using System.Threading.Tasks;
using GatherUp.API.Dtos;
using GatherUp.API.Security;
using GatherUp.BL;
using GatherUp.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialController : ControllerBase
{
    private readonly FinanceService        _financeService;
    private readonly EventDashboardService _dashboardService;

    public FinancialController(FinanceService financeService, EventDashboardService dashboardService)
    {
        _financeService   = financeService;
        _dashboardService = dashboardService;
    }

    private IActionResult? EnsureCallerManagesEvent(int eventId)
    {
        var ev = _dashboardService.GetEventDetails(eventId);
        if (ev == null) return NotFound(new { error = "האירוע לא נמצא." });
        if (ev.EventManagerId != User.GetUserId()) return Forbid();
        return null;
    }

    [Authorize]
    [HttpGet("event/{eventId}/summary")]
    public IActionResult GetSummary([FromRoute] int eventId)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;
        return Ok(_financeService.GetAccountSummary(eventId));
    }

    [Authorize]
    [HttpGet("event/{eventId}/receipts")]
    public IActionResult GetReceiptsReport([FromRoute] int eventId)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;
        return Ok(_financeService.GetFlattenedReceiptsReport(eventId));
    }

    /// <summary>
    /// הוספת חוב לספק — אם vendorId עדיין לא קיים, זו בפועל רישום ספק חדש
    /// (עם ה-Id/שם/סכום שסופקו); אם הוא כבר קיים, הסכום פשוט מצטרף לחוב הקיים.
    /// </summary>
    [Authorize]
    [HttpPost("event/{eventId}/vendor/{vendorId}/debt")]
    public IActionResult AddVendorDebt([FromRoute] int eventId, [FromRoute] int vendorId, [FromBody] AddVendorDebtRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;

        var vendor = _financeService.RegisterOrAddVendorDebt(eventId, vendorId, request.Name, request.Amount);
        return Ok(vendor);
    }

    /// <summary>
    /// רישום תשלום — המנהל מאשר תשלום בשם משתתף.
    /// Route: POST /api/financial/event/{eventId}/person/{personId}/payment
    /// </summary>
    [Authorize]
    [HttpPost("event/{eventId}/person/{personId}/payment")]
    public IActionResult RegisterPayment(
        [FromRoute] int eventId,
        [FromRoute] int personId,
        [FromBody] PaymentRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;

        _financeService.RegisterPayment(eventId, personId, request.Amount);
        return NoContent();
    }

    [Authorize]
    [HttpPost("event/{eventId}/reminders")]
    public IActionResult SendReminders([FromRoute] int eventId, [FromBody] PaymentReminderRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;
        _financeService.SendPaymentReminders(eventId, request.Message);
        return NoContent();
    }

    /// <summary>
    /// העלאת קבלה — [Authorize] רגיל; בדיקת בעלות אירוע נקודתית דרך vendorId.
    /// </summary>
    [Authorize]
    [HttpPost("vendor/{vendorId}/receipt")]
    public async Task<IActionResult> UploadReceipt(
        [FromRoute] int vendorId,
        [FromForm]  string receiptNumber,
        [FromForm]  decimal amount,
        [FromForm]  DateTime date,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BusinessValidationException("יש לצרף קובץ קבלה.");

        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(file.FileName));
        using (var stream = new FileStream(tempPath, FileMode.Create))
            await file.CopyToAsync(stream);

        try
        {
            var receipt = _financeService.UploadVendorReceipt(vendorId, receiptNumber, amount, date, tempPath);
            return StatusCode(201, receipt);
        }
        finally
        {
            if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath);
        }
    }
}