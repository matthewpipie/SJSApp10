using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Drawing;

namespace SJSApp10.iOS
{
    public partial class ScheduleTableView : UITableView
    {

        static NSString cellID = new NSString("ScheduleCellID");
        public List<string> LeftEntries = new List<string> { };
        public List<string> RightEntries = new List<string> { };

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
                return controller.LeftEntries.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(ScheduleTableView.cellID) as ScheduleTableCell;
                if (cell == null)
                {
                    cell = new ScheduleTableCell();
                }

                int row = indexPath.Row;
                cell.LeftValue.Text = controller.LeftEntries[row];
                cell.RightValue.Text = controller.RightEntries[row];
                return cell;
            }
        }
        class ScheduleTableCell : UITableViewCell
        {
            public UILabel LeftValue { get; set; }
            public UILabel RightValue { get; set; }

            public ScheduleTableCell()
            {
                int len = 170;
                LeftValue = new UILabel(new RectangleF(0, 0, len, 50));
                RightValue = new UILabel(new RectangleF(len, 0, 999, 50));
                LeftValue.BackgroundColor = UIColor.Red;
                RightValue.BackgroundColor = UIColor.Green;

                AddSubview(LeftValue);
                AddSubview(RightValue);
            }
        }
    }
}