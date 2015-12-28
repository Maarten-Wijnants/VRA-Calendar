using System;
using System.Collections.Generic;
using System.Globalization;

using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Util;

namespace CustomAndroidCalendar
{
    /// <summary>
    /// Date changed event arguments
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DateChangedTo : EventArgs
    {
        private DateTime newDate;

        public DateTime Date
        {
            set
            {
                newDate = value;
            }
            get
            {
                return this.newDate;
            }
        }
    }

    public class CustomAndroidCalendarView : View
    {
        private List<CustomAndroidCalendarCell> dayCells;       //A gridcell for every day in the current month
        private DateTime firstDayOfMonth, lastDayOfMonth;
        private int weeksInMonth;
        private DateTime selectedDate;
        private CustomAndroidCalendarButton previousButton;
        private CustomAndroidCalendarButton nextButton;
        private List<DateTime> markedDates;                     
        private List<DateTime> holidays;

        public event EventHandler<DateChangedTo> DateChanged;   //Event raised when the user selects a date

        public Color backgroundColor { get; set; }
        public Color monthColor { get; set; }
        public Color weekdayColor { get; set; }
        public Color selectionColor { get; set; }
        public Color markerColor { get; set; }
        public Color cellBackgroundColor { get; set; }
        public Color gridColor { get; set; }
        public Color cellTextColor { get; set; }
        public Color buttonColor { get; set; }
        public Color holidayColor { get; set; }
        public Color weekendColor { get; set; }
        public Color todayColor { get; set; }
        public bool smallMarker { get; set; }
        public bool roundedCells { get; set; }
        public bool todayMarker { get; set; }

        public CustomAndroidCalendarView(Context context) : base(context)
        {
            this.SetWillNotDraw(false);

            baseSettings();

            //Dafault day to show is today
            setDate(DateTime.Now);
        }

        public CustomAndroidCalendarView(Context context, IAttributeSet attr) : base(context, attr)
        {
            this.SetWillNotDraw(false);

            baseSettings();

            //Dafault day to show is today
            setDate(DateTime.Now);
        }

        public CustomAndroidCalendarView(Context context, DateTime toShow) : base(context)
        {
            this.SetWillNotDraw(false);

            baseSettings();
            setDate(toShow);
        }

        private void baseSettings()
        {
            //List containing a cell for every day in the current month
            dayCells = new List<CustomAndroidCalendarCell>();

            //Default colors
            backgroundColor = Color.White;
            monthColor = Color.Black;
            selectionColor = Color.LightBlue;
            markerColor = Color.DeepPink;
            cellBackgroundColor = Color.Transparent;
            gridColor = Color.Black;
            cellTextColor = Color.Black;
            buttonColor = Color.Black;
            weekdayColor = Color.Black;
            holidayColor = Color.Gray;
            weekendColor = Color.Transparent;
            todayColor = Color.Gray;

            //Other Settings
            smallMarker = false;
            roundedCells = false;
            todayMarker = false;
        }

        /// <summary>
        /// Called when [touch event].
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (previousButton.checkTouch((int)e.GetX(), (int)e.GetY()))
            {
                //Show previous month
                DateTime newDate = new DateTime(selectedDate.AddMonths(-1).Year, selectedDate.AddMonths(-1).Month, 1);
                setDate(newDate);
            }
            else if (nextButton.checkTouch((int)e.GetX(), (int)e.GetY()))
            {
                //Show next month
                DateTime newDate = new DateTime(selectedDate.AddMonths(1).Year, selectedDate.AddMonths(1).Month, 1);
                setDate(newDate);
            }
            else
            {
                //Check if a date cell was touched
                foreach (CustomAndroidCalendarCell cell in dayCells)
                {
                    if (cell.checkTouch((int)e.GetX(), (int)e.GetY()))
                    {
                        //Select the touched cell
                        deselectAll();
                        cell.selectedDate = true;
                        setDate(cell.date);
                        break;
                    }
                }
            }

            //Force redraw
            Invalidate();

            return base.OnTouchEvent(e);
        }

        /// <summary>
        /// Sets the selected date.
        /// </summary>
        /// <param name="newDate">The date to select.</param>
        public void setDate(DateTime? newDate)
        {
            if (this.Handle == IntPtr.Zero)
                return;

            //Hard code monday as the first day of the week and january 1 is week 1 of the year
            DateTimeFormatInfo dfi = new DateTimeFormatInfo();
            dfi.FirstDayOfWeek = DayOfWeek.Monday;
            dfi.CalendarWeekRule = CalendarWeekRule.FirstDay;

            Calendar cal = dfi.Calendar;

            if (newDate != null)
            {
                //Make sure the selecteddate is in the format Y M D 0 0 0
                DateTime setDate = (DateTime)newDate;
                selectedDate = new DateTime(setDate.Year, setDate.Month, setDate.Day);
            }
            else
            {
                selectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }

            firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            //Get the week numbers of the first and last day of the selected month
            int weekNumberLast = cal.GetWeekOfYear(lastDayOfMonth, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int weekNumberFirst = cal.GetWeekOfYear(firstDayOfMonth, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            if (weekNumberFirst > 52)       //Returns 53 if jan 1 is not the first day of a new week
            {                               //Fixed by using 'CalendarWeekRule.FirstDay', so should never be used
                weekNumberFirst = 1;
                weekNumberLast += 1;
            }

            //The number of weeks to show
            weeksInMonth = weekNumberLast - weekNumberFirst + 1;

            dayCells.Clear();

            //Create a new instance of a customAndroidCalendarCell for every day of the selected month
            for (DateTime day = firstDayOfMonth; day <= lastDayOfMonth; day = day.AddDays(1))
            {
                CustomAndroidCalendarCell cell = new CustomAndroidCalendarCell(this.Context);

                cell.date = day;

                if (day == selectedDate)
                {
                    cell.selectedDate = true;
                }
                else
                {
                    cell.selectedDate = false;
                }

                cell.selectionColor = selectionColor;
                cell.markerColor = markerColor;
                cell.gridColor = gridColor;
                cell.cellTextColor = cellTextColor;
                cell.cellBackgroundColor = cellBackgroundColor;
                cell.holidayColor = holidayColor;
                cell.weekendColor = weekendColor;
                cell.todayColor = todayColor;

                cell.smallMarker = smallMarker;
                cell.roundedCells = roundedCells;

                cell.marked = false;
                cell.holiday = false;

                //Every cell holds its x and y coordinates
                //X coordinate: day of week (monday = 0, sunday = 6)
                //Y coordinate: week of month (first week = 0)
                int dayOfWeek = (int)cell.date.DayOfWeek;
                if (dayOfWeek == 0)
                {
                    dayOfWeek = 7;
                    cell.weekendCell = true;
                }
                else if (dayOfWeek == 6)
                {
                    cell.weekendCell = true;
                }
                else
                {
                    cell.weekendCell = false;
                }

                dayOfWeek -= 1;
                cell.gridX = dayOfWeek;

                int weekNumber = cal.GetWeekOfYear(day, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int firstWeekNumber = cal.GetWeekOfYear(firstDayOfMonth, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                if (firstWeekNumber > 52)       //Returns 53 if jan 1 is not the first day of a new week
                {
                    firstWeekNumber = 1;
                    weekNumber += 1;
                }

                if (weekNumber > 52 && day.Month < 2)
                    weekNumber = 1;

                int weekOfMonth = weekNumber - firstWeekNumber;
                cell.gridY = weekOfMonth;

                dayCells.Add(cell);
            }

            //Tell every cell if it is marked and a holiday
            setMarkedDates(markedDates);
            setHolidays(holidays);

            //Force redraw
            Invalidate();

            try
            {
                //Raise event to notify listeners of the newly selected date
                DateChangedTo dct = new DateChangedTo();
                dct.Date = selectedDate;
                DateChanged.Invoke(this, dct);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error date changed event: " + e.ToString());
            }
            
        }

        /// <summary>
        /// Called when drawing
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            string dayName;
            int numberOfRows = weeksInMonth;
            int numberofColumns = 7;
            int firstRowHeight = this.Height / 6;
            int rowHeight = (this.Height - firstRowHeight) / numberOfRows;
            int columnWidth = this.Width / numberofColumns;

            Paint backgroundPaint = new Paint();
            backgroundPaint.Color = backgroundColor;

            Paint monthPaint = new Paint();
            monthPaint.Color = monthColor;
            monthPaint.TextSize = firstRowHeight / 4;

            Paint weekdayPaint = new Paint();
            weekdayPaint.Color = weekdayColor;
            weekdayPaint.TextSize = firstRowHeight / 4;

            Paint gridPaint = new Paint();
            gridPaint.Color = Android.Graphics.Color.Pink;
            gridPaint.SetStyle(Paint.Style.Stroke);
            gridPaint.StrokeWidth = 2;

            //Draw background
            canvas.DrawRect(new Rect(0, 0, this.Width, this.Height), backgroundPaint);

            //Draw buttons
            previousButton = new CustomAndroidCalendarButton(this.Context);
            nextButton = new CustomAndroidCalendarButton(this.Context);
            previousButton.previousButton = true;
            nextButton.previousButton = false;
            previousButton.buttonColor = buttonColor;
            nextButton.buttonColor = buttonColor;

            previousButton.baseX = 0;
            previousButton.endX = columnWidth;
            previousButton.baseY = 0;
            previousButton.endY = firstRowHeight;

            nextButton.baseX = this.Width - columnWidth;
            nextButton.endX = this.Width;
            nextButton.baseY = 0;
            nextButton.endY = firstRowHeight;

            previousButton.Draw(canvas);
            nextButton.Draw(canvas);

            //Draw month name
            string monthName = selectedDate.ToString("MMMM yyyy");
            Rect monthNameRect = new Rect();
            monthPaint.GetTextBounds(monthName, 0, monthName.Length, monthNameRect);
            canvas.DrawText(monthName, this.Width / 2 - monthNameRect.Width() / 2, firstRowHeight - firstRowHeight / 2 - monthNameRect.Height() / 2, monthPaint);

            //Draw weekday names
            for (int i = 0; i < 7; i++)
            {
                if (i < 6)
                    dayName = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i + 1];
                else
                    dayName = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[0];

                canvas.DrawText(dayName, columnWidth * i + columnWidth / 10, firstRowHeight - firstRowHeight / 10, weekdayPaint);
            }

            //Draw cells
            foreach (CustomAndroidCalendarCell cell in dayCells)
            {
                cell.baseX = cell.gridX * columnWidth;
                cell.endX = cell.baseX + columnWidth;
                cell.baseY = firstRowHeight + cell.gridY * rowHeight;
                cell.endY = cell.baseY + rowHeight;

                cell.Draw(canvas);
            }

            if (todayMarker)
            {
                //Mark today
                foreach (CustomAndroidCalendarCell cell in dayCells)
                {
                    if (cell.date == DateTime.Today)
                    {
                        cell.todayCell();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the marked dates.
        /// </summary>
        /// <param name="_markedDates">A DateTime List containing all the dates to mark</param>
        public void setMarkedDates(List<DateTime> _markedDates)
        {
            if (_markedDates != null)
            {
                markedDates = new List<DateTime>();
                markedDates = _markedDates;
                unmarkAll();

                foreach (DateTime toMark in markedDates)
                {
                    foreach (CustomAndroidCalendarCell cell in dayCells)
                    {
                        if (cell.date.Year == toMark.Year && cell.date.Month == toMark.Month && cell.date.Day == toMark.Day)
                        {
                            //This day needs to be marked
                            cell.marked = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the holidays.
        /// </summary>
        /// <param name="_holidays">A DateTime List containing all holidays</param>
        public void setHolidays(List<DateTime> _holidays)
        {
            if (_holidays != null)
            {
                unholidayAll();
                holidays = new List<DateTime>();
                holidays = _holidays;

                foreach (DateTime holiday in holidays)
                {
                    foreach (CustomAndroidCalendarCell cell in dayCells)
                    {
                        if (cell.date.Year == holiday.Year && cell.date.Month == holiday.Month && cell.date.Day == holiday.Day)
                        {
                            //This day is a holiday
                            cell.holiday = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deselects all dates
        /// </summary>
        private void deselectAll()
        {
            foreach (CustomAndroidCalendarCell cell in dayCells)
            {
                cell.selectedDate = false;
            }
        }

        /// <summary>
        /// Unmarks all dates
        /// </summary>
        private void unmarkAll()
        {
            foreach (CustomAndroidCalendarCell cell in dayCells)
            {
                cell.marked = false;
            }
        }

        /// <summary>
        /// Unholiday all dates
        /// </summary>
        private void unholidayAll()
        {
            foreach (CustomAndroidCalendarCell cell in dayCells)
            {
                cell.holiday = false;
            }
        }
    }
}