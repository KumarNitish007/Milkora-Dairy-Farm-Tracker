# Milkora Dairy Farm Tracker — Database (MS SQL Server)

Central master database for the Milkora offline-first architecture. The local
SQLite copy on each device is created and kept in sync automatically by
Dotmim.Sync (added in a later step) — you only ever create **this** database by hand.

## Design principles

- **GUID primary keys everywhere** (`UNIQUEIDENTIFIER`) so records created offline
  on different devices never collide when synced.
- **UTC timestamps** (`CreatedAt` / `UpdatedAt` via `SYSUTCDATETIME()`) so sync
  ordering is timezone-safe.
- **Stored procedures only** — the API never runs inline SQL. All parameters are
  typed and bound with `SqlParameter` (SQL-injection safe).
- **Computed persisted columns** for values that are always derived:
  - `MilkProductionLog.TotalMilk`, `TotalValue`
  - `MilkSales.TotalAmount`
  - `BreedingRecords.ExpectedDeliveryDate` = `InseminationDate + 280 days`

## Files (run in order)

| # | File | Purpose |
|---|------|---------|
| 00 | `00_CreateDatabase.sql` | Creates `MilkoraDB` if missing |
| 01 | `01_Tables.sql` | 8 tables + indexes + FKs |
| 02 | `02_StoredProcedures.sql` | All 35 stored procedures |
| 03 | `03_SeedData.sql` | Sample animals / logs / feed (skip for production) |

All scripts are **idempotent** — safe to re-run.

## Tables

`Animals`, `MilkProductionLog`, `MilkSales`, `Expenses`, `Income`,
`HealthRecords`, `BreedingRecords`, `FeedInventory`.

## Stored procedures

- **Animals:** `sp_GetAllAnimals`, `sp_GetAnimalById`, `sp_InsertAnimal`, `sp_UpdateAnimal`, `sp_DeleteAnimal`
- **Milk log:** `sp_GetMilkLogsByDate`, `sp_GetMilkLogsByDateRange`, `sp_InsertMilkLog`, `sp_UpdateMilkLog`
- **Sales:** `sp_GetSalesByDateRange`, `sp_InsertSale`, `sp_UpdatePaymentStatus`
- **Expenses:** `sp_GetExpensesByDateRange`, `sp_InsertExpense`, `sp_UpdateExpense`, `sp_DeleteExpense`
- **Income:** `sp_GetIncomeByDateRange`, `sp_InsertIncome`
- **Health:** `sp_GetHealthRecordsByAnimal`, `sp_GetDueReminders`, `sp_InsertHealthRecord`
- **Breeding:** `sp_GetBreedingRecordsByAnimal`, `sp_InsertBreedingRecord`, `sp_UpdateCalving`
- **Feed:** `sp_GetFeedInventory`, `sp_InsertFeedItem`, `sp_UpdateStock`, `sp_GetLowStock`
- **Reports:** `sp_GetDashboardSummary`, `sp_GetMonthlyReport`, `sp_GetProfitLossReport`, `sp_GetPerAnimalYield`

Every `Insert*` procedure accepts an optional `@Id` (GUID). If `NULL`, the server
generates one with `NEWID()`; either way the id is returned via `SELECT` so ADO.NET
(and offline clients that pass their own GUID) can read it back.

## How to deploy

### Option A — SQL Server / LocalDB with `sqlcmd`
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 00_CreateDatabase.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 01_Tables.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 02_StoredProcedures.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 03_SeedData.sql
```
Replace `-S "(localdb)\MSSQLLocalDB"` with your server, e.g. `-S localhost -U sa -P <password>`.

### Option B — SSMS / Azure Data Studio
Open each file and execute in order (00 → 03).

## Connection string (for the API's `appsettings.json`, next step)
```
Server=(localdb)\MSSQLLocalDB;Database=MilkoraDB;Trusted_Connection=True;TrustServerCertificate=True;
```
