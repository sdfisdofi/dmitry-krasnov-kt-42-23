namespace dmitry_krasnov_kt_42_23.Database.Helpers
{
    // Типы колонок для SQL Server
    public class ColumnType
    {
        public const string Date = "datetime2";
        public const string Guid = "uniqueidentifier";
        public const string String = "nvarchar";      // nvarchar - поддерживает кириллицу
        public const string Text = "nvarchar(max)";
        public const string Bool = "bit";
        public const string Int = "int";
        public const string Long = "bigint";
        public const string Decimal = "money";
        public const string Double = "decimal(9,2)";
    }
}
