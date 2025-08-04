using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;

namespace CPS.FileStorage
{
    public static class Utils
    {
        public static void SaveDocumentsToDb(string subssn, List<string> pdfPaths, string connectionString)
        {
            if (pdfPaths == null || pdfPaths.Count == 0)
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (var path in pdfPaths)
                {
                    if (!System.IO.File.Exists(path))
                        continue;

                    byte[] data = System.IO.File.ReadAllBytes(path);
                    Guid fileId = Guid.NewGuid();
                    string fileName = Path.GetFileName(path);
                    string fileDesc = Path.GetFileNameWithoutExtension(path);
                    string fileType = Path.GetExtension(path);

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO CPS_FILES (ID, FILENAME, DOCBINARY, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
                                                              VALUES (@ID, @FILENAME, @DOCBINARY, @USER, GETDATE(), @USER, GETDATE())", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", fileId);
                        cmd.Parameters.AddWithValue("@FILENAME", fileName);
                        cmd.Parameters.AddWithValue("@DOCBINARY", data);
                        cmd.Parameters.AddWithValue("@USER", 1);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand linkCmd = new SqlCommand(@"INSERT INTO CPS_FILESLINK (CPSID, PARENTTABLE, FILEID, FILETYPE, FILEDESC, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
                                                                    VALUES (@CPSID, @PARENTTABLE, @FILEID, @FILETYPE, @FILEDESC, @USER, GETDATE(), @USER, GETDATE())", conn))
                    {
                        linkCmd.Parameters.AddWithValue("@CPSID", subssn);
                        linkCmd.Parameters.AddWithValue("@PARENTTABLE", "MEMB_MASTERS");
                        linkCmd.Parameters.AddWithValue("@FILEID", fileId);
                        linkCmd.Parameters.AddWithValue("@FILETYPE", fileType);
                        linkCmd.Parameters.AddWithValue("@FILEDESC", fileDesc);
                        linkCmd.Parameters.AddWithValue("@USER", 1);
                        linkCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the specified file to the CPS_FILES table and returns its identifier.
        /// </summary>
        public static Guid SaveFile(string filePath, string connectionString)
        {
            byte[] data = File.ReadAllBytes(filePath);
            Guid fileId = Guid.NewGuid();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"INSERT INTO CPS_FILES (ID, FILENAME, DOCBINARY, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
                                             VALUES (@ID, @FILENAME, @DOCBINARY, 1, GETDATE(), 1, GETDATE())", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", fileId);
                    cmd.Parameters.AddWithValue("@FILENAME", Path.GetFileName(filePath));
                    cmd.Parameters.AddWithValue("@DOCBINARY", data);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
            return fileId;
        }

        public static List<DocumentFile> RetrieveDocumentsFromDb(string subssn, string connectionString)
        {
            var documents = new List<DocumentFile>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT f.FILENAME, f.DOCBINARY, fl.FILETYPE
                                                          FROM CPS_FILESLINK fl
                                                          JOIN CPS_FILES f ON fl.FILEID = f.ID
                                                          WHERE fl.CPSID = @CPSID", conn))
                {
                    cmd.Parameters.AddWithValue("@CPSID", subssn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            documents.Add(new DocumentFile
                            {
                                FileName = reader.GetString(0),
                                Data = (byte[])reader["DOCBINARY"],
                                FileType = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return documents;
        }
    }
}
