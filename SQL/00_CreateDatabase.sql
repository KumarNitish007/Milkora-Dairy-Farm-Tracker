/* =============================================================================
   Milkora Dairy Farm Tracker
   00_CreateDatabase.sql  --  Creates the central MS SQL Server database.
   Run order: 00 -> 01 -> 02 -> 03
   Idempotent: safe to re-run.
   ============================================================================= */

IF DB_ID(N'MilkoraDB') IS NULL
BEGIN
    PRINT 'Creating database MilkoraDB...';
    CREATE DATABASE [MilkoraDB];
END
ELSE
    PRINT 'Database MilkoraDB already exists - skipping create.';
GO

USE [MilkoraDB];
GO
