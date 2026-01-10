\set QUIET on

SELECT count(*) = 0 as is_new_sub FROM pg_subscription WHERE subname = 't1_replication_sub' \gset

\if :is_new_sub
    \echo 'Subscription does not exist. Creating...'
    CREATE SUBSCRIPTION t1_replication_sub
    CONNECTION 'host=primary port=5432 dbname=postgres user=postgres password=postgres'
    PUBLICATION t1_replication_pub;
\else
    \echo 'Subscription already exists. Skipping.'
\endif