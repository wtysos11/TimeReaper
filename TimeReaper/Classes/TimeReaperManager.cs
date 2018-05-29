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

        private ListItem selectedItem;
        public ListItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public bool first = false;
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
        }

        public void AddTodoItem(string title, string notes, string time)
        {

            this.allItems.Add(new ListItem(title, notes, time));
            ListItem item = allItems[allItems.Count - 1];
            using (var statement = conn.Prepare("INSERT INTO todolist VALUES(?,?,?,?)"))
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

        public void UpdateTodoItem(string time, string title, string notes, string graph)
        {
            this.selectedItem.title = title;
            this.selectedItem.notes = notes;
            this.selectedItem.SetTime(time);
            using (var statement = conn.Prepare("UPDATE todolist SET title = ?,notes = ?,deadline = ? WHERE id = ?;"))
            {
                statement.Bind(1, title);
                statement.Bind(2, notes);

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = "yyyy/MM/dd";
                DateTime nowTime = Convert.ToDateTime(this.selectedItem.deadline, dateFormat);
                statement.Bind(3, nowTime.Year.ToString() + "-" + nowTime.Month.ToString() + "-" + nowTime.Day.ToString());
                statement.Bind(4, graph);
                statement.Bind(5, this.selectedItem.getId());

                statement.Step();
            }

            this.selectedItem = null;
        }
        ListItem getListItem(string id)
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

    }
}
