using System;
using System.Text.Json;
using System.Threading.Tasks;
using GatherUp.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GatherUp.API.Middleware;

/// <summary>
/// המידלוור המרכזי לטיפול בשגיאות. עוטף את כל הצינור בבלוק try-catch גלובלי
/// אחד, כדי שלא יהיה צורך בבלוקי try-catch ידניים בתוך הקונטרולרים. מתפוס את
/// שלושת החריגים העסקיים שהוגדרו ב-GatherUp.Core.Exceptions ומתרגם אותם
/// לסטטוס HTTP תקני, ולכל חריג אחר (כשל דיסק, באג לא צפוי וכו') מחזיר 500.
/// חובה להיות הראשון בצינור (ב-Program.cs) כדי שיוכל לעטוף גם את כל שאר
/// המידלוורים שאחריו.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            await WriteErrorResponse(context, 404, "NOT_FOUND", ex.Message);
        }
        catch (ReceiptLockedException ex)
        {
            await WriteErrorResponse(context, 400, "RECEIPT_LOCKED", ex.Message);
        }
        catch (BusinessValidationException ex)
        {
            await WriteErrorResponse(context, 400, "VALIDATION_ERROR", ex.Message);
        }
        catch (Exception ex)
        {
            // שגיאת שרת לא צפויה - באג, או תקלת דיסק/קריאת XML.
            await WriteErrorResponse(context, 500, "INTERNAL_SERVER_ERROR", "אירעה שגיאת שרת קריטית. פרטים: " + ex.Message);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var errorBody = new { error = message, code };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorBody));
    }
}
