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
    public enum RenderMode { RenderMinutes, RenderHour, RenderAll }

    public class CONSTANTS
    {
        public const int TICK_INTERVAL_MS = 125;
        public const int MS_IN_SEC = 1000;
        public const double DEG_PER_SEC = 6;
        public const double DEG_PER_HOUR = 30;
        public const double SEC_IN_MIN = 60;
        public const double MIN_IN_HR = 60;
    }

    public partial class Time
    {
        public double degreeInterval;
        DispatcherTimer dTimer;
        private double secondDegrees, minuteDegrees, hourDegrees;
        private double currHour, currMin, currSec;
        private string date;
        Image minImage, secImage, hrImage;
        Label timeLabel;
        Canvas analogCanvas, digiCanvas;

        public Time(Image min, Image sec, Image hr, Label lb)
        {
            minImage = min;
            secImage = sec;
            hrImage = hr;
            timeLabel = lb;
        }

        public void Start()
        {
            degreeInterval = (double)(CONSTANTS.DEG_PER_SEC * CONSTANTS.TICK_INTERVAL_MS) / CONSTANTS.MS_IN_SEC;
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, CONSTANTS.TICK_INTERVAL_MS);
            updateTime();
            synchronizeHands();
            dTimer.Start();
        }


        private void linkResources()
        {
            Canvas mCanvas = (Canvas)Application.Current.FindResource("main_canvas");

        }

        private void updateTime()
        {
            String[] cultureNames = { "en-US" };
            DateTime currentDateTime = DateTime.Now;
            var culture = new CultureInfo(cultureNames[0]);
            string currentTime = currentDateTime.ToString(culture);
            string[] dateTimeElements = currentTime.Split(' ');
            date = dateTimeElements[1];
            string[] timeElements = dateTimeElements[1].Split(':');
            currHour = Convert.ToDouble(timeElements[0]);
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
            if (renderMode == RenderMode.RenderMinutes)
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
            RotateTransform transform = new RotateTransform(secondDegrees, secImage.Width / 2, secImage.Height / 2);
            secondDegrees = (secondDegrees + degreeInterval) >= 360 ? 0 : secondDegrees + degreeInterval;
            secImage.RenderTransform = transform;

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

            timeLabel.Content = hour + " : " + min + " : " + sec;
        }
    }
}
