namespace Inheritance
{
    public class Member : Person
    {
        private readonly List<Loan> _loans = new List<Loan>();

        public int MaxLoans { get; }
        public decimal DiscountPercent { get; }

        public IReadOnlyList<Loan> Loans
        {
            get { return _loans.AsReadOnly(); }
        }

        protected Member(string personId, string fullName, string phone, int maxLoans, decimal discountPercent)
            : base(personId, fullName, phone)
        {
            if (maxLoans <= 0) throw new ArgumentException("Max loans must be greater than zero.");
            if (discountPercent < 0 || discountPercent > 100) throw new ArgumentException("Discount must be between 0 and 100.");

            MaxLoans = maxLoans;
            DiscountPercent = discountPercent;
        }

        public Loan Borrow(LibraryItem item, int loanId, DateOnly borrowDate)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }


            int activeLoans = 0;
            foreach (Loan existing in _loans)
            {
                if (existing.Status == LoanStatus.Borrowed) activeLoans++;
            }

            if (activeLoans >= MaxLoans)
                throw new InvalidOperationException($"{FullName} already has {activeLoans} active loans (maximum {MaxLoans}).");

            Loan loan = new Loan(loanId, borrowDate, this, item);
            item.MarkBorrowed();
            _loans.Add(loan);

            return loan;
        }
    }
}