# QuickLoan - Loan Management System

A modern loan application management system built with ASP.NET Core MVC.

## Features

### For Applicants
- Apply for loans with detailed application forms
- Track loan application status
- View loan history
- Receive notifications on application updates

### For Administrators
- View all loan applications in a clean table layout
- Filter applications by status (All, Pending, Approved, Rejected, Draft)
- Approve or reject loan applications
- Add admin notes to applications
- View detailed information for each application

## Admin Loan Applications View

The admin panel displays all loan applications in a responsive table format with the following columns:

- **Applicant**: Full name of the loan applicant
- **Contact**: Email and mobile phone number
- **Amount**: Loan amount requested and term length
- **Product**: Product type and monthly repayment amount
- **Status**: Current application status (color-coded badges)
- **Date**: Application submission date
- **Action**: Quick action buttons (Approve/Reject for pending, View Details for others)

### Status Filtering
Applications can be filtered by status:
- All
- Pending
- Approved
- Rejected
- Draft

### Color Coding
Applications are color-coded by status with left border indicators:
- Green: Approved
- Red: Rejected
- Blue: Pending/Submitted
- Gray: Draft

## Technical Stack

- **Framework**: ASP.NET Core MVC
- **Frontend**: Bootstrap 5, Custom CSS
- **Language**: C# (Backend), HTML/CSS/JavaScript (Frontend)

## Getting Started

### Prerequisites
- .NET 6.0 or higher
- SQL Server (or your preferred database)

### Installation

1. Clone the repository
```bash
git clone <repository-url>
cd QuickLoan
```

2. Update database connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  }
}
```

3. Run database migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

5. Navigate to `https://localhost:5001` (or your configured port)

## Solutions Structure
<img width="349" height="190" alt="image" src="https://github.com/user-attachments/assets/53a61997-8dfb-44d4-9f15-acb552069001" />

## API Endpoints

### Admin Controller
- `GET /Admin/LoanApplications` - Get all loan applications
- `GET /Admin/LoanApplications?status={status}` - Filter by status
- `POST /Admin/ApproveLoan` - Approve a loan application
- `POST /Admin/RejectLoan` - Reject a loan application

### Loan Controller
- `GET /Loan/Apply` - Loan application form
- `POST /Loan/Apply` - Submit loan application
- `GET /Loan/Details/{id}` - View loan details
- `GET /Loan/MyLoans` - View user's loan applications

### Screens
<img width="651" height="792" alt="image" src="https://github.com/user-attachments/assets/b215a976-692e-4659-a408-df669ea046d7" />
<img width="648" height="571" alt="image" src="https://github.com/user-attachments/assets/ab2fe42c-8002-477e-bd98-2f23b38ef0dc" />
<img width="604" height="555" alt="image" src="https://github.com/user-attachments/assets/dbc1f7b3-d385-41be-a2f4-02a7161c5a66" />
<img width="636" height="783" alt="image" src="https://github.com/user-attachments/assets/23a5cd38-1dd2-4dd4-810a-224503113d6f" />
<img width="616" height="768" alt="image" src="https://github.com/user-attachments/assets/8cc0bfe5-afdb-4c8a-9b0a-8e6143bb6f61" />
<img width="610" height="604" alt="image" src="https://github.com/user-attachments/assets/59a9bbfa-1a2c-479d-93f3-241c63db25f9" />


