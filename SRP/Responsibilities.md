> **ملاحظة عن طريقة التفكير:**
>
> وأنا بحدد الـresponsibilities لكل class، اعتمدت بشكل أساسي على فكرة الـ**policies / rules** علشان أحدد الـ**independent reasons to change**.
>
> والمقصود بالـpolicy أو الـrule هنا هو قاعدة أو قرار الـbusiness بيحدده، والقاعدة دي هي اللي بتحدد الـbehavior بتاع الـsystem في موقف معين.
>
> فكنت بسأل نفسي: **لو الـrule دي اتغيرت، إيه الجزء من الـcode اللي هيحتاج يتغير؟**
>
> ولو لقيت إن فيه rules مختلفة ممكن تتغير بشكل مستقل عن بعض، فده معناه إن عندي أسباب مختلفة للـchange، وبالتالي غالبًا عندي responsibilities مختلفة محتاجة تتفصل.
>
> بالطريقة دي، أنا مكنتش بقسم الـclass لمجرد إن كل method تبقى class لوحدها، لكن كنت بحاول أحدد إيه الـbehaviors اللي بتتغير مع بعض، وإيه الـbehaviors اللي ليها أسباب مستقلة للتغيير.

## CheckoutBasket

### Responsibilities

- **Basket Management**
  - Manages the basket's product lines and gift-wrap state.

- **Coupon Handling**
  - Stores and interprets coupon text to determine the applicable discount.

- **Pricing Calculation**
  - Calculates the subtotal, discount amount, and grand total.

- **Gift Message Generation**
  - Generates customer-facing gift messages based on the basket contents.

- **Payment Authorization**
  - Generates a payment authorization code based on the basket's payment information and total.

### Why Having Them Together Is a Problem

These responsibilities have different reasons to change. Basket management can change independently from coupon rules, pricing rules, gift-message wording, and payment authorization rules. Keeping all of them in one class means a change in one responsibility requires modifying the same class that handles unrelated responsibilities, making the class harder to maintain and increasing the risk of affecting unrelated behavior.

============================================================
## GradeBook

### Responsibilities

1. **Grade Recording**
2. **Grading**
3. **Honor Roll Eligibility**
4. **Transcript Generation**
5. **CSV Export**

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. If the grading policy changes, we should not need to modify the same class that handles transcript generation or CSV export. Similarly, if the CSV format changes, it should not require changing the grading logic. Keeping these responsibilities together means that multiple independent changes can affect the same class, which violates the Single Responsibility Principle.
==============================================================
## KitchenTicket

### Responsibilities

1. **Kitchen Ticket Item Recording**
2. **Allergen Detection**
3. **Preparation Time Estimation**
4. **Thermal Ticket Rendering**
5. **Expo Lane Routing**

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. For example, allergen rules may change because of new regulations, while preparation-time calculations may change because of a different kitchen workflow. The thermal ticket format may also change because of a different printer or layout requirement, and the expo lane rules may change independently.

Keeping all of these responsibilities in the same class means that changes in different parts of the system would require modifying the same class. This makes the class responsible for multiple independent reasons to change, which violates the Single Responsibility Principle.

===========================================
## AppointmentDesk

### Responsibilities

1. **Appointment Booking / Management**
2. **Business Hours Policy**
3. **Appointment Slot Search**
4. **ICS Calendar Export**
5. **SMS Reminder Generation**

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. For example, business hours rules may change because of new clinic policies, while the slot search rules may change because of a different scheduling process. The ICS calendar format may change because of calendar interoperability requirements, and the SMS reminder message may change independently because of communication requirements.

Keeping these responsibilities together means that changes in different areas would require modifying the same class. This gives the class multiple independent reasons to change, which violates the Single Responsibility Principle.

==========================================================
## CourseEnrollmentDesk

### Responsibilities

1. **Enrollment / Registration**
2. **Waitlist Management**
3. **Welcome Packet Generation**
4. **Tuition Invoice Generation**

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. Enrollment and waitlist rules may change because of different operational requirements, while the welcome packet content may change because of marketing requirements. Tuition invoice formatting and tax rules may also change independently because of financial requirements.

Keeping these responsibilities together means that changes in different areas would require modifying the same class. This gives the class multiple independent reasons to change, which violates the Single Responsibility Principle.
=================================================
## LoanDesk

### Responsibilities

1. **Loan Application Data** — Stores the applicant's loan request details, such as requested amount, credit score, employment duration, and collateral status.
2. **Risk Assessment and Eligibility** — Calculates the applicant's risk score and determines whether the application is eligible.
3. **Required Document Determination** — Determines which documents the applicant must provide based on compliance and application rules.
4. **Decision Letter Generation** — Generates the applicant-facing approval or rejection letter.
5. **Underwriter CSV Export** — Formats application data and decision details as a CSV row for underwriter/analytics use.

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. The risk model may change based on decisions by the risk committee, while required-document rules may change because of new regulations. The wording of decision letters may change because of legal or communication requirements, and the CSV schema may change to meet analytics or reporting needs. Keeping all these responsibilities in one class means that independent changes in these areas would require modifying the same class, making it harder to maintain and increasing the risk of affecting unrelated behavior.
=========================================
## SubscriptionBilling

### Responsibilities

1. **Subscription Billing State** — Stores customer and billing-period information, and tracks failed payments.
2. **Proration Calculation** — Calculates the amount due based on when the subscription becomes active within the billing period.
3. **Invoice Number Generation** — Creates sequential invoice numbers using the billing period.
4. **Dunning Email Generation** — Creates payment-failure reminder emails, including the notice severity and balance details.
5. **Ledger Journal Formatting** — Formats billing information as a journal row for accounting export.

### Why Having Them Together Is a Problem

These responsibilities have different and independent reasons to change. Proration rules may change because of finance or billing policies, while the invoice numbering scheme may change because of operational or fiscal requirements. Dunning email wording and notice levels may change because of collections or legal requirements, and the ledger export format may change because of accounting or integration needs. Keeping these responsibilities in one class means that independent changes in these areas would require modifying the same class, making it harder to maintain and increasing the risk of affecting unrelated behavior.
====================================
## SupportTicket

### Responsibilities


1. Support Ticket State and Customer Messages
Stores the ticket’s identity, subject, body, opening time, and current priority. It also appends new customer messages to the ticket.

2. Priority Classification
Analyzes the ticket’s subject and body using keyword-based rules to determine its priority (P1, P2, or P3).

3. SLA Deadline and Breach Calculation
Determines the ticket’s SLA deadline based on its priority and checks whether that deadline has passed.

4. Public Reply Generation
Creates a customer-facing reply using the ticket details, agent name, priority, and SLA deadline.

5. Internal Escalation Message Formatting
Produces the internal escalation blurb, including the ticket ID, priority, SLA deadline, and keyword-scan indicator.

Why Having Them Together Is a Problem

These responsibilities change for different reasons. Ticket state and message handling change with the support workflow, while priority classification changes as keyword rules and support playbooks evolve. SLA calculations change when operational policies are updated, and public reply wording changes with customer-experience requirements. Internal escalation formatting may also change independently to meet operational needs. Keeping all of these in one class makes changes less isolated and increases the chance that modifying one concern will affect anothe
=============================================
WardBoard
Responsibilities

1. Bed Assignment and Ward State Management
Stores the patient assigned to each bed and the corresponding acuity score. It also validates and normalizes patient information when assigning a bed.

2. Acuity Scoring
Calculates a patient’s acuity score from heart rate and oxygen saturation (SpO2).

3. Pager Alert Policy and Log Management
Determines whether an assignment meets the pager alert threshold, records pager messages, and returns and clears the accumulated pager log.

4. Nurse Handoff Note Generation
Builds a formatted handoff note using the bed’s patient and acuity information, including the date and status wording.

5. Census CSV Export
Exports the current bed assignments and acuity scores as CSV, ordered by bed number.

Why Having Them Together Is a Problem

The responsibilities have different reasons to change. Bed assignment and ward state change with the ward workflow, while acuity scoring changes with clinical scoring rules. Pager thresholds and logging behavior may be updated independently of the scoring formula. Handoff note wording and CSV structure are separate presentation and export concerns. Keeping them in one class couples these changes and makes the class harder to maintain.
===============================================
WarehousePickList
Responsibilities
Pick List State Management
Stores the requested SKU, warehouse location, required quantity, and available stock for each pick-list line.
Stock Allocation
Determines how many units can actually be allocated for each SKU based on the requested quantity and available stock.
Walking Order Calculation
Determines the order in which items should be picked based on aisle and bin location.
Picker Instruction Generation
Generates the human-readable picking instructions and shortage warnings for the picker.
WMS XML Batch Generation
Formats the allocated items into the XML structure required by the Warehouse Management System.
Why Having Them Together Is a Problem

These responsibilities have different reasons to change. Pick-list state changes with the warehouse workflow, while stock allocation changes when inventory or backorder policies change. The walking-order algorithm can change independently as warehouse layout or routing technology evolves. Picker instructions may change because of handheld-device UX requirements, while the WMS XML format changes according to an external integration contract. Keeping all of these concerns in one class couples unrelated changes and makes the class harder to maintain.
=======================================================