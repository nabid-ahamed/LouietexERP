# Functionality, Input and Output - LouietexERP System

## Table 6.1 Functionality, Input and Output

| Functionality | Input | Output |
| :--- | :--- | :--- |
| **User Registration** | Full Name, Email, Password, Profile Picture, Registration Date | User account created, pending admin approval notification |
| **User Authentication** | Email, Password | Login successful, redirection to dashboard or error message |
| **Employee Management** | Full Name, Email, Department, Role, Joining Date | Staff record created/updated, confirmation message |
| **Order Creation** | Buyer Name, Style/Item Code, Total Quantity, Delivery Deadline | Order record stored, confirmation message, dashboard update |
| **Production Logging** | Linked Order ID, Line Number, Target Quantity, Actual Output | Production record saved, efficiency calculation, real-time chart sync |
| **QC Inspection** | Production ID, Defect Count, QC Status (Pass/Fail), Remarks | Inspection result logged, production status updated |
| **Inventory Tracking** | Material Name, Category, Current Quantity, Minimum Stock Level | Stock levels updated, low stock alert if below minimum |
| **Dashboard Analytics** | Filter criteria (Date, Status, Line), Refresh trigger | Dynamic visual charts, performance metrics summary |
| **PDF Report Export** | Report Type (Order/Production/QC), Filter range | Professional PDF document generated and downloaded |
| **Admin Approval** | Pending User ID, Approval/Rejection Decision | User access granted/denied, role assignments finalized |
| **Profile Update** | New Name, New Email, New Profile Picture | Change request submitted for admin review, pending status |
