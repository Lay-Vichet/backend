-- initial migration: create subscriptions table
CREATE TABLE IF NOT EXISTS subscriptions (
    id uuid PRIMARY KEY,
    name text NOT NULL,
    cost numeric NOT NULL DEFAULT 0,
    currency text NOT NULL DEFAULT 'USD',
    billing_cycle text NOT NULL DEFAULT 'Monthly',
    start_date timestamp without time zone NOT NULL DEFAULT now(),
    next_due_date timestamp without time zone NULL,
    notes text NOT NULL DEFAULT ''
);
