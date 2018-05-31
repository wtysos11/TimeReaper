﻿using System;
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
using System.Diagnostics;

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
            timer = null;
            timerState = 0;
        }
        //储存结构
        TimeReaperManager timeReaper;

        //计时器相关
        DispatcherTimer timer;
        DateTimeOffset beginTime;
        DateTimeOffset endTime;
        int timerState;//0表示没有工作，1表示正常计时，2表示番茄钟计时

        //番茄钟相关
        int pomotodoWorkInterval = 25;//默认为25分钟
        int pomotodoShortBreak = 5;//默认为5分钟
        int pomotodoLongBreak = 15;//默认为15分钟
        int pomotodoPeriod = 0;//短休息数量，3次短休息后一次长休息，当pomotodoPeriod==3时进行一次长休息
        bool work = false;//是否处于休息状态
        bool isPressButton = false;//用户是否按下按钮（番茄钟能否切换到下一个状态）
        bool needPress = false;//用户是否需要按下按钮（计时器抵达终点，按钮等待点击以提交任务）


        /*正计时函数*/
        private void Timer_Tick(object sender, object e)
        {
            if(!needPress)
            {
                int realHour = DateTimeOffset.Now.Hour - beginTime.Hour;
                int realMinute = DateTimeOffset.Now.Minute - beginTime.Minute;
                int realSecond = DateTimeOffset.Now.Second - beginTime.Second;
                if (realSecond < 0)
                {
                    realSecond += 60;
                    realMinute--;
                }
                if (realHour > 0)
                {
                    realMinute += realHour * 60;
                }

                string strMinute = realMinute.ToString();
                string strSecond = realSecond.ToString();
                if (realSecond < 10)
                {
                    strSecond = "0" + strSecond;
                }
                MainTopStart.Content = strMinute + ":" + strSecond;
            }
        }

        /*番茄计时函数*/
        private void Pomotodo_Tick(object sender, object e )
        {
            int realHour = DateTimeOffset.Now.Hour - beginTime.Hour;
            int realMinute = DateTimeOffset.Now.Minute - beginTime.Minute;
            int realSecond = DateTimeOffset.Now.Second - beginTime.Second;
            if (realSecond < 0)
            {
                realSecond += 60;
                realMinute--;
            }
            if (realHour > 0)
            {
                realMinute += realHour * 60;
            }
            //计算倒计时
            int needMinute;
            if (work)
            {
                needMinute = pomotodoWorkInterval;
            }
            else if (!work && pomotodoPeriod == 3)
            {
                needMinute = pomotodoLongBreak;
            }
            else
            {
                needMinute = pomotodoShortBreak;
            }

            realMinute = needMinute - realMinute - 1;
            realSecond = 60 - realSecond;
            //结束工作
            if (realMinute == -1 && realSecond == 60)//计时器倒计时达到界限
            {
                needPress = true;
                isPressButton = false;
                if(work)
                {
                    MainTopStart.Content = "点击休息";
                }
                else
                {
                    MainTopStart.Content = "点击工作";
                }
            }

            //进入下一步
            if(needPress && isPressButton)
            {
                if (pomotodoPeriod == 4)
                {
                    pomotodoPeriod = 0;
                }
                pomotodoPeriod++;
                if (work)//工作结束，将列表中提交的任务进行结束计时装载
                {
                    foreach (ListItem listitem in timeReaper.AllItems)
                    {
                        timeReaper.AddTaskItem(listitem.getId(), beginTime, DateTimeOffset.Now);
                        listitem.isDoing = false;
                    }
                }

                beginTime = DateTimeOffset.Now;
                work = !work;
                needPress = false;

                //直接刷新下一个状态的realMinute
                if (work)
                {
                    realMinute = pomotodoWorkInterval;
                }
                else if (!work && pomotodoPeriod == 3)
                {
                    realMinute = pomotodoLongBreak;
                }
                else
                {
                    realMinute = pomotodoShortBreak;
                }
                realSecond = 0;
                
            }
            else if(needPress && !isPressButton)
            {
                return;//等待用户按下按钮，计时器暂停
            }

            string strMinute = realMinute.ToString();
            string strSecond = realSecond.ToString();
            if (realSecond < 10)
            {
                strSecond = "0" + strSecond;
            }
            MainTopStart.Content = strMinute + ":" + strSecond;

        }

        //计时器开始计时
        /*
         用DateTimeOffset.Now记录下开始时间
         用DateTimeOffset.Now记录下结束时间
         使用id拿取ListItem，然后生成TaskItem，存进数据库
         */
        private void MainTopStart_Click(object sender, RoutedEventArgs e)
        {
            if(timer==null)
            {
                string timerMode = (MainTopSelect.SelectedValue as ComboBoxItem).Content.ToString();
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                if(timerMode.Equals("正常计时"))
                {
                    timerState = 1;
                    timer.Tick += Timer_Tick;
                    beginTime = DateTimeOffset.Now;
                    needPress = false;
                    timer.Start();
                }
                else if(timerMode.Equals("番茄钟计时"))
                {
                    timerState = 2;
                    timer.Tick += Pomotodo_Tick;
                    beginTime = DateTimeOffset.Now;
                    work = true;
                    needPress = false;
                    timer.Start();
                }
            }
            else
            {
                if (timerState == 2 && needPress)
                {
                    isPressButton = true;
                }
                else if (timerState == 1 && !needPress)
                {
                    needPress = true;
                    isPressButton = false;
                    MainTopStart.Content = "暂停，点击提交";
                }
                else if (timerState == 1 && needPress && !isPressButton)
                {
                    isPressButton = true;
                    needPress = false;
                    foreach (ListItem listitem in timeReaper.AllItems)
                    {
                        timeReaper.AddTaskItem(listitem.getId(), beginTime, DateTimeOffset.Now);
                        listitem.isDoing = false;
                    }
                    MainTopStart.Content = "开始计时";
                    timer.Stop();
                    timer = null;
                    timerState = 0;
                }
            }
        }

        /*取消计时函数
         作用：取消计时器，记录最后时间，(未完成：创建已完成计时任务对象并记录)
             */
        private void MainTopCancel_Click(object sender, RoutedEventArgs e)
        {
            if(timer!=null)
            {
                timer.Stop();
                endTime = DateTimeOffset.Now;
                timer = null;
                MainTopStart.Content = "开始计时";
                timerState = 0;
            }

        }

        /*
         * 使用list<ListItem> 来维护将计时任务中曾工作过的对象
         * 
         主页面左侧列表的点击事件
         如果番茄计时结束或正在进行计时的时候点击列表中的元素：
            1.元素改变颜色
            2.元素加入到删除列表之中
        在番茄工作时间结束或是计时的中断：
            更新并加入数据库中
             */
        private void MainLeftItemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListItem listitem = e.ClickedItem as ListItem;
            if(!listitem.isDoing)//加入删除列表
            { 

            }
            else//移除删除列表
            {

            }
            listitem.isDoing = !listitem.isDoing;
        }


    }
 
}
