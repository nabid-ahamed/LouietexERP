# Complexity of Data Functions - LouietexERP System

## Table 5.2 Complexity of Data Functions

| Function Name | Types | Fields | DET | RET |
| :--- | :--- | :--- | :--- | :--- |
| **Users** | ILF | UserID, FullName, Email, Password, Phone, Role, IsApproved, IsDisabled, ProfilePicturePath, RegistrationDate | 10 | 1 |
| **Roles** | ILF | RoleID, RoleName, NormalizedName, ConcurrencyStamp | 4 | 1 |
| **Employees** | ILF | EmployeeID, FullName, Email, Department, Role, Joining Date | 6 | 1 |
| **Orders** | ILF | OrderID, BuyerName, StyleCode, TotalQuantity, DeliveryDate, Status, CreatedAt | 7 | 1 |
| **Productions** | ILF | ProductionID, OrderID, LineNumber, Supervisor, TargetQuantity, ActualOutput, DefectCount, Status, StartDate, EndDate, CreatedAt, UpdatedAt | 12 | 1 |
| **QCInspections** | ILF | InspectionID, ProductionID, CheckedByUserId, DefectCount, Remarks, QCStatus, InspectionDate, InspectorName, CreatedAt, UpdatedAt | 10 | 1 |
| **Inventory** | ILF | InventoryID, Name, Category, Quantity, MinStockLevel, Supplier | 6 | 1 |
| **ProfileRequests** | ILF | RequestID, UserId, NewFullName, NewEmail, NewPhoneNumber, NewProfilePicturePath, RequestDate, Status, RequestType, IsProcessed, ProcessedByUserId, ProcessedDate | 12 | 1 |
