DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'bamboeup') THEN
    CREATE ROLE bamboeup LOGIN PASSWORD 'bamboeup_dev';
  END IF;
END $$;

SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'BamboeUP.HR' AND pid <> pg_backend_pid();

DROP DATABASE IF EXISTS "BamboeUP.HR";
CREATE DATABASE "BamboeUP.HR" OWNER bamboeup;
GRANT ALL PRIVILEGES ON DATABASE "BamboeUP.HR" TO bamboeup;
