import { FileType } from './rm-processor.enums';
import { RmParsedRecord } from './rm-records.model';

/**
 * Checks if a value is a record object that includes our $type discriminator.
 * This is a TypeScript "type guard" that narrows the type from `unknown`.
 *
 * @param value - The value to check.
 * @returns `true` if the value is a non-null object with a `$type` property, otherwise `false`.
 */
function isDiscriminatedRecord(value: unknown): value is { $type: string; [key: string]: unknown } {
  // We ensure the value is an object, not null, and has the '$type' key.
  return typeof value === 'object' && value !== null && '$type' in value;
}

/**
 * Identifies the file type by inspecting the parsed record objects from the server.
 * This function is a direct port of the logic from the C# `FileTypeIdentifier.Identify` method,
 * adapted for the TypeScript/JavaScript environment.
 *
 * It iterates through the records and uses the `$type` discriminator property to identify
 * the first record that defines the file's overall type.
 *
 * @param parsedRecords - An array of parsed record objects. For type safety, this is treated as `unknown[]`.
 * @returns The identified `FileType`. Returns `FileType.Unknown` if the array is empty or no identifying record is found.
 */
export function identifyFileType(parsedRecords: unknown[]): FileType {
  // Guard against null, undefined, or empty arrays, matching the C# implementation.
  if (!parsedRecords || parsedRecords.length === 0) {
    return FileType.Unknown;
  }

  for (const record of parsedRecords) {
    // Use our type guard to safely inspect each item in the array.
    // This protects against items that might be null, primitives, or not have the discriminator.
    if (!isDiscriminatedRecord(record)) {
      continue;
    }

    // This switch statement is the TypeScript equivalent of the C# `is` operator checks.
    // It is highly performant and provides excellent type safety.
    // Based on the `$type` string, the TypeScript compiler can infer the full
    // type of `record` within each case block (e.g., it knows `record` is ICollectionHeader080).
    switch (record.$type) {
      case 'CollectionHeader080':
        return FileType.CollectionRequest;

      case 'StatusUserSetHeader080':
        return FileType.StatusReport;

      // These cases fall through, mimicking the C# `||` condition.
      // If any of these record types are found, the file is identified as a Reply.
      case 'ReplyTransmissionStatus900':
      case 'ReplyUserSetStatus900':
      case 'ReplyRejectedMessage901':
      case 'ReplyTransmissionRejectReason901':
        return FileType.Reply;

      // No default case is needed; we simply continue the loop if the type isn't one we care about.
    }
  }

  // If the loop completes without finding an identifying record, return Unknown.
  return FileType.Unknown;
}