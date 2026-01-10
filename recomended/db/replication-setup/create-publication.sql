DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_publication WHERE pubname = 't1_replication_pub') THEN
        CREATE PUBLICATION t1_replication_pub FOR TABLE t1;
    END IF;
END
$$;