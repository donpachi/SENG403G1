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
        SoundModule sound = new SoundModule();

        public MainWindow()
        {
            InitializeComponent();
            this.KeyUp += MainWindow_KeyUp;
            time = new Time(minute_hand_image, second_hand_image, hour_hand_image, time_label);
            time.Start();
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
            if (e.Key == Key.B)
            {
                digital_canvas.Visibility = Visibility.Visible;
                analog_canvas.Visibility = Visibility.Visible;
            }
        }

        private void clickButtonConfirm(object sender, RoutedEventArgs e)
        {

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

        }

        // Listener for the Dismiss button
        private void clickDismiss(object sender, RoutedEventArgs e)
        {

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
