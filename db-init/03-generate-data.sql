INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
SELECT
    '2025-10-01'::DATE + (random() * 120)::INT,
    round((random() * 10000)::NUMERIC, 2),
    0,
    gen_random_uuid(),
    jsonb_build_object(
        'account_number', 'UA' || lpad((random()*1000000000)::TEXT, 10, '0'),
        'client_id', (random()*50000)::INT + 1,
        'operation_type', CASE WHEN random() < 0.7 THEN 'online' ELSE 'offline' END
    )
FROM generate_series(1, 100000);