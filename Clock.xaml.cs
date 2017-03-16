using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SENG403
{
    #region namespace constants 
    //NOTE** should really have this in a constants class elsewhere
    public enum RenderMode { RenderSecond, RenderMinutes, RenderHour, RenderAll, DontRender}

    public class CONSTANTS
    {
        public const int TICK_INTERVAL_MS = 125;
        public const int MS_IN_SEC = 1000;
        public const double DEG_PER_SEC = 6;
        public const double DEG_PER_HOUR = 30;
        public const double DEG_PER_MIN = 6;
        public const double SEC_IN_MIN = 60;
        public const double MIN_IN_HR = 60;
        public const int SECOND_HAND_OFFSET = 41;
    }
    #endregion

    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : UserControl
    {
        #region class variables
        private static Mutex mutex = new Mutex();
        private static double hourOffset = 0;
        public static double HourOffset { get { return hourOffset; } set {hourOffset = value;} }
        private double minuteOffset = 0;
        public double degreeInterval;
        DispatcherTimer dTimer;
        private double secondDegrees, minuteDegrees, hourDegrees, requestedMinuteAngle, requestedHourAngle;
        private double currHour, currMin, currSec;
        private string date, timestring, meridiem;
        Boolean animateClock;
        DateTime currentDateTime;

        public delegate void TimeUpdateEvent(object o, String arg);
        public static event TimeUpdateEvent UpdateTime;
        #endregion

        public Clock()
        {
            InitializeComponent();
            degreeInterval = (double)(CONSTANTS.DEG_PER_SEC * CONSTANTS.TICK_INTERVAL_MS) / CONSTANTS.MS_IN_SEC;
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, CONSTANTS.TICK_INTERVAL_MS);
            animateClock = true;
            updateTime();
            updateTimeLabel();
            synchronizeHands();
            dTimer.Start();

            MainWindow.OnTimeZoneChange += OnTimeZoneChange;
        }

        #region class-specific functions
        public static String GetDate()
        {
            DateTime now = Now();
            DateTime time = new DateTime(now.Year, now.Month, now.Day);
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

        public static DateTime Now()
        {
            return DateTime.Now.AddHours(hourOffset);
        }
        #endregion

        #region event handlers
        private void fireUpdateTime(String arg)
        {
            UpdateTime(this, arg);
        }

        private void OnTimeZoneChange(double offset)
        {
            hourOffset = offset;
            updateTime();
            updateTimeLabel();
            synchronizeHands();
        }

        private void hourMouseEnter(object sender, MouseEventArgs e) { Mouse.OverrideCursor = Cursors.Hand; }
        private void hourMouseLeave(object sender, MouseEventArgs e) { Mouse.OverrideCursor = Cursors.Arrow; }
        private void hourMouseDown(object sender, MouseButtonEventArgs e)
        {
            hour_hand_image.CaptureMouse();
            digital_canvas.Visibility = Visibility.Visible;
            second_hand_image.Visibility = Visibility.Hidden;
            DisableAnimations();
            requestedHourAngle = hourDegrees;
        }

        private void hourMouseRelease(object sender, MouseButtonEventArgs e)
        {
            hour_hand_image.ReleaseMouseCapture();
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Tick += (o, i) =>
            //{
            //    if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
            //    {
                    //TODO update time here
                    HourOffset = (int)(requestedHourAngle / CONSTANTS.DEG_PER_HOUR) - DateTime.Now.Hour;
            Console.WriteLine("Houroffset: " + hourOffset);
                    second_hand_image.Visibility = Visibility.Visible;
                    digital_canvas.Visibility = Visibility.Hidden;
                    EnableAnimations();
                    updateTime();
                    updateTimeLabel();
                    synchronizeHands();
            //        (o as DispatcherTimer).Stop();
            //    }
            //};
            //timer.Interval = new TimeSpan(0, 0, 2);
            //timer.Start();
        }

        private void hourMouseDrag(object sender, MouseEventArgs e)
        {
            if (hour_hand_image.IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(center_pin_image);
                Point center = new Point(center_pin_image.ActualWidth/2, center_pin_image.ActualHeight/2);
                requestedHourAngle = CalculateAngle(currentPoint, center);
                RotateTransform transform = new RotateTransform(requestedHourAngle, hour_hand_image.Width / 2, hour_hand_image.Height);
                hour_hand_image.RenderTransform = transform;
                updateTimeLabel(currMin, (int)(requestedHourAngle / CONSTANTS.DEG_PER_HOUR));
            }
        }

        private void minuteMouseEnter(object sender, MouseEventArgs e) { Mouse.OverrideCursor = Cursors.Hand; }
        private void minuteMouseLeave(object sender, MouseEventArgs e) { Mouse.OverrideCursor = Cursors.Arrow; }
        private void minuteMouseDown(object sender, MouseButtonEventArgs e)
        {
            minute_hand_image.CaptureMouse();
            digital_canvas.Visibility = Visibility.Visible;
            second_hand_image.Visibility = Visibility.Hidden;
            DisableAnimations();
            Point currentPoint = Mouse.GetPosition(center_pin_image);
            Point center = new Point(center_pin_image.ActualWidth / 2, center_pin_image.ActualHeight / 2);
            initAngle = CalculateAngle(currentPoint, center);
            requestedMinuteAngle = minuteDegrees;
        }
        private double initAngle;
        private int slavedHour;
        private void minuteMouseRelease(object sender, MouseButtonEventArgs e)
        {
            minute_hand_image.ReleaseMouseCapture();
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Tick += (o, i) =>
            //{
            //    if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
            //    {
            //TODO update time here
            minuteOffset = (int)(requestedMinuteAngle / CONSTANTS.DEG_PER_MIN) - DateTime.Now.Minute;
            HourOffset = slavedHour - DateTime.Now.Hour;
            //Console.WriteLine("minoffset: " + minuteOffset);
            second_hand_image.Visibility = Visibility.Visible;
                    digital_canvas.Visibility = Visibility.Hidden;
                    EnableAnimations();
                    updateTime();
                    updateTimeLabel();
                    synchronizeHands();
                    //(o as DispatcherTimer).Stop();
            //    }
            //};
            //timer.Interval = new TimeSpan(0, 0, 2);
            //timer.Start();
        }
        
        private void minuteMouseDrag(object sender, MouseEventArgs e)
        {
            if (minute_hand_image.IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(center_pin_image);
                Point center = new Point(center_pin_image.ActualWidth / 2, center_pin_image.ActualHeight / 2);
                requestedMinuteAngle = CalculateAngle(currentPoint, center);
                RotateTransform transform = new RotateTransform(requestedMinuteAngle, minute_hand_image.Width / 2, minute_hand_image.Height);
                minute_hand_image.RenderTransform = transform;

                //slave the hour hand
                double delta = requestedMinuteAngle - initAngle;

                double hourdelta = delta / 12;
                Console.WriteLine(hourdelta);
                if (delta >= 0)
                {
                    if (hourdelta < 0)
                    {
                        hourDegrees -= hourdelta;
                    }
                    else if (hourdelta > 20)
                    {
                        hourDegrees += (30 - hourdelta);
                    }
                    else
                        hourDegrees += hourdelta;
                }

                if (delta < 0)
                {
                    if (hourdelta >= 0)
                    {
                        hourDegrees -= hourdelta;
                    }
                    else if (hourdelta < -20)
                        hourDegrees -= (hourdelta + 30);
                    else
                        hourDegrees += hourdelta;
                }

                transform = new RotateTransform(hourDegrees, hour_hand_image.Width / 2, hour_hand_image.Height);
                hour_hand_image.RenderTransform = transform;

                slavedHour = (int)(hourDegrees / CONSTANTS.DEG_PER_HOUR);
                updateTimeLabel((int)(requestedMinuteAngle / CONSTANTS.DEG_PER_MIN), slavedHour);
                initAngle = requestedMinuteAngle;
            }
        }
        #endregion

        #region time-specific functions
        private void updateTime()
        {
            String[] cultureNames = { "en-US" };
            currentDateTime = DateTime.Now.AddHours(hourOffset);
            currentDateTime = currentDateTime.AddMinutes(minuteOffset);
            Console.WriteLine(currentDateTime.ToString());
            var culture = new CultureInfo(cultureNames[0]);
            string currentTime = currentDateTime.ToString(culture);
            string[] dateTimeElements = currentTime.Split(' ');
            date = dateTimeElements[0];
            timestring = dateTimeElements[1];
            meridiem = dateTimeElements[2];
            string[] timeElements = dateTimeElements[1].Split(':');
            currHour = Convert.ToDouble(timeElements[0]);               //add UTC offset from time zone
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

        private void renderAngles(RenderMode renderMode){
            if (renderMode == RenderMode.RenderSecond){
                RotateTransform transform = new RotateTransform(secondDegrees, second_hand_image.Width / 2, second_hand_image.Height - CONSTANTS.SECOND_HAND_OFFSET);
                secondDegrees = (secondDegrees + degreeInterval) >= 360 ? 0 : secondDegrees + degreeInterval;
                second_hand_image.RenderTransform = transform;
            }

            else if (renderMode == RenderMode.RenderMinutes){
                RotateTransform transform = new RotateTransform(minuteDegrees, minute_hand_image.Width / 2, minute_hand_image.Height);
                minute_hand_image.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderHour){
                RotateTransform transform = new RotateTransform(hourDegrees, hour_hand_image.Width / 2, hour_hand_image.Height);
                hour_hand_image.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderAll){
                RotateTransform transform = new RotateTransform(secondDegrees, second_hand_image.Width / 2, second_hand_image.Height - CONSTANTS.SECOND_HAND_OFFSET);
                second_hand_image.RenderTransform = transform;

                transform = new RotateTransform(minuteDegrees, minute_hand_image.Width / 2, minute_hand_image.Height);
                minute_hand_image.RenderTransform = transform;

                transform = new RotateTransform(hourDegrees, hour_hand_image.Width / 2, hour_hand_image.Height);
                hour_hand_image.RenderTransform = transform;
            }
            else throw new NotImplementedException("Unexpected analog clock render mode");
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            if (animateClock){
                renderAngles(RenderMode.RenderSecond);
                if (secondDegrees % CONSTANTS.DEG_PER_SEC == 0){    //every second update
                    updateTime();
                    computeAngles();
                    renderAngles(RenderMode.RenderMinutes);
                    updateTimeLabel();
                    fireUpdateTime(GetDate());
                }
                if (minuteDegrees % CONSTANTS.DEG_PER_HOUR == 0){   //every minute update
                    renderAngles(RenderMode.RenderHour);
                }
            }
        }

        private double CalculateAngle(Point dst, Point src)
        {
            double angle = Math.Atan((dst.Y - src.Y) / (dst.X - src.X));
            angle = angle * 180 / Math.PI;
            if (dst.X < src.X)
            {
                angle += 180;
            }
            return angle += 90; //translate to clock-oriented degrees (0 at the top)
        }

        private void updateTimeLabel()
        {
            string sec, min, hour;
            if (currSec < 10)
                sec = "0" + currSec.ToString();
            else
                sec = currSec.ToString();
            if (currMin < 10)
                min = "0" + currMin.ToString();
            else
                min = currMin.ToString();
            if (currHour < 10)
                hour = "  " + currHour.ToString();
            else
                hour = currHour.ToString();
            if (meridiem == "PM")
                hour = (currHour + 12).ToString();
            if (currHour == 12 && currMin == 0 && currSec >= 0)
                fireUpdateTime(GetDate());
            time_label.Content = hour + " . " + min + " . " + sec;
        }

        private void updateTimeLabel(double m, double h)
        {
            string sec, min, hour;
            if (currSec < 10)
                sec = "0" + currSec.ToString();
            else
                sec = currSec.ToString();
            if (m < 10)
                min = "0" + m.ToString();
            else
                min = m.ToString();
            if (h < 10)
                hour = "  " + h.ToString();
            else
                hour = h.ToString();
            if (meridiem == "PM")
                hour = (h + 12).ToString();
            time_label.Content = hour + " . " + min + " . " + sec;
        }
        #endregion
    }
}

