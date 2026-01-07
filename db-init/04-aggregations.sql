CREATE TABLE client_operation_totals (
    client_id INT NOT NULL,
    operation_type TEXT NOT NULL,
    total_amount NUMERIC(15,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (client_id, operation_type)
);

CREATE OR REPLACE FUNCTION update_totals_on_status_change()
RETURNS TRIGGER AS $$
DECLARE
    cid INT;
    optype TEXT;
BEGIN
    IF OLD.status = 0 AND NEW.status = 1 THEN
        cid := (NEW.message->>'client_id')::INT;
        optype := NEW.message->>'operation_type';
        INSERT INTO client_operation_totals (client_id, operation_type, total_amount)
        VALUES (cid, optype, NEW.amount)
        ON CONFLICT (client_id, operation_type)
        DO UPDATE SET total_amount = client_operation_totals.total_amount + EXCLUDED.total_amount;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_totals
AFTER UPDATE OF status ON t1
FOR EACH ROW EXECUTE FUNCTION update_totals_on_status_change();