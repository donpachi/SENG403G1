using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SENG403
{
    public enum RenderMode { RenderSecond, RenderMinutes, RenderHour, RenderAll, DontRender }

    public class CONSTANTS
    {
        public const int TICK_INTERVAL_MS = 125;
        public const int MS_IN_SEC = 1000;
        public const double DEG_PER_SEC = 6;
        public const double DEG_PER_HOUR = 30;
        public const double SEC_IN_MIN = 60;
        public const double MIN_IN_HR = 60;
    }

    public class Time
    {
        private static double hourOffset = 0;
        public double HourOffset { get { return hourOffset; } set { hourOffset = value; } }
        public double degreeInterval;
        DispatcherTimer dTimer;
        private double secondDegrees, minuteDegrees, hourDegrees;
        private double currHour, currMin, currSec;
        private string date, timestring, meridiem;
        Image minImage, secImage, hrImage;
        Label timeLabel, dateLabel;
        Boolean animateClock;

        public Time(Image min, Image sec, Image hr, Label lb, Label dlb)
        {
            minImage = min;
            secImage = sec;
            hrImage = hr;
            timeLabel = lb;
            dateLabel = dlb;
        }

        public void Start()
        {
            degreeInterval = (double)(CONSTANTS.DEG_PER_SEC * CONSTANTS.TICK_INTERVAL_MS) / CONSTANTS.MS_IN_SEC;
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, CONSTANTS.TICK_INTERVAL_MS);
            animateClock = true;
            updateTime();
            synchronizeHands();
            dTimer.Start();
            dateLabel.Content = GetDate();
        }

        public String GetDate()
        {
            String[] eDates = date.Split('/');
            DateTime time = new DateTime(Convert.ToInt16(eDates[2]), Convert.ToInt16(eDates[0]), Convert.ToInt16(eDates[1]));
            return time.ToString("D");
        }

        public void DisableAnimations()
        {
            animateClock = false;
        }

        public void EnableAnimations()
        {
            animateClock = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime Now()
        {
            return DateTime.Now.AddHours(hourOffset);
        }


        private void updateTime()
        {
            String[] cultureNames = { "en-US" };
            DateTime currentDateTime = DateTime.Now;
            var culture = new CultureInfo(cultureNames[0]);
            string currentTime = currentDateTime.ToString(culture);
            string[] dateTimeElements = currentTime.Split(' ');
            date = dateTimeElements[0];
            timestring = dateTimeElements[1];
            meridiem = dateTimeElements[2];
            string[] timeElements = dateTimeElements[1].Split(':');
            currHour = hourOffset + Convert.ToDouble(timeElements[0]);               //add UTC offset from time zone
            currMin = Convert.ToDouble(timeElements[1]);
            currSec = Convert.ToDouble(timeElements[2]);
        }

        private void synchronizeHands()
        {
            computeAngles();
            renderAngles(RenderMode.RenderAll);
        }

        private void computeAngles()
        {
            secondDegrees = currSec * CONSTANTS.DEG_PER_SEC;
            minuteDegrees = (currMin * CONSTANTS.DEG_PER_SEC) + (currSec / CONSTANTS.SEC_IN_MIN) * 6;
            hourDegrees = (currHour * CONSTANTS.DEG_PER_HOUR) + (currMin / CONSTANTS.MIN_IN_HR) * 30;
        }

        private void renderAngles(RenderMode renderMode)
        {

            if (renderMode == RenderMode.RenderSecond)
            {
                RotateTransform transform = new RotateTransform(secondDegrees, secImage.Width / 2, secImage.Height / 2);
                secondDegrees = (secondDegrees + degreeInterval) >= 360 ? 0 : secondDegrees + degreeInterval;
                secImage.RenderTransform = transform;
            }

            else if (renderMode == RenderMode.RenderMinutes)
            {
                RotateTransform transform = new RotateTransform(minuteDegrees, minImage.Width / 2, minImage.Height / 2);
                minImage.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderHour)
            {
                RotateTransform transform = new RotateTransform(hourDegrees, hrImage.Width / 2, hrImage.Height / 2);
                hrImage.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderAll)
            {
                RotateTransform transform = new RotateTransform(secondDegrees, secImage.Width / 2, secImage.Height / 2);
                secImage.RenderTransform = transform;

                transform = new RotateTransform(minuteDegrees, minImage.Width / 2, minImage.Height / 2);
                minImage.RenderTransform = transform;

                transform = new RotateTransform(hourDegrees, hrImage.Width / 2, hrImage.Height / 2);
                hrImage.RenderTransform = transform;
            }
            else throw new NotImplementedException("Unexpected analog clock render mode");
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            if (animateClock)
            {
                renderAngles(RenderMode.RenderSecond);
                if (secondDegrees % CONSTANTS.DEG_PER_SEC == 0)
                {
                    updateTime();
                    computeAngles();
                    renderAngles(RenderMode.RenderMinutes);
                    updateTimeLabel();
                }
                if (minuteDegrees % CONSTANTS.DEG_PER_HOUR == 0)
                {
                    renderAngles(RenderMode.RenderHour);
                }
            }
        }

        private void updateTimeLabel()
        {

            string sec, min, hour;
            if (currSec < 10)
            {
                sec = "0" + currSec.ToString();
            }
            else
                sec = currSec.ToString();
            if (currMin < 10)
            {
                min = "0" + currMin.ToString();
            }
            else
                min = currMin.ToString();
            if (currHour < 10)
            {
                hour = "  " + currHour.ToString();
            }
            else hour = currHour.ToString();

            if (meridiem == "PM")
                hour = (currHour + 12).ToString();

            if (currHour == 12 && currMin == 0 && currSec >= 0)
            {
                dateLabel.Content = GetDate();
            }
               

            timeLabel.Content = hour + " : " + min + " : " + sec;
        }
    }
}
