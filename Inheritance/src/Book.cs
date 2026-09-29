using Inheritance;

public class Book : LibraryItem
{
    public Book(string catalogNumber, string title, decimal baseLateFee)
        : base(catalogNumber, title, baseLateFee, 21, 1m)
    {
    }
}
