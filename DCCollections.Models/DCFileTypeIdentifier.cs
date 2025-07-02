using RMCollectionProcessor.Models;

namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Defines the types of RM files that can be processed.
    /// </summary>
    public enum DCFileType
    {
        /// <summary>
        /// The file type could not be determined.
        /// </summary>
        Unknown,
        /// <summary>
        /// A file containing collection requests (PAIN.008).
        /// </summary>
        CollectionRequest,
        /// <summary>
        /// A file containing status reports (PAIN.002).
        /// </summary>
        StatusReport,
        /// <summary>
        /// A reply file from the bank acknowledging a transmission.
        /// </summary>
        Reply
    }

    /// <summary>
    /// Provides functionality to identify the type of a parsed RM file.
    /// </summary>
    public static class DCFileTypeIdentifier
    {
        /// <summary>
        /// Identifies the file type by inspecting the parsed record objects.
        /// </summary>
        /// <param name="parsedRecords">An array of record objects parsed by the FileHelpers engine.</param>
        /// <returns>The identified <see cref="DCFileType"/>.</returns>
        public static DCFileType Identify(object[] parsedRecords)
        {
            if (parsedRecords == null || parsedRecords.Length == 0)
            {
                return DCFileType.Unknown;
            }

            foreach (var record in parsedRecords)
            {
                if (record is CollectionHeader080)
                {
                    return DCFileType.CollectionRequest;
                }
                if (record is StatusUserSetHeader080)
                {
                    return DCFileType.StatusReport;
                }
                if (record is ReplyTransmissionStatus900 ||
                    record is ReplyUserSetStatus900 ||
                    record is ReplyRejectedMessage901 ||
                    record is ReplyTransmissionRejectReason901)
                {
                    return DCFileType.Reply;
                }
            }

            return DCFileType.Unknown;
        }
    }
}