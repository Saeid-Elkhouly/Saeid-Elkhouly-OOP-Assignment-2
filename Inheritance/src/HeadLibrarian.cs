using Inheritance;

public class HeadLibrarian : Staff
{
    public HeadLibrarian(string personId, string fullName, string phone, DateOnly hireDate, decimal monthlySalary)
        : base(personId, fullName, phone, hireDate, monthlySalary, 400m)
    {
    }

    public void ChangeLateFee(LibraryItem item, decimal newFee)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        item.ChangeBaseLateFee(newFee);
    }

    public void Withdraw(LibraryItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        item.Withdraw();
    }

    public void Restore(LibraryItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        item.Restore();
    }
}
