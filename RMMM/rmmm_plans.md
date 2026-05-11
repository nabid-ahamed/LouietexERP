# RMMM Planning - LouietexERP System

## Table 5.4 RMMM Planning for Critical Risks

### Project Risk 1: Infrastructure Failure
| Field | Description |
| :--- | :--- |
| **Risk Name** | System crashes, server downtime, or database connectivity failures. |
| **Probability** | 30-40% (Moderate) |
| **Impact** | Serious |
| **Description** | The system may go offline due to server overload, database locks, or cloud infrastructure failure, preventing all ERP modules from functioning. |
| **Mitigation & Monitoring** | Use of Azure/AWS load balancers, health checks via Application Insights, and automated database backups every 24 hours. |
| **Management** | DevOps team triggers immediate failover to redundant server; restores database state from the most recent point-in-time recovery log. |
| **Status** | Managed |

---

### Project Risk 2: Security & Data Breach
| Field | Description |
| :--- | :--- |
| **Risk Name** | Unauthorized access, data leakage, or malicious hacking attempts. |
| **Probability** | 40-50% (High) |
| **Impact** | Catastrophic |
| **Description** | Attackers may exploit weak passwords or vulnerabilities to access sensitive buyer orders, production specs, or personal staff information. |
| **Mitigation & Monitoring** | Enforce ASP.NET Identity 2FA, daily security log reviews, encryption of sensitive database fields, and mandatory HTTPS. |
| **Management** | Security officer immediately disables compromised accounts, audits the `UserAccessLogs`, and resets all session tokens. |
| **Status** | Active Monitoring |

---

### Project Risk 3: Operational Data Misuse
| Field | Description |
| :--- | :--- |
| **Risk Name** | Staff misuse of the platform or significant manual data entry errors. |
| **Probability** | 20-30% (Moderate) |
| **Impact** | Tolerable to Serious |
| **Description** | Production staff may accidentally delete records or enter incorrect QC results, leading to faulty analytics and reporting. |
| **Mitigation & Monitoring** | Role-Based Access Control (RBAC) to restrict "Delete" actions, comprehensive audit logs for all mutations, and UI-level input validation. |
| **Management** | Supervisors review the `AuditLog` to identify the source of the error and use the "Rollback" feature or manual correction to restore data. |
| **Status** | Managed via RBAC |

---

### Project Risk 4: Data Inconsistency
| Field | Description |
| :--- | :--- |
| **Risk Name** | Discrepancy between Order status and actual Production/QC metrics. |
| **Probability** | 15-25% (Low) |
| **Impact** | Serious |
| **Description** | Failures in the application logic may lead to orders appearing as "Shipped" while production shows as "Incomplete." |
| **Mitigation & Monitoring** | Implementation of strict database transaction scopes, foreign key constraints, and weekly automated data reconciliation scripts. |
| **Management** | Technical lead runs the `DataIntegrityFixer` script to re-sync counters and identifies the logic bug for immediate patching. |
| **Status** | Preventative Measures |
