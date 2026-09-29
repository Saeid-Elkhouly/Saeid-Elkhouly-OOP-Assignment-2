namespace Inheritance
{
    public class PremiumMember : Member
    {
        private const int PointsPerReturnedLoan = 5;

        public int ReadingPoints
        {
            get
            {
                int points = 0;
                foreach (Loan loan in Loans)
                {
                    if (loan.Status == LoanStatus.Returned) points += PointsPerReturnedLoan;
                }
                return points;
            }
        }

        public PremiumMember(string personId, string fullName, string phone, decimal discountPercent)
            : base(personId, fullName, phone, 10, discountPercent)
        {
        }
    }
}