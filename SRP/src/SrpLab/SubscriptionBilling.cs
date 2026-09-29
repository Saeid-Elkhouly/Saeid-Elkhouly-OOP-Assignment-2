namespace SrpLab;

public sealed class SubscriptionBilling
{
    public string CustomerId { get; }
    public decimal MonthlyPrice { get; }
    public DateOnly PeriodStart { get; }
    public DateOnly PeriodEnd { get; }
    public int FailedPayments { get; private set; }

    public SubscriptionBilling(
        string customerId,
        decimal monthlyPrice,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        CustomerId = customerId;
        MonthlyPrice = monthlyPrice;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public void RegisterFailedPayment()
    {
        FailedPayments++;
    }
}

public sealed class ProrationCalculator
{
    public decimal Prorate(
        decimal monthlyPrice,
        DateOnly periodStart,
        DateOnly periodEnd,
        DateOnly activeFrom)
    {
        if (activeFrom <= periodStart)
        {
            return monthlyPrice;
        }

        if (activeFrom >= periodEnd)
        {
            return 0m;
        }

        var totalDays = periodEnd.DayNumber - periodStart.DayNumber;

        if (totalDays <= 0)
        {
            return monthlyPrice;
        }

        var used = periodEnd.DayNumber - activeFrom.DayNumber;

        return Math.Round(monthlyPrice * used / totalDays, 2);
    }
}

public sealed class InvoiceNumberGenerator
{
    private static int _invoiceSeq = 1000;

    public string NextInvoiceNumber(DateOnly periodStart)
    {
        var number = ++_invoiceSeq;
        return $"INV-{periodStart:yyyyMM}-{number:D5}";
    }
}

public sealed class DunningEmailGenerator
{
    private readonly ProrationCalculator _prorationCalculator = new();
    private readonly InvoiceNumberGenerator _invoiceNumberGenerator = new();

    public string Generate(SubscriptionBilling subscription, string customerName, DateOnly asOf)
    {
        var amount = _prorationCalculator.Prorate(
            subscription.MonthlyPrice,
            subscription.PeriodStart,
            subscription.PeriodEnd,
            subscription.PeriodStart);

        var invoice = _invoiceNumberGenerator.NextInvoiceNumber(
            subscription.PeriodStart);

        var severity = subscription.FailedPayments switch
        {
            <= 1 => "friendly reminder",
            2 => "second notice",
            _ => "final notice before suspension"
        };

        return $"Subject: {severity} {invoice}\n" +
               $"Hi {customerName},\n" +
               $"Balance {amount:C} as of {asOf:o} ({subscription.FailedPayments} failures).\n";
    }
}

public sealed class LedgerJournalFormatter
{
    private readonly ProrationCalculator _prorationCalculator = new();
    private readonly InvoiceNumberGenerator _invoiceNumberGenerator = new();

    public string Format(SubscriptionBilling subscription, DateOnly activeFrom)
    {
        var invoice = _invoiceNumberGenerator.NextInvoiceNumber(
            subscription.PeriodStart);

        var amount = _prorationCalculator.Prorate(
            subscription.MonthlyPrice,
            subscription.PeriodStart,
            subscription.PeriodEnd,
            activeFrom);

        return $"{subscription.CustomerId},{invoice},{amount:0.00},AR-SUB";
    }
}