using System.Collections.Generic;
using System;
using IFoundBackend.Model.Abstracts;
using IFoundBackend.Model;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.IO;

namespace IFoundBackend.Data
{
    public class SqlAccess
    {
        private string _connectionString;
        public SqlAccess(string connectionString) {
            _connectionString = connectionString;
        }


        //public SqlCommand PrepareCreateImageCommand(Image image)
        //{

        //    return createImageCommand;
        //}

        //public SqlCommand PrepareCreateTargetPersonCommand(Image image)
        //{
        //    SqlCommand createImageCommand = new SqlCommand("insert into Images output INSERTED.imageID values('@ImageFile')");
        //    var sqlParam = createImageCommand.Parameters.AddWithValue("@ImageFile", image.ImageFile);
        //    sqlParam.SqlDbType = SqlDbType.VarBinary;

        //    return createImageCommand;
        //}

        public static byte[] GetPhoto(string filePath)
        {
            FileStream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] photo = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return photo;
        }

        public void CreatePost(Image image, Target target,string sqlQuery)
        {
 
            using (SqlConnection objConn = new SqlConnection(_connectionString))
            {
                objConn.Open();
                using (SqlTransaction trans = objConn.BeginTransaction())
                {
                    //SqlCommand createImageCommand = PrepareCreateImageCommand(image);
                    SqlCommand createImageCommand = new SqlCommand("insert into Images values('@ImageFile');SELECT CAST(scope_identity() AS int)");
                    //var sqlParam = createImageCommand.Parameters.AddWithValue("@ImageFile", image.ImageFile);
                    //createImageCommand.Parameters.Add("@ImageFile", SqlDbType.VarBinary, -1).Value = image.ImageFile;
                    createImageCommand.Connection = objConn;
                    createImageCommand.Transaction = trans;
                    createImageCommand.Parameters.Add("@ImageFile", SqlDbType.Image, image.ImageFile.Length).Value = image.ImageFile;
                    
                    try
                    {
                        int x = (int)createImageCommand.ExecuteScalar();
                        //Console.WriteLine(x);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        objConn.Close();
                    }

                }
            }
           
                 

        }
    }
}
