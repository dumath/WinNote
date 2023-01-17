using System.Linq;
using System.Data.SqlClient;
using System.Collections.ObjectModel;


namespace WinNote
{
    internal sealed class ViewModel
    {
        // TODO: Спрятать строку подключения.
        // DS создан конструктором.

        #region Fields.
        // Коллекция под XAML. PropertyChanged+, CollectionChanged+.
        private ObservableCollection<Record> _rows = new ObservableCollection<Record>(); //TODO: DataSet напрямую?
        private SqlConnectionStringBuilder _connectionString = null; // Собираем строку.
        private DS myBase; // Абстракция бд. Типизированная. Промежуточное.
        private DSTableAdapters.RecordsTableAdapter adapter; // Типизированный адаптер.
        #endregion

        #region Properties.
        public ObservableCollection<Record> Rows { get => this._rows; }
        public SqlConnectionStringBuilder ConnectionString { get => _connectionString; } // Использует Connection Grid.
        public string DebugString { get; set; } // Отладочная строка.
        public DS MyBase { get => myBase; } // Временно убрано.
        #endregion

        #region Const.
        // Основные команды T_SQL. Для дебага. Через SqlCommand - объект.
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

        #region CTORS.
        public ViewModel()
        {
            // Убираем nullExeption.
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
            // После завершения - финализируем.
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                // Ставим строку.
                sqlConnection.ConnectionString = @"Строка убрана.";
                // Направляем адаптер.
                adapter.Connection = sqlConnection;
                // Заполняем DataSet.
                adapter.Fill(myBase.Records); 
                // Заполняем разметку.
                foreach(DS.RecordsRow v in myBase.Records.Rows)
                    this._rows.Add(new Record(v));
            }
        }

        /// <summary>
        /// Метод добавления новой строки в БД.
        /// </summary>
        /// <param name="record"> Значение, указанное пользователем. </param>
        public void Add(string record)
        {
            // После завершения - финализируем.
            using (SqlConnection connection = new SqlConnection())
            {
                // Ставим строку.
                connection.ConnectionString = @"Строка убрана";
                // Направляем адаптер.
                adapter.Connection = connection;
                // Создаем объект типизированной строки.
                DS.RecordsRow newRow = myBase.Records.NewRecordsRow();
                // Инициализируем столбец строкой пользователя.
                newRow.Record = record;
                // Добавляем в DSet.
                myBase.Records.AddRecordsRow(newRow);
                // Синхронизируем с БД.
                int i = adapter.Update(myBase); //TODO: Сделать проверку. Можно myBase.Records
                // Обновляем представление.
                this._rows.Add(new Record(newRow));
                // Меняем, возможно. State's. На всякий.
                myBase.Records.AcceptChanges();
            }
        }

        /// <summary>
        /// Метод изменения строки в базе данных.
        /// </summary>
        /// <param name="record"> Новое значение столбца "Record". </param>
        /// <param name="row"> SelectedRecord, с обработчика. </param>
        public void Edit(string record, Record row)
        {
            // После завершения - финализируем.
            using (SqlConnection connection = new SqlConnection())
            {
                // Ставим строку.
                connection.ConnectionString = @"Строка убрана";
                // Направляем адаптер.
                adapter.Connection = connection;
                // Ищем строку по ID и меняем значение столбца.
                myBase.Records.FindByRecordID(row.RecordID).Record = record;
                // Синхронизируем.
                int i = adapter.Update(myBase.Records); //TODO: Поставить проверку. А тут только таблицу?
                // Ищем строку в коллекции представления. Обновляем значение.
                this._rows.First(x => x.RecordID == row.RecordID).Data = record;
                // Меняем, возможно. State's. На всякий.
                myBase.Records.AcceptChanges();
            }
        }

        /// <summary>
        /// Метод удаления строки из базы данных.
        /// </summary>
        /// <param name="record"> SelectedRecord с обработчика. </param>
        public void Delete(Record row)
        {
            // После завершения - финализируем.
            using (SqlConnection connection = new SqlConnection())
            {
                // Ставим строку.
                connection.ConnectionString = @"Строка убрана.";
                // Направляем адаптер.
                adapter.Connection = connection;
                // Ищем строку в DataSet по ID, ставим метку.
                myBase.Records.FindByRecordID(row.RecordID).Delete();
                // Синхронизируем. 
                int i = adapter.Update(myBase.Records); //TODO: Сделать проверку.
                // Удаляем в представлении.
                this._rows.Remove(row);
                // Меняем, возможно.  State's. На всякий.
                myBase.Records.AcceptChanges();
            }
        }
        #endregion
    }
}
