using System.Linq;
using System.Data.SqlClient;
using System.Collections.ObjectModel;


namespace WinNote
{
    internal sealed class ViewModel
    {
        public string DebugString { get; set; }

        private ObservableCollection<Record> _rows = new ObservableCollection<Record>();
        public ObservableCollection<Record> Rows { get => this._rows; }

        #region Const.
        // Основные команды T_SQL и сообщения ошибок.
        public const string CREATE = "CREATE"; // Создать.
        public const string INSERT = "INSERT INTO"; // Вставить.
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE FROM";
        public const string VALUES = "VALUES";
        public const string INTO = "INTO";
        public const string SET = "SET";
        public const string FROM = "FROM"; // Из.
        public const string SELECT = "SELECT"; // Выборка.
        public const string WHERE = "WHERE"; // Условие.
        public const string ORDER_BY = "ORDER BY"; // Сортировка.
        public const string TABLE = "TABLE"; // Таблица.
        #endregion

        #region Connection, Adaptation.
        private SqlConnectionStringBuilder _connectionString = null;
        public SqlConnectionStringBuilder ConnectionString { get => _connectionString; }

        private DS myBase;
        public DS MyBase { get => myBase; }
        private DSTableAdapters.RecordsTableAdapter adapter;
        #endregion

        #region CTORS.
        public ViewModel()
        {
            this._connectionString = new SqlConnectionStringBuilder();
            myBase = new DS();
            this.adapter = new DSTableAdapters.RecordsTableAdapter();
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Метод первого подключения и заполнения. DEBUG.
        /// </summary>
        public void Connect()
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = @"Data Source=DESKTOP-HOESTCP\SQLEXPRESS;Initial Catalog=NotesDB;Integrated Security=True;";
                adapter.Connection = sqlConnection;
                adapter.Fill(myBase.Records);
                foreach(DS.RecordsRow v in myBase.Records.Rows)
                    this._rows.Add(new Record(v));
            }
        }
        public void Add(string record)
        {
            using(SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=DESKTOP-HOESTCP\SQLEXPRESS;Initial Catalog=NotesDB;Integrated Security=True;";
                adapter.Connection = connection;
                DS.RecordsRow newRow = myBase.Records.NewRecordsRow();
                newRow.Record = record;
                myBase.Records.AddRecordsRow(newRow);
                int i = adapter.Update(myBase); //TODO: Сделать проверку.
                this._rows.Add(new Record(newRow));
                myBase.Records.AcceptChanges();
            }
        }

        /// <summary>
        /// Метод изменения строки в базе данных.
        /// </summary>
        /// <param name="record"> Новое значение столбца "Record". </param>
        public void Edit(string record, Record row)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=DESKTOP-HOESTCP\SQLEXPRESS;Initial Catalog=NotesDB;Integrated Security=True;";
                adapter.Connection = connection;
                myBase.Records.FindByRecordID(row.RecordID).Record = record;
                int i = adapter.Update(myBase.Records);
                this._rows.First(x => x.RecordID == row.RecordID).Data = record;
                myBase.Records.AcceptChanges();
            }
        }

        /// <summary>
        /// Метод удаления строки из базы данных.
        /// </summary>
        /// <param name="record"></param>
        public void Delete(Record row)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=DESKTOP-HOESTCP\SQLEXPRESS;Initial Catalog=NotesDB;Integrated Security=True;";
                adapter.Connection = connection;
                myBase.Records.FindByRecordID(row.RecordID).Delete();
                int i = adapter.Update(myBase.Records); //TODO: Сделать проверку.
                this._rows.Remove(row);
                myBase.Records.AcceptChanges();
            }
        }
        #endregion
    }
}
