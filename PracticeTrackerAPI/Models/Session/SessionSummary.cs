namespace PracticeTrackerAPI.Models.Session
{
    public record SessionSummary
    {
        /// <summary>
        /// Amount of practice sessions.
        /// </summary>
        public int Amount;

        /// <summary>
        /// Mean practice duration in minutes.
        /// </summary>
        public decimal? DurationMean;

        /// <summary>
        /// Variance of practice duration in square minutes.
        /// </summary>
        public decimal? DurationVariance;

        /// <summary>
        /// Median practice duration.
        /// </summary>
        public Duration? DurationMedian;

        /// <summary>
        /// Minimum practice duration.
        /// </summary>
        public Duration? DurationMinimum;

        /// <summary>
        /// Maximum practice duration.
        /// </summary>
        public Duration? DurationMaximum;

        /// <summary>
        /// Date of the first practice session.
        /// </summary>
        public DateOnly? FirstDate;

        /// <summary>
        /// Date of the last practice session.
        /// </summary>
        public DateOnly? LastDate;

        /// <summary>
        /// Amount of different days practiced.
        /// </summary>
        public int DayAmount;
    }
}
