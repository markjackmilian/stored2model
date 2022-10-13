using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace stored2model.core;

public class Worker
{
    private readonly string _connectionString;
    private readonly string _storedName;
    private readonly string _outputFileName;
    private readonly bool _nullableString;

    public Worker(string connectionString, string storedName, string outputFileName, bool nullableString = false)
    {
        this._connectionString = connectionString;
        this._storedName = storedName;
        this._outputFileName = outputFileName;
        this._nullableString = nullableString;
    }

    public void Do()
    {
        var columnsInfo = this.GetColumnCollection();
        this.WriteFile(columnsInfo);
    }

    private void WriteFile(IEnumerable<ColumnDescriptor> columnsInfo)
    {
        var builder = new StringBuilder();
        foreach (var columnDescriptor in columnsInfo)
            builder.AppendLine(columnDescriptor.ToString());

        var template = File.ReadAllText("OutPutTemplate.txt");
        template = template.Replace("{{props}}", builder.ToString());
        
        File.WriteAllText(this._outputFileName,template);
        Console.WriteLine(template);
    }

    private IEnumerable<ColumnDescriptor> GetColumnCollection()
    {
        var connection = new SqlConnection(this._connectionString);
        var sql = $"sp_describe_first_result_set";
        var command = new SqlCommand(sql, connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@tsql",this._storedName);
        connection.Open();
   
        using var dataSet = new DataSet("ColumnsInfo");
        using var dataAdapter = new SqlDataAdapter();
        dataAdapter.SelectCommand = command;
        dataAdapter.Fill(dataSet);
        connection.Close();
        
        foreach (DataRow row in dataSet.Tables[0].Rows)
        {
            yield return new ColumnDescriptor(row, this._nullableString);
        }
    }
}