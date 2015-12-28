using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using CustomAndroidCalendar;

namespace Example
{
    [Activity(Label = "Calendar Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            CustomAndroidCalendarView cal = FindViewById<CustomAndroidCalendarView>(Resource.Id.cal);

            //Customize colors
            //cal.backgroundColor = Android.Graphics.Color.LightGray;
            //cal.gridColor = Android.Graphics.Color.Black;
            //cal.cellBackgroundColor = Android.Graphics.Color.Transparent;
            //cal.cellTextColor = Android.Graphics.Color.Black;
            //cal.monthColor = Android.Graphics.Color.DarkGray;
            //cal.markerColor = Android.Graphics.Color.Red;
            //cal.selectionColor = Android.Graphics.Color.Gray;
            //cal.buttonColor = Android.Graphics.Color.Gray;
            //cal.weekdayColor = Android.Graphics.Color.Gray;
            //cal.holidayColor = Android.Graphics.Color.DarkGreen;
            //cal.weekendColor = Android.Graphics.Color.Rgb(222, 222, 222);
            //cal.todayColor = Android.Graphics.Color.Black;


            cal.smallMarker = false;        //If true, use a smaller marker (default = false)
            cal.todayMarker = true;         //If true, indicate the current day (default = false)
            cal.roundedCells = false;       //If true, use cells with rounded corners (default = false)

            //Show a date (default = today)
            cal.setDate(DateTime.Now);

            //Pass dates to mark in a list
            cal.setMarkedDates(new List<DateTime>() { DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2), DateTime.Now.AddDays(7) });

            //Pass holidays in a list
            cal.setHolidays(new List<DateTime>() { DateTime.Now.AddDays(-8), DateTime.Now.AddDays(-6), DateTime.Now.AddDays(6), DateTime.Now.AddDays(8) });

            //Listen for the DateChanged event
            cal.DateChanged += Cal_DateChanged;
        }

        private void Cal_DateChanged(object sender, DateChangedTo e)
        {
            this.Title = e.Date.ToString("dd/MM/yyyy");
        }
    }
}

