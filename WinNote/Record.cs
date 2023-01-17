using Windows.UI.Xaml.Controls;

namespace WinNote
{
    /// <summary>
    /// Модель - упрощение. Для разметки.
    /// </summary>
    internal sealed class Record : StackPanel
    {
        #region Fields.
        private DS.RecordsRow data; // Данные с БД.
        #endregion

        #region Properties.
        /// <summary>
        /// Свойство номера строки. PKey.
        /// Readonly.
        /// IDENTITY прописан в БД.
        /// </summary>
        public int RecordID { get => data.RecordID; }

        /// <summary>
        /// Свойство записи.
        /// Используется для изменения данных.
        /// </summary>
        public string Data
        {
            get => data.Record;
            set
            {
                data.Record = value;
                // Обновляем значение в разметке, после изменения.
                (this.Children[1] as ListBoxItem).Content = value; //TODO: Масштабировать.
            }
        }
        #endregion

        #region CTORS.
        /// <summary>
        /// Активный конструктор. Для инициализации данных с бд.
        /// Пакует в итемы значения.
        /// </summary>
        /// <param name="row"> Строка с БД. !Типизированная. </param>
        public Record(DS.RecordsRow row)
        {
            this.data = row; //TODO: !Ссылка.
            // Ставим ориентацию строки.
            Orientation = Orientation.Horizontal;
            // Отключаем обработчик стака, чтобы не блокировал выделения наложенных ивентов.
            IsHitTestVisible = false; 
            // Пакуем.
            foreach (var d in data.ItemArray)
                Children.Add(new ListBoxItem { Content = d.ToString() });
        }
        #endregion
    }
}
