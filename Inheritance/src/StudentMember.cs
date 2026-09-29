namespace Inheritance
{
    public class StudentMember : Member
    {
        public StudentMember(string personId, string fullName, string phone)
            : base(personId, fullName, phone, 3, 0m)
        {
        }
    }
}