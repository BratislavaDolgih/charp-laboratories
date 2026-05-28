using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;

namespace AgroDbApp.Data;

public static class Pg
{
    private static NpgsqlConnection? _connection;

    private const string ConnectionString =
        "Host=127.0.0.1;Port=5432;Username=postgres;Password=1234567;Database=agro_db";
    private const string Host = "127.0.0.1";
    private const int Port = 5432;
    private const string Database = "agro_db";

    public static NpgsqlConnection GetOpenConnection()
    {
        if (_connection == null)
            _connection = new NpgsqlConnection(ConnectionString);

        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        return _connection;
    }

    public static void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
            _connection.Close();
    }

    public static string Q(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }

    public static string GetConnectionInfo()
    {
        return $"{Database}@{Host}:{Port}";
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
        string sql = $@"SELECT * FROM public.{Q(objectName)};";
        return SelectTable(sql);
    }

    public static List<List<string>> SelectAll(string objectName)
    {
        var result = new List<List<string>>();

        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        string sql = $@"SELECT * FROM public.{Q(objectName)};";

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var headers = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            headers.Add(reader.GetName(i));
        }

        result.Add(headers);

        while (reader.Read())
        {
            var row = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row.Add(reader.IsDBNull(i) ? string.Empty : reader[i]?.ToString() ?? string.Empty);
            }

            result.Add(row);
        }

        return result;
    }

    public static DataTable SelectPage(string objectName, string keyColumn, int pageSize, int pageNumber)
    {
        int offset = pageSize * Math.Max(0, pageNumber - 1);

        string sql = $@"
            SELECT *
            FROM public.{Q(objectName)}
            ORDER BY {Q(keyColumn)}
            LIMIT @limit OFFSET @offset;";

        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", offset);

        using var adapter = new NpgsqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int CountRows(string objectName)
    {
        string sql = $@"SELECT COUNT(*) FROM public.{Q(objectName)};";
        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        object? result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public static int CountRowsWhereLike(string tableName, string columnName, string pattern)
    {
        string sql = $@"
            SELECT COUNT(*)
            FROM public.{Q(tableName)}
            WHERE {Q(columnName)} LIKE @pattern;";

        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        cmd.Parameters.AddWithValue("@pattern", pattern);
        object? result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public static DataRow? GetById(string tableName, string keyColumn, object id)
    {
        string sql = $@"
            SELECT *
            FROM public.{Q(tableName)}
            WHERE {Q(keyColumn)} = @id;";

        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
        cmd.Parameters.AddWithValue("@id", id);

        using var adapter = new NpgsqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);

        return table.Rows.Count > 0 ? table.Rows[0] : null;
    }

    public static int DeleteByIds(string tableName, string keyColumn, IEnumerable<object> ids)
    {
        int total = 0;

        foreach (var id in ids)
        {
            string sql = $@"
                DELETE FROM public.{Q(tableName)}
                WHERE {Q(keyColumn)} = @id;";

            using var cmd = new NpgsqlCommand(sql, GetOpenConnection());
            cmd.Parameters.AddWithValue("@id", id);
            total += cmd.ExecuteNonQuery();
        }

        return total;
    }

    public static int Insert(string tableName, Dictionary<string, object?> values)
    {
        string columns = string.Join(", ", values.Keys.Select(Q));
        string parameters = string.Join(", ", values.Keys.Select(k => "@" + k));

        string sql = $@"
            INSERT INTO public.{Q(tableName)} ({columns})
            VALUES ({parameters});";

        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());

        foreach (var pair in values)
            cmd.Parameters.AddWithValue("@" + pair.Key, pair.Value ?? DBNull.Value);

        return cmd.ExecuteNonQuery();
    }

    public static int Update(string tableName, string keyColumn, object id, Dictionary<string, object?> values)
    {
        string setClause = string.Join(", ", values.Keys.Select(k => $"{Q(k)} = @{k}"));

        string sql = $@"
            UPDATE public.{Q(tableName)}
            SET {setClause}
            WHERE {Q(keyColumn)} = @id;";

        using var cmd = new NpgsqlCommand(sql, GetOpenConnection());

        foreach (var pair in values)
            cmd.Parameters.AddWithValue("@" + pair.Key, pair.Value ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery();
    }
}
