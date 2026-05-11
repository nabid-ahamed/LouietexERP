# Calculation of Total Degree of Influence (TDI) - LouietexERP System

## Table 5.7 Calculation of Total Degree of Influence (TDI)

| SI | General System Characteristics | Brief Description | DI |
| :--- | :--- | :--- | :--- |
| 01 | Data Communication | System requires online communication between modules and real-time status notifications for production updates. | 4 |
| 02 | Distributed Data Processing | Data processing is centralized but accessed by multiple departments (Admin, Production, QC) across the factory. | 3 |
| 03 | Performance | Fast response times are critical for real-time tracking of production lines and generating live analytics. | 4 |
| 04 | Heavily Used Configuration | Multiple concurrent users (Admins, Managers, Inspectors) access the system during high-volume manufacturing shifts. | 4 |
| 05 | Transaction Rate | High frequency of updates for hourly production logs, QC results, and inventory stock changes. | 4 |
| 06 | Online Data Entry | 100% of data entry (Orders, Employees, Production, QC) is performed through interactive web forms. | 5 |
| 07 | End-User Efficiency | User-friendly UI with intuitive glassmorphism design, responsive layouts, and streamlined workflow navigation. | 5 |
| 08 | Online Update | Status updates for Orders and Productions are immediately reflected in the dashboard pulse feed. | 5 |
| 09 | Complex Processing | Complex calculations for line efficiency, defect rates, and multi-stage status validation logic. | 4 |
| 10 | Reusability | Core modules like Authentication, Export Services, and PDF generation are designed for modular reusability. | 3 |
| 11 | Installation Ease | Standard web deployment with automated database migrations and environment configuration. | 3 |
| 12 | Operational Ease | Ease of operation via automated data seeding, dashboard summaries, and simplified administrative tools. | 4 |
| 13 | Multiple Sites | Designed for a single manufacturing facility but architected to support future branch expansion. | 2 |
| 14 | Facilitate Change | System architecture allows easy modification of models and roles (e.g., ProfileRequest workflow). | 4 |

**Total Degree of Influence (TDI): 54**
