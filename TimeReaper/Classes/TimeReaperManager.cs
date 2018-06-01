using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeReaper.Classes
{
    class TimeReaperManager
    {
        private SQLiteConnection conn;
        private ObservableCollection<ListItem> allItems = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> AllItems { get { return this.allItems; } }
        private ObservableCollection<TaskItem> allTasks = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> AllTasks { get { return this.allTasks; } }

        private ListItem selectedItem;
        public ListItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        
        private static TimeReaperManager _instance;
        public static TimeReaperManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new TimeReaperManager();
            }

            return _instance;
        }

        public TimeReaperManager()
        {
            conn = new SQLiteConnection("timeReaper.db");
            using (var statement = conn.Prepare("CREATE TABLE IF NOT EXISTS todolist (id CHAR(36),title VARCHAR(255),notes VARCHAR(255),deadline DATE, PRIMARY KEY (id));"))
            {
                statement.Step();
            }
            using (var statement = conn.Prepare("SELECT * FROM todolist;"))
            {
                while (statement.Step() == SQLiteResult.ROW)
                {
                    ListItem item = new ListItem((string)statement[0], (string)statement[1], (string)statement[2], (string)statement[3]);//id title deadline notes
                    this.allItems.Add(item);
                }
            }
            using (var statement = conn.Prepare("CREATE TABLE IF NOT EXISTS tasklist (id CHAR(36),taskId CHAR(36),beginTime DATETIME,endtime DATETIME, PRIMARY KEY (taskId));"))
            {
                statement.Step();
            }
            using (var statement = conn.Prepare("SELECT * FROM tasklist;"))
            {
                while (statement.Step() == SQLiteResult.ROW)
                {
                    TaskItem task = createDoingTask((string)statement[0], (string)statement[1], (string)statement[2],(string) statement[3]);
                    this.allTasks.Add(task);
                }
            }
        }

        public void AddTodoItem(string title, string notes, string time)
        {

            this.allItems.Add(new ListItem(title, time, notes));
            ListItem item = allItems[allItems.Count - 1];
            using (var statement = conn.Prepare("INSERT INTO todolist VALUES(?,?,?,?);"))
            {
                statement.Bind(1, item.getId());
                statement.Bind(2, item.title);
                statement.Bind(3, item.notes);

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = "yyyy/MM/dd";
                DateTime nowTime = Convert.ToDateTime(item.deadline, dateFormat);
                statement.Bind(4, nowTime.Year.ToString() + "-" + nowTime.Month.ToString() + "-" + nowTime.Day.ToString());
                statement.Step();
            }
        }
        public void AddTaskItem(string id, DateTimeOffset beginTime, DateTimeOffset endTime)
        {
            TaskItem item = createDoingTask(id, beginTime, endTime);
            allTasks.Add(item);
            using (var statement = conn.Prepare("INSERT INTO tasklist VALUES(?,?,?,?);"))
            {
                statement.Bind(1, item.getId());
                statement.Bind(2, item.getTaskId());
                statement.Bind(3, item.getStrTime(beginTime));
                statement.Bind(4, item.getStrTime(endTime));
                
                statement.Step();
            }
        }

        public void RemoveTodoItem(ListItem item)
        {
            using (var statement = conn.Prepare("DELETE FROM todolist WHERE id = ?;"))
            {
                statement.Bind(1, item.getId());
                statement.Step();
            }
            this.allItems.Remove(item);
            this.selectedItem = null;
        }

        public void RemoveTaskitem(TaskItem item)
        {
            using (var statement = conn.Prepare("DELETE FROM tasklist WHERE id = ?;"))
            {
                statement.Bind(1, item.getTaskId());
                statement.Step();
            }
            this.allTasks.Remove(item);
        }

        public void UpdateTodoItem(ListItem item)
        {
            using (var statement = conn.Prepare("UPDATE todolist SET title = ?,notes = ?,deadline = ? WHERE id = ?;"))
            {
                statement.Bind(1, item.title);
                statement.Bind(2, item.notes);

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = "yyyy/MM/dd";
                DateTime nowTime = Convert.ToDateTime(item.deadline, dateFormat);
                statement.Bind(3, nowTime.Year.ToString() + "-" + nowTime.Month.ToString() + "-" + nowTime.Day.ToString());
                statement.Bind(4, item.getId());

                statement.Step();
            }
            
        }

        public void UpdateTaskItem(TaskItem item)
        {
            using (var statement = conn.Prepare("UPDATE tasklist SET id = ?,beginTime = ?,endTime = ? WHERE taskId = ?;"))
            {
                statement.Bind(1, item.getId());
                statement.Bind(2, item.getStrTime(item.beginTime));
                statement.Bind(3, item.getStrTime(item.endTime));
                statement.Bind(4, item.getTaskId());

                statement.Step();
            }

        }
        public ListItem getListItem(string id)
        {
            foreach(ListItem item in allItems)
            {
                if(item.getId().Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public TaskItem createDoingTask(string id, DateTimeOffset beginTime,DateTimeOffset endTime)
        {
            ListItem item = this.getListItem(id);
            return new TaskItem(item, beginTime, endTime);
        }
        public TaskItem createDoingTask(string id, string bTime, string eTime)
        {
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
            dateFormat.ShortDatePattern = "yyyy/MM/dd/hh/mm/ss";
            DateTime begintime = Convert.ToDateTime(bTime, dateFormat);
            DateTime endtime = Convert.ToDateTime(eTime, dateFormat);

            DateTimeOffset beginTime = new DateTimeOffset(begintime);
            DateTimeOffset endTime = new DateTimeOffset(endtime);
            ListItem item = this.getListItem(id);
            return new TaskItem(item, beginTime, endTime);
        }
        public TaskItem createDoingTask(string id,string taskId, string bTime, string eTime)
        {
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
            dateFormat.ShortDatePattern = "yyyy/MM/dd/hh/mm/ss";
            DateTime begintime = Convert.ToDateTime(bTime, dateFormat);
            DateTime endtime = Convert.ToDateTime(eTime, dateFormat);

            DateTimeOffset beginTime = new DateTimeOffset(begintime);
            DateTimeOffset endTime = new DateTimeOffset(endtime);
            ListItem item = this.getListItem(id);
            return new TaskItem(item, beginTime, endTime,taskId);
        }
    }
}
