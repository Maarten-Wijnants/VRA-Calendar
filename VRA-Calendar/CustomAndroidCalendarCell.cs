using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace CustomAndroidCalendar
{
    public class CustomAndroidCalendarCell : View
    {
        public DateTime date { get; set; }
        public int baseX { get; set; }
        public int baseY { get; set; }
        public int endX { get; set; }
        public int endY { get; set; }
        public int gridX { get; set; }
        public int gridY { get; set; }
        public bool selectedDate { get; set; }
        public bool marked { get; set; }
        public bool holiday { get; set; }
        public Color selectionColor { get; set; }
        public Color markerColor { get; set; }
        public Color cellBackgroundColor { get; set; }
        public Color gridColor { get; set; }
        public Color cellTextColor { get; set; }
        public Color holidayColor { get; set; }
        public Color weekendColor { get; set; }
        public Color todayColor { get; set; }
        public bool smallMarker { get; set; }
        public bool roundedCells { get; set; }
        public bool weekendCell { get; set; }

        private int gridLineWidth, cornerRadius;
        private Canvas myCanvas;

        public CustomAndroidCalendarCell(Context context) : base(context)
        {
            this.SetWillNotDraw(false);
        }

        /// <summary>
        /// Called when drawing
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        protected override void OnDraw(Canvas canvas)
        {
            gridLineWidth = 2;
            cornerRadius = 20;
            myCanvas = canvas;

            Paint gridPaint = new Paint();
            gridPaint.Color = gridColor;
            gridPaint.SetStyle(Paint.Style.Stroke);
            gridPaint.StrokeWidth = gridLineWidth;

            Paint cellPaint = new Paint();
            if (weekendCell)
                cellPaint.Color = weekendColor;
            else
                cellPaint.Color = cellBackgroundColor;

            Paint selectedPaint = new Paint();
            selectedPaint.Color = selectionColor;

            Paint markedPaint = new Paint();
            markedPaint.Color = markerColor;

            Paint textPaint = new Paint();
            textPaint.TextSize = (endY - baseY) / 3;
            if (!holiday)
                textPaint.Color = cellTextColor;
            else
                textPaint.Color = holidayColor;     //If this day is a holiday it gets a different color

            //The background of the cell
            Rect cellRect = new Rect(baseX + 1, baseY + 1, endX - 1, endY - 1);
            canvas.DrawRect(cellRect, cellPaint);

            //The grid
            if (!roundedCells)
            {
                Rect gridRect = new Rect(baseX, baseY, endX, endY);
                canvas.DrawRect(gridRect, gridPaint);
            }
            else
            {
                RectF gridRoundRect = new RectF(baseX, baseY, endX, endY);
                canvas.DrawRoundRect(gridRoundRect, cornerRadius, cornerRadius, gridPaint);
            }

            if (selectedDate)
            {
                //This is the selected date, it gets a different color
                if (!roundedCells)
                {
                    Rect selectionRect = new Rect(baseX + gridLineWidth / 2, baseY + gridLineWidth / 2, endX - gridLineWidth / 2, endY - gridLineWidth / 2);
                    canvas.DrawRect(selectionRect, selectedPaint);
                }
                else
                {
                    RectF gridRoundRect = new RectF(baseX + gridLineWidth / 2, baseY + gridLineWidth / 2, endX, endY);
                    canvas.DrawRoundRect(gridRoundRect, cornerRadius, cornerRadius, selectedPaint);
                }
            }

            if (marked)
            {
                //This day needs to be marked

                if ((!smallMarker && !roundedCells) || (!smallMarker && roundedCells))
                {
                    //Big marker
                    Rect markRect = new Rect(baseX + gridLineWidth / 2, endY - (endY - baseY) / 6, endX - gridLineWidth / 2, endY - (endY - baseY) / 8);
                    canvas.DrawRect(markRect, markedPaint);
                }
                else if (smallMarker && !roundedCells)
                {
                    //Small marker and no rounded cells
                    Rect markRect = new Rect(endX - (endX - baseX) / 8, endY - (endY - baseY) / 6, endX, endY);
                    canvas.DrawRect(markRect, markedPaint);
                }
                else if (smallMarker && roundedCells)
                {
                    //Small marker and rounded cells
                    canvas.DrawCircle(endX - (endX - baseX) / 6, endY - (endX - baseX) / 6, (endX - baseX) / 12, markedPaint);
                }
            }

            //Draw the day
            canvas.DrawText(date.Day.ToString(), baseX + (endX - baseX) / 6, baseY + textPaint.TextSize, textPaint);
        }

        /// <summary>
        /// Today, so mark it
        /// </summary>
        public void todayCell()
        {
            Paint gridPaint = new Paint();
            gridPaint.SetStyle(Paint.Style.Stroke);
            gridPaint.Color = todayColor;
            gridPaint.StrokeWidth = gridLineWidth * 4;

            if (!roundedCells)
            {
                Rect gridRect = new Rect(baseX, baseY, endX, endY);
                myCanvas.DrawRect(gridRect, gridPaint);
            }
            else
            {
                RectF gridRoundRect = new RectF(baseX, baseY, endX, endY);
                myCanvas.DrawRoundRect(gridRoundRect, cornerRadius, cornerRadius, gridPaint);
            }
        }

        /// <summary>
        /// Check if a touch is within this cells limits
        /// </summary>
        /// <param name="x">The x coordinate of the touch</param>
        /// <param name="y">The y coordinate of the touch</param>
        /// <returns></returns>
        public bool checkTouch(int x, int y)
        {
            bool touched = false;

            if (x > baseX && x < endX)
            {
                if (y > baseY && y < endY)
                {
                    touched = true;
                }
            }

            return touched;
        }
    }
}