using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.ComponentModel;

namespace SENG403
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni;
        Time time;
        SoundModule sound = new SoundModule();         //base sound module to be copied into each alarm
                                                       // (each alarm gets their own instance which can be set accordingly)
        AlarmHandler alarmHandler = new AlarmHandler();
        Double snoozeTime = 0;
        Boolean editVal = false;
        Alarm editedAlarm;


        public MainWindow()
        {
            InitializeComponent();
            CreateTrayIcon();

            this.KeyUp += MainWindow_KeyUp;
            time = new Time(minute_hand_image, second_hand_image, hour_hand_image, time_label, date_label);
            time.Start();

            // populate sounds comboBox with available .wav files in Sound directory
            string[] availableSounds = sound.getSounds();
            for (int i = 0; i < availableSounds.Length; i++)
            {
                comboBoxSounds.Items.Add(availableSounds[i]);
            }

            updateAlarmsList();

            Alarm.onRing += onAlarmRing;
        }

        /// <summary>
        /// Gets the current list of alarms from the alarm class alarmList and updates the UI alarm list.
        /// </summary>
        public void updateAlarmsList()
        {
            alarmList.Items.Clear();
            Alarm[] theAlarms = alarmHandler.getAlarms();
            for (int i = 0; i < theAlarms.Length; i++)
            {
                string daysCode = theAlarms[i].getDays();
                string daysConverted = "";
                for (int j = 0; j < daysCode.Length; j++)
                {
                    char c = daysCode[j];
                    if (c.Equals('1'))
                    {
                        switch (j)
                        {
                            case 0:
                                daysConverted += "Su. ";
                                break;
                            case 1:
                                daysConverted += "Mo. ";
                                break;
                            case 2:
                                daysConverted += "Tu. ";
                                break;
                            case 3:
                                daysConverted += "We. ";
                                break;
                            case 4:
                                daysConverted += "Th. ";
                                break;
                            case 5:
                                daysConverted += "Fr. ";
                                break;
                            case 6:
                                daysConverted += "Sa. ";
                                break;
                        }
                    }
                }

                String nextAlarm = "Alarm " + (i + 1) + ": " + theAlarms[i].getDateTime() + " " + daysConverted;
                //only add the alarm if it isn't in the list already
                if (!alarmList.Items.Contains(nextAlarm))
                    alarmList.Items.Add(nextAlarm);
            }
        }

        //event method to trigger when the window changes states
        protected override void OnStateChanged(EventArgs e)
        {

            base.OnStateChanged(e);
        }

        private void MinimizeWindow()
        {
            if (this.WindowState == WindowState.Maximized || this.WindowState == WindowState.Normal)
            {
                this.Hide();
                this.WindowState = WindowState.Minimized;
                time.DisableAnimations();
            }
        }

        private void MaximizeWindow()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                ni.Visible = false;
                this.Show();
                this.WindowState = WindowState.Maximized;
                time.EnableAnimations();
            }
        }



        // Once the window has been closed, update the settings
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            alarmHandler.populateSettings();
            Properties.Settings.Default.Save();
        }


        public void onAlarmRing()
        {
            MaximizeWindow();
            Console.WriteLine("ring");
            textBlock.Text = alarmHandler.getCurrentAlarm().getMessage();
            textBlock.Visibility = Visibility.Visible;
            buttonDismissAlarm.Visibility = Visibility.Visible;
            buttonSnoozeAlarm.Visibility = Visibility.Visible;
        }

        private void CreateTrayIcon()
        {
            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Resource/Main.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    MaximizeWindow();
                };

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.B)
            {
                digital_canvas.Visibility = Visibility.Visible;
                analog_canvas.Visibility = Visibility.Visible;
            }
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

            String message = messageBox.Text;
            //TEMPRORARY/ROUGH to make functionality work:
            DateTime theTime = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month,
                System.DateTime.Now.Day, theHour, theMinute, 0);

            // set the sound for the alarm being created (selected from comboBox)
            string selectedSound = comboBoxSounds.Text;
            sound.setSound(selectedSound);

            //TODO: need to pass in snooze time

            // create new alarm object
            alarmHandler.setNewAlarm(theTime, alarmDaysChecked, sound, message);

            if (editVal == true)
            {
                alarmHandler.deleteAlarm(editedAlarm);
                editVal = false;
            }
            //update the UI with all alarms in the alarm arraylist
            updateAlarmsList();

            resetAlarmPanel();
            canvasAlarmSet.Visibility = Visibility.Hidden;
            alarmList.Visibility = Visibility.Visible;
            buttonSetAlarm.Visibility = Visibility.Visible;
            buttonEditAlarm.Visibility = Visibility.Visible;
            buttonDeleteAlarm.Visibility = Visibility.Visible;
            // DEBUG - print out days checked to console
            // System.Diagnostics.Debug.WriteLine("DAYS: "+alarmDaysChecked);
        }

        private void clickButtonCancel(object sender, RoutedEventArgs e)
        {
            resetAlarmPanel();
            canvasAlarmSet.Visibility = Visibility.Hidden;
            alarmList.Visibility = Visibility.Visible;
            time_canvas.Visibility = Visibility.Visible;
            buttonSetAlarm.Visibility = Visibility.Visible;
        }

        //-------------------------------------------------------------radiobuttons
        // Selected the 1 minute snooze radio button for an alarm
        private void select1Min(object sender, RoutedEventArgs e)
        {
            snoozeTime = 1;
        }

        // Selected the 2 minute snooze radio button for an alarm
        private void select2Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 2;
        }

        // Selected the 5 minute snooze radio button for an alarm
        private void select5Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 5;
        }

        // Selected the 10 minute snooze radio button for an alarm
        private void select10Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 10;
        }

        // Selected the 15 minute snooze radio button for an alarm
        private void select15Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 15;
        }

        // Selected the 30 minute snooze radio button for an alarm
        private void select30Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 30;
        }

        // Selected the 60 minute snooze radio button for an alarm
        private void select60Mins(object sender, RoutedEventArgs e)
        {
            snoozeTime = 60;
        }
        //-------------------------------------------------------------end radiobuttons
        //=================================================================== end alarm set screen



        //=================================================================== active alarm screen listeners
        // Listener for when the snooze button is pressed
        private void clickSnooze(object sender, RoutedEventArgs e)
        {
            alarmHandler.getCurrentAlarm().snooze(snoozeTime);
            textBlock.Visibility = Visibility.Hidden;
            buttonDismissAlarm.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
        }

        // Listener for the Dismiss button
        private void clickDismiss(object sender, RoutedEventArgs e)
        {
            Alarm ringingAlarm = alarmHandler.getCurrentAlarm();
            ringingAlarm.setRinging(false);
            textBlock.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
            buttonDismissAlarm.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
            alarmHandler.endAlarm(ringingAlarm);
            updateAlarmsList();
        }
        //=================================================================== end active alarm screen listeners


        #region Display functions
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
            alarmList.Visibility = Visibility.Hidden;
            buttonEditAlarm.Visibility = Visibility.Hidden;
            buttonDeleteAlarm.Visibility = Visibility.Hidden;
            buttonSetAlarm.Visibility = Visibility.Hidden;
            canvasAlarmSet.Visibility = Visibility.Visible;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //=================================================================== end time display screen listeners
        #endregion


        //listener for selecting an alarm in the alarm UI list
        private void selectedAlarmItem(object sender, SelectionChangedEventArgs e)
        {
            buttonEditAlarm.Visibility = Visibility.Visible;
            buttonDeleteAlarm.Visibility = Visibility.Visible;
        }

        private void closeApplicationClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimizeApplicationClick(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {
            string selectedAlarm = alarmList.SelectedItem.ToString();
            if (selectedAlarm == null) { return; }
            else
            {
                for (int n = 1; n <= alarmHandler.getAlarms().Length; n++)
                {
                    if (selectedAlarm.Contains("Alarm " + n))
                    {
                        alarmHandler.deleteAlarm(alarmHandler.getAlarms()[n - 1]);
                        updateAlarmsList();
                    }
                }
            }
        }

        private void editClick(object sender, RoutedEventArgs e)
        {
            alarmList.Visibility = Visibility.Hidden;
            buttonEditAlarm.Visibility = Visibility.Hidden;
            buttonDeleteAlarm.Visibility = Visibility.Hidden;
            buttonSetAlarm.Visibility = Visibility.Hidden;
            canvasAlarmSet.Visibility = Visibility.Visible;
            editVal = true;
            Alarm alarm;
            String dates;
            string selectedAlarm = alarmList.SelectedItem.ToString();
            if (selectedAlarm == null) { return; }
            else
            {
                for (int n = 1; n <= alarmHandler.getAlarms().Length; n++)
                {
                    if (selectedAlarm.Contains("Alarm " + n))
                    {
                        alarm = alarmHandler.getAlarms()[n - 1];
                        editedAlarm = alarm;
                        messageBox.Text = alarm.getMessage();
                        textBoxHourEntry.Text = alarm.getHour().ToString();
                        textBoxMinuteEntry.Text = alarm.getMinute().ToString();
                        dates = alarm.getDays();
                        if (dates != "0000000")
                        {
                            if (dates[0] == '1') { checkBox_Sunday.IsChecked = true; }
                            if (dates[1] == '1') { checkBox_Monday.IsChecked = true; }
                            if (dates[2] == '1') { checkBox_Tuesday.IsChecked = true; }
                            if (dates[3] == '1') { checkBox_Wednesday.IsChecked = true; }
                            if (dates[4] == '1') { checkBox_Thursday.IsChecked = true; }
                            if (dates[5] == '1') { checkBox_Friday.IsChecked = true; }
                            if (dates[6] == '1') { checkBox_Saturday.IsChecked = true; }
                        }
                        comboBoxSounds.SelectedItem = alarm.getSound();
                    }
                }
            }
        }

        private void resetAlarmPanel()
        {
            messageBox.Text = "No Message Set";
            textBoxHourEntry.Text = "";
            textBoxMinuteEntry.Text = "";
            checkBox_Sunday.IsChecked = false;
            checkBox_Monday.IsChecked = false;
            checkBox_Tuesday.IsChecked = false;
            checkBox_Wednesday.IsChecked = false;
            checkBox_Thursday.IsChecked = false;
            checkBox_Friday.IsChecked = false;
            checkBox_Saturday.IsChecked = false;
            comboBoxSounds.SelectedIndex = 0;
        }
    }
}
