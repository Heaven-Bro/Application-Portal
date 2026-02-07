# Application Portal Technical Documentation

## Overview
This system is a full-stack application portal for a university department. It supports application submission workflows, equipment loan management, administrative review, chairman availability scheduling, and notifications for all application-related events.

## Goals And Scope
- Provide a guided, step-based application process for applicants.
- Enable administrators to review, approve, reject, and schedule appointments for applications.
- Manage equipment loans with assignment, return requests, and damage handling.
- Maintain a clear audit trail and real-time notifications for both applicants and admins.
- Offer a clean, mobile-friendly user experience.

## Methodology
The project follows an Agile, iterative delivery model.
- Features are built in small increments.
- Each increment delivers end-to-end functionality across API, domain logic, and UI.
- Feedback is incorporated quickly to refine workflows and UX.

## Architecture
The solution follows a layered architecture inspired by Clean Architecture.

```
WebApp (Blazor UI)
   |
Api (ASP.NET Core controllers)
   |
Application (CQRS handlers, validation, DTOs)
   |
Domain (Entities, enums, business rules)
   |
Infrastructure (EF Core, repositories, persistence)
   |
Database (MySQL)
```

Key properties:
- Domain layer has no dependencies on other layers.
- Application layer contains business use cases and validation.
- Infrastructure implements persistence and external services.
- API layer is a thin HTTP boundary.
- WebApp is a client that calls APIs.

## Design Patterns
- CQRS with MediatR for command and query separation.
- Repository pattern for persistence abstraction.
- Factory method on domain entities for controlled creation, for example notification creation.
- Layered architecture to separate concerns and reduce coupling.
- DTO mapping to keep API models stable and decoupled from domain entities.

## Tech Stack
Frontend:
- Blazor WebAssembly
- Tailwind style utilities and custom CSS
- Blazored.Toast for user feedback

Backend:
- ASP.NET Core Web API (.NET 9)
- MediatR for CQRS
- FluentValidation for request validation
- JWT authentication with role-based authorization

Database:
- MySQL
- EF Core with Pomelo provider

## Core Domain Areas
Applications:
- Services define multi-step application workflows.
- Applicants submit steps with text and documents.
- Admins approve, reject, or schedule follow-ups.

Equipment:
- Equipment items can be assigned to an application.
- Applicants request return.
- Admins approve, reject, or mark damage.

Notifications:
- Admins get alerts for new applications, step submissions, return requests, and disputes.
- Applicants get alerts for approvals, rejections, and return decisions.

Chairman Availability:
- Admins configure weekly time slots.
- Applicants view availability on the home page in a mobile-friendly format.

## Database Design Summary
The database is normalized around core processes and workflows.

Key tables and responsibilities:
- `Users`: accounts, roles, and status.
- `Services`: configurable services offered.
- `ServiceSteps`: ordered steps for a service.
- `Applications`: applicant requests with status and current step.
- `Submissions`: step submissions with status and content.
- `Documents`: uploaded files tied to submissions or users.
- `Equipment`: inventory for equipment loans.
- `EquipmentAssignments`: assignment lifecycle and return workflow.
- `Notifications`: user-specific alerts and unread tracking.
- `ChairmanAvailabilitySlots`: weekly scheduling slots.

## API Surface
The API is organized by domain controllers. Routes below are shown as path examples.

Auth:
- `POST /api/auth/login`
- `POST /api/auth/register`

Applications:
- `GET /api/Application/{id}`
- `POST /api/Application`
- `POST /api/Application/{id}/submit-step`

Admin Applications:
- `GET /api/AdminApplication`
- `POST /api/AdminApplication/{id}/approve`
- `POST /api/AdminApplication/{id}/reject`
- `POST /api/AdminApplication/{id}/schedule`

Services:
- `GET /api/Services`
- `POST /api/Services`

Users:
- `GET /api/Users`
- `POST /api/Users/{id}/deactivate`

Equipment:
- `GET /api/Equipment`
- `POST /api/Equipment`

Equipment Assignments:
- `GET /api/EquipmentAssignments/application/{applicationId}`
- `POST /api/EquipmentAssignments/{id}/request-return`
- `POST /api/EquipmentAssignments/{id}/approve-return`
- `POST /api/EquipmentAssignments/{id}/reject-return`
- `POST /api/EquipmentAssignments/{id}/mark-damaged`
- `POST /api/EquipmentAssignments/{id}/acknowledge-damage`
- `POST /api/EquipmentAssignments/{id}/dispute-damage`

Documents:
- `POST /api/Document/upload`

Chairman Availability:
- `GET /api/ChairmanAvailability`
- `POST /api/ChairmanAvailability`

Notifications:
- `GET /api/Notifications`
- `GET /api/Notifications/unread-count`
- `POST /api/Notifications/{id}/read`
- `POST /api/Notifications/read-all`

## System Design And Data Flow
Application submission flow:
1. Applicant creates an application.
2. API persists application and steps.
3. Notification is generated for admins.
4. Admin reviews and approves or rejects steps.
5. Applicant receives a notification and status updates.

Equipment return flow:
1. Applicant requests a return.
2. Admin is notified.
3. Admin approves, rejects, or marks damage.
4. Applicant receives a notification with the decision.
5. Equipment availability is updated accordingly.

Notifications flow:
1. Business actions create `Notification` entities.
2. Notifications are stored with `IsRead = false`.
3. UI polls unread count and lists notifications.
4. Users mark items read to clear badges.

Chairman availability flow:
1. Admin updates weekly slots in settings.
2. Slots are stored in `ChairmanAvailabilitySlots`.
3. Home page queries and renders a mobile-friendly schedule.

## Security
- JWT authentication for all protected endpoints.
- Role-based authorization for Admin and Applicant actions.
- Server-side validation with FluentValidation.

## Error Handling And UX
- API returns explicit error messages for invalid requests.
- UI displays toasts for success and failure feedback.
- UI disables actions during in-flight operations to avoid duplicates.

## Performance Considerations
- Queries are scoped to the authenticated user.
- Notifications are paged with a `take` parameter.
- Read/unread counts are fetched separately for lightweight updates.

## Deployment And Configuration
- Connection string is configured in application settings.
- Database migrations are managed by EF Core.
- JWT settings are configured in appsettings or environment variables.

## Future Improvements
- Real-time notifications using SignalR.
- Admin notification filters and advanced search.
- Background jobs for scheduled reminders.
