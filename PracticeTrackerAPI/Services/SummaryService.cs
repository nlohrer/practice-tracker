using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public class SummaryService : ISummaryService
    {
        /// <summary>
        /// Query a session summary from the database.
        /// </summary>
        /// <param name="username">The <paramref name="username"/> of the user who's practice sessions should be summarized.</param>
        /// <param name="context">The database context for the user's sessions.</param>
        /// <returns>A summary of the provided user's practice sessions.</returns>
        public async Task<SessionSummary> GetSessionSummary(string username, SessionContext context)
        {
            // return default object if no user was found
            bool userExists = await context.Sessions.AnyAsync(session => session.Username == username);
            if (!userExists)
            {
                return new SessionSummary { Amount = 0 };
            }

            // using postgreSQL's aggregation methods for the summary
            // querying the mean of the squared duration is a shortcut to calculate the variance
            string queryString = """
                select
                    count(date) "Amount",
                    count (distinct date) "DayAmount",
                    max(duration) "DurationMaximum",
                    min(duration) "DurationMinimum",
                    max(date) "LastDate",
                    min(date) "FirstDate",
                    round(avg(minutes)) "DurationMean",
                    round(avg(minutes * minutes)) "DurationSquaredMean",
                    percentile_cont(0.5) within group(order by duration) "DurationMedian"
                from (
                        select "Duration" duration, "Date" date, extract(epoch from "Duration"::interval)/60 minutes
                        from "Sessions"
                        where "Username" = @username) sessions
                """;
            NpgsqlParameter usernameParam = new NpgsqlParameter("username", username);
            SummaryQuery summaryQuery = context.Database
                .SqlQueryRaw<SummaryQuery>(queryString, usernameParam)
                .First();

            SessionSummary summary = summaryQuery.ToSummary();

            return summary;
        }

        private record SummaryQuery(int Amount,
            int DayAmount,
            TimeSpan DurationMaximum,
            TimeSpan DurationMinimum,
            DateOnly LastDate,
            DateOnly FirstDate,
            double DurationMean,
            double DurationSquaredMean,
            TimeSpan DurationMedian)
        {
            public SessionSummary ToSummary()
            {
                return new SessionSummary
                {
                    Amount = this.Amount,
                    DayAmount = this.DayAmount,
                    DurationMaximum = new Duration { Hours = this.DurationMaximum.Hours, Minutes = this.DurationMinimum.Minutes },
                    DurationMinimum = new Duration { Hours = this.DurationMinimum.Hours, Minutes = this.DurationMinimum.Minutes },
                    DurationMean = this.DurationMean,
                    DurationMedian = new Duration { Hours = this.DurationMedian.Hours, Minutes = this.DurationMedian.Minutes },
                    DurationVariance = (this.DurationSquaredMean - this.DurationMean * this.DurationMean), // Var(X) = E(X^2) - (E(X))^2
                    FirstDate = this.FirstDate,
                    LastDate = this.LastDate
                };
            }
        }
    }
}
