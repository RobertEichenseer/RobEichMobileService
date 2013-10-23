using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Security.Credentials;

namespace RobEichMobileService
{
    public class TodoItem
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        public string NewContent { get; set; }

        #region 02_03 Push Notification
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }
        #endregion 02_03 Push Notification
    }

    #region 03_01 Existing DB
    public class DownloadItem
    {
        public int id { get; set; }
        public int ExistingSession_Id { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
    #endregion 03_01 Existing DB

    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        #region 01_02 Authentication
        private MobileServiceUser user;
        private async System.Threading.Tasks.Task Authenticate()
        {
            #region 04_01 Cache Logon
            //PasswordCredential passwordCredential = LogonCacher.GetCredential();
            //if (passwordCredential != null)
            //{
            //    App.MobileService.CurrentUser = new MobileServiceUser(passwordCredential.UserName);
            //    App.MobileService.CurrentUser.MobileServiceAuthenticationToken = passwordCredential.Password;
            //    user = App.MobileService.CurrentUser;
            //}
            #endregion 04_01 Cache Logon

            while (user == null)
            {
                string message;
                try
                {
                    user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                    message = string.Format("You are now logged in - {0}", user.UserId);
                }
                catch (Exception exception)
                {
                    message = "You must log in. Login Required";
                }

                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }

            #region 04_01 Cache Logon
            LogonCacher.SaveCredential(App.MobileService.CurrentUser.UserId, App.MobileService.CurrentUser.MobileServiceAuthenticationToken); 
            #endregion 04_01 Cache Logon

        }
        #endregion 01_02 Authentication


        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);

            #region 03_02 Existing DB
            //var downloadItemTable = App.MobileService.GetTable<DownloadItem>();
            //var downloadItems = await downloadItemTable.ToListAsync();

            //DownloadItem newDownloadItem = new DownloadItem()
            //{
            //    Caption = "Slides",
            //    Description = "Mobile Service",
            //    Url = "http://someurl.de"
            //};
            //await downloadItemTable.InsertAsync(newDownloadItem); 

            #endregion 03_02 Existing DB
        }

        private async void RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
            }

            #region 06_01 Existing Database Table
            //var downloadItems = await (App.MobileService.GetTable("DownloadItem").ReadAsync(""));
            #endregion 06_01 Existing Database Table
        }

        private async void UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list 
            await todoTable.UpdateAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TextInput.Text , NewContent = "New Info" };

            #region 02_04 Push Notification
            todoItem.Channel = App.CurrentChannel.Uri; 
            #endregion 02_04 Push Notification

            InsertTodoItem(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            UpdateCheckedTodoItem(item);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region 01_01 Authentication
            await Authenticate();
            #endregion 01_01 Authentication
            RefreshTodoItems();
        }
    }
}
