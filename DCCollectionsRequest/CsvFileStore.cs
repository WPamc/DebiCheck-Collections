using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RMCollectionProcessor.Models;

namespace RMCollectionProcessor
{
    public class CsvFileStore
    {
        private readonly string _path;
        private readonly object _lock = new();

        public CsvFileStore(string path)
        {
            _path = path;
            if (!File.Exists(_path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                File.WriteAllText(_path,
                    "Filename,DataSetStatus,GenerationNumber,RecordSequenceNumber,SubmissionDate,PaymentInformation,RequestedCollectionDate,InstructedAmount,MandateReference,ContractReference,RelatedCycleDate,FileType\n");
            }
        }

        public void AddRecords(IEnumerable<TransactionRecord> records)
        {
            lock (_lock)
            {
                var existing = new HashSet<string>();
                if (File.Exists(_path))
                {
                    foreach (var line in File.ReadLines(_path).Skip(1))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(',');
                        if (parts.Length >= 4)
                        {
                            var key = $"{parts[2].Trim()},{parts[3].Trim()}";
                            existing.Add(key);
                        }
                    }
                }

                using var writer = new StreamWriter(_path, append: true);
                foreach (var r in records)
                {
                    var key = $"{r.GenerationNumber.Trim()},{r.RecordSequenceNumber.Trim()}";
                    if (existing.Add(key))
                    {
                        writer.WriteLine(string.Join(',', new[]
                        {
                            r.Filename,
                            r.DataSetStatus,
                            r.GenerationNumber,
                            r.RecordSequenceNumber,
                            r.SubmissionDate,
                            r.PaymentInformation,
                            r.RequestedCollectionDate,
                            r.InstructedAmount,
                            r.MandateReference,
                            r.ContractReference,
                            r.RelatedCycleDate,
                            r.FileType
                        }));
                    }
                }
            }
        }
    }
}
