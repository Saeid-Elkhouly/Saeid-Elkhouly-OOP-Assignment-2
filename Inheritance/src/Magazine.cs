namespace Inheritance
{
    public class Magazine : LibraryItem
    {
        public Magazine(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee, 3, 0.5m)
        {
        }
    }
}