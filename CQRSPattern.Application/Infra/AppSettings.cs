namespace CQRSPattern.Application.Infra
{
    /// <summary>
    /// Appsettings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Number of retries when processing a message results in an exception.
        /// </summary>
        public int RetryOnFail { get; set; }

        /// <summary>
        /// Number of records we fetch per page.
        /// </summary>
        public int PageSize { get; set; }

        public string[] ProjectsToSyncProjectIds { get; set; }
        public string[] ProjectsToSyncReproCodes { get; set; }

        /// <summary>
        /// Enable / Disable swagger
        /// </summary>
        public bool EnableSwagger { get; set; }

    }
}
