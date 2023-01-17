using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace WinNote
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields.
        private ViewModel vm; // Модель представление.
        #endregion

        #region CTORS.
        /// <summary>
        /// Пассивный.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(); // Создаем М.-П..
            dataBook.ItemsSource = vm.Rows; // Биндим коллецию.
            appIntent.ItemsSource = new List<System.Data.SqlClient.ApplicationIntent>() // Ставим варианты намерений/разрешений на ComboBox.
            { 
                System.Data.SqlClient.ApplicationIntent.ReadOnly,
                System.Data.SqlClient.ApplicationIntent.ReadWrite
            };
        }
        #endregion

        #region Connection.
        /// <summary>
        /// Стока подлкючения. !Отключено. Для дебага, до масштабированной версии.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Значения события. </param>
        private void On_Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Normal
                //#region Сервер.
                //vm.ConnectionString.DataSource =
                //    serverName.Text != string.Empty ? serverName.Text : throw new ArgumentException(Messages.SERVER_NAME_ERROR);
                //#endregion

                //#region База данных.
                //vm.ConnectionString.InitialCatalog =
                //    dbName.Text != string.Empty ? dbName.Text : throw new ArgumentException(Messages.DB_NAME_ERROR);
                //#endregion

                //#region Время подключения.
                //vm.ConnectionString.ConnectTimeout = 
                //    connectionTimeout.Text != string.Empty ? int.Parse(connectionTimeout.Text) : throw new ArgumentException(Messages.TIME_OUT_ERROR);
                //#endregion

                //#region Авторизация.
                //vm.ConnectionString.IntegratedSecurity = securityCB.IsChecked.Value;
                //#endregion

                //#region Шифрование данных.
                //vm.ConnectionString.Encrypt = encryptCB.IsChecked.Value;
                //#endregion

                //#region Сертификат сервера.
                //vm.ConnectionString.TrustServerCertificate = trustCerCB.IsChecked.Value;
                //#endregion

                //#region Намерение приложения.
                //vm.ConnectionString.ApplicationIntent = (System.Data.SqlClient.ApplicationIntent)appIntent.SelectedValue;
                //#endregion

                //#region Поддержка отказоустойчисвости.
                //vm.ConnectionString.MultiSubnetFailover = multiSub.IsChecked.Value;
                //#endregion
                #endregion

                vm.Connect(); // Соединяемся с БД.

                #region Окна приложения.
                // Меняем сетки, со строки подключения на данные.
                connectionGrid.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
                #endregion
            }
            catch (FormatException fEx)
            {
                // Отладочное.
                MessageDialog msg = new MessageDialog(Messages.INPUT_STRING);
                msg.ShowAsync();
            }
            catch (Exception ex)
            {
                // Отладочное. Общий.
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
            }
            finally { } //  Можно убрать. Соединение в using.Dispose().
        }
        #endregion

        #region Add, Edit and Delete methods.
        // Исключения из vm - распространяем в обработчик. Обработка тут.
        // using.Dispose на подключениях в vm.

        /// <summary>
        /// Обработчик события. Добавляет запись в базу данных.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Значения события. </param>
        private async void add_Record(object sender, RoutedEventArgs e)
        {
            // Создаем окно для пользователя.
            AddDataDialog dataDialog = new AddDataDialog(); 
            try
            {
                var result = await dataDialog.ShowAsync(); // Дожидаемся результата ввода. TODO: Task - опустить?
                if (result == ContentDialogResult.Primary)
                {
                    if (!string.IsNullOrEmpty(dataDialog.Data)) // Возможно, стоит убрать. Пустая строка - тоже строка.
                        vm.Add(dataDialog.Data);
                    //TODO: Повесить уведомление, что строка отклонена - пустая. Добавить в класс Messages.
                }
            }
            catch (Exception ex)
            {
                // Общий , для дебага.
                MessageDialog message = new MessageDialog(ex.Message);
                await message.ShowAsync();
            }
            finally { } // using.Dispose() в vm.
        }

        /// <summary>
        /// Обработчик события. Изменяет запись в азе данных.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Значения события. </param>
        private async void edit_Record(object sender, RoutedEventArgs e)
        {
            try
            {
                EditDataDialog editDialog = new EditDataDialog();
                var result = await editDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (!string.IsNullOrEmpty(editDialog.Data)) // Пустая строка? Принять/Отклонить.
                        vm.Edit(editDialog.Data, dataBook.SelectedItem as Record);
                    // Уведомить, пустая строка не прошла. Добавить в Messages.
                }
            }
            catch(Exception ex)
            {
                // Общий.Для дебага.
                MessageDialog message = new MessageDialog(ex.Message);
                message.ShowAsync();
            }
            finally { }
        }

        /// <summary>
        /// Обработчик события. Удаляет запись из базы данных.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Значения события. </param>
        private async void delete_Record(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteDataDialog deleteDialog = new DeleteDataDialog();
                //TODO: Поставить безымянную заглушку на CloseButtonClickClick.
                var result = await deleteDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // Приводим к типу Record из obj.
                    Record row = dataBook.SelectedItem as Record;
                    if (row != null)
                        vm.Delete(row);
                    //TODO: проверить выключение кнопок в AppBar. Не выключаются.
                }
            }
            catch(Exception ex)
            {
                // Общий. Для дебага.
                MessageDialog message = new MessageDialog(ex.Message);
                await message.ShowAsync();
            }
            finally { } // using.Dispose() В vm.
        }
        #endregion

        #region ListView eventHandlers.
        // TODO: Перенести в Partial.
        // Обработчики событий List View. Включаем/отключаем кнопки appBara.
        /// <summary>
        /// Обработчик события.
        /// Происходит, когда пользователь щелкает на строке.
        /// Включает кнопку "edit_Bar_Button".
        /// </summary>
        /// <param name="s"> Контрол - отправитель. </param>
        /// <param name="arg"> Аргументы события. </param>
        private void show_Edit_Delete_Button(object sender, ItemClickEventArgs arg)
        {
            //Включаем кнопку "edit_Bar_Button", при получении фокуса.
            edit_Bar_Button.IsEnabled = true;
            delete_Bar_Button.IsEnabled = true;
        }

        /// <summary>
        /// Обработчик события.
        /// Происходит, когда пользователь нажимает ПКМ в области ListView, либо на Item'e.
        /// Снимает выделение с Item и выключает кнопку "edit_Bar_Button".
        /// </summary>
        /// <param name="sender"> Контрол - отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void hide_Edit_Delete_Button(object sender, RightTappedRoutedEventArgs e)
        {
            //TODO: Весит на ListView - предположительно, лучше бы повесить на крайний Grid.
            dataBook.SelectedItem = null;
            //Выключаем кнопку, поскольку Item не выбран.
            edit_Bar_Button.IsEnabled = false;
            delete_Bar_Button.IsEnabled = false;
        }
        #endregion

        #region ChangingGrid.
        private void On_Connection_Grid(object sender, RoutedEventArgs e)
        {
            MessageDialog confirm = new MessageDialog("Вы действительно хотите выйти?", "Подтверждение");
            confirm.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
            confirm.Commands.Add(new UICommand("Да", new UICommandInvokedHandler(this.confirmed_Exit)));
            confirm.DefaultCommandIndex = 0U;
            confirm.Commands.Add(new UICommand("Нет"));
            confirm.CancelCommandIndex = 1U;
            confirm.ShowAsync();
        }

        private void confirmed_Exit(IUICommand command)
        {
            dataGrid.Visibility = Visibility.Collapsed;
            connectionGrid.Visibility = Visibility.Visible;
            vm = new ViewModel();
        }
        #endregion
    }
}
