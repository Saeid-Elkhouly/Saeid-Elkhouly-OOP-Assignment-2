namespace SrpLab;

public sealed class AppointmentDesk
{
    private readonly HashSet<DateTimeOffset> _booked = new();

    public TimeOnly Open { get; }
    public TimeOnly Close { get; }
    public int SlotMinutes { get; }

    public AppointmentDesk(TimeOnly open, TimeOnly close, int slotMinutes)
    {
        Open = open;
        Close = close;
        SlotMinutes = slotMinutes;
    }

    public bool TryBook(DateTimeOffset slot)
    {
        if (!_booked.Contains(slot))
        {
            _booked.Add(slot);
            return true;
        }

        return false;
    }

    public bool IsBooked(DateTimeOffset slot)
    {
        return _booked.Contains(slot);
    }
}

public sealed class BusinessHoursPolicy
{
    public bool IsWithinBusinessHours(
        DateTimeOffset when,
        TimeOnly open,
        TimeOnly close,
        int slotMinutes)
    {
        if (when.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday)
        {
            return false;
        }

        var time = TimeOnly.FromDateTime(when.DateTime);

        return time >= open &&
               time.AddMinutes(slotMinutes) <= close;
    }
}

public sealed class AppointmentSlotSearch
{
    private readonly BusinessHoursPolicy _businessHoursPolicy = new();

    public DateTimeOffset? FindNextSlot(
        DateTimeOffset from,
        int searchHours,
        TimeOnly open,
        TimeOnly close,
        int slotMinutes,
        Func<DateTimeOffset, bool> isBooked)
    {
        var cursor = Align(from, slotMinutes);
        var end = from.AddHours(searchHours);

        while (cursor < end)
        {
            var withinBusinessHours =
                _businessHoursPolicy.IsWithinBusinessHours(
                    cursor,
                    open,
                    close,
                    slotMinutes);

            if (withinBusinessHours && !isBooked(cursor))
            {
                return cursor;
            }

            cursor = cursor.AddMinutes(slotMinutes);
        }

        return null;
    }

    private DateTimeOffset Align(DateTimeOffset from, int slotMinutes)
    {
        var minutes = from.Minute - (from.Minute % slotMinutes);

        return new DateTimeOffset(
            from.Year,
            from.Month,
            from.Day,
            from.Hour,
            minutes,
            0,
            from.Offset);
    }
}

public sealed class IcsCalendarExporter
{
    public string Export(
        DateTimeOffset slot,
        int slotMinutes,
        string patientName,
        string clinician)
    {
        var uid = Guid.NewGuid();
        var end = slot.AddMinutes(slotMinutes);

        return "BEGIN:VCALENDAR\nVERSION:2.0\nBEGIN:VEVENT\n" +
               $"UID:{uid}\n" +
               $"DTSTART:{slot:yyyyMMdd'T'HHmmss'Z'}\n" +
               $"DTEND:{end:yyyyMMdd'T'HHmmss'Z'}\n" +
               $"SUMMARY:Visit {patientName} / {clinician}\n" +
               "END:VEVENT\nEND:VCALENDAR\n";
    }
}

public sealed class SmsReminderGenerator
{
    public string Generate(
        DateTimeOffset slot,
        string clinicPhone)
    {
        return $"Reminder: appointment {slot:MMM dd HH:mm}. " +
               $"Call {clinicPhone} to reschedule.";
    }
}