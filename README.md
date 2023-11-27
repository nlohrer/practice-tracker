# practice-tracker
API for tracking practice sessions.

# Usage
## Docker Compose
Run `docker compose up -d` in the root directory and access the API via `http://localhost:10000/api/Session`. Run `docker compose down` to remove the containers.

## dotnet run
Make sure you have PostgreSQL listening on port 54321, with password 'pass' for user 'postgres'. You could use Docker for that: `docker run -d -p 54321:5432 -e POSTGRES_PASSWORD=pass postgres`. After that, run:

```bash
git clone https://github.com/nlohrer/practice-tracker.git
cd PracticeTrackerAPI
dotnet ef database update
dotnet run
```
