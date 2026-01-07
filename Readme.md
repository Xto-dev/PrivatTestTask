# Transaction Processing System — Test Task (PrivatBank)

This project implements a transaction processing system with partitioning, background processing, incremental aggregation, and logical replication.  
The solution is designed to be close to real production scenarios.

---

## Implemented Requirements

| # | Requirement | Implementation |
|---|------------|----------------|
| 1 | Partitioned table `t1` | Range partitioning by `operation_date` (4 months) |
| 2 | ≥ 100k records | Generated during initialization using `generate_series()` |
| 3 | Unique `operation_guid` | Separate registry table + `BEFORE INSERT` trigger |
| 4 | Insert a record every 5 seconds | .NET Background Worker with precise timing |
| 5 | Update status every 3 seconds | .NET Background Worker with time-based logic |
| 6 | Aggregation by `client_id` and `operation_type` | Incremental aggregation table with trigger |
| 7 | Replication of `t1` | PostgreSQL logical replication |

---

## Key Design Decisions

### Background Processing
PostgreSQL does not reliably support scheduled tasks with second-level precision.  
To ensure accurate timing and predictable behavior, background logic is implemented in an external .NET Worker Service.

### Aggregation Strategy
Instead of materialized views, incremental aggregation is used.  
This approach keeps aggregated data always up to date and scales well with large data volumes.

### GUID Uniqueness
Due to PostgreSQL limitations on unique indexes in partitioned tables, GUID uniqueness is enforced via a separate non-partitioned table.

---

## How to Run

```bash
docker-compose up --build
```

The system automatically:

1. Initializes the primary database with test data,
2. Initializes the replica schema,
3. Configures logical replication,
4. Starts background services.

---

## Tech Stack

- PostgreSQL 16
- .NET 10 (Worker Service, Npgsql)
- Docker Compose
