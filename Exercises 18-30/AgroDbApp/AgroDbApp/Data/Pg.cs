using System;
using System.Data;
using Npgsql;

namespace AgroDbApp.Data;

public static class Pg
{
    private static NpgsqlConnection? _connection;

    private static readonly string ConnectionString =
        "Host=127.0.0.1;Port=5432;Username=postgres;Password=1234567;Database=agro_db";

    public static NpgsqlConnection GetOpenConnection()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(ConnectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        return _connection;
    }

    public static DataTable SelectTable(string sql)
    {
        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        using var adapter = new NpgsqlDataAdapter(cmd);

        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static DataTable SelectAllFrom(string objectName)
    {
        // objectName подставляем только из доверенного списка форм, не от пользователя
        var sql = $"SELECT * FROM public.{objectName}";
        return SelectTable(sql);
    }

    public static object? ExecuteScalar(string sql)
    {
        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        return cmd.ExecuteScalar();
    }

    public static void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
        }
    }
}