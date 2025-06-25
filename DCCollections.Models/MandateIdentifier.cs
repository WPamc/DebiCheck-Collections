using RMCollectionProcessor.Models;

namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Defines the types of RM Mandate files that can be processed.
    /// </summary>
    public enum MandateFileType
    {
        /// <summary>
        /// The mandate file type could not be determined.
        /// </summary>
        Unknown,
        /// <summary>
        /// A file containing mandate initiation requests (MDTERMS).
        /// </summary>
        Initiation,
        /// <summary>
        /// A file containing mandate amendment requests (MDTEAMND).
        /// </summary>
        Amendment,
        /// <summary>
        /// A file containing mandate cancellation requests (MDTECANC).
        /// </summary>
        Cancellation
    }

    /// <summary>
    /// Provides functionality to identify the type of a parsed RM Mandate file.
    /// </summary>
    public static class MandateIdentifier
    {
        /// <summary>
        /// Identifies the mandate file type by inspecting the parsed record objects.
        /// It checks for unique transaction records first, then falls back to the service type in the header.
        /// </summary>
        /// <param name="parsedRecords">An array of record objects parsed by the FileHelpers engine.</param>
        /// <returns>The identified <see cref="MandateFileType"/>.</returns>
        public static MandateFileType Identify(object[] parsedRecords)
        {
            if (parsedRecords == null || parsedRecords.Length == 0)
            {
                return MandateFileType.Unknown;
            }

            foreach (var record in parsedRecords)
            {
                if (record is MandateInitiationTxLine01)
                {
                    return MandateFileType.Initiation;
                }
                if (record is MandateAmendmentTxLine01)
                {
                    return MandateFileType.Amendment;
                }
                if (record is MandateCancellationTxLine01)
                {
                    return MandateFileType.Cancellation;
                }
                if (record is MandateUserSetHeader080 header)
                {
                    switch (header.ServiceType.Trim())
                    {
                        case "MDTERMS":
                            return MandateFileType.Initiation;
                        case "MDTEAMND":
                            return MandateFileType.Amendment;
                        case "MDTECANC":
                            return MandateFileType.Cancellation;
                    }
                }
            }

            return MandateFileType.Unknown;
        }
    }
}
