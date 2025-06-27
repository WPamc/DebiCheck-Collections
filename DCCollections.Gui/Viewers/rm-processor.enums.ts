/**
 * Defines the types of RM files that can be processed.
 * This is a direct port of the C# RMCollectionProcessor.Models.FileType enum.
 */
export enum FileType {
  /**
   * The file type could not be determined.
   */
  Unknown = 'Unknown',

  /**
   * A file containing collection requests (PAIN.008).
   */
  CollectionRequest = 'CollectionRequest',

  /**
   * A file containing status reports (PAIN.002).
   */
  StatusReport = 'StatusReport',

  /**
   * A reply file from the bank acknowledging a transmission.
   */
  Reply = 'Reply',
}