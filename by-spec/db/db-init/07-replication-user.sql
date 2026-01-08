SELECT pg_create_physical_replication_slot('replica_slot');

CREATE USER repl_user WITH REPLICATION ENCRYPTED PASSWORD 'postgres';

GRANT SELECT ON ALL TABLES IN SCHEMA public TO repl_user;

GRANT CONNECT ON DATABASE postgres TO repl_user;
GRANT CONNECT ON DATABASE postgres TO postgres;