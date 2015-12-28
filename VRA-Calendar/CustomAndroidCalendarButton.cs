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
    public class CustomAndroidCalendarButton : View
    {
        public bool previousButton { get; set; }
        public int baseX { get; set; }
        public int baseY { get; set; }
        public int endX { get; set; }
        public int endY { get; set; }
        public Color buttonColor { get; set; }

        public CustomAndroidCalendarButton(Context context) : base(context)
        {
            this.SetWillNotDraw(false);
        }

        /// <summary>
        /// Called when drawing
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        protected override void OnDraw(Canvas canvas)
        {
            string buttonText;

            Paint textPaint = new Paint();
            textPaint.Color = buttonColor;
            textPaint.TextSize = (endY - baseY) / 2;

            //Previousbutton and nextbutton have a different location and text
            if (previousButton)
            {
                buttonText = "<";

                canvas.DrawText(buttonText, baseX + (endX - baseX) / 6, baseY + (endY - baseY) / 2, textPaint);
            }
            else
            {
                buttonText = ">";

                Rect buttonTextRect = new Rect();
                textPaint.GetTextBounds(buttonText, 0, buttonText.Length, buttonTextRect);

                canvas.DrawText(buttonText, endX - buttonTextRect.Width() - (endX - baseX) / 6, baseY + (endY - baseY) / 2, textPaint);
            }
        }

        /// <summary>
        /// Check if a touch is within my limits
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