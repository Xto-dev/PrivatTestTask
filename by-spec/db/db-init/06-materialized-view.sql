CREATE MATERIALIZED VIEW IF NOT EXISTS transaction_status_summary AS
SELECT client_id, type, SUM(amount) AS total_amount
FROM (
    SELECT
        (message ->> 'client_id')::INT AS client_id,
        CASE
            WHEN (message ->> 'operation_type') = 'online' THEN 'online'
            ELSE 'offline'
        END AS type,
        amount
    FROM TRANSACTION
    WHERE status = 1
)
GROUP BY client_id, type;

CREATE UNIQUE INDEX transaction_status_summary_unique_idx
ON transaction_status_summary (client_id, type);

REFRESH MATERIALIZED VIEW CONCURRENTLY transaction_status_summary;