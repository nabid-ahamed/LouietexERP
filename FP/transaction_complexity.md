# Complexity of Transaction Functions - LouietexERP System

## Table 5.3 Complexity of Transaction Functions

| Function Name | Type | Fields | DET | FTR |
| :--- | :--- | :--- | :--- | :--- |
| **User Registration** | EI | FullName, Email, Password, Phone, ProfilePic, BtnSubmit, UserID, Status | 8 | 2 |
| **Approve User (Admin)** | EI | UserID, AdminID, ApprovalStatus, BtnApprove, BtnReject | 5 | 2 |
| **Create Buyer Order** | EI | BuyerName, StyleCode, TotalQuantity, DeliveryDate, BtnSave, OrderID, Status | 7 | 1 |
| **Assign Production Line** | EI | OrderID, LineNumber, TargetQuantity, Supervisor, BtnStart, ProductionID | 6 | 2 |
| **Record QC Result** | EI | ProductionID, DefectCount, QCStatus, Remarks, BtnLog, InspectorID | 6 | 2 |
| **Generate PDF Report** | EO | ReportType, DateRange, FilterCriteria, ExportBtn, FilePath, CalculatedStats | 6 | 3 |
| **Dashboard View** | EQ | FilterDate, ModuleSelector, ChartData, SummaryCards, RefreshBtn | 5 | 4 |
| **Update Inventory Stock** | EI | MaterialID, QuantityChange, Category, Supplier, BtnUpdate, TransactionID | 6 | 1 |
| **Submit Profile Request** | EI | UserId, NewName, NewEmail, NewPhone, NewPic, BtnSubmit, RequestID | 7 | 2 |
| **Manage Employee Record** | EI | FullName, Email, Dept, Role, JoiningDate, BtnSave, EmployeeID | 7 | 1 |
| **Search Productions** | EQ | SearchTerm, LineFilter, StatusFilter, SearchBtn, ListResults | 5 | 1 |
