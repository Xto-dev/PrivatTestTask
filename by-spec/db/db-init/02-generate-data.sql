INSERT INTO TRANSACTION (date, status, amount, guid, message)
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
FROM generate_series(1, 1000000);