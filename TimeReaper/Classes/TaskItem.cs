using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeReaper.Classes
{
    //已经完成计时的部分
    class TaskItem
    {
        DateTimeOffset beginTime;
        DateTimeOffset endTime;
        ListItem item;//ListItem id
        TaskItem(ListItem item,DateTimeOffset begin,DateTimeOffset end)
        {
            beginTime = begin;
            endTime = end;
            this.item = item;
        }
    }
}
