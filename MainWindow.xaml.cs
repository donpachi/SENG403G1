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
        Alarm currentAlarm;

        public MainWindow()
        {
            InitializeComponent();

            // Create a list of Alarms
            this.alarmList = new List<Alarm>();

            // The Currently RINGING alarm, if applicable
            this.currentAlarm = null;

            // Start the clock
            startclock();
        }

        //this ends the alarm and sets it to the next scheduled date if repeat is true, else it deletes the alarm
        public void endAlarm(Alarm alarm)
        {
            if (alarm.getRepeatVal() == true)
            {
                alarm.reset();
            }
            else { deleteAlarm(alarm); }
        }

        //this removes the specified alarm from alarmList even if it is set to repeat
        public void deleteAlarm(Alarm alarm)
        {
            alarmList.Remove(alarm);
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
                    // If the current time matches an alarm, set the current alarm
                    // Also set "currently ringing" to be true
                    currentAlarm = alarm;
                    currentAlarm.setRinging(true);
                }

                // If there IS an alarm currently triggered and it is ringing, print alarm is ringing to screen
                if (currentAlarm != null && currentAlarm.isRinging())
                {
                    timeLabel.Text = "ALARM IS RINGING";
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
            alarmList.Add(new Alarm(DateTime.Parse(alarmTime), false));

            // Error checking the format of the input time still has to be handled
        }

        // If the user clicks the alarm time text box, clear it of the default text for convenience
        private void setAlarmTextBox_GotFocus(object sender, EventArgs e)
        {
            setAlarmTextBox.Clear();    // Clear the text box once clicked
        }

        // If snooze is pressed, delay the currently ringing alarms time by a certain amount
        private void snoozeButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAlarm != null && currentAlarm.isRinging())
            {
                currentAlarm.snooze(0.1);
                currentAlarm.setRinging(false);
                currentAlarm = null;
            }
        }

        // Once the stop button is pressed, it stops the alarm ringing
        // Need to decide what to do with the alarm once "stop" is pressed. Repeat tomorrow? Delete? Etc
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAlarm != null)
            {
                currentAlarm.setRinging(false);
                currentAlarm = null;
            }
        }
    }

    // The alarm class which holds the required data
    public class Alarm
    {
        String time;
        String date;
        DateTime settime;
        DateTime datetime;
        Boolean repeat;
        bool currentlyRinging;

        public Alarm(DateTime datetime, Boolean repeat)
        {
            // For setting alarms on specific days, we can use datetime.parse to convert two strings of date and time into one datetime object
            // We can then compare them using a datetime method
            this.datetime = datetime;
            this.settime = datetime;
            this.time = datetime.ToLongTimeString();
            this.date = datetime.ToLongDateString();
            this.repeat = repeat;
            bool currentlyRinging = false;
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

        public Boolean getRepeatVal() { return repeat; }

        // Return true if this alarm is currently ringing
        public bool isRinging()
        {
            return currentlyRinging;
        }

        // Set the boolean value true/false depending on whether the alarm is ringing
        public void setRinging(bool val)
        {
            this.currentlyRinging = val;
        }
    }
}
