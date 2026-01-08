CREATE TABLE t1_guid_registry (
    operation_guid UUID PRIMARY KEY
);

CREATE OR REPLACE FUNCTION enforce_guid_uniqueness()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO t1_guid_registry (operation_guid) VALUES (NEW.operation_guid);
    RETURN NEW;
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Duplicate operation_guid: %', NEW.operation_guid;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_enforce_guid_uniqueness
BEFORE INSERT ON t1
FOR EACH ROW EXECUTE FUNCTION enforce_guid_uniqueness();