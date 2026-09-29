namespace SrpLab;

public sealed class CheckoutBasket
{
    private readonly List<(string Sku, decimal Price, int Qty)> _lines = new();
    private string? _couponRaw;
    private bool _giftWrap;

    public void AddLine(string sku, decimal price, int qty)
    {
        if (qty <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(qty));
        }

        _lines.Add((sku, price, qty));
    }

    public void ApplyCouponText(string? couponText)
    {
        _couponRaw = couponText;
    }

    public void EnableGiftWrap()
    {
        _giftWrap = true;
    }

    public IReadOnlyList<(string Sku, decimal Price, int Qty)> Lines => _lines;

    public string? CouponRaw => _couponRaw;

    public bool GiftWrapEnabled => _giftWrap;
}



public sealed class CouponParser
{
    public  decimal CalculateDiscount(string? couponText, decimal subTotal)
    {
        if (string.IsNullOrWhiteSpace(couponText))
        {
            return 0m;
        }

        var t = couponText.Trim().ToUpperInvariant();

        if (t.StartsWith("SAVE") &&
            int.TryParse(t[4..], out var pct) &&
            pct is > 0 and <= 50)
        {
            return Math.Round(subTotal * pct / 100m, 2);
        }

        if (t.Contains("FREESHIP"))
        {
            return 0m;
        }

        if (t == "WELCOME10")
        {
            return Math.Min(10m, subTotal);
        }

        return 0m;
    }
}


public sealed class PricingCalculator
{
    private readonly CouponParser _couponParser;

    public PricingCalculator(CouponParser couponParser)
    {
        _couponParser = couponParser;
    }

    public decimal CalculateSubTotal(
        IReadOnlyList<(string Sku, decimal Price, int Qty)> lines)
    {
        return lines.Sum(l => l.Price * l.Qty);
    }

    public decimal CalculateDiscount(
        IReadOnlyList<(string Sku, decimal Price, int Qty)> lines,
        string? couponText)
    {
        var subTotal = CalculateSubTotal(lines);

        return _couponParser.CalculateDiscount(couponText, subTotal);
    }

    public decimal CalculateGrandTotal(
        IReadOnlyList<(string Sku, decimal Price, int Qty)> lines,
        string? couponText,
        bool giftWrap)
    {
        var subTotal = CalculateSubTotal(lines);
        var discount = _couponParser.CalculateDiscount(couponText, subTotal);

        var total = subTotal - discount;

        if (giftWrap)
        {
            total += 4.99m;
        }

        return Math.Max(0m, total);
    }
}

public sealed class GiftMessageGenerator
{
    public string Generate(
        string fromName,
        IReadOnlyList<(string Sku, decimal Price, int Qty)> lines,
        decimal grandTotal)
    {
        var items = string.Join(", ", lines.Select(l => l.Sku));

        return $"Dear friend,\n" +
               $"A gift from {fromName} awaits ({items}).\n" +
               $"Total surprise value: {grandTotal:C}\n";
    }
}


public sealed class PaymentAuthorizer
{
    public string Authorize(
        decimal grandTotal,
        string cardLast4,
        int itemCount)
    {
        var payload = $"{grandTotal:0.00}|{cardLast4}|{itemCount}";
        var hash = payload.GetHashCode();

        return $"AUTH-{Math.Abs(hash):X8}";
    }
}