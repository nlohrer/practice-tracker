# practice-tracker
API for tracking practice sessions.

# Usage
## Docker Compose
Run `docker compose up -d` in the root directory and access the web app via `http://localhost:10000`, or access the API directly via `http://localhost:10000/api/Session`. You might need to run the command a second time as the database might not be initialized properly the first time. Run `docker compose down` to remove the containers.

## dotnet run
To run the API locally on Linux, you need to have the .NET SDK 8.0 installed, as well as the EF Core tools, which you can install with:

```bash
dotnet tool install --global dotnet-ef
```

Make sure you have PostgreSQL listening on port 54321, with password 'pass' for user 'postgres'. You could use Docker for that:

```bash
docker run -d -p 54321:5432 -e POSTGRES_PASSWORD=pass postgres
```

After that, run:

```bash
git clone https://github.com/nlohrer/practice-tracker.git
cd practice-tracker/PracticeTrackerAPI
dotnet ef database update
dotnet run
```

The web app will be available under `http://localhost:5263`, and the API will be available under `http://localhost:5263/api/Session`.
