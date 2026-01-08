CREATE EXTENSION IF NOT EXISTS pg_cron;
UPDATE cron.job SET nodename = '';
GRANT USAGE ON SCHEMA cron TO postgres;

SELECT cron.schedule(
    'insert_new_transaction_row',
    '*/1 * * * *',
    $$
        INSERT INTO Transaction (date, status, amount, guid, message)
        SELECT
            '2025-10-01':: DATE + (random() * 120)::INT,
            0,
            (random() * 10000)::INT,
            gen_random_uuid(),
            jsonb_build_object(
                'account_number', 'UA' || lpad((random() * 1e10)::TEXT, 10, '0'),
                'client_id', (random() * 1e6)::INT,
                'operation_type', CASE WHEN random() < 0.5 THEN 'online' ELSE 'offline' END
            )
        FROM generate_series(1, 1);
    $$
)