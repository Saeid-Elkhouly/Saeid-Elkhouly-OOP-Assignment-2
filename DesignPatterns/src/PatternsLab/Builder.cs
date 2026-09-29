namespace PatternsLab;

public class CourseRegistration
{
    public string StudentEmail { get; }
    public string CourseCode { get; }
    public string AccessMode { get; }
    public string? GroupCode { get; }
    public string? DiscountCode { get; }
    public bool SendWhatsApp { get; }
    public bool SendEmailWelcome { get; }
    public string? MentorNote { get; }
    public DateOnly? PreferredStart { get; }

    public CourseRegistration(
        string studentEmail,
        string courseCode,
        string accessMode,
        string? groupCode,
        string? discountCode,
        bool sendWhatsApp,
        bool sendEmailWelcome,
        string? mentorNote,
        DateOnly? preferredStart)
    {
        StudentEmail = studentEmail;
        CourseCode = courseCode;
        AccessMode = accessMode;
        GroupCode = groupCode;
        DiscountCode = discountCode;
        SendWhatsApp = sendWhatsApp;
        SendEmailWelcome = sendEmailWelcome;
        MentorNote = mentorNote;
        PreferredStart = preferredStart;
    }
    public override string ToString()
        => $"{StudentEmail} → {CourseCode} [{AccessMode}] group={GroupCode ?? "-"} discount={DiscountCode ?? "-"} wa={SendWhatsApp} mail={SendEmailWelcome}";

}

public class CourseRegistrationBuilder
{
    private readonly string _studentEmail;
    private readonly string _courseCode;
    private readonly string _accessMode;
    private readonly bool _sendWhatsApp;
    private readonly bool _sendEmailWelcome;

    private string? _groupCode;
    private string? _discountCode;
    private string? _mentorNote;
    private DateOnly? _preferredStart;


    public CourseRegistrationBuilder(
        string studentEmail,
        string courseCode,
        string accessMode,
        bool sendWhatsApp,
        bool sendEmailWelcome)
    {
        _studentEmail = studentEmail;
        _courseCode = courseCode;
        _accessMode = accessMode;
        _sendWhatsApp = sendWhatsApp;
        _sendEmailWelcome = sendEmailWelcome;
    }
    public CourseRegistrationBuilder WithGroupCode(string groupCode)
    {
        _groupCode = groupCode;
        return this;
    }
    public CourseRegistrationBuilder WithDiscountCode(string discountCode)
    {
        _discountCode = discountCode;
        return this;
    }
    public CourseRegistrationBuilder WithMentorNote(string mentorNote)
    {
        _mentorNote = mentorNote;
        return this;
    }
    public CourseRegistrationBuilder WithPreferedStart(DateOnly preferedStart)
    {
        _preferredStart = preferedStart;
        return this;
    }

    public CourseRegistration Build()
    {
        if (_accessMode == "LiveGroup" && _groupCode is null)
        {
            throw new InvalidOperationException("LiveGroup requires GroupCode");
        }
        if (_accessMode == "VideosOnly" && _groupCode is not null)
        {
            throw new InvalidOperationException("VideosOnly cannot have GroupCode");
        }

        return new CourseRegistration(studentEmail: _studentEmail, courseCode: _courseCode, accessMode: _accessMode, groupCode: _groupCode, discountCode: _discountCode, sendWhatsApp: _sendWhatsApp, sendEmailWelcome: _sendEmailWelcome, mentorNote: _mentorNote, preferredStart: _preferredStart);
    }
}
public static class RegistrationCallSites
{
    public static CourseRegistration CreateLiveStudent()
    {
        return new CourseRegistrationBuilder(
                "sara@mail.com",
                "SEF-101",
                "LiveGroup",
                true,
                true)
            .WithGroupCode("G1")
            .WithDiscountCode("EARLY10")
            .WithMentorNote("Needs evening slot")
            .WithPreferedStart(new DateOnly(2026, 10, 1))
            .Build();
    }

    public static CourseRegistration CreateVideosOnly()
    {
        return new CourseRegistrationBuilder(
                "ali@mail.com",
                "SEF-101",
                "VideosOnly",
                false,
                true)
            .Build();
    }
}