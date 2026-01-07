CREATE SUBSCRIPTION t1_replication_sub
CONNECTION 'host=primary port=5432 dbname=postgres user=postgres password=postgres'
PUBLICATION t1_replication_pub;