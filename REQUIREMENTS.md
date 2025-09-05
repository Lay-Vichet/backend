Functional Requirements (FRs):

User Authentication:

FR1.1: The system must allow a user to register with a unique email and a password.

FR1.2: The system must securely store the password using hashing and salting (e.g., bcrypt).

FR1.3: The system must allow a registered user to log in with their email and password.

FR1.4: Upon successful login, the system must generate a secure JSON Web Token (JWT) for session management.

Database Setup:

FR1.5: The backend must connect to a PostgreSQL database instance.

FR1.6: The system must have the users table as previously defined, with columns for user_id, email, password_hash, and timestamps.

API Endpoints:

FR1.7: An API endpoint POST /api/auth/register must be created to handle user registration.

FR1.8: An API endpoint POST /api/auth/login must be created to handle user login.

Non-Functional Requirements (NFRs):

Security: Passwords must never be stored in plain text. JWTs must be short-lived and securely stored.

Performance: Login and registration requests must be processed in under 500ms.

Reliability: The system must handle and return clear error messages for invalid input (e.g., email already in use, incorrect password).

Scalability: The authentication system must be designed to handle a growing number of users without significant performance degradation.
