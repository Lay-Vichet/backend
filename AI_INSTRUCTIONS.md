AI Agent Instructions: Enforce .NET Best Practices and Coding Standards

Purpose

- Always produce code that is maintainable, secure, and idiomatic for .NET 9.

Formatting & Quality

- Enable nullable reference types and prefer non-nullable references.
- Prefer expression-bodied members for single-line returns where readable.
- Use PascalCase for public types and members, camelCase for private locals.
- Keep methods small (single responsibility); prefer composition over inheritance.
- Add XML doc comments for public surface area.
- Add or update unit tests for new behavior (happy path + relevant edge cases).
- Run `dotnet build` and `dotnet test` after changes; fix failing tests before finishing.

Architecture & Design

- Follow Clean Architecture: keep UI/API thin, application layer contains business logic, domain contains entities/interfaces, infrastructure contains implementations.
- Depend on abstractions (interfaces) in higher layers; wire concrete implementations only in the composition root (`Program.cs`).
- Use DTOs for data exchanged across layers; avoid passing persistence entities to controllers.

Security & Resilience

- Validate inputs and return appropriate status codes.
- Avoid leaking exceptions in responses; prefer structured logging and generic errors to clients.
- Use cancellation tokens for long-running operations when applicable.

Dependency Management

- Pin package versions intentionally. Prefer LTS and widely-used libraries.
- Avoid adding unnecessary dependencies; prefer the BCL.

Testing

- Write unit tests for application logic; mock external dependencies.
- For controllers, use integration tests with TestServer when behavior depends on middleware.

Code Review & CI

- Ensure changes include or update tests; require passing CI before merging.
- Add a brief changelog entry for non-trivial changes.

When in doubt

- Prefer clarity and correctness over clever optimizations.
- If breaking change is necessary, document migration steps.

Note: This file is authoritative for automated agents working on this repository. Always follow it unless a human maintainer provides an explicit exception.
