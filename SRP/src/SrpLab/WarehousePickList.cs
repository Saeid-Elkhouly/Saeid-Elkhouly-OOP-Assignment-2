namespace SrpLab;

public sealed class WarehousePickList
{
    private readonly PickListState _state = new();
    private readonly StockAllocator _stockAllocator = new();
    private readonly WalkingOrderCalculator _walkingOrderCalculator = new();
    private readonly PickerScriptGenerator _pickerScriptGenerator = new();
    private readonly WmsXmlBatchGenerator _wmsXmlBatchGenerator = new();

    public void AddNeed(
        string sku,
        string aisle,
        int bin,
        int qtyNeeded,
        int qtyOnHand)
    {
        _state.AddNeed(sku, aisle, bin, qtyNeeded, qtyOnHand);
    }

    public IReadOnlyList<(string Sku, int Allocated)> Allocate()
    {
        return _stockAllocator.Allocate(_state.GetLines());
    }

    public IReadOnlyList<(string Aisle, int Bin, string Sku, int Qty)> WalkingOrder()
    {
        return _walkingOrderCalculator.Calculate(_state.GetLines());
    }

    public string PickerScript()
    {
        return _pickerScriptGenerator.Generate(
            _state.GetLines(),
            _stockAllocator,
            _walkingOrderCalculator);
    }

    public string WmsXmlBatch(string batchId)
    {
        return _wmsXmlBatchGenerator.Generate(
            batchId,
            _stockAllocator.Allocate(_state.GetLines()));
    }
}

internal sealed class PickListState
{
    private readonly List<PickLine> _lines = new();

    public void AddNeed(
        string sku,
        string aisle,
        int bin,
        int qtyNeeded,
        int qtyOnHand)
    {
        _lines.Add(new PickLine(
            sku,
            aisle,
            bin,
            qtyNeeded,
            qtyOnHand));
    }

    public IReadOnlyList<PickLine> GetLines()
    {
        return _lines;
    }
}

internal sealed record PickLine(
    string Sku,
    string Aisle,
    int Bin,
    int QtyNeeded,
    int QtyOnHand);

internal sealed class StockAllocator
{
    public IReadOnlyList<(string Sku, int Allocated)> Allocate(
        IReadOnlyList<PickLine> lines)
    {
        var result = new List<(string, int)>();

        foreach (var line in lines)
        {
            var allocated = Math.Min(line.QtyNeeded, line.QtyOnHand);
            result.Add((line.Sku, allocated));
        }

        return result;
    }
}

internal sealed class WalkingOrderCalculator
{
    public IReadOnlyList<(string Aisle, int Bin, string Sku, int Qty)> Calculate(
        IReadOnlyList<PickLine> lines)
    {
        return lines
            .OrderBy(line => line.Aisle)
            .ThenBy(line => line.Bin)
            .Select(line => (
                line.Aisle,
                line.Bin,
                line.Sku,
                Math.Min(line.QtyNeeded, line.QtyOnHand)))
            .Where(item => item.Item4 > 0)
            .ToList();
    }
}

internal sealed class PickerScriptGenerator
{
    public string Generate(
        IReadOnlyList<PickLine> lines,
        StockAllocator stockAllocator,
        WalkingOrderCalculator walkingOrderCalculator)
    {
        var walkingOrder = walkingOrderCalculator.Calculate(lines);

        var steps = walkingOrder
            .Select((step, index) =>
                $"{index + 1}. Go aisle {step.Aisle} bin {step.Bin}: pick {step.Qty} × {step.Sku}");

        var allocations = stockAllocator.Allocate(lines);

        var shortfalls = allocations.Where(allocation =>
        {
            var need = lines.First(line => line.Sku == allocation.Sku).QtyNeeded;
            return allocation.Allocated < need;
        });

        var warning = shortfalls.Any()
            ? "SHORTAGES: " + string.Join(", ", shortfalls.Select(s => s.Sku))
            : "SHORTAGES: none";

        return string.Join('\n', steps) + "\n" + warning;
    }
}

internal sealed class WmsXmlBatchGenerator
{
    public string Generate(
        string batchId,
        IReadOnlyList<(string Sku, int Allocated)> allocations)
    {
        var parts = allocations
            .Select(allocation =>
                $"<line sku=\"{allocation.Sku}\" qty=\"{allocation.Allocated}\" />");

        return $"<batch id=\"{batchId}\">{string.Join("", parts)}</batch>";
    }
}