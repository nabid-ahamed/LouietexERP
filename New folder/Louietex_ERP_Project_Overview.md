# Louietex ERP — Enterprise Resource Planning for Garment Manufacturing
## Comprehensive Project Overview & System Documentation

Welcome to the official technical and operational documentation for **Louietex ERP**. This project is a state-of-the-art enterprise resource planning web application designed specifically for the apparel and garment manufacturing industry. Built with the high-performance **.NET 10.0** framework, the system bridges the gap between buyer orders (merchandising), shop-floor production lines, quality control (QC), materials inventory management, and human resources (HR).

---

## Table of Contents
1. [Executive Summary & Core Objectives](#1-executive-summary--core-objectives)
2. [System Architecture & Technology Stack](#2-system-architecture--technology-stack)
3. [Core Business Modules & Workflows](#3-core-business-modules--workflows)
4. [Domain Data Model & Database Schema](#4-domain-data-model--database-schema)
5. [Software Metrics & Estimation (Function Point Analysis)](#5-software-metrics--estimation-function-point-analysis)
6. [System Design & Project Management Diagrams](#6-system-design--project-management-diagrams)
7. [Step-by-Step Live Demonstration Guide](#7-step-by-step-live-demonstration-guide)
8. [Prerequisites & Running Locally](#8-prerequisites--running-locally)

---

## 1. Executive Summary & Core Objectives

Garment manufacturing involves high-velocity environments with multiple moving parts: thousands of yards of fabric, hundreds of sewing operators, dozens of production lines, and tight export delivery deadlines. **Louietex ERP** was developed to solve the classic coordination problems in these factories by offering a single, integrated source of truth.

### Key Business Objectives:
*   **Operational Visibility**: Live tracking of target vs. actual outputs on individual sewing/cutting lines.
*   **Quality Assurance**: Direct linkage between production batches and Quality Control (QC) inspections, reducing waste and identifying defective lines early.
*   **Inventory Security**: Real-time alerts when crucial materials (e.g., fabric, thread, zippers, packaging) drop below safe minimum stock levels.
*   **Identity & Access Governance**: Multi-tier role-based access control (RBAC) ensuring that sensitive operations (like approving new users, editing inventory, or finalizing shipments) are strictly regulated.
*   **Corporate Reporting**: Instant generation of professional, export-ready A4 PDF reports for factory management.

---

## 2. System Architecture & Technology Stack

The application is engineered using clean architecture principles, leveraging Microsoft's latest enterprise web stack.

```
       +-------------------------------------------------------------+
       |                         Client View                         |
       |  (Bootstrap 5 + HSL Theme Styling + Responsive Dashboards)  |
       +------------------------------+------------------------------+
                                      |
                                      v
       +-------------------------------------------------------------+
       |             ASP.NET Core 10.0 MVC Controllers               |
       |     (Role-Based Security + Identity Account Controllers)    |
       +------------------------------+------------------------------+
                                      |
                       +--------------+--------------+
                       |                             |
                       v                             v
       +-------------------------------+     +-----------------------+
       |  Entity Framework Core 10.0   |     |      QuestPDF Engine  |
       |  (Database Abstraction Layer) |     |  (Binary PDF Exports) |
       +---------------+---------------+     +-----------------------+
                       |
                       v
       +-------------------------------+
       |    SQL Server / SQLite DB     |
       +-------------------------------+
```

### Technology Highlights:
*   **Backend Core**: **ASP.NET Core 10.0 (MVC)** utilizing C# 14. Highly performant, cross-platform, and leveraging native dependency injection.
*   **Database & ORM**: **Entity Framework Core 10.0** with **SQL Server** database provider for persistent, transactional storage, and migration history tracking.
*   **Security & Identity**: **ASP.NET Core Identity** heavily customized. Includes a custom `ApplicationUser` entity with approval flows (`IsApproved`, `IsDisabled`) and a dynamic profile change approval queue.
*   **Report Generation**: **QuestPDF** engine. Unlike heavy HTML-to-PDF wrappers, QuestPDF uses a fluent binary layout engine to construct native PDF reports efficiently.
*   **Frontend UI**: Clean layout utilizing glassmorphism themes, dynamic sidebar navigation, interactive charts (Chart.js integration), and responsive tables styled using custom HSL CSS palettes.

---

## 3. Core Business Modules & Workflows

### 3.1. Identity, Security & Admin Approvals
*   **Registration Security**: When a new staff member registers, their account is locked by default (`IsApproved = false`). Only a **SuperAdmin** or **Admin** can review and approve new accounts via the dashboard.
*   **Multi-Role Access Control**: 
    *   `SuperAdmin`: Full control over roles, data, and settings.
    *   `Admin`: User management, approvals, inventory control.
    *   `HR`: Employee rosters, department allocations, hiring logs.
    *   `Merchandiser`: Orders creation, delivery tracking, buyer liaison.
    *   `ProductionManager`: Shop floor lines, scheduling, efficiency monitoring.
    *   `QC`: Defect auditing, quality passes/fails.
    *   `OperationsManager`: Cross-department reporting and macro dashboards.
*   **Profile Audit Queue**: When users request profile updates (change of email, phone, or uploading a new profile picture), the requests are placed in an administrative queue (`ProfileRequest`) for formal approval, maintaining data integrity.

### 3.2. Merchandising & Order Management
*   Acts as the customer-facing portal.
*   Captures orders from international buyers (e.g., Uniqlo, GAP, Next, Primark, Puma).
*   Tracks style/item codes (e.g., `UN-TS-2501`), order quantities, delivery dates, and status codes (Pending, In Production, Shipped, Completed).

### 3.3. Shop-Floor Production Tracking
*   Allocates orders into specific **Production Lines** (e.g., Line-01, Line-02).
*   Monitors supervisor performance, targets set, and actual hourly outputs.
*   Calculates **Line Efficiency** dynamically:
    $$\text{Efficiency (\%)} = \frac{\text{Actual Output}}{\text{Target Quantity}} \times 100$$

### 3.4. Quality Control (QC) & Inspection
*   QC Officers log inspections against active production lines.
*   Captures **Defect Counts** and categorizes status: **Passed**, **Failed**, or **Recheck Required**.
*   Directly links defect rates to the production line dashboard, enabling managers to pause lines with critical stitching or fabric issues.

### 3.5. Inventory & Supply Chain Control
*   Tracks materials across three categories: **Fabric** (Cotton, Denim, Fleece), **Accessories** (YKK Zippers, Sewing Threads, Buttons), and **Packaging** (Polybag, Cartons, Hangtags).
*   **Low Stock Warning System**: Compares current quantities against individual `MinStockLevel` values. When stock dips below thresholds, automatic system alerts are generated to trigger re-orders.

### 3.6. Human Resources (HR)
*   Manages employee profiles, roles, joining dates, and departments (Cutting, Sewing, Finishing, QC, HR, Packing, etc.).
*   Generates team size analytics and growth trends over time.

---

## 4. Domain Data Model & Database Schema

The database relies on strong referential integrity, modeled via Entity Framework Core. Below is the explanation of the primary domain entities.

### Entity Relationships Matrix:
1.  **ApplicationUser (extends IdentityUser)**: Holds authentication data, profiles, and administration flags (`IsApproved`, `ProfilePicturePath`, `PendingProfilePicturePath`).
2.  **Order**: The central business driver. Can contain multiple production runs.
    *   *Relationship*: `Order` 1 ---> N `Production`
3.  **Production**: Tracks shop floor assembly details.
    *   *Relationship*: `Production` 1 ---> N `QCInspection`
4.  **QCInspection**: Logs quality metrics. Holds foreign key to `Production` and a foreign key to `ApplicationUser` (`CheckedByUserId` representing the inspector).
5.  **Inventory**: Standalone dictionary representing storehouse items with warning alerts.
6.  **Employee**: Holds factory payroll and roster data.
7.  **ProfileRequest**: A specialized queue logging user details waiting for administrative oversight.
    *   *Relationship*: `ProfileRequest` N ---> 1 `ApplicationUser` (both requester and processor).

---

## 5. Software Metrics & Estimation (Function Point Analysis)

An advanced software engineering metric was compiled to scientifically size and estimate the Louietex ERP system using Albrecht's **Function Point (FP) Analysis**.

### 5.1. Transactional Functions Complexity (UFP TF = 48)
Transactional functions represent the user interfaces and actions (External Inputs, External Outputs, External Inquiries) mapped to the database:

| Transaction Function | Type | DETs | FTRs | Complexity | UFP Contribution |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **User Registration / Management** | EI | 8 | 2 | Average | 4 |
| **Admin User Approval** | EI | 5 | 2 | Average | 4 |
| **Create Buyer Order** | EI | 7 | 1 | Low | 3 |
| **Assign Production Line** | EI | 6 | 2 | Average | 4 |
| **Record QC Result** | EI | 6 | 2 | Average | 4 |
| **Inventory Stock Management** | EI | 6 | 1 | Low | 3 |
| **Profile Request Submission** | EI | 7 | 2 | Average | 4 |
| **Manage Employee Record** | EI | 7 | 1 | Low | 3 |
| **Admin Dashboard Analytics** | EQ | 5 | 4 | Average | 4 |
| **Production Analytics PDF** | EO | 6 | 3 | Average | 5 |
| **Search Production Details** | EQ | 5 | 1 | Low | 3 |
| **Login / Authentication** | EI | 2 | 2 | Low | 3 |
| **System Pulse Feed** | EQ | 10 | 3 | Average | 4 |

### 5.2. Data Functions Complexity (UFP DF = 63)
Data functions represent internal logical files (ILF) and external interface files (EIF) managed by EF Core:

| Data Entity (ILF) | DETs | RETs | Complexity | UFP Contribution |
| :--- | :---: | :---: | :---: | :---: |
| **Users** | 10 | 1 | Low | 7 |
| **Roles** | 4 | 1 | Low | 7 |
| **Employees** | 6 | 1 | Low | 7 |
| **Orders** | 7 | 1 | Low | 7 |
| **Productions** | 12 | 1 | Low | 7 |
| **QCInspections** | 10 | 1 | Low | 7 |
| **Inventory** | 6 | 1 | Low | 7 |
| **ProfileRequests** | 12 | 1 | Low | 7 |
| **AuditLogs** | 8 | 1 | Low | 7 |

*   **Total Unadjusted Function Points (UFP)** = $\text{UFP (TF)} + \text{UFP (DF)} = 48 + 63 = \mathbf{111}$

### 5.3. Value Adjustment Factor (VAF) Calculation
Complexity factors rated on a scale of 0 to 5 based on 14 General System Characteristics (GSCs):

1.  **Data Communications** (4): System requires online communication and real-time alerts.
2.  **Distributed Data Processing** (3): System accessed by factory admins, managers, and inspectors.
3.  **Performance** (4): Live dashboard updates require fast execution speeds.
4.  **Heavily Used Configuration** (4): Concurrent shift check-ins during operations.
5.  **Transaction Rate** (4): Frequent hourly production and QC updates.
6.  **Online Data Entry** (5): 100% of data is inputted via interactive forms.
7.  **End-User Efficiency** (5): Glassmorphism responsive design maximizes operator input speed.
8.  **Online Update** (5): Status feeds are refreshed immediately on update.
9.  **Complex Processing** (4): Calculates live line efficiencies and defect metrics.
10. **Reusability** (3): Native modular services like custom `IExportService`.
11. **Installation Ease** (3): Setup handled via dotnet build and SQLite/SQL Server scripts.
12. **Operational Ease** (4): Simple admin database seeding tools built in.
13. **Multiple Sites** (2): Single factory implementation, ready for branch scaling.
14. **Facilitate Change** (4): Entity frameworks allow quick expansion.

*   **Total Degree of Influence (TDI)** = **54**
*   **Value Adjustment Factor (VAF)** = $0.65 + (0.01 \times \text{TDI}) = 0.65 + 0.54 = \mathbf{1.19}$

### 5.4. Adjusted Function Points (AFP)
$$\text{AFP} = \text{Total UFP} \times \text{VAF} = 111 \times 1.19 = \mathbf{132.09}$$
This value (**132.09 AFP**) indicates a robust, mid-sized enterprise system, reflecting high developer productivity and complex business logic.

---

## 6. System Design & Project Management Diagrams

The repository contains a vast suite of visual diagrams located in the `Documents/` and `Gann Chart/` directories. These files are saved in highly scalable SVG formats:

### 6.1. System Requirements & Diagrams Maps:
*   **Use Case Diagram**: `Documents/use_case_diagram.svg`
    *   *Visualizes user roles (Admin, HR, Merchandiser, Supervisor, QC Officer) and their exact interaction boundaries within the ERP system.*
*   **Class Diagram**: `Documents/ClassDia/class_diagram.svg`
    *   *Highlights database entity layouts, property structures, data types, and primary-key/foreign-key associations.*
*   **System Sequence Diagram**: `Documents/Sequence/sequence_diagram.svg` and `full_system_sequence.svg`
    *   *Models the step-by-step lifecycles of data transactions, such as registration requests flowing from the user to the database and returning admin approvals.*
*   **CRC Cards (Class Responsibility Collaboration)**: `Documents/CRC/crc_cards.svg`
    *   *Tracks object-oriented roles, operations, and collaborating classes.*
*   **Gantt Project Schedule**: `Gann Chart/gantt_chart.svg`
    *   *Details the project timeline, showcasing analysis, UI prototyping, database building, security hardening, and deployment preparation.*

### 6.2. Activity Diagrams (`Documents/`):
Detailed step-by-step activity pipelines:
*   `activity_user_registration.svg` - Sign-up and approval checks.
*   `activity_add_admin.svg` - Setup of super users.
*   `activity_dashboard_analytics.svg` - Aggregating dynamic operational indicators.
*   `activity_inventory_management.svg` - Logging incoming materials and triggering alerts.
*   `activity_manage_orders.svg` - Processing order status changes.
*   `activity_production_tracking.svg` - Lines scheduling.
*   `activity_qc_inspection.svg` - Audit evaluations.
*   `activity_export_reports.svg` - Requesting and compiling PDF files.
*   `activity_view_profile.svg` - Editing profile settings.

### 6.3. Swimlane Process Flows (`Documents/Swimlane/`):
Multi-actor responsibilities mapped out visually:
*   `swimlane_user_registration.svg`
*   `swimlane_add_admin.svg`
*   `swimlane_manage_customer.svg` (Merchandiser <=> Buyer workflow)
*   `swimlane_manage_employees.svg`
*   `swimlane_production_flow.svg`
*   `swimlane_qc_inspection.svg`
*   `swimlane_reports_analytics.svg`
*   `swimlane_view_profile.svg`

---

## 7. Step-by-Step Live Demonstration Guide

To maximize your score during your demonstration, use this polished script, showcasing the pre-populated seed data and system capabilities.

### 7.1. Stage 1: The Landing & Login Experience
1.  **Open the Browser** to the application homepage.
2.  **Point out the Landing page layout** (`Homecontroller1/Landing`), explaining the modern dashboard layout, brand aesthetics, and system overview.
3.  **Explain the Security Gate**: Show that pages are locked behind ASP.NET Core Identity.
4.  **Log in as the SuperAdmin** to show full operational powers:
    *   **Email**: `admin@louietex.com`
    *   **Password**: `Admin@123`

### 7.2. Stage 2: The Command Center (Dashboard)
1.  **Examine the Live KPIs**: Show the statistics for Total Orders, Running Productions, Pending QC Audits, and Low Stock Alerts.
2.  **Point out the Active Activity Feed**: Highlight how the system records real-time logs (e.g., "New Order created", "Low Stock Alert triggered", "New employee joined").
3.  **Show the Dynamic Chart.js integrations**:
    *   Click on **Order Status Chart** (Doughnut) to show how orders are grouped by state (Shipped, Pending, Completed).
    *   Change the **Production Chart Timeframe** (Daily, Weekly, Monthly) to show target vs. actual outputs. Emphasize that the chart fetches live JSON data using AJAX endpoints.

### 7.3. Stage 3: Merchandising & Production Pipeline
1.  Navigate to **Orders**. Show the pre-seeded orders from international buyers (Uniqlo, GAP, Puma).
2.  Show that you can create a new order, edit statuses, or filter active contracts.
3.  Navigate to **Production Tracking**. Show that sewing line outputs are logged hourly.
4.  Point out the **Efficiency Calculator** displaying line percentage ratings based on supervisor output.

### 7.4. Stage 4: Quality Control & Inventory Safety Logs
1.  Navigate to **QC Audits**. Show historical checks logged by inspector names.
2.  Point out a "Failed" status which automatically adds defect quantities directly to the manufacturing dashboard.
3.  Navigate to **Inventory**. Highlight the **Low Stock Warnings**. Find items highlighted in red (items whose current stock is below `MinStockLevel`), proving that the factory will never run out of fabric or thread unexpectedly.

### 7.5. Stage 5: System Admin Tools & PDF Reporting
1.  Navigate to **Reports Dashboard**.
2.  Click on **Export PDF** inside Inventory or Production.
3.  **Open the generated A4 PDF file**: Highlight the clean, professional, branded layout compiled dynamically by the backend **QuestPDF** engine (showing company headers, date stamps, and clean bordered tables).
4.  Log out, click Register, sign up as a new user, and log back in as Admin to show the **Admin Approval Panel** approving/disapproving users.

---

## 8. Prerequisites & Running Locally

### 8.1. System Requirements:
*   **.NET 10.0 SDK** or newer installed.
*   **Visual Studio 2022/2026** (with ASP.NET workload) or **VS Code**.
*   Local database provider (SQL Server/LocalDB or SQLite).

### 8.2. Launch Commands:
1.  Open your terminal inside the project directory:
    ```powershell
    cd C:\Users\hp\source\repos\LouietexERP
    ```
2.  Restore external packages:
    ```powershell
    dotnet restore
    ```
3.  Apply database migrations and initialize tables:
    ```powershell
    dotnet ef database update
    ```
4.  Run the application server:
    ```powershell
    dotnet run
    ```
5.  Access the web app via: `https://localhost:7141` or `http://localhost:5242` (confirm ports via `Properties/launchSettings.json`).

---
*Document prepared on May 20, 2026, for the Louietex ERP Project Demonstration.*
