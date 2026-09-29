namespace SrpLab;

public sealed class CourseEnrollmentDesk
{
    private readonly HashSet<string> _seated = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _waitlist = new();
    public int SeatedCount => _seated.Count;

    public int Capacity { get; }
    public decimal Tuition { get; }
    public string CourseCode { get; }

    public CourseEnrollmentDesk(string courseCode, int capacity, decimal tuition)
    {
        CourseCode = courseCode;
        Capacity = capacity;
        Tuition = tuition;
    }

    public string Register(string studentEmail)
    {
        if (string.IsNullOrWhiteSpace(studentEmail))
        {
            throw new ArgumentException("email");
        }

        var email = studentEmail.Trim();

        if (_seated.Contains(email) || _waitlist.Contains(email))
        {
            return "ALREADY_REGISTERED";
        }

        if (_seated.Count < Capacity)
        {
            _seated.Add(email);
            return "SEATED";
        }

        _waitlist.Add(email);
        return $"WAITLIST:{_waitlist.Count}";
    }

    public bool IsSeated(string studentEmail)
    {
        return _seated.Contains(studentEmail);
    }

    public IReadOnlyList<string> Waitlist => _waitlist;

    public void SeatStudent(string studentEmail)
    {
        _seated.Add(studentEmail);
    }

    public void RemoveFromWaitlist(string studentEmail)
    {
        _waitlist.Remove(studentEmail);
    }
}

public sealed class WaitlistManager
{
    public int GetPosition(
        IReadOnlyList<string> waitlist,
        string studentEmail)
    {
        var index = waitlist.ToList().FindIndex(
            x => x.Equals(studentEmail, StringComparison.OrdinalIgnoreCase));

        return index < 0 ? -1 : index + 1;
    }

    public void Promote(
        CourseEnrollmentDesk enrollmentDesk,
        int seats)
    {
        while (
            seats > 0 &&
            enrollmentDesk.Waitlist.Count > 0 &&
         enrollmentDesk.SeatedCount < enrollmentDesk.Capacity)
        {
            var next = enrollmentDesk.Waitlist[0];s

            enrollmentDesk.RemoveFromWaitlist(next);
            enrollmentDesk.SeatStudent(next);

            seats--;
        }
    }
}

public sealed class WelcomePacketGenerator
{
    private readonly WaitlistManager _waitlistManager = new();

    public string Generate(
        CourseEnrollmentDesk enrollmentDesk,
        string studentEmail,
        string studentName)
    {
        var status = enrollmentDesk.IsSeated(studentEmail)
            ? "confirmed seat"
            : $"waitlist #{_waitlistManager.GetPosition(
                enrollmentDesk.Waitlist,
                studentEmail)}";

        return $"# Welcome to {enrollmentDesk.CourseCode}\n" +
               $"Hi {studentName},\n" +
               $"Your status: **{status}**.\n" +
               $"Bring a laptop. Discord onboarding link: " +
               $"https://example.invalid/{enrollmentDesk.CourseCode.ToLowerInvariant()}\n";
    }
}

public sealed class TuitionInvoiceGenerator
{
    public string Generate(
        CourseEnrollmentDesk enrollmentDesk,
        string studentEmail)
    {
        if (!enrollmentDesk.IsSeated(studentEmail))
        {
            return $"{enrollmentDesk.CourseCode},WAITLIST,0.00";
        }

        var vat = Math.Round(enrollmentDesk.Tuition * 0.14m, 2);

        return $"{enrollmentDesk.CourseCode},TUITION," +
               $"{enrollmentDesk.Tuition:0.00},VAT,{vat:0.00}," +
               $"TOTAL,{(enrollmentDesk.Tuition + vat):0.00}";
    }
}