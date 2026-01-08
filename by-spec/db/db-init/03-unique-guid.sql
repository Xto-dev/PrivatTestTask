CREATE TABLE transaction_guid_unique (
    guid UUID NOT NULL PRIMARY KEY
);

CREATE OR REPLACE FUNCTION enforce_unique_guid()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO transaction_guid_unique (guid) VALUES (NEW.guid);
    RETURN NEW;
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Duplicate GUID detected: %', NEW.guid;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER trg_enforce_unique_guid
BEFORE INSERT ON Transaction
FOR EACH ROW EXECUTE FUNCTION enforce_unique_guid();