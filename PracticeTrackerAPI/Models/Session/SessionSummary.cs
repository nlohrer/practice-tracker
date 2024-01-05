namespace PracticeTrackerAPI.Models.Session
{
    public record SessionSummary
    {
        /// <summary>
        /// Amount of practice sessions.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Mean practice duration in minutes.
        /// </summary>
        public double? DurationMean {  get; set; }

        /// <summary>
        /// Variance of practice duration in square minutes.
        /// </summary>
        public double? DurationVariance { get; set; }

        /// <summary>
        /// Median practice duration.
        /// </summary>
        public Duration? DurationMedian { get; set; }

        /// <summary>
        /// Minimum practice duration.
        /// </summary>
        public Duration? DurationMinimum { get; set; }

        /// <summary>
        /// Maximum practice duration.
        /// </summary>
        public Duration? DurationMaximum { get; set; }

        /// <summary>
        /// Date of the first practice session.
        /// </summary>
        public DateOnly? FirstDate { get; set; }

        /// <summary>
        /// Date of the last practice session.
        /// </summary>
        public DateOnly? LastDate { get; set; }

        /// <summary>
        /// Amount of different days practiced.
        /// </summary>
        public int DayAmount { get; set; }
    }
}
