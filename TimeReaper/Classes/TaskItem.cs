using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeReaper.Classes
{
    //已经完成计时的部分
    class TaskItem
    {
        public DateTimeOffset beginTime;
        public DateTimeOffset endTime;
        public ListItem item;//ListItem id
        public string title;
        string taskId;
        public TaskItem(ListItem item,DateTimeOffset begin,DateTimeOffset end)
        {
            beginTime = begin;
            endTime = end;
            this.item = item;
            this.title = item.title;
            this.taskId = Guid.NewGuid().ToString();
        }
        public TaskItem(ListItem item, DateTimeOffset begin, DateTimeOffset end,string taskId)
        {
            beginTime = begin;
            endTime = end;
            this.item = item;
            this.title = item.title;
            this.taskId = Guid.NewGuid().ToString();
        }

        public string getId()
        {
            return item.getId();
        }
        string zeroExtend(int i)
        {
            string t = i.ToString();
            if(i<10)
            {
                t = "0" + t;
            }
            return t;
        }

        public string getStrTime(DateTimeOffset date)
        {
            string ans;
            string year = date.Year.ToString();
            if(date.Year < 10)
            {
                year = "000" + year;
            }
            else if(date.Year < 100)
            {
                year = "00" + year;
            }
            else if(date.Year < 1000)
            {
                year = "0" + year;
            }

            string month = zeroExtend(date.Month);
            string day = zeroExtend(date.Day);
            string hour = zeroExtend(date.Hour);
            string minute = zeroExtend(date.Minute);
            string second = zeroExtend(date.Second);

            ans = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
            return ans;
        }

        public string getTaskId()
        {
            return taskId;
        }
        
    }
}
