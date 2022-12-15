
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace StudiumHelper
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String pathDirectoryEventList = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "StudiumHelper" );
        private static String pathEventList = Path.Combine( pathDirectoryEventList, "eventList.json" );
        private static String pathNoteList = Path.Combine( pathDirectoryEventList, "noteList.json" );
        private StackPanel activePage;
        Page mainPage;
        Page toDoPage;
        Page schedulePage;
        Page addEvent;
        EventList eventList;
        public MainWindow( )
        {
            InitializeComponent( );

            CheckIfListsExist( );
            
            eventList = new EventList( );
            eventList.SortByDate( );


            mainPage = new MainPage( );
            MainPage.EventAddBtnClick += ChangePage;

            toDoPage = new ToDoPage( );

            schedulePage = new SchedulePage( );

            addEvent = new AddEvent( );

            NavigateToPage(mainPage);
            
            
        }

        private void OpenAddEvent( object sender, MouseButtonEventArgs e )
        {
            //actualPage = new addEvent();
            
            NavigateToPage( addEvent, sender );
        }

        private void OpenMainPage( object sender, MouseButtonEventArgs e )
        {
            //actualPage = new mainPage();
            NavigateToPage( mainPage, sender );
            
        }

        private void StackPanel_MouseEnter( object sender, MouseEventArgs e )
        {

        }

        private void OnLoad( object sender, EventArgs e )
        {
            //actualPage = new mainPage();
            
        }

        private void NavigateToPage( Page page )
        {
           
            ChangeActivePage( mainPageStp );
            frame.NavigationService.Navigate( page );
            page.UpdateLayout( );

        }
        private void NavigateToPage( Page page , Object sender )
        {

            ChangeActivePage( sender );
            frame.NavigationService.Navigate( page );
            page.UpdateLayout( );
            
        }

        private void ChangeActivePage( Object sender )
        {
            if( sender == null )
            {
                return;
            }
            StackPanel menuUI = sender as StackPanel;

            if ( menuUI != null )
            { 
                menuUI.Background = ( SolidColorBrush ) new BrushConverter( ).ConvertFrom( "#FF1D3770" ); 
            }

            if( activePage != null )
            {
                activePage.Background = Brushes.Transparent;
            }
            activePage = menuUI;
        }
        private void CheckIfListsExist( )
        {
            
            if ( !Directory.Exists( pathDirectoryEventList ))
            {
                 NotesList.isCreatedFirstTime = true;  
                 Directory.CreateDirectory( pathDirectoryEventList );
                
            }
            if (!File.Exists( pathEventList ))
            {
                var myFile = File.Create( pathEventList );
                myFile.Close( );
            }
            if ( !File.Exists( pathNoteList ))
            {
                var myFile = File.Create( pathNoteList );
                myFile.Close( );
            }

        }

        private void OpenSchedule( object sender, MouseButtonEventArgs e )
        {
            NavigateToPage( schedulePage, sender );
        }

        private void OpenToDo( object sender, MouseButtonEventArgs e )
        {

            NavigateToPage( toDoPage, sender );
        }

        private void ChangeWindowTitle( String title )
        {
            this.Title = title;
        }

        private void MouseEnter( object sender, MouseEventArgs e )
        {
            StackPanel menuUI = sender as StackPanel;
            if( menuUI != null )
            {
                (menuUI).Background = ( SolidColorBrush )new BrushConverter( ).ConvertFrom( "#FF1D3770" );
                
            }
          
        }

        private void MouseLeaveStackPanel(object sender, MouseEventArgs e)
        {
            StackPanel menuUI = sender as StackPanel;
            if ( menuUI != null && menuUI != activePage )
            {
                (menuUI).Background = Brushes.Transparent;

            }
        }

        private void ChangePage( object sender, EventArgs e )
        {
            NavigateToPage( addEvent, stckpnl_add);
        }


    }
}
