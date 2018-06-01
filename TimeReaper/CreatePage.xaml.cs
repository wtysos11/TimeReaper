using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TimeReaper.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TimeReaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        public CreatePage()
        {
            this.InitializeComponent();
            timeReaper = TimeReaperManager.getInstance();
        }

        TimeReaperManager timeReaper;

        /*检查输入合法性,未完成*/
        bool checkValid()
        {
            return true;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(checkValid())
            {
                string timeStr = CreateDDLDateInput.Date.ToString();
                timeReaper.AddTodoItem(CreateTitleInput.Text, CreateNoteInput.Text, timeStr);
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTitleInput.Text = "";
            CreateNoteInput.Text = "";
            CreateDDLDateInput.Date = DateTimeOffset.Now;
        }
    }
}
