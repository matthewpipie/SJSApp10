﻿// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SJSApp10.iOS
{
    [Register ("ScheduleViewController")]
    partial class ScheduleViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SJSApp10.iOS.ScheduleTableView ScheduleTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ScheduleTableView != null) {
                ScheduleTableView.Dispose ();
                ScheduleTableView = null;
            }
        }
    }
}