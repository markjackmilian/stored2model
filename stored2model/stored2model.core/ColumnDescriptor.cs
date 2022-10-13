using System.Data;

namespace stored2model.core;

class ColumnDescriptor
{
    private readonly DataRow _row;
    private readonly bool _nullableString;

    private readonly Dictionary<Func<string, bool>, string> SqlTypeToClr = new()
    {
        {s => s.StartsWith("varchar"),"string"},
        {s => s.StartsWith("nvarchar"),"string"},
        {s => s == "smallint","int"},
        {s => s == "int","int"},
        {s => s == "bigint","long"},
        {s => s == "smallmoney","decimal"},
        {s => s == "money","decimal"},
        {s => s == "decimal","decimal"},
        {s => s.Contains("decimal"),"decimal"},
        {s => s == "numeric","decimal"},
        {s => s.Contains("real"),"double"},
        {s => s == "tinyint","byte"},
        {s => s == "bit","bool"},
        {s => s == "bit","bool"},
        {s => s == "date","DateTime"},
        {s => s == "datetime","DateTime"},
        {s => s == "datetime2","DateTime"},
        {s => s == "datetimeoffset","DateTime"},
        {s => s == "time","TimeSpan"},
    };

    public ColumnDescriptor(DataRow row, bool nullableString)
    {
        this._row = row;
        this._nullableString = nullableString;
    }

    public bool IsHidden => (bool)this._row[0];
    public int Order => (int)this._row[1];
    public string Name => (string)this._row[2];
    public bool Nullable => (bool)this._row[3];
    public string SystemTypeName => (string)this._row[5];
    public int Precision => (int)this._row[6];

    public override string ToString()
    {
        var systemTypeToClr = this.SystemTypeToClr();
        var writableBool = this.WritableNull(systemTypeToClr == "string");
        return $"   public {systemTypeToClr}{writableBool} {this.Name} {{ get; set; }}";
    }

    private string WritableNull(bool isString)
    {
        if (!this.Nullable) return string.Empty;
        if (isString && !this._nullableString) return string.Empty;

        return "?";
    } 

    private string SystemTypeToClr()
    {
        try
        {
            return this.SqlTypeToClr.First(f => f.Key(this.SystemTypeName)).Value;   
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}