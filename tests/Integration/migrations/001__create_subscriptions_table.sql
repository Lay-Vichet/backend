-- initial migration: create subscriptions table
CREATE TABLE IF NOT EXISTS subscriptions (
    id uuid PRIMARY KEY,
    name text NOT NULL,
    monthly_cost numeric NOT NULL DEFAULT 0,
    start_date timestamp without time zone NOT NULL DEFAULT now()
);
