
using StudiumHelper.Windows;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudiumHelper
{
    /// <summary>
    /// Interaktionslogik für addEvent.xaml
    /// </summary>
    public partial class AddEvent : Page
    {
        EventList eventList = new EventList( );

        private Border unfoldedEvent;
        public bool changed { get; set; }
        public AddEvent( )
        {
            
            InitializeComponent( );
            UpdateEvents( );
            EventList.ListSizeChanged += UpdateList;
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UpdateList( object sender, EventArgs e )
        {
            UpdateEvents( );
        }
        private void UpdateEvents(  )
        {
            allEventsItemsControl.ItemsSource = null;
            var events = eventList.GetList();

            if (eventList.GetList().Count > 0)
                allEventsItemsControl.ItemsSource = events;
            
        }
        private void textBoxEventName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String name = textBoxEventName.Text;
            DateTime start;
            DateTime end;

            if ( EventTimePicker.Value != null || EventTimePickerEnd.Value != null)
            {
                start = dateTimeWithoutSeconds(EventTimePicker.Value);
                end = dateTimeWithoutSeconds(EventTimePickerEnd.Value);
            }else
            {
                SetErrorLabel( "Bitte tragen Sie die Uhrzeiten von ihren Veranstaltungen ein " );
                return;
            }
            
            DayOfWeek eventDay = (DayOfWeek)GetChekedButton( );
            if ( ValidInput( name, start, end, eventDay ))
            {
                start = GetNextDate(eventDay, start);
                end = new DateTime(start.Year, start.Month, start.Day, end.Hour, end.Minute, 0);

                eventList.AddEvent(new Event(name, textBoxProf.Text, textBoxRoom.Text, eventDay, start, end));
                Error_lbl.Visibility = Visibility.Hidden;
            }
            
            //MessageBox.Show(textBoxEventName.Text + textBoxProf.Text + textBoxRoom.Text + (DayOfWeek)getChekedButton() + EventTimePicker.Value.ToString());
            UpdateEvents();
            this.UpdateLayout();


        }

        private DateTime GetNextDate(DayOfWeek dayOfWeek, DateTime d1)
        {
            while(d1.DayOfWeek != dayOfWeek)
            {
                d1 = d1.AddDays(1); 
               
            }
            return d1;
        }
        private DateTime dateTimeWithoutSeconds(DateTime? dateTime)
        {


            return new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, 0);
        }
        private int GetChekedButton()
        {
            if (btn.IsChecked == true) return 1;
            else if (btn2.IsChecked == true) return 2;
            else if (btn3.IsChecked == true) return 3;
            else if (btn4.IsChecked == true) return 4;
            else if (btn5.IsChecked == true) return 5;
            else if (btn6.IsChecked == true) return 6;

            return -1;
        }

        private void SetErrorLabel(String errorCode)
        {
            Error_lbl.Visibility = Visibility.Visible;
            Error_lbl.Content = errorCode;

        }
        private void EventClick(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if( border == unfoldedEvent)
            {
                FoldEvent(border);
            }else
            {
                UnfoldEvent(sender as Border, 175, TimeSpan.FromSeconds(0.5));
            }
            
        }

        private void UnfoldEvent( Border border, int newWidth, TimeSpan duration )
        {
            if( unfoldedEvent != null )
            {
                Console.WriteLine("Folded Event");
                FoldEvent(unfoldedEvent);
                
            }
            Image image = StaticHelperClass.FindChild<Image>( border, "homeIcon" );

            StackPanel stackPanel = border.Child as StackPanel;
            stackPanel = stackPanel.Children[1] as StackPanel;
            AnimateBordersHeight( border, newWidth, duration );
            RotateImage( image, 180 );
            stackPanel.Visibility = Visibility.Visible;
           
        }
     
        private void FoldEvent( Border border )
        {
            AnimateBordersHeight(border, 40, TimeSpan.FromSeconds( 0.5 ));
            unfoldedEvent = null;
            Image image = StaticHelperClass.FindChild<Image>( border, "homeIcon" );
            RotateImage( image, 0 );
        }

        private void AnimateBordersHeight( Border dp, int newHeight, TimeSpan duration )
        {
            if ( dp == null ) return;

            DoubleAnimation animation = new DoubleAnimation( );
            animation.Duration = duration;
            animation.From = dp.ActualHeight;
            animation.To = newHeight;

            dp.BeginAnimation( StackPanel.HeightProperty, animation );
            unfoldedEvent = dp;
            
        }
        private void RotateImage( Image image, int degrees )
        {
            image.RenderTransform = new RotateTransform( );
            TransformGroup group = new TransformGroup( );
            RotateTransform rotateTransform = new RotateTransform( degrees, image.ActualWidth/2, image.ActualHeight / 2 );
            TranslateTransform tt = new TranslateTransform( );

            group.Children.Add( rotateTransform );
            group.Children.Add( tt );
            
            image.RenderTransform = group;
        }

        private bool ValidInput( String name,  DateTime start, DateTime end, DayOfWeek? dayOfWeek )
        {
            if (name.Equals("") || name == null )         
            {
                SetErrorLabel( "Bitte tragen Sie den Namen der Veranstaltung ein" );

                return false;
            }

            if ( eventList.Find( name ))
            {
                SetErrorLabel( "Es existiert bereits so ein Name" );
                return false;

            }else if(( end - start ).TotalSeconds <= 0 )
            {
                SetErrorLabel( "Es ist ein Fehler bei einer Uhrzeit aufgetreten" );
                return false;

            }else if( dayOfWeek == null )
            {
                SetErrorLabel("Bitte wählen Sie einen Tag aus");
                return false;

            }

            return true;
        }

        private void DeleteEvent( object sender, RoutedEventArgs e )
        {
           
            DateTime d1 = GetHour(sender);
            DateTime d2 = GetEnd(sender);
            Event thisEvent = GetEvent( GetName( sender ), GetHour(sender), GetEnd(sender));

           eventList.Remove( thisEvent );
            eventList.OnListSizeChanged( EventArgs.Empty );

        }

        private void EditEvent(object sender, RoutedEventArgs e)
        {
            Window editWindow = new EditWindows( GetEvent( GetName( sender ), GetHour( sender ), GetEnd( sender )));
            
            editWindow.ShowDialog();

        }

        private String GetName( object sender )
        {
            // Hidden Label to get easy the name
            return ((( sender as Button ).Parent as StackPanel ).Children[ 0 ] as TextBox ).Text;
        }
        private DateTime GetHour(object sender)
        {
            // Hidden Label to get easy the name
            return DateTime.Parse( ((( sender as Button ).Parent as StackPanel).Children[1] as TextBox ).Text, CultureInfo.InvariantCulture);
        }
        private DateTime GetEnd(object sender)
        {
            // Hidden Label to get easy the name
            return DateTime.Parse((((sender as Button).Parent as StackPanel).Children[2] as TextBox).Text, CultureInfo.InvariantCulture);
        }

        private Event GetEvent( String name , DateTime hour, DateTime end)
        {
            foreach(Event nextEvent in eventList.GetList())
            {
                if ( nextEvent.name == name && hour.Equals(nextEvent.hour) && end.Equals(nextEvent.end))
                {
                    return nextEvent;
                }
            }
            return null;
        }
    }
    
}
