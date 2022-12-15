using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StudiumHelper
{
    /// <summary>
    /// Interaktionslogik für SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {
        private EventList eventlist;
        private Thickness margin = new Thickness(0, 0, 0, 0);
        private int itemsCount = 0;
        public SchedulePage()
        {
            InitializeComponent();
            eventlist = new EventList();
            EventList.ListSizeChanged += UpdateSchedule;
            InitializeSchedule();
        }

        private void UpdateSchedule(object Sender, EventArgs e)
        {
            RemoveAllSchedules( );
            InitializeSchedule( );
        }
        private void RemoveAllSchedules()
        {
            
            List<DependencyObject> stackPanels = StaticHelperClass.GetChildrenOfType<StackPanel>(dataGrid);
            if( stackPanels != null )
            {
                foreach(StackPanel stackPanel in stackPanels)
                {
                    dataGrid.Children.Remove( stackPanel );
                }
            }

            this.UpdateLayout( );
        }

        private void RemoveEvent( )
        {

        }
        private void InitializeSchedule( )
        {
            foreach ( Event thisEvent in eventlist.GetList( ))
            {
                AddEventDataGrid( thisEvent );
            }
        }

        private void AddEventDataGrid( Event thisEvent )
        {
            itemsCount++;
            
                int row = GetRowByHour( thisEvent.hour );
                int column = (int) thisEvent.dayOfWeek;

                Style style = Application.Current.FindResource( "eventPanel" ) as Style;
                
                StackPanel stackPanel = new StackPanel();

                stackPanel.Margin = new Thickness(0, GetTopMargin(thisEvent.hour), 0, GetDownMargin(thisEvent.end));
                stackPanel.Style = style;
                stackPanel.Background = ( SolidColorBrush ) new BrushConverter( ).ConvertFrom( "#34cfeb" );

                
                dataGrid.Children.Add( stackPanel );
                
                Label name = new Label( );
                Label room = new Label( );

                SetLabel( name, thisEvent.name );
                SetStyle( name, "nameLabel" );

                
                SetLabel( room, "Raum: " + thisEvent.room );
                SetStyle( room, "roomLabel" );

                stackPanel.Children.Add( name );
                stackPanel.Children.Add( room );
           
                Grid.SetRow( stackPanel, row );   
                Grid.SetColumn( stackPanel, column );
                Grid.SetRowSpan(stackPanel, GetSpan( thisEvent.hour, thisEvent.end ));
                
        }

        private void BorderMouseOver( object sender, EventArgs e )
        {
            (sender as Border).Padding = new Thickness(20);
           
        }

        private void BorderMouseLeave( object sender, EventArgs e )
        {
            (sender as Border).Padding = new Thickness(0);
        }
        private int GetSpan( DateTime start, DateTime end )
        {
            if((start - end).TotalMinutes == 0)
            {
                return 1;
            }

            if(start.Minute == 0 && end.Minute == 0)
            {
                return (end - start).Hours;
            }
            else
            {
                return (end - start).Hours + 1;
            }
            
        }

        private void SetStyle( Control ui, String style ) 
        {
            Style styleToPanel = Application.Current.FindResource(style) as Style;
            ui.Style = styleToPanel;
        }

        private int GetRowByHour(DateTime dateTime)
        {
            int counter;
            
            
            counter = dateTime.Hour - 7;

            //Console.WriteLine( counter );

            //setMargin(timeLeft);
            return counter;
        }

        private TimeSpan GetRowLength(int row) 
        {
            if (row % 3 == 0 && row == 9)
            {
                return TimeSpan.FromMinutes(35);
            }
            else if (row % 3 == 0)
            {
                return TimeSpan.FromMinutes(20);
            }
            else
            {
                return TimeSpan.FromMinutes(45);
            }
        }

        private double GetTopMargin( DateTime dateTime )
        {
           
            return (dateTime.Minute / 59.0) * 40;
        }

        private double GetDownMargin( DateTime dateTime )
        {

           
            double span = (dateTime.Minute / 59.0) * 40;

           
            if ( span == 0 )
            {
               
                return 0;
            }
            else
            {
         
                return 40 - span;
            }
            
        }

        private double getRowSize(int row)
        {
            if(row % 3 == 0 && row == 9)
            {
                return 40;
            }
            if (row % 3 == 0)
            {
                return 15;
            }
            else return 40;
        }

        private bool checkIfSpanBiggerThenZero(DateTime d1, DateTime d2)
        {
            if((d1 - d2).TotalMinutes > 0)
            {
                return true;
            }
            return false;
        }

        private void SetLabel(Label label, String input)
        {
            label.Content = input;
        }

        public void Update( )
        {
            if(itemsCount != eventlist.GetList( ).Count)
            {
                DeleteEvents();
                foreach( Event thisEvent in eventlist.GetList( ))
                {
                    AddEventDataGrid(thisEvent);
                }
            }
        }

        public void DeleteEvents()
        {
            dataGrid.Children.Clear();
        }
    }
}
