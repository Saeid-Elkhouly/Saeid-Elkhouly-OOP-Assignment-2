namespace Inheritance
{
    public class Loan
    {
        public int LoanId { get; }
        public DateOnly BorrowDate { get; }
        public Member Member { get; }
        public LibraryItem Item { get; }
        public LoanStatus Status { get; private set; }
        public DateOnly? ReturnDate { get; private set; }

        public DateOnly DueDate
        {
            get { return BorrowDate.AddDays(Item.LoanPeriodDays); }
        }

        public decimal LateFee
        {
            get
            {
                if (ReturnDate == null) return 0m;

                int daysLate = ReturnDate.Value.DayNumber - DueDate.DayNumber;
                if (daysLate <= 0) return 0m;

                return daysLate * Item.GetDailyLateFee() * (1 - Member.DiscountPercent / 100m);
            }
        }

        internal Loan(int loanId, DateOnly borrowDate, Member member, LibraryItem item)
        {
            if (loanId <= 0) throw new ArgumentException("Loan ID must be greater than zero.");
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (item == null) throw new ArgumentNullException(nameof(item));

            LoanId = loanId;
            BorrowDate = borrowDate;
            Member = member;
            Item = item;
            Status = LoanStatus.Borrowed;
        }

        internal void MarkReturned(DateOnly returnDate)
        {
            if (Status != LoanStatus.Borrowed) throw new InvalidOperationException($"Loan {LoanId} cannot be returned because its status is {Status}.");
            if (returnDate < BorrowDate) throw new ArgumentException("Return date cannot be earlier than the borrow date.");

            Item.MarkReturned();
            Status = LoanStatus.Returned;
            ReturnDate = returnDate;
        }

        internal void MarkLost()
        {
            if (Status != LoanStatus.Borrowed) throw new InvalidOperationException($"Loan {LoanId} cannot be marked as lost because its status is {Status}.");

            Status = LoanStatus.Lost;
        }
    }
}