Saya ingin mendesain sistem authorization enterprise-grade dengan konsep Ultimate Separation of Duties (SoD) untuk aplikasi internal perusahaan berbasis ASP.NET Core + Blazor + SQL Server.

Lakukan analisa mendalam dan desain arsitektur terbaik dengan target:

1. SANGAT AMAN
2. Audit-ready
3. Tidak memiliki privilege escalation loophole
4. Future-proof untuk module sensitif lain
5. Clean Architecture
6. High performance
7. Mudah di-maintain
8. Mendukung RBAC + ABAC + Approval-based access
9. Zero-trust untuk sensitive module
10. 100/100 secure design

====================================================
BUSINESS CASE
====================================================

Saya memiliki kebutuhan authorization seperti ini:

Ada 3 tipe actor:

1. ADMIN
- Bisa membuat user
- Bisa edit user
- Bisa assign role
- Bisa assign menu permission
- Bisa assign payroll access ke user lain
- TIDAK BOLEH membuka Payroll List
- TIDAK BOLEH melihat data payroll
- TIDAK BOLEH melihat salary
- TIDAK BOLEH melakukan privilege escalation untuk dirinya sendiri
- Bahkan jika admin memberikan payroll access ke dirinya sendiri, system harus tetap menolak akses

2. KEPALA PAYROLL (PAYROLL HEAD)
- Bisa membuka Payroll List
- Bisa melihat salary
- Bisa approve siapa yang boleh akses payroll
- Bisa revoke payroll access
- Menjadi owner dari sensitive module Payroll
- Menjadi approval authority payroll access

3. USER BIASA
- Hanya dapat membuka menu sesuai permission yang diberikan

====================================================
CORE SECURITY REQUIREMENTS
====================================================

Payroll merupakan sensitive/confidential module.

Saya ingin implementasi:

1. ROLE-BASED ACCESS CONTROL (RBAC)
2. ATTRIBUTE-BASED ACCESS CONTROL (ABAC)
3. APPROVAL-BASED ACCESS CONTROL
4. SENSITIVE MODULE OWNERSHIP
5. ZERO TRUST SECURITY
6. DUAL AUTHORIZATION
7. NO SELF-APPROVAL
8. NO PRIVILEGE ESCALATION
9. DENY BY DEFAULT
10. AUDITABLE SECURITY

====================================================
CRITICAL SECURITY RULES
====================================================

Rule #1
Admin dapat memberikan payroll access kepada orang lain.

Tetapi Admin sendiri tidak dapat menggunakan payroll access.

Rule #2
Admin tidak boleh dapat meng-assign dirinya sendiri menjadi payroll authorized user.

Rule #3
Jika admin mengubah database langsung atau assign permission payroll ke dirinya sendiri:
system tetap harus menolak akses.

Rule #4
Payroll access harus membutuhkan:

A. Permission
AND
B. Payroll approval
AND
C. Active approval
AND
D. Sensitive module ownership validation

Jika salah satu gagal:
ACCESS DENIED.

Rule #5
Self-approval forbidden.

Contoh:
Admin tidak boleh approve dirinya sendiri.
Payroll Head tidak boleh approve dirinya sendiri jika policy melarang.

Rule #6
Sensitive module tidak boleh hanya berdasarkan role.

Harus multi-layer validation.

Rule #7
Menu visibility harus berbeda dengan execution permission.

Contoh:
Menu payroll tidak muncul untuk admin.

Tetapi admin masih dapat assign permission payroll kepada orang lain.

Rule #8
System harus memiliki immutable audit log.

Semua action wajib terekam:

- siapa assign access
- siapa approve
- siapa revoke
- kapan dilakukan
- before value
- after value
- IP address
- device/browser
- correlation id

Rule #9
Need emergency break-glass mechanism.

Tetapi:
- membutuhkan approval
- temporary access
- auto expiry
- mandatory audit

Rule #10
Sensitive module harus scalable untuk masa depan:

Contoh future module:
- Payroll
- Finance
- HR Confidential
- Treasury
- Compliance
- Audit
- Trading Approval
- Settlement Override

====================================================
EXPECTED OUTPUT
====================================================

Tolong hasilkan:

1. Security architecture terbaik
2. Authorization flow diagram
3. Database schema lengkap
4. Table structure SQL Server
5. ERD recommendation
6. Menu permission architecture
7. Policy-based authorization strategy
8. Claims-based security strategy
9. Service layer architecture
10. Middleware security flow
11. Authentication + Authorization flow
12. Approval workflow design
13. Sensitive module ownership design
14. Anti privilege escalation design
15. Zero-trust design
16. Audit log architecture
17. Security event tracking
18. Session validation strategy
19. Caching strategy
20. High performance strategy
21. SQL indexing recommendation
22. Clean architecture folder structure
23. ASP.NET Core implementation example
24. Blazor implementation example
25. Sample database seed
26. Authorization code sample
27. Permission evaluation engine
28. Threat model analysis
29. Attack vector analysis
30. Mitigation strategy

====================================================
IMPORTANT DESIGN CONSTRAINTS
====================================================

Technology Stack:
- ASP.NET Core
- Blazor Server
- Entity Framework Core
- SQL Server
- Clean Architecture
- Repository Pattern
- JWT or Cookie Authentication
- Claims-based Authorization

Performance target:
- Authorization validation < 10ms
- Support 10,000+ users
- High scalability

Security target:
- Enterprise-grade
- SOC2 ready
- Audit ready
- Internal banking-grade security

IMPORTANT:
Jangan membuat desain sederhana.

Saya ingin desain ultimate enterprise authorization system dengan SoD (Separation of Duties) yang sangat aman dan minim loophole.

Tunjukkan kemungkinan vulnerability dan bagaimana mencegahnya.

Berikan juga rekomendasi mana yang OVER-ENGINEERING dan mana yang benar-benar diperlukan.