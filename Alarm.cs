using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SENG403
{

    public class AlarmHandler
    {

        // Create a list of Alarms
        List<Alarm> alarmList;

        public AlarmHandler()
        {

            // Create a list of Alarms
            this.alarmList = new List<Alarm>();

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
            int day = dayofweek(dateAndTime.Day, dateAndTime.Month, dateAndTime.Year);
            // Get only the time from the DateTime object
            String time = dateAndTime.ToLongTimeString();

            // Check every second if the current time is the one we're checking for
            // If so, throw an exception (play sound later on)
            foreach (Alarm alarm in alarmList)
            {
                
                // If the current time is one of the alarms, then throw an exception
                if (DateTime.Now.ToString().Equals(alarm.getDateTime().ToString()))
                {
                    if (alarm.getDays() == "0000000" || alarm.getDays()[day].Equals("1"))
                    {
                        throw exception;    // This needs to be paired with the sound system
                    }

                }
            }
        }

        // Handle the input data once the "set alarm" button is pressed
        //days string should be 7 digits long, "1" represents a selected day, "0" represents a non-selected day
        public void setNewAlarm(DateTime time, String days)
        {
            // Gather the input information and create a string representation of a DateTime object

            // Add AM/PM to the time String
            

            // Create a new alarm and append it to the alarmList
            alarmList.Add(new Alarm(time, days)); //repeat value hard coded to false for now
            //sort(alarmList);

            // Error checking the format of the input time still has to be handled
        }

        //taken from stackoverflow, needs to be converted to C++
        private int dayofweek(int d, int m, int y)
        {
            int[] t= { 0, 3, 2, 5, 0, 3, 5, 1, 4, 6, 2, 4 };
            if (m < 3)
            {
                y -= m;
            }
            return (y + y / 4 - y / 100 + y / 400 + t[m - 1] + d) % 7;
        }




    }

    // The alarm class which holds the required data
    public class Alarm
    {
        String period; // AM or PM setting - may not be necessary
        DateTime settime;
        DateTime time;
        String days;
        Boolean repeat = false;


        public Alarm(DateTime time, String days)
        {
            this.time = time;
            this.settime = time;
            this.period = null; // AM or PM setting
            this.days = days;
            if (days != "0000000") { repeat = true; }
        }

        // Return the Date and Time this alarm is set to
        public String getDateTime()
        {
            return time.ToLongTimeString();
        }

        public String getDays() { return days; }

        //This snoozes the alarm for a set amount of minutes
        public void snooze(Double minutes)
        {
            time = DateTime.Now.AddMinutes(minutes);
        }

        //this resets the alarm to the original pre-snooze configuration
        public void reset() { time = settime; }

        public Boolean getRepeatVal() { return repeat; }
    }
}
