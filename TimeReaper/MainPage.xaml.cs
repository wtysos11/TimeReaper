using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板
using TimeReaper.Classes;

namespace TimeReaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            timeReaper = TimeReaperManager.getInstance();
        }

        TimeReaperManager timeReaper;
        string timerMode = "正常计时";
        //计时器开始计时
        /*
         用DateTimeOffset.Now记录下开始时间
         用DateTimeOffset.Now记录下结束时间
         使用id拿取ListItem，然后生成TaskItem，存进数据库
         */
    }
}
