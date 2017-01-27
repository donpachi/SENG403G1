using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    public enum RenderMode {RenderMinutes, RenderHour, RenderAll}

    public class CONSTANTS
    {
        public const int TICK_INTERVAL_MS = 125;
        public const int MS_IN_SEC = 1000;
        public const double DEG_PER_SEC = 6;
        public const double DEG_PER_HOUR = 30;
        public const double SEC_IN_MIN = 60;
        public const double MIN_IN_HR = 60;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double degreeInterval;
        DispatcherTimer dTimer;
        private double secondDegrees, minuteDegrees, hourDegrees;
        private double currHour, currMin, currSec;
        private string date;
        public MainWindow()
        {
            InitializeComponent();
            updateTime();
            initTimerElements();
        }

        private void initTimerElements()
        {
            degreeInterval = (double)(CONSTANTS.DEG_PER_SEC*CONSTANTS.TICK_INTERVAL_MS)/CONSTANTS.MS_IN_SEC;
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, CONSTANTS.TICK_INTERVAL_MS);
            updateTime();
            synchronizeHands();
            dTimer.Start();
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
                RotateTransform transform = new RotateTransform(minuteDegrees, minute_hand_image.Width / 2, minute_hand_image.Height / 2);
                minute_hand_image.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderHour)
            {
                RotateTransform transform = new RotateTransform(hourDegrees, hour_hand_image.Width / 2, hour_hand_image.Height / 2);
                hour_hand_image.RenderTransform = transform;
            }
            else if (renderMode == RenderMode.RenderAll)
            {
                RotateTransform transform = new RotateTransform(secondDegrees, second_hand_image.Width / 2, second_hand_image.Height / 2);
                second_hand_image.RenderTransform = transform;

                transform = new RotateTransform(minuteDegrees, minute_hand_image.Width / 2, minute_hand_image.Height / 2);
                minute_hand_image.RenderTransform = transform;

                transform = new RotateTransform(hourDegrees, hour_hand_image.Width / 2, hour_hand_image.Height / 2);
                hour_hand_image.RenderTransform = transform;
            }
            else throw new NotImplementedException();
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            RotateTransform transform = new RotateTransform(secondDegrees, second_hand_image.Width / 2, second_hand_image.Height / 2);
            secondDegrees = (secondDegrees + degreeInterval) >= 360 ? 0 : secondDegrees + degreeInterval;
            second_hand_image.RenderTransform = transform;


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
                hour = "  "+currHour.ToString();
            }
            else hour = currHour.ToString();

            time_label.Content = hour + " : " + min + " : " + sec;
        }
    }
}
