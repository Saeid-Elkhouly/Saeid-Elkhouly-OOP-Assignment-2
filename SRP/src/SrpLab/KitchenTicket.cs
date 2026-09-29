namespace SrpLab;

public sealed class KitchenTicket
{
    private readonly List<(string Item, List<string> Ingredients, int PrepMinutes)> _items = new();

    public void AddItem(string item, IEnumerable<string> ingredients, int prepMinutes)
    {
        _items.Add((
            item,
            ingredients.Select(i => i.Trim().ToLowerInvariant()).ToList(),
            prepMinutes));
    }

    public IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> Items => _items;
}

public sealed class AllergenDetector
{
    public IReadOnlyList<string> Detect(
        IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> items)
    {
        var hits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (_, ingredients, _) in items)
        {
            foreach (var ing in ingredients)
            {
                if (ing.Contains("milk") || ing.Contains("cheese") || ing.Contains("butter"))
                {
                    hits.Add("dairy");
                }

                if (ing.Contains("wheat") || ing.Contains("flour") || ing.Contains("bread"))
                {
                    hits.Add("gluten");
                }

                if (ing.Contains("peanut") || ing.Contains("almond") || ing.Contains("cashew"))
                {
                    hits.Add("nuts");
                }

                if (ing.Contains("shrimp") || ing.Contains("prawn") || ing.Contains("crab"))
                {
                    hits.Add("shellfish");
                }
            }
        }

        return hits.OrderBy(x => x).ToList();
    }
}

public sealed class PreparationTimeEstimator
{
    private readonly AllergenDetector _allergenDetector = new();

    public int Estimate(
        IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> items,
        int openStations)
    {
        if (openStations <= 0)
        {
            openStations = 1;
        }

        var sequential = items.Sum(i => i.PrepMinutes);
        var parallel = (int)Math.Ceiling(sequential / (double)openStations);

        if (_allergenDetector.Detect(items).Count > 0)
        {
            parallel += 3;
        }

        var longest = items.Count == 0 ? 0 : items.Max(i => i.PrepMinutes);

        return Math.Max(parallel, longest);
    }
}

public sealed class ThermalTicketRenderer
{
    private readonly AllergenDetector _allergenDetector = new();
    private readonly PreparationTimeEstimator _timeEstimator = new();

    public string Render(
        int orderNumber,
        IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> items)
    {
        var width = 32;
        var line = new string('=', width);

        var body = string.Join(
            '\n',
            items.Select(i => $"* {i.Item.ToUpperInvariant()} ({i.PrepMinutes}m)"));

        var allergens = _allergenDetector.Detect(items);

        var allergyLine = allergens.Count == 0
            ? "ALLERGENS: none"
            : "ALLERGENS: " + string.Join(",", allergens);

        var eta = _timeEstimator.Estimate(items, 2);

        return $"{line}\n" +
               $"ORDER #{orderNumber}\n" +
               $"ETA {eta} MIN\n" +
               $"{body}\n" +
               $"{allergyLine}\n" +
               $"{line}\n";
    }
}

public sealed class ExpoLaneRouter
{
    private readonly AllergenDetector _allergenDetector = new();
    private readonly PreparationTimeEstimator _timeEstimator = new();

    public string GetLane(
        IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> items)
    {
        if (_allergenDetector.Detect(items).Count > 0)
        {
            return "LANE-ALLERGY";
        }

        return _timeEstimator.Estimate(items, 2) > 20
            ? "LANE-SLOW"
            : "LANE-FAST";
    }
}