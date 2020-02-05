using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Loader
{
    public class Program
    {
        //string StringaDiConnessione = ConfigurationManager.ConnectionStrings["StringaDiConnessione"].ConnectionString;
        private string StringaDiConnessione = @"Data Source= AULA2-PC10\SQLEXPRESS;Initial Catalog =eLearning; User ID = sa; Password=Pa$$w0rd1!";
        

        #region FILE TO BINARY
        public bool databaseFilePut(string varFilePath, string nome)
        {
            //const string StringaDiConnessione = "Data Source=aula2-pc10;Initial Catalog = TestPerVarBinary; User ID = sa; Password=Pa$$w0rd1!";
            //var StringaDiConnessione = ConfigurationManager.ConnectionStrings["StringaDiConnessione"].ConnectionString;
            bool esito = false;

            //dichiara un array di byte
            byte[] file;

            //istanzia il "convertitore"
            using (var stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    //legge il file, e lo converte
                    file = reader.ReadBytes((int)stream.Length);
                }

                //invia il file al DB
                using (SqlConnection connection = new SqlConnection(StringaDiConnessione))
                {
                    using (var sqlWrite = new SqlCommand("INSERT INTO Materiali_Didattici (Materiale, Tipo, Id_Lezione_FK) Values(@File,@Nome,@Lezione)", connection))
                    {
                        connection.Open();
                        sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                        sqlWrite.Parameters.Add("@Nome", SqlDbType.VarChar, nome.Length).Value = nome;
                        sqlWrite.Parameters.Add("@Lezione", SqlDbType.Int, nome.Length).Value = 1;
                        esito = sqlWrite.ExecuteNonQuery() > 0;
                        connection.Close();
                        return esito;
                    }
                }

            }
        }
        #endregion

        #region BINARY TO FILE
        public bool databaseFileRead(string Nome, string varPathToNewLocation)
        {
            //const string StringaDiConnessione = "Data Source=aula2-pc10;Initial Catalog = TestPerVarBinary; User ID = sa; Password=Pa$$w0rd1!";
            bool esito = false;

            using (SqlConnection connection = new SqlConnection(StringaDiConnessione))
            using (var sqlQuery = new SqlCommand(@"SELECT [Materiale] FROM [eLearning].[dbo].[Materiali_Didattici] WHERE [Tipo] = @Nome", connection))
            {
                connection.Open();
                sqlQuery.Parameters.AddWithValue("@Nome", Nome);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                {
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        try
                        {
                            //new FileStream()
                            using (var fs = new FileStream(varPathToNewLocation, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(blob, 0, blob.Length);
                            }

                            esito = true;
                        }
                        catch (Exception)
                        {
                            esito = false;
                        }

                    }
                }

                connection.Close();
            }
            return esito;
        }
        #endregion

        #region BINARY TO WEB

        public byte[] getFileDB(int Id)
        {
            //var StringaDiConnessione = ConfigurationManager.ConnectionStrings["StringaDiConnessione"].ConnectionString;

            bool esito = false;
            byte[] file = null;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = StringaDiConnessione;
                using (var sqlQuery = new SqlCommand(@"SELECT [Materiale] FROM [eLearning].[dbo].[Materiali_Didattici] WHERE [Id_Materiale_PK] = @Id", connection)) //Adonet
                {
                    connection.Open();
                    sqlQuery.Parameters.AddWithValue("@Id", Id);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    {
                        if (sqlQueryResult != null)
                        {
                            try
                            {
                                sqlQueryResult.Read();
                                var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                                sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                                file = blob;
                                esito = true;

                                return file;
                            }
                            catch (Exception)
                            {
                                esito = false;
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                        return file;
                    }
                }
            }
        }

        #endregion

          #region TESTS

        #region FILE TO BYTE
        public bool TESTBYTEPNG()
        {
            string percorso = @"" /*inserisci il percorso del file*/;
            string nome = "PNG4";
            bool risultato = databaseFilePut(percorso, nome);
            return risultato;
        }
        public bool TESTBYTEPDF()
        {
            string percorso = @"" /*inserisci il percorso del file*/;
            string nome = "PDF2";
            bool risultato = databaseFilePut(percorso, nome);
            return risultato;
        }
        public bool TESTBYTEDOCX()
        {
            string percorso = @"" /*inserisci il percorso del file*/;
            string nome = "DOCX2";
            bool risultato = databaseFilePut(percorso, nome);
            return risultato;
        }
        #endregion

        #region BYTE TO FILE
        public void TESTFILEPNG()
        {
            string newpath = Path.Combine(@"" /*inserisci il percorso dove salvare il file*/, "Nome.est");
            databaseFileRead("PNG4", newpath);

        }
        public void TESTFILEPDF()
        {
            string newpath = Path.Combine(@"" /*inserisci il percorso dove salvare il file*/, "Nome.est");
            databaseFileRead("PDF1", newpath);
        }
        public void TESTFILEDOCX()
        {
            string newpath = Path.Combine(@"" /*inserisci il percorso dove salvare il file*/, "Nome.est");
            databaseFileRead("DOCX1", newpath);
        }
        #endregion

        #endregion

        #region MAIN
        public static void Main(string[] args)
        {
            /*
            * databaseFilePut(@"C:\Lavori in Classe\MaterialeDBLoad\Loader\Documento_PDF\Analisi E-Learning.docx", "DOCX2");
            //bool successo = TESTBYTEDOCX();
            TESTFILEPNG();
            TESTFILEPDF();
            TESTFILEDOCX();
            */
            //var varavar = getFileDB(2002);
            Program blu = new Program();
            blu.TESTBYTEPNG();
            blu.TESTBYTEDOCX();
            blu.TESTBYTEPDF();

        }

        #endregion
    }
}
