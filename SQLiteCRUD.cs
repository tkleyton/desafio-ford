using System;
using System.Collections.Generic;
using System.Data.SQLite;
using desafioford;

public class SQLiteCRUD
{
    public static SQLiteConnection CreateConnection()
    {
        SQLiteConnection sqliteConnection = new SQLiteConnection("Data Source= Database.db; Version = 3; New = True; Compress = True; ");
        try
        {
            sqliteConnection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return sqliteConnection;
    }

    public static void CreateTable(SQLiteConnection sqliteConnection)
    {
        try
        {
            SQLiteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "CREATE TABLE Frota ( id_carro INTEGER, placa TEXT NOT NULL UNIQUE, cor TEXT, data TEXT, PRIMARY KEY(id_carro AUTOINCREMENT) )";
            sqliteCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static void InsertData(SQLiteConnection sqliteConnection, Carro carro)
    {
        SQLiteCommand sqliteCommand;
        sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "INSERT INTO Frota(placa, cor, data) VALUES($placa,$cor,$data);";
        sqliteCommand.Parameters.AddWithValue("$placa", carro.Placa);
        sqliteCommand.Parameters.AddWithValue("$cor", carro.Cor);
        sqliteCommand.Parameters.AddWithValue("$data", DateTime.Now.ToShortDateString());
        sqliteCommand.ExecuteNonQuery();
        sqliteConnection.Close();
    }

    public static void DeleteData(SQLiteConnection sqliteConnection, Carro carro)
    {
        SQLiteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "DELETE FROM Frota WHERE id_carro=$id_carro AND placa=$placa";
        sqliteCommand.Parameters.AddWithValue("$id_carro", carro.id_carro);
        sqliteCommand.Parameters.AddWithValue("$placa", carro.Placa);
        sqliteCommand.ExecuteNonQuery();
        sqliteConnection.Close();
    }

    public static List<Carro> ReadData(SQLiteConnection sqliteConnection, string selectStatement="SELECT * FROM Frota")
    {
        SQLiteDataReader sqliteDatareader;
        SQLiteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = selectStatement;

        try
        {
            sqliteDatareader = sqliteCommand.ExecuteReader();
        }
        catch
        {
            CreateTable(sqliteConnection);
            sqliteDatareader = sqliteCommand.ExecuteReader();
        }

        var listCarros = new List<Carro>();
        while (sqliteDatareader.Read())
        {
            int id_carro = sqliteDatareader.GetInt32(0);
            string placa = sqliteDatareader.GetString(1);
            string cor = sqliteDatareader.GetString(2);
            string data = sqliteDatareader.GetString(3);

            Carro carro = new Carro(placa, cor);
            carro.id_carro = id_carro;
            carro.Data = data;
            
            listCarros.Add(carro);
            //Console.WriteLine($"{id_carro}\t{placa}\t{cor}\t{data}");
        }
        sqliteConnection.Close();
        return listCarros;
    }    
}