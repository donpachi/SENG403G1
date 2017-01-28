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
        public MainWindow()
        {
            InitializeComponent();

            // Start the clock
            startclock();
        }

        // The method that creates a tick event and sets the interval
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
            // Get the current date and time and convert it to a single string
            // In the form: 'yyyy-mm-dd hh:mm:ss AM/PM' (i.e: 2017-01-28 12:20:00 PM)
            String dateAndTime = DateTime.Now.ToString();
            
            // An exception for testing purposes
            Exception exception = new Exception();

            // Split the dateAndTime string into an array of two entries, one for date and one for time
            String[] dateAndTimeArray = dateAndTime.Split(new char[] { ' ' }, 2); // Split at the first occurence of ' ' only
            String date = dateAndTimeArray[0];  // Store the date in the form: yyyy-mm-dd
            String time = dateAndTimeArray[1];  // Store the time in the form: hh:mm:ss AM/PM
            timeLabel.Text = time;  // Display only the time on the UI

            // Check every second if the current time is the one we're checking for
            // If so, throw an exception
            if (time == "12:20:00 PM")
            {
                throw exception;    // This works
            }

        }
    }
}
