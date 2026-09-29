namespace Inheritance
{
    public class Librarian : Staff
    {
        public Librarian(string personId, string fullName, string phone, DateOnly hireDate, decimal monthlySalary)
            : base(personId, fullName, phone, hireDate, monthlySalary, 0m)
        {
        }

        public void ProcessReturn(Loan loan, DateOnly returnDate)
        {
            if (loan == null) throw new ArgumentNullException(nameof(loan));
            loan.MarkReturned(returnDate);
        }

        public void MarkAsLost(Loan loan)
        {
            if (loan == null) throw new ArgumentNullException(nameof(loan));
            loan.MarkLost();
        }
    }
}