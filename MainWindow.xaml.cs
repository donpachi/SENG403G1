#define DEMO
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
using System.IO;
namespace SENG403
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Time time;
        SoundModule sound = new SoundModule();         //base sound module to be copied into each alarm
                                                        // (each alarm gets their own instance which can be set accordingly)
        AlarmHandler alarmHandler = new AlarmHandler();
        Alarm aRingingAlarm;


        public MainWindow()
        {
            InitializeComponent();
            this.KeyUp += MainWindow_KeyUp;
            time = new Time(minute_hand_image, second_hand_image, hour_hand_image, time_label, date_label);
            time.Start();


            // populate sounds comboBox with available .wav files in Sound directory
            string[] availableSounds = sound.getSounds();
            for(int i = 0; i<availableSounds.Length; i++)
            {
                comboBoxSounds.Items.Add(availableSounds[i]);
            }

            //LEFT OFF HERE
            aRingingAlarm = new AlarmHandler().getCurrentAlarm();
            Alarm.onRing += onAlarmRing;
            //aRingingAlarm.alarmIsRinging += ringingAlarm;             //currently causes nullpointerexception,
                                                                        // see setRinging() method in Alarm class

        }

        public void onAlarmRing()
        {
            Console.WriteLine("ring");
            buttonDismissAlarm.Visibility = Visibility.Visible;
        }

        //// custom event for handling a ringing alarm from an alarm object
        //public void ringingAlarm(object sender, EventArgs e)
        //{
        //    buttonDismissAlarm.Visibility = Visibility.Visible;
        //}


        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
            if (e.Key == Key.B)
            {
                digital_canvas.Visibility = Visibility.Visible;
                analog_canvas.Visibility = Visibility.Visible;
            }

#if DEMO    //allow custom time manipulation
            if(e.Key == Key.Right)
            {
                //TODO increment tick rate
            }

            if(e.Key == Key.Left)
            {
                //TODO decrement tick rate
            }

            if(e.Key == Key.Up)
            {
                //TODO increment day
            }

            if (e.Key == Key.Down)
            {
                //TODO decrement day
            }

            if (e.Key == Key.Space)
            {
                //TODO resume normal speed
            }
#endif
        }

        // Get all set values on the interface and send them to the alarm class to make an alarm
        private void clickButtonConfirm(object sender, RoutedEventArgs e)
        {
            // convert the hour and minute entries to integers so that they may be used for
            // the alarm's DateTime
            int theHour = Convert.ToInt32(textBoxHourEntry.Text);
            int theMinute = Convert.ToInt32(textBoxMinuteEntry.Text);

            // string which holds 0 or 1 for each day of the week (Sunday = 0th, Monday = 1th, ..., Saturday = 6th)
            string alarmDaysChecked = "";

            // Build days string before creating alarm
            if (checkBox_Sunday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Monday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Tuesday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Wednesday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Thursday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Friday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            if (checkBox_Saturday.IsChecked == true) { alarmDaysChecked += "1"; }
            else { alarmDaysChecked += "0"; }

            //TEMPRORARY/ROUGH to make functionality work:
            DateTime theTime = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month,
                System.DateTime.Now.Day, theHour, theMinute, 0);

            // set the sound for the alarm being created (selected from comboBox)
            string selectedSound = comboBoxSounds.Text;
            sound.setSound(selectedSound);

            //TODO: need to pass in snooze time

            // create new alarm object
            alarmHandler.setNewAlarm(theTime, alarmDaysChecked, sound);

            canvasAlarmSet.Visibility = Visibility.Hidden;
            buttonSetAlarm.Visibility = Visibility.Visible;

            // DEBUG - print out days checked to console
            // System.Diagnostics.Debug.WriteLine("DAYS: "+alarmDaysChecked);
        }

        private void clickButtonCancel(object sender, RoutedEventArgs e)
        {
            canvasAlarmSet.Visibility = Visibility.Hidden;
            time_canvas.Visibility = Visibility.Visible;
            buttonSetAlarm.Visibility = Visibility.Visible;
        }

        // Listener for when an alarm sound from dropdown menu is selected (comboBox)
        // I am not 100% sure on this type of event listener though
        private void selectedSound(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        //-------------------------------------------------------------checkboxes
        // Checked the repeat weekly checkbox
        private void repeatWeeklyIsChecked(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Sunday checkbox for an alarm
        private void checkedSunday(object sender, RoutedEventArgs e)
        {
           
        }

        // Checked the Monday checkbox for an alarm
        private void checkedMonday(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Tuesday checkbox for an alarm
        private void checkedTuesday(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Wednesday checkbox for an alarm
        private void checkedWednesday(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Thursday checkbox for an alarm
        private void checkedThursday(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Friday checkbox for an alarm
        private void checkedFriday(object sender, RoutedEventArgs e)
        {

        }

        // Checked the Saturday checkbox for an alarm
        private void checkedSaturday(object sender, RoutedEventArgs e)
        {

        }
        //-------------------------------------------------------------endcheckboxes

        //-------------------------------------------------------------radiobuttons
        // Selected the 1 minute snooze radio button for an alarm
        private void select1Min(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 2 minute snooze radio button for an alarm
        private void select2Mins(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 5 minute snooze radio button for an alarm
        private void select5Mins(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 10 minute snooze radio button for an alarm
        private void select10Mins(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 15 minute snooze radio button for an alarm
        private void select15Mins(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 30 minute snooze radio button for an alarm
        private void select30Mins(object sender, RoutedEventArgs e)
        {

        }

        // Selected the 60 minute snooze radio button for an alarm
        private void select60Mins(object sender, RoutedEventArgs e)
        {

        }
        //-------------------------------------------------------------end radiobuttons
        //=================================================================== end alarm set screen



        //=================================================================== active alarm screen listeners
        // Listener for when the snooze button is pressed
        private void clickSnooze(object sender, RoutedEventArgs e)
        {
            /*
            if (alarmHandler. != null && currentAlarm.isRinging())
            {
                currentAlarm.snooze(0.1);
                currentAlarm.setRinging(false);
                currentAlarm = null;
            }*/
        }

        // Listener for the Dismiss button
        private void clickDismiss(object sender, RoutedEventArgs e)
        {
            Alarm ringingAlarm = alarmHandler.getCurrentAlarm();
            ringingAlarm.setRinging(false);
            buttonDismissAlarm.Visibility = Visibility.Hidden;
        }
        //=================================================================== end active alarm screen listeners



        //=================================================================== time display screen listeners
        // Listener for when the toggleButton is checked
        // when clicked, if state is analog, switch to digital. Else switch to analog.
        private void displayModeToggle(object sender, RoutedEventArgs e)
        {
            if (analog_canvas.IsVisible)
            {
                toggleDisplayButton.Content = "Analog";
                analog_canvas.Visibility = Visibility.Hidden;
                digital_canvas.Visibility = Visibility.Visible;
            }
            else
            {
                toggleDisplayButton.Content = "Digital";
                analog_canvas.Visibility = Visibility.Visible;
                digital_canvas.Visibility = Visibility.Hidden;
            }
        }

        // Listener for the set alarm button
        // Hides the Time Display base canvas and makes the set alarm canvas visible
        private void gotoSetAlarm(object sender, RoutedEventArgs e)
        {
            buttonSetAlarm.Visibility = Visibility.Hidden;
            canvasAlarmSet.Visibility = Visibility.Visible;
        }
        //=================================================================== end time display screen listeners
    }
}
