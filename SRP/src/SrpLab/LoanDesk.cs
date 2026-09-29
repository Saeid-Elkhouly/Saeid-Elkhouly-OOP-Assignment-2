namespace SrpLab;

public sealed class LoanDesk
{
    public decimal RequestedAmount { get; }
    public int CreditScore { get; }
    public int EmploymentMonths { get; }
    public bool HasCollateral { get; }

    public LoanDesk(
        decimal requestedAmount,
        int creditScore,
        int employmentMonths,
        bool hasCollateral)
    {
        RequestedAmount = requestedAmount;
        CreditScore = creditScore;
        EmploymentMonths = employmentMonths;
        HasCollateral = hasCollateral;
    }
}

public sealed class LoanRiskAssessor
{
    public decimal RiskScore(LoanDesk loan)
    {
        decimal score = 100m;
        score -= Math.Max(0, 700 - loan.CreditScore) * 0.15m;

        if (loan.EmploymentMonths < 6)
        {
            score -= 20m;
        }

        if (loan.RequestedAmount > 50_000m && !loan.HasCollateral)
        {
            score -= 25m;
        }

        if (loan.RequestedAmount > 150_000m)
        {
            score -= 10m;
        }

        return Math.Clamp(score, 0m, 100m);
    }

    public bool IsEligible(LoanDesk loan)
    {
        return RiskScore(loan) >= 55m && loan.CreditScore >= 580;
    }
}

public sealed class RequiredDocumentsPolicy
{
    private readonly LoanRiskAssessor _riskAssessor = new();

    public IReadOnlyList<string> GetRequiredDocuments(LoanDesk loan)
    {
        var docs = new List<string>
        {
            "National ID",
            "Proof of income (3 months)"
        };

        if (loan.RequestedAmount > 40_000m)
        {
            docs.Add("Bank statements (6 months)");
        }

        if (loan.HasCollateral)
        {
            docs.Add("Collateral ownership deed");
        }

        if (loan.EmploymentMonths < 12)
        {
            docs.Add("Employer letter");
        }

        if (!_riskAssessor.IsEligible(loan))
        {
            docs.Add("Manual underwriter referral form");
        }

        return docs;
    }
}

public sealed class DecisionLetterGenerator
{
    private readonly LoanRiskAssessor _riskAssessor = new();
    private readonly RequiredDocumentsPolicy _documentsPolicy = new();

    public string Generate(LoanDesk loan, string applicantName)
    {
        var riskScore = _riskAssessor.RiskScore(loan);

        if (_riskAssessor.IsEligible(loan))
        {
            var documents = _documentsPolicy.GetRequiredDocuments(loan);

            return $"Dear {applicantName},\n" +
                   $"Your request for {loan.RequestedAmount:C} is pre-approved (risk {riskScore:0}).\n" +
                   $"Please upload: {string.Join("; ", documents)}.\n";
        }

        return $"Dear {applicantName},\n" +
               $"We are unable to approve {loan.RequestedAmount:C} at this time.\n" +
               $"Reference risk={riskScore:0}. You may reapply after improving documentation.\n";
    }
}

public sealed class UnderwriterCsvExporter
{
    private readonly LoanRiskAssessor _riskAssessor = new();

    public string ExportRow(LoanDesk loan, string applicationId)
    {
        var riskScore = _riskAssessor.RiskScore(loan);
        var eligibility = _riskAssessor.IsEligible(loan) ? "Y" : "N";

        return $"{applicationId},{loan.CreditScore},{loan.EmploymentMonths}," +
               $"{(loan.HasCollateral ? 1 : 0)},{riskScore:0.00},{eligibility}";
    }
}