using Inheritance;

// I created program.cs by AI 
DateOnly borrowDay = new DateOnly(2026, 9, 1);

StudentMember student = new StudentMember("S-001", "Ahmed Ali", "01000000001");
PremiumMember premium = new PremiumMember("P-001", "Sara Hassan", "01000000002", 20m);

Librarian librarian = new Librarian("E-001", "Mona Adel", "01000000003", new DateOnly(2020, 3, 1), 3000m);
Shelver shelver = new Shelver("E-002", "Omar Samy", "01000000004", new DateOnly(2022, 6, 15), 2500m, "Fiction");
HeadLibrarian head = new HeadLibrarian("E-003", "Laila Nabil", "01000000005", new DateOnly(2015, 1, 10), 5000m);

Book book1 = new Book("B-001", "Clean Code", 1.00m);
Book book2 = new Book("B-002", "The Pragmatic Programmer", 1.00m);
Book book3 = new Book("B-003", "Refactoring", 1.00m);
Book book4 = new Book("B-004", "Design Patterns", 1.00m);
Book withdrawnBook = new Book("B-005", "Old Atlas", 1.00m);
DVD dvd = new DVD("D-001", "Inception", 1.50m);
Magazine magazine = new Magazine("M-001", "National Geographic", 1.00m);


Console.WriteLine("=== 1. Things that must NOT compile ===");

// Person p = new Person("P-9", "X", "0100");                 // must NOT compile: protected constructor
// Member m = new Member("M-9", "X", "0100", 3, 0m);          // must NOT compile: protected constructor
// Staff s = new Staff("E-9", "X", "0100", borrowDay, 1m, 0m); // must NOT compile: protected constructor
// LibraryItem i = new LibraryItem("X", "X", 1m, 1, 1m);      // must NOT compile: protected constructor
// student.FullName = "New Name";                             // must NOT compile: get-only property
// student.Loans.Add(null);                                   // must NOT compile: IReadOnlyList has no Add
// book1.IsOnLoan = true;                                     // must NOT compile: private set


Console.WriteLine();
Console.WriteLine("=== 2. Borrowing rules ===");

head.Withdraw(withdrawnBook);
try
{
    student.Borrow(withdrawnBook, 1, borrowDay);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (withdrawn item): {ex.Message}");
}

Loan studentLoan1 = student.Borrow(book1, 2, borrowDay);
Loan studentLoan2 = student.Borrow(book2, 3, borrowDay);
Loan studentLoan3 = student.Borrow(book3, 4, borrowDay);

try
{
    premium.Borrow(book1, 5, borrowDay);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (already on loan): {ex.Message}");
}

try
{
    student.Borrow(book4, 6, borrowDay);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (4th item for a student): {ex.Message}");
}

librarian.MarkAsLost(studentLoan3);
Console.WriteLine($"Borrowed -> Lost is legal. Loan {studentLoan3.LoanId} status: {studentLoan3.Status}");


Console.WriteLine();
Console.WriteLine("=== 3. Monthly pay for every kind of staff ===");

List<Staff> staffList = new List<Staff> { librarian, head, shelver };
foreach (Staff staff in staffList)
{
    Console.WriteLine($"{staff.GetType().Name} {staff.FullName}: {staff.GetMonthlyPay():F2}");
}


Console.WriteLine();
Console.WriteLine("=== 4. Loan period and daily late fee for every kind of item ===");

List<LibraryItem> items = new List<LibraryItem> { book1, dvd, magazine };
foreach (LibraryItem item in items)
{
    Console.WriteLine($"{item.GetType().Name} '{item.Title}': {item.LoanPeriodDays} days, daily late fee {item.GetDailyLateFee():F2}");
}


Console.WriteLine();
Console.WriteLine("=== 5. Premium member returns a DVD 5 days late ===");

Loan dvdLoan = premium.Borrow(dvd, 10, borrowDay);
librarian.ProcessReturn(dvdLoan, dvdLoan.DueDate.AddDays(5));
Console.WriteLine($"Due date: {dvdLoan.DueDate}");
Console.WriteLine($"Late fee: {dvdLoan.LateFee:F2}");
Console.WriteLine($"Reading points: {premium.ReadingPoints}");


Console.WriteLine();
Console.WriteLine("=== 6. Illegal status changes ===");

try
{
    librarian.ProcessReturn(dvdLoan, dvdLoan.DueDate.AddDays(6));
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (return twice): {ex.Message}");
}

try
{
    librarian.MarkAsLost(dvdLoan);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (lost after return): {ex.Message}");
}


Console.WriteLine();
Console.WriteLine("=== 7. Validation rules ===");

try
{
    shelver.GiveRaise(0);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (zero raise): {ex.Message}");
}

try
{
    head.ChangeLateFee(book1, -2m);
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (negative fee): {ex.Message}");
}

try
{
    StudentMember nameless = new StudentMember("S-002", "", "01000000009");
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (empty name): {ex.Message}");
}

Loan magazineLoan = premium.Borrow(magazine, 11, borrowDay);
try
{
    librarian.ProcessReturn(magazineLoan, borrowDay.AddDays(-1));
}
catch (Exception ex)
{
    Console.WriteLine($"Rejected (return before borrow date): {ex.Message}");
}

shelver.GiveRaise(10);
shelver.Reassign("Children");
Console.WriteLine($"Shelver after 10% raise: {shelver.MonthlySalary:F2}, section: {shelver.Section}");
