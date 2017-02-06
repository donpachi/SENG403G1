using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Create a list of Alarms
        List<Alarm> alarmList;

        public MainWindow()
        {
            InitializeComponent();

            // Create a list of Alarms
            this.alarmList = new List<Alarm>();

            // Start the clock
            startclock();
        }

        // Start the timer, create a tick event and set tick interval to one second
        private void startclock()
        {
            // Create a timer object
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);   // Set the timer interval to one second
            timer.Tick += tickevent;    // After every one second, we trigger a tick event
            timer.Start();  // Start the timer
        }

        // Tick event that occurs every one second
        private void tickevent(object sender, EventArgs e)
        {
            // Get the CURRENT date and time in the form: 'yyyy-mm-dd hh:mm:ss AM/PM' (i.e: 2017-01-28 12:20:00 PM)
            DateTime dateAndTime = DateTime.Now;
            
            // An exception for testing purposes
            Exception exception = new Exception();

            // Get only the time from the DateTime object
            String time = DateTime.Now.ToLongTimeString();
            timeLabel.Text = time;  // Display only the time on the UI

            // Check every second if the current time is the one we're checking for
            // If so, throw an exception (play sound later on)
            foreach (Alarm alarm in alarmList)
            {
                // If the current time is one of the alarms, then throw an exception
                if (DateTime.Now.ToString().Equals(alarm.getDateTime().ToString()))
                {
                    alarm.reset();
                    throw exception;    // This works
                }
            }
        }

        // Handle the input data once the "set alarm" button is pressed
        private void setAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            // Gather the input information and create a string representation of a DateTime object
            String alarmTime = DateTime.Now.ToLongDateString() + " " + setAlarmTextBox.Text;

            // Add AM/PM to the time String
            if (AmPmBox.SelectedIndex == 0)
            {
                alarmTime = alarmTime + " AM";
            } else if (AmPmBox.SelectedIndex == 1)
            {
                alarmTime = alarmTime + " PM";
            }

            // Create a new alarm and append it to the alarmList
            alarmList.Add(new Alarm(DateTime.Parse(alarmTime)));

            // Error checking the format of the input time still has to be handled
        }

        // If the user clicks the alarm time text box, clear it of the default text for convenience
        private void setAlarmTextBox_GotFocus(object sender, EventArgs e)
        {
            setAlarmTextBox.Clear();    // Clear the text box once clicked
        }

    }

    // The alarm class which holds the required data
    public class Alarm
    {
        String period; // AM or PM setting - may not be necessary
        String time;
        String date;
        DateTime settime;
        DateTime datetime;

        public Alarm(DateTime datetime)
        {
            // For setting alarms on specific days, we can use datetime.parse to convert two strings of date and time into one datetime object
            // We can then compare them using a datetime method
            this.datetime = datetime;
            this.settime = datetime;
            this.period = null; // AM or PM setting
            this.time = datetime.ToLongTimeString();
            this.date = datetime.ToLongDateString();
        }

        // Return the Date and Time this alarm is set to
        public DateTime getDateTime()
        {
            return this.datetime;
        }

        //This snoozes the alarm for a set amount of minutes
        public void snooze(Double minutes)
        {
            datetime = DateTime.Now.AddMinutes(minutes);
        }

        //this resets the alarm to the original pre-snooze configuration
        public void reset() { datetime = settime; }

    }
}
