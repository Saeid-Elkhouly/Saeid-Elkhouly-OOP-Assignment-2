namespace Inheritance
{
    public class LibraryItem
    {
        private readonly decimal _feeMultiplier;

        public string CatalogNumber { get; }
        public string Title { get; }
        public int LoanPeriodDays { get; }
        public decimal BaseLateFee { get; private set; }
        public bool IsWithdrawn { get; private set; }
        public bool IsOnLoan { get; private set; }

        protected LibraryItem(string catalogNumber, string title, decimal baseLateFee, int loanPeriodDays, decimal feeMultiplier)
        {
            if (string.IsNullOrWhiteSpace(catalogNumber)) throw new ArgumentException("Catalog number must not be empty.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title must not be empty.");
            if (baseLateFee <= 0) throw new ArgumentException("Base late fee must be greater than zero.");
            if (loanPeriodDays <= 0) throw new ArgumentException("Loan period must be greater than zero.");
            if (feeMultiplier <= 0) throw new ArgumentException("Fee multiplier must be greater than zero.");

            CatalogNumber = catalogNumber;
            Title = title;
            BaseLateFee = baseLateFee;
            LoanPeriodDays = loanPeriodDays;
            _feeMultiplier = feeMultiplier;
        }

        public decimal GetDailyLateFee()
        {
            return BaseLateFee * _feeMultiplier;
        }

        internal void ChangeBaseLateFee(decimal newFee)
        {
            if (newFee <= 0) throw new ArgumentException("Late fee must be greater than zero.");

            BaseLateFee = newFee;
        }

        internal void Withdraw()
        {
            if (IsWithdrawn) throw new InvalidOperationException($"'{Title}' is already withdrawn.");

            IsWithdrawn = true;
        }

        internal void Restore()
        {
            if (!IsWithdrawn) throw new InvalidOperationException($"'{Title}' is not withdrawn.");

            IsWithdrawn = false;
        }

        internal void MarkBorrowed()
        {
            if (IsWithdrawn) throw new InvalidOperationException($"'{Title}' is withdrawn from circulation and cannot be borrowed.");
            if (IsOnLoan) throw new InvalidOperationException($"'{Title}' is already on loan.");

            IsOnLoan = true;
        }

        internal void MarkReturned()
        {
            if (!IsOnLoan) throw new InvalidOperationException($"'{Title}' is not on loan.");

            IsOnLoan = false;
        }
    }
}