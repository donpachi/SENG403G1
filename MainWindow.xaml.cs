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
using System.Collections.ObjectModel;

namespace SENG403
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni;
        SoundModule sound = new SoundModule();         // Base sound module to be copied into each alarm
        ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();     // Acquire the collection of timezones (from system)
        string currentTimeZone = TimeZone.CurrentTimeZone.StandardName;     // Standard name of the current timezone
        int currentTimeZoneIndex = -1;                                      // An INDEX which points to an element in the timeZones collection
        string nativeTimeZone = TimeZone.CurrentTimeZone.StandardName;
        int nativeTimeZoneIndex = -1;
        bool nativeInDSaving = false;

        AlarmHandler alarmHandler = new AlarmHandler(); // The main alarm handler object for this program
        Double snoozeTime = 0;
        Boolean editVal = false;
        Alarm editedAlarm;

        /// <summary>
        /// Main window of the system.
        /// </summary>
        public MainWindow()
        {
            MissedAlarmHandler.MissedAlarm += handleMissedAlarm;

            InitializeComponent();
            CreateTrayIcon();
            date_label.Content = Clock.GetDate().Replace(',', ' ');

            this.KeyUp += MainWindow_KeyUp;

            // Populate sounds comboBox with available .wav files in Sound directory
            string[] availableSounds = sound.getSounds();
            for (int i = 0; i < availableSounds.Length; i++)
            {
                comboBoxSounds.Items.Add(availableSounds[i]);
            }

            updateAlarmsList();

            // Populate the timezones combobox with all available timezones
            // Update the comboBox with the current timezone so that on startup it is not blank
            for (int i = 0; i < timeZones.Count; i++)
            {
                comboBoxTimeZone.Items.Add(timeZones[i]);
                string stdName = timeZones[i].StandardName;

                //only go into here if the native time zone is found
                if (stdName.Equals(currentTimeZone))
                {
                    comboBoxTimeZone.Text = timeZones[i].ToString();
                    currentTimeZoneIndex = i;
                    nativeTimeZoneIndex = currentTimeZoneIndex;
                }
            }

            // Boolean flag to see if native time zone is currently in daylight saving time
            nativeInDSaving = timeZones[nativeTimeZoneIndex].IsDaylightSavingTime(Clock.Now());

            Alarm.onRing += OnAlarmRing;
            Clock.UpdateTime += UpdateTimeLabel;
        }

        /// <summary>
        /// Handle a missed alarm event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleMissedAlarm(object sender, EventArgs e)
        {
            // Show the screen that says missed alarm
            missedAlarmNotification.Visibility = Visibility.Visible;
            dismissButton.Visibility = Visibility.Visible;
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

                String nextAlarm = "Alarm " + (i + 1) + ": " + theAlarms[i].getDateTime();
                // Only add the alarm if it isn't in the list already
                if (!alarmList.Items.Contains(nextAlarm))
                    if (daysConverted.Equals("Su. Mo. Tu. We. Th. Fr. Sa. "))
                    {
                        daysConverted = "Daily";
                    } else if (daysConverted.Equals(""))
                    {
                        daysConverted = "Today Only";
              
                    }
                    alarmList.Items.Add(nextAlarm + "\nDays: " + daysConverted + "\n");
            }
        }

        /// <summary>
        /// Event method to trigger when the wnidow changes states
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }

        /// <summary>
        /// Method to handle window minimzation to tray.
        /// </summary>
        private void MinimizeWindow()
        {
            if (this.WindowState == WindowState.Maximized || this.WindowState == WindowState.Normal)
            {
                this.Hide();
                this.WindowState = WindowState.Minimized;
                ClockUC.DisableAnimations();
            }
        }

        /// <summary>
        /// Method to handle window maximation to screen.
        /// </summary>
        private void MaximizeWindow()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Show();
                this.WindowState = WindowState.Maximized;
                ClockUC.EnableAnimations();
            }
        }



        /// <summary>
        /// Once the window has been closed, update the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            alarmHandler.populateSettings();
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// When an alarm is ringing, handle UI and logic for a ringing alarm.
        /// </summary>
        private void OnAlarmRing()
        {
            MaximizeWindow();
            Console.WriteLine("ring");
            textBlock.Text = alarmHandler.getCurrentAlarm().getMessage();
            textBlock.Visibility = Visibility.Visible;
            buttonDismissAlarm.Visibility = Visibility.Visible;
            buttonSnoozeAlarm.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Update the time label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void UpdateTimeLabel(object sender, String args)
        {
            date_label.Content = args.Replace(',', ' ');
        }

        /// <summary>
        /// Create a tray icon of the alarm.
        /// </summary>
        private void CreateTrayIcon()
        {
            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new Icon("Resource/Main.ico");
            ni.Visible = true;
            ni.DoubleClick += (s, a) =>
            {
                this.Show();
                MaximizeWindow();
            };
            ni.Text = "Alarm Clock";
        }

        /// <summary>
        /// Close the window if the user pressed escape.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.B)
            {
                ClockUC.digital_canvas.Visibility = Visibility.Visible;
                ClockUC.analog_canvas.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Collect all entered values in UI and enter them to create an alarm object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickButtonConfirm(object sender, RoutedEventArgs e)
        {
            SoundModule newSound = new SoundModule();

            // convert the hour and minute entries to integers so that they may be used for
            // the alarm's DateTime
            if (textBoxHourEntry.Text != "" && textBoxMinuteEntry.Text != "")
            {
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
                DateTime theTime = new System.DateTime(Clock.Now().Year, Clock.Now().Month,
                    Clock.Now().Day, theHour, theMinute, 0);

                // set the sound for the alarm being created (selected from comboBox)
                string selectedSound = comboBoxSounds.Text;
                newSound.setSound(selectedSound);

                // create new alarm object
                alarmHandler.setNewAlarm(theTime, alarmDaysChecked, newSound, message);

                if (editVal == true)
                {
                    alarmHandler.deleteAlarm(editedAlarm);
                    editVal = false;
                }
                //update the UI with all alarms in the alarm arraylist
                updateAlarmsList();
            }

            resetAlarmPanel();

            if (alarmHandler.alarmList.Count != 0)
            {
                buttonEditAlarm.Visibility = Visibility.Visible;
                buttonDeleteAlarm.Visibility = Visibility.Visible;
            }
            else
            {
                buttonEditAlarm.Visibility = Visibility.Hidden;
                buttonDeleteAlarm.Visibility = Visibility.Hidden;
            }
            canvasAlarmSet.Visibility = Visibility.Hidden;
            alarmList.Visibility = Visibility.Visible;
            buttonSetAlarm.Visibility = Visibility.Visible;
            buttonEditAlarm.Visibility = Visibility.Visible;
            buttonDeleteAlarm.Visibility = Visibility.Visible;
            comboBoxTimeZone.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// If user presses cancel, update UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickButtonCancel(object sender, RoutedEventArgs e)
        {
            resetAlarmPanel();
            canvasAlarmSet.Visibility = Visibility.Hidden;
            alarmList.Visibility = Visibility.Visible;
            time_canvas.Visibility = Visibility.Visible;
            buttonSetAlarm.Visibility = Visibility.Visible;
            comboBoxTimeZone.Visibility = Visibility.Visible;
        }

        #region Radio Buttons
        // Selected the 1 minute snooze radio button for an alarm
        private void select1Min(object sender, RoutedEventArgs e) { snoozeTime = 1; }

        // Selected the 2 minute snooze radio button for an alarm
        private void select2Mins(object sender, RoutedEventArgs e) { snoozeTime = 2; }

        // Selected the 5 minute snooze radio button for an alarm
        private void select5Mins(object sender, RoutedEventArgs e) { snoozeTime = 5; }

        // Selected the 10 minute snooze radio button for an alarm
        private void select10Mins(object sender, RoutedEventArgs e) { snoozeTime = 10; }

        // Selected the 15 minute snooze radio button for an alarm
        private void select15Mins(object sender, RoutedEventArgs e) { snoozeTime = 15; }

        // Selected the 30 minute snooze radio button for an alarm
        private void select30Mins(object sender, RoutedEventArgs e) { snoozeTime = 30; }

        // Selected the 60 minute snooze radio button for an alarm
        private void select60Mins(object sender, RoutedEventArgs e) { snoozeTime = 60; }
        #endregion Radio Buttons


        /// <summary>
        /// Listener for when snooze is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickSnooze(object sender, RoutedEventArgs e)
        {
            alarmHandler.getCurrentAlarm().snooze(snoozeTime);
            alarmHandler.currentAlarm = null;
            textBlock.Visibility = Visibility.Hidden;
            buttonDismissAlarm.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Listener for when dismiss is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickDismiss(object sender, RoutedEventArgs e)
        {
            Alarm ringingAlarm = alarmHandler.getCurrentAlarm();
            ringingAlarm.setRinging(false);
            textBlock.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
            buttonDismissAlarm.Visibility = Visibility.Hidden;
            buttonSnoozeAlarm.Visibility = Visibility.Hidden;
            alarmHandler.endAlarm(ringingAlarm);
            alarmHandler.currentAlarm = null;
            updateAlarmsList();
        }

        #region Display functions

        /// <summary>
        /// Listener for when Analog/digital is pressed. Toggle to the selected display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayModeToggle(object sender, RoutedEventArgs e)
        {
            if (ClockUC.analog_canvas.IsVisible)
            {
                toggleDisplayButton.Content = "Analog";
                ClockUC.analog_canvas.Visibility = Visibility.Hidden;
                ClockUC.digital_canvas.Visibility = Visibility.Visible;
            }
            else
            {
                toggleDisplayButton.Content = "Digital";
                ClockUC.analog_canvas.Visibility = Visibility.Visible;
                ClockUC.digital_canvas.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Listener for the set alarm button. Hides the time display base canvas and makes the set alarm canvas visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gotoSetAlarm(object sender, RoutedEventArgs e)
        {
            alarmList.Visibility = Visibility.Hidden;
            buttonEditAlarm.Visibility = Visibility.Hidden;
            buttonDeleteAlarm.Visibility = Visibility.Hidden;
            buttonSetAlarm.Visibility = Visibility.Hidden;
            comboBoxTimeZone.Visibility = Visibility.Hidden;
            canvasAlarmSet.Visibility = Visibility.Visible;
            comboBoxTimeZone.Visibility = Visibility.Hidden;
            buttonConfirmAlarm.IsEnabled = false;

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion


        /// <summary>
        /// Listener for selecting an alarm item from the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handle edit alarm button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editClick(object sender, RoutedEventArgs e)
        {
            alarmList.Visibility = Visibility.Hidden;
            buttonEditAlarm.Visibility = Visibility.Hidden;
            buttonDeleteAlarm.Visibility = Visibility.Hidden;
            buttonSetAlarm.Visibility = Visibility.Hidden;
            comboBoxTimeZone.Visibility = Visibility.Hidden;
            canvasAlarmSet.Visibility = Visibility.Visible;
            comboBoxTimeZone.Visibility = Visibility.Hidden;
            editVal = true;
            Alarm alarm;
            String dates;
            string selectedAlarm = "";

            if(alarmList.SelectedItem != null)
                selectedAlarm = alarmList.SelectedItem.ToString();
            

            if (selectedAlarm.Equals("")) { return; }
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

        /// <summary>
        /// Reset alarm panel.
        /// </summary>
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

        /// <summary>
        /// Handle a dismiss button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the screen that says missed alarm
            missedAlarmNotification.Visibility = Visibility.Hidden;
            dismissButton.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Listener for when the time zone combobox is closed (hence a time zone is selected)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TimeZoneChangeEvent(double offset);
        public static event TimeZoneChangeEvent OnTimeZoneChange;
        private void TimeZoneChanged(double offset)
        {
            OnTimeZoneChange(offset);
        }

        /// <summary>
        /// Handle drop-down close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropDownClosed(object sender, EventArgs e)
        {
            //if time zone selection is different, update the time and the current time zone
            if (!comboBoxTimeZone.SelectedItem.Equals(timeZones[currentTimeZoneIndex]))
            {
                currentTimeZoneIndex = comboBoxTimeZone.SelectedIndex;
                currentTimeZone = timeZones[currentTimeZoneIndex].StandardName;

                int newOffset;
                //if the currently selected time zone has the same offset as the native time zone
                if (timeZones[currentTimeZoneIndex].BaseUtcOffset.Hours == timeZones[nativeTimeZoneIndex].BaseUtcOffset.Hours)
                {
                    newOffset = 0;
                    if (nativeInDSaving)
                        newOffset += 1;
                }
                else
                {
                    int t = timeZones[currentTimeZoneIndex].BaseUtcOffset.Hours;
                    int t_native = timeZones[nativeTimeZoneIndex].BaseUtcOffset.Hours;
                    newOffset = (t_native - t) * -1;
                }

                //subtract an hour off of the offset if daylight saving time is in effect
                if (nativeInDSaving)
                {
                    newOffset -= 1;
                }

                ClockUC.HourOffset = newOffset;
                TimeZoneChanged(newOffset); //changed this to event so the clock can update the hands itself
            }
        }


        /// <summary>
        /// Listener for hour entry box lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HourEntryLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                short min = Convert.ToInt16(textBoxMinuteEntry.Text);
                short hour = Convert.ToInt16(textBoxHourEntry.Text);
                if ((min < 60 && min >= 0) && (hour <= 12 && hour >= 0))
                    buttonConfirmAlarm.IsEnabled = true;
            }
            catch (InvalidCastException)
            {
                buttonConfirmAlarm.IsEnabled = false;
            }
            catch (FormatException)
            {
                buttonConfirmAlarm.IsEnabled = false;
            }
        }

        private void MinuteEntryLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                short min = Convert.ToInt16(textBoxMinuteEntry.Text);
                short hour = Convert.ToInt16(textBoxHourEntry.Text);
                if ((min < 60 && min >= 0) && (hour <= 23 && hour >= 0))
                    buttonConfirmAlarm.IsEnabled = true;
            }
            catch (InvalidCastException)
            {
                buttonConfirmAlarm.IsEnabled = false;
            }
            catch (FormatException)
            {
                buttonConfirmAlarm.IsEnabled = false;
            }
        }
    }
}
