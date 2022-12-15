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
using System.Windows.Shapes;

namespace StudiumHelper.Windows
{
    /// <summary>
    /// Interaktionslogik für EditWindows.xaml
    /// </summary>
    /// 
  
    public partial class EditWindows : Window
    {
        private Event thisEvent;
        private EventList eventList = new EventList( );
         public EditWindows( )
        {
            InitializeComponent( );
        }

        public EditWindows( Event thisEvent )
        {
           
            InitializeComponent( );

            this.thisEvent = thisEvent;
            txtbx_name.Text = thisEvent.name;
            txtbx_prof.Text = thisEvent.prof;
            txtbx_room.Text = thisEvent.room;

            tmpck_start.Text = DatetimeToString( thisEvent.hour );
            tmpck_end.Text = DatetimeToString( thisEvent.end );
            SetRightRadioButton( );
        }

        private void click_abort( object sender, RoutedEventArgs e )
        {
            this.Close();
        }

        private String DatetimeToString( DateTime dateTime )
        {
            return dateTime.Hour + ":" + dateTime.Minute;
        }
        
        private RadioButton GetCheckedRadioButton ( )
        {
            DependencyObject [ ] radios = stpa_radiobuttons.GetChildrenOfType< RadioButton >( ).ToArray( );

            foreach( RadioButton radio in radios )
            {
                if( radio.IsChecked == true )
                {
                    return radio;
                }
            }
            return null;
        }

        private DayOfWeek GetDayOfWeek( RadioButton radioButton )
        {
            String strNumber = radioButton.Name.Substring( radioButton.Name.Length - 1 );
            int dayOfWeek = Int32.Parse( strNumber ) ;

            return ( DayOfWeek ) dayOfWeek;
            
        }
        private void SetRightRadioButton()
        {
           
            switch ((( int ) thisEvent.dayOfWeek) - 1 )
            {
                case 0: 
                  btn1.IsChecked = true;
                  break;
                case 1:
                    btn2.IsChecked = true;
                    break;
                case 2:
                    btn3.IsChecked = true;
                    break;
                case 3:
                    btn4.IsChecked = true;
                    break;
                case 4:
                    btn5.IsChecked = true;
                    break;
                case 5:
                    btn6.IsChecked = true;
                    break;

            }
        }

        private void click_save( object sender, RoutedEventArgs e )
        {
            if(ValidInput())
            {
                thisEvent.name = txtbx_name.Text;
                thisEvent.prof = txtbx_prof.Text;
                thisEvent.room = txtbx_room.Text;

                RadioButton radio = GetCheckedRadioButton();
                DayOfWeek dayOfWeek = GetDayOfWeek(radio);

                thisEvent.end = GetNextDate(dayOfWeek, DateTimeWithoutSeconds(tmpck_end.Value));
                thisEvent.dayOfWeek = dayOfWeek;

                DateTime start = DateTimeWithoutSeconds(tmpck_start.Value);
                thisEvent.hour = new DateTime(thisEvent.end.Year, thisEvent.end.Month, thisEvent.end.Day, start.Hour, start.Minute, 0);
                eventList.ListItemChanged();


                this.Close();
            }else
            {
                SetErrorLabel("Es ist ein Fehler aufgetreten");
            }
            
        }

        private void SetErrorLabel(String errorCode)
        {
            ErrorLabel.Content = errorCode;
        }
        private DateTime GetNextDate( DayOfWeek dayOfWeek, DateTime d1 )
        {
           
            if( dayOfWeek == d1.DayOfWeek && ( d1 - DateTime.Now ).TotalSeconds < 0)
            {
                return d1.AddDays( 7 );
            }

            while ( d1.DayOfWeek != dayOfWeek )
            {
                d1 = d1.AddDays( 1 );
            }
            
            return d1;
        }

        private DateTime DateTimeWithoutSeconds( DateTime? dateTime )
        {


            return new DateTime( dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, 0 );
        }

        private bool ValidInput()
        {
            if(txtbx_name.Text.TrimEnd().Equals("") || txtbx_name == null)
            {
                SetErrorLabel("Bitte tragen Sie einen Namen ein");
                return false;
            }

            if (eventList.Find(txtbx_name.Text))
            {
                SetErrorLabel("Es existiert bereits so ein Name");
                return false;
            }

            if (tmpck_start == null)
                return false;
            if (tmpck_end == null)
                return false;

            DateTime start = DateTimeWithoutSeconds(tmpck_start.Value);
            DateTime end = DateTimeWithoutSeconds(tmpck_end.Value);
            
            if ((end - start).TotalMinutes <= 0)
                return false;
            RadioButton radioButton = GetCheckedRadioButton();

            if (radioButton == null)
                return false;

            return true;
        }
    }
}
