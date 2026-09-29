namespace SrpLab;

public sealed class WardBoard
{
    private readonly WardState _wardState = new();
    private readonly AcuityScorer _acuityScorer = new();
    private readonly PagerAlertLog _pagerAlertLog = new();
    private readonly NurseHandoffNoteBuilder _handoffNoteBuilder = new();
    private readonly CensusCsvExporter _censusCsvExporter = new();

    public void AssignBed(int bed, string patientId, int heartRate, int spo2)
    {
        if (bed <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bed));
        }

        if (string.IsNullOrWhiteSpace(patientId))
        {
            throw new ArgumentException("patient required");
        }

        var normalizedPatientId = patientId.Trim().ToUpperInvariant();
        var acuityScore = _acuityScorer.Score(heartRate, spo2);

        _wardState.AssignBed(bed, normalizedPatientId, acuityScore);
        _pagerAlertLog.RecordIfRequired(bed, acuityScore);
    }

    public int ScoreAcuity(int heartRate, int spo2)
    {
        return _acuityScorer.Score(heartRate, spo2);
    }

    public string BuildHandoffNote(int bed)
    {
        if (!_wardState.TryGetBedDetails(bed, out var patient, out var acuity))
        {
            return $"Bed {bed}: empty";
        }

        return _handoffNoteBuilder.Build(bed, patient, acuity, DateTime.UtcNow);
    }

    public IReadOnlyList<string> DrainPagerLog()
    {
        return _pagerAlertLog.Drain();
    }

    public string ExportCensusCsv()
    {
        return _censusCsvExporter.Export(_wardState.GetCensusRows());
    }
}

internal sealed class WardState
{
    private readonly Dictionary<int, string> _bedPatient = new();
    private readonly Dictionary<int, int> _vitalsScore = new();

    public void AssignBed(int bed, string patientId, int acuityScore)
    {
        _bedPatient[bed] = patientId;
        _vitalsScore[bed] = acuityScore;
    }

    public bool TryGetBedDetails(int bed, out string patient, out int acuity)
    {
        if (!_bedPatient.TryGetValue(bed, out patient!))
        {
            acuity = 0;
            return false;
        }

        acuity = _vitalsScore[bed];
        return true;
    }

    public IReadOnlyList<CensusRow> GetCensusRows()
    {
        return _bedPatient.Keys
            .OrderBy(bed => bed)
            .Select(bed => new CensusRow(
                bed,
                _bedPatient[bed],
                _vitalsScore[bed]))
            .ToList();
    }
}

internal sealed class AcuityScorer
{
    public int Score(int heartRate, int spo2)
    {
        var score = 0;

        if (heartRate > 120 || heartRate < 45)
        {
            score += 4;
        }
        else if (heartRate > 100)
        {
            score += 2;
        }

        if (spo2 < 90)
        {
            score += 5;
        }
        else if (spo2 < 94)
        {
            score += 2;
        }

        return Math.Min(score, 10);
    }
}

internal sealed class PagerAlertLog
{
    private readonly List<string> _pagerLog = new();

    public void RecordIfRequired(int bed, int acuityScore)
    {
        if (acuityScore >= 8)
        {
            _pagerLog.Add($"CODE-YELLOW bed={bed} at {DateTime.UtcNow:HH:mm}");
        }
    }

    public IReadOnlyList<string> Drain()
    {
        var copy = _pagerLog.ToList();
        _pagerLog.Clear();
        return copy;
    }
}

internal sealed class NurseHandoffNoteBuilder
{
    public string Build(
        int bed,
        string patient,
        int acuity,
        DateTime utcNow)
    {
        var tone = acuity >= 8
            ? "ESCALATE"
            : acuity >= 4
                ? "WATCH"
                : "STABLE";

        return $"[HANDOFF {utcNow:yyyy-MM-dd}] Bed {bed} · {patient} · acuity={acuity} · {tone}";
    }
}

internal sealed class CensusCsvExporter
{
    public string Export(IReadOnlyList<CensusRow> rows)
    {
        var lines = new List<string> { "bed,patient,acuity" };

        foreach (var row in rows)
        {
            lines.Add($"{row.Bed},{row.PatientId},{row.AcuityScore}");
        }

        return string.Join('\n', lines);
    }
}

internal sealed record CensusRow(int Bed, string PatientId, int AcuityScore);