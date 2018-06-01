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

        //负责处理更改与创建之间的关系
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(timeReaper.SelectedItem!=null)
            {
                CreateTopTitle.Text = "更改任务信息";
                CreateButton.Content = "Update";
                CreateTitleInput.Text = timeReaper.SelectedItem.title;
                CreateNoteInput.Text = timeReaper.SelectedItem.notes;
                CreateDDLDateInput.Date = timeReaper.SelectedItem.deadline;
                CreateDDLTimeInput.Time = new TimeSpan(timeReaper.SelectedItem.deadline.Hour, timeReaper.SelectedItem.deadline.Minute, 0);
            }
        }

        /*检查输入合法性,未完成*/
        bool checkValid()
        {
            return true;
        }

        //补零
        string FixedData(string current,int x)
        {
            if (x < 10)
                return "0" + current;
            else
                return current;
        }

        string getTimeStr(DateTimeOffset date,TimeSpan time)
        {
            string year = FixedData(date.Year.ToString(), date.Year);
            string month = FixedData(date.Month.ToString(), date.Month);
            string day = FixedData(date.Day.ToString(), date.Day);
            string hour = FixedData(time.Hours.ToString(), time.Hours);
            string minute = FixedData(time.Minutes.ToString(), time.Minutes);

            return year + "-" + month + "-" + day + " " + hour + ":" + minute + ":00";
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(checkValid())
            {
                string timeStr = getTimeStr(CreateDDLDateInput.Date, CreateDDLTimeInput.Time);
                if (timeReaper.SelectedItem==null)//创建
                {
                    timeReaper.AddTodoItem(CreateTitleInput.Text, CreateNoteInput.Text, timeStr);
                    
                }
                else//更改
                {
                    ListItem item = timeReaper.SelectedItem;
                    item.title = CreateTitleInput.Text;
                    item.notes = CreateNoteInput.Text;
                    item.SetTime(getTimeStr(CreateDDLDateInput.Date,CreateDDLTimeInput.Time));
                    timeReaper.UpdateTodoItem(item);
                    timeReaper.SelectedItem = null;
                }
                Frame.Navigate(typeof(MainPage));

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTitleInput.Text = "";
            CreateNoteInput.Text = "";
            CreateDDLDateInput.Date = DateTimeOffset.Now;
            CreateDDLTimeInput.Time = new TimeSpan(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);
        }
    }
}
