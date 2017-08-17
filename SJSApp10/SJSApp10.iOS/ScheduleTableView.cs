using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace SJSApp10.iOS
{
    public partial class ScheduleTableView : UITableView
    {

        static NSString cellID = new NSString("ScheduleCellID");
        public List<string> entries = new List<string> { };

        public ScheduleTableView(IntPtr handle) : base (handle)
        {
            this.RegisterClassForCellReuse(typeof(UITableViewCell), cellID);
            this.Source = new ScheduleTableDataSource(this);
        }
        /*public void addEntry(string entry)
        {
                this.entries.Add("d");
                this.ReloadData();
        }*/

        class ScheduleTableDataSource : UITableViewSource
        {
            ScheduleTableView controller;

            public ScheduleTableDataSource(ScheduleTableView controller)
            {
                this.controller = controller;
            }

            // Returns the number of rows in each section of the table
            public override nint RowsInSection(UITableView tableView, nint section)
            {
                return controller.entries.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(ScheduleTableView.cellID);

                int row = indexPath.Row;
                cell.TextLabel.Text = controller.entries[row];
                return cell;
            }
        }
    }
}