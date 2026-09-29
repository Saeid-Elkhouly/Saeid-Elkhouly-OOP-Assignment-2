namespace Inheritance
{
    public class Shelver : Staff
    {
        public string Section { get; private set; }

        public Shelver(string personId, string fullName, string phone, DateOnly hireDate, decimal monthlySalary, string section)
            : base(personId, fullName, phone, hireDate, monthlySalary, 0m)
        {
            if (string.IsNullOrWhiteSpace(section)) throw new ArgumentException("Section must not be empty.");

            Section = section;
        }

        public void Reassign(string newSection)
        {
            if (string.IsNullOrWhiteSpace(newSection)) throw new ArgumentException("Section must not be empty.");

            Section = newSection;
        }
    }
}