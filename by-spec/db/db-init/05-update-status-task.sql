CREATE OR REPLACE FUNCTION update_transaction_status_by_seconds()
RETURNS void AS $$
DECLARE
    sec integer := EXTRACT(SECOND FROM now())::int;
BEGIN
    IF (sec % 2) = 0 THEN
        UPDATE TRANSACTION
        SET status = 1
        WHERE status = 0
        AND (id % 2) = 0;
    ELSE
        UPDATE TRANSACTION
        SET status = 2
        WHERE status = 0
        AND (id % 2) = 1;
    END IF;
END;
$$ LANGUAGE plpgsql;

SELECT cron.schedule(
    'update_transaction_status_by_seconds_task',
    '*/1 * * * *',
    $$
        SELECT update_transaction_status_by_seconds();
    $$
);