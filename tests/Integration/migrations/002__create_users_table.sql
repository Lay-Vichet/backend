-- create users table for authentication
CREATE TABLE IF NOT EXISTS users (
    user_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    email text NOT NULL UNIQUE,
    password_hash text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now()
);
-- Migration: create users table
CREATE TABLE IF NOT EXISTS users (
    user_id uuid PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now()
);
