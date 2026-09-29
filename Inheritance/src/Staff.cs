namespace Inheritance
{
    public class Staff : Person
    {
        private readonly decimal _allowance;

        public DateOnly HireDate { get; }
        public decimal MonthlySalary { get; private set; }

        protected Staff(string personId, string fullName, string phone, DateOnly hireDate, decimal monthlySalary, decimal allowance)
            : base(personId, fullName, phone)
        {
            if (monthlySalary <= 0) throw new ArgumentException("Monthly salary must be greater than zero.");
            if (allowance < 0) throw new ArgumentException("Allowance must not be negative.");

            HireDate = hireDate;
            MonthlySalary = monthlySalary;
            _allowance = allowance;
        }

        public void GiveRaise(decimal percent)
        {
            if (percent <= 0) throw new ArgumentException("Raise percentage must be greater than zero.");

            MonthlySalary = MonthlySalary + MonthlySalary * percent / 100m;
        }

        public decimal GetMonthlyPay()
        {
            return MonthlySalary + _allowance;
        }
    }
}