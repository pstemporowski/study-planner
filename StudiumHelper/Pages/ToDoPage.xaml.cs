using StudiumHelper.CustomEventArgs;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudiumHelper
{
    /// <summary>
    /// Interaktionslogik für ToDoPage.xaml
    /// </summary>
    public partial class ToDoPage : Page
    {
        private static bool canCreate = true;
        NotesList noteList = new NotesList();
        EventList eventList = new EventList();
        System.Threading.Timer threadTimer;
        public Grid mouseWasDownOn;
        private double positionInElementX;
        private double positionInElementY;
        private int lastZIndex = 0;
        private Grid lastClickedGrid;
        private Grid lastCreatedGrid;
        private bool createdGridSaved = true;
        private String titleBeforeFocus;
        private bool createdNewMemo = false;
        private String txbBoxBeforeFocus;
        private String txtBoxChanged = "";

        public ToDoPage()
        {
            InitializeComponent();
            EventList.EventAdded += NewEventElement;

        }

        private void InitializedWindow(object sender, EventArgs e)
        {
            //noteList.AddEventsToNote(eventList.GetList());
            InitializeStandardEvents( noteList.getList( ));
        }

        public void NewEventElement(object sender, EventAddedArgs e)
        {
            noteList.AddNote(e.Data.name, 100, 100);
            InitializeNote(100, 100, e.Data.name);
        }

        private void InitializeNote(double x, double y, String title)
        {
            InitializeNote(x, y, title, 0);
        }

        private void InitializeNote(double x, double y, String title, int zIndex)
        {
            InitializeNote(x, y, title, zIndex, null);
        }

        private void InitializeNote(double x, double y, String title, int zIndex, List<String> memoList)
        {

            
            createdGridSaved = false;
            Grid grid = new Grid();
            Border border = new Border();
            StackPanel stackPanel = new StackPanel();
            
            CheckBox checkBox = new CheckBox();
            TextBox label = new TextBox();
            Button button = new Button();
            button.Style = getStyle("addButton");
            label.Style = getStyle("titleBox");
            TextBox txtBox = new TextBox();
            txtBox.Style = getStyle("checkBoxText");

            grid.Style = getStyle("noteGrid");
            grid.Name = "noteGrid";
            grid.MinHeight = 150;
            grid.MinWidth = 200;
            grid.MouseDown += new MouseButtonEventHandler(OnMouseDown);
            grid.Margin = new Thickness(x, y, 10, 10);
            Grid.SetZIndex(grid, zIndex);

            button.Content = "+";

            button.Click += new RoutedEventHandler(OnButtonClick);
            //grid.Children.Add(stackPanelHorizontal);
            stackPanel.Style = getStyle("notePanel");

            label.Text = title;
            label.LostFocus += new RoutedEventHandler(labelToCheck_LostFocus);
            label.GotFocus += new RoutedEventHandler(labelToCheck_GotFocus);
            label.Name = "labelToCheck";

            checkBox.Click += checkBox_CheckedChanged;

            gridToDo.Children.Add(grid);
            grid.Children.Add(border);
            grid.Children.Add(stackPanel);
            
            //BitmapImage image = new BitmapImage(new Uri("/MyProject;component/Icons/down.png", UriKind.Relative));
            
            //border.Child = stackPanel;
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(button);

            if ( !( memoList is null ))
            {
                foreach (String memo in memoList)
                {
                    Console.WriteLine("Hinzufügen von Memo");
                    InitializeCheckBox( stackPanel, memo );
                }
            }

            Button buttonToDelete = InitializeNoteDeleteButton();
            grid.Children.Add(buttonToDelete);
            lastCreatedGrid = grid;
            noteList.SaveJson();
        }
        private Button InitializeNoteDeleteButton()
        {
            Button buttonToDelete = new Button
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,

                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Color.FromRgb(1, 1, 1)) { Opacity = 0.0 },
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,

                Content = new Image
                {
                    Source = new BitmapImage(new Uri("/Icons/deleteButton.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            buttonToDelete.Click += new RoutedEventHandler(buttonToDelete_Click);
            return buttonToDelete;
        }
        
        void buttonToDelete_Click( Object sender, RoutedEventArgs e )
        {
            Button button = sender as Button;
            RemoveNote( button.Parent as Grid );
        }

        private void RemoveNote( Grid grid )
        {
            noteList.Remove( noteList.FindNoteByTitle( noteList.getTitleItemOfGrid( grid ).Text ));

            gridToDo.Children.Remove(grid);
        }

        void labelToCheck_GotFocus(object sender, RoutedEventArgs e)
        {

            titleBeforeFocus = (sender as TextBox).Text;
        }

        void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            canCreate = false;
            txbBoxBeforeFocus = (sender as TextBox).Text;
        }

        void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox memo_txb = sender as TextBox;
            if (memo_txb is null) Console.WriteLine("Memo is null");
            Grid grid = ((memo_txb.Parent as StackPanel).Parent as StackPanel).Parent as Grid;
            TextBox title = noteList.getTitleItemOfGrid(grid);
            Note note = noteList.FindNoteByTextBox(title);

            canCreate = true;
            if ( createdNewMemo )
            {
                note.AddMemo(memo_txb.Text);
                
                if(noteList is null)
                {
                    Console.WriteLine("Note has been not found");
                }
                else
                {
                    Console.WriteLine("Note has been found");
                }
                createdNewMemo = false;
            }else
            {
                
                note.ChangeMemo( txbBoxBeforeFocus, memo_txb.Text );
                Console.WriteLine( "Memo has been changed" );
                txtBoxChanged = memo_txb.Text;
                
            }
        }

        //private void check
        void labelToCheck_LostFocus( object sender, RoutedEventArgs e )
        {
            TextBox titleTxB = sender as TextBox;
            //Console.WriteLine("TitleTxb beim Focuslost: " + titleTxB.Text);
            Grid grid = (titleTxB.Parent as StackPanel).Parent as Grid;
            
            if ( sender == null )
                            return;
           
            Note note = noteList.FindNoteByTitle(titleTxB.Text);
            String titleTxbValue = titleTxB.Text;
            
            //Namesfindung
            if ( note != null && !titleBeforeFocus.Equals(titleTxbValue))
            {
                Console.WriteLine("Ein gefundenes NoteTitle: " + note.title);
              
                int counter = 1;
                titleTxbValue = titleTxbValue + counter;

                while (note != null)
                {
                    titleTxbValue = titleTxbValue.Substring(0, (titleTxbValue.Length - ((int) Math.Log10(counter) + 1))) + counter;
                    counter++;
                    note = noteList.FindNoteByTitle(titleTxbValue);
                    //Console.WriteLine("Note gefunden!");
                }
            } else if( lastCreatedGrid != null )
            {
                if (lastCreatedGrid.Equals(grid))
                {
                    Console.WriteLine("Note has been added " + lastCreatedGrid.Equals(grid));

                    noteList.AddNote(titleTxB.Text, grid.Margin.Left, grid.Margin.Top, lastZIndex++);
                    lastCreatedGrid = null;
                    return;
                }
            }

            titleTxB.Text = titleTxbValue;
            //note.title = titleTxbValue;
            noteList.ChangeTitle(titleBeforeFocus, titleTxbValue);
            titleTxB.UpdateLayout();

            
            //Console.WriteLine("TitleBeforeFocus : " + titleBeforeFocus);
        }

        
        private Style getStyle( String style )
        {
            return Application.Current.FindResource( style ) as Style;
        }

        private void checkBox_CheckedChanged( object sender, System.EventArgs e )
        {
            SetCheckBoxValue( sender );
        }
        private void SetCheckBoxValue( object sender )
        {

            if ( sender == null )
            {
                return;
            }

            CheckBox checkBox = sender as CheckBox;
            StackPanel stackPanelHorizontal = checkBox.Parent as StackPanel;
            TextBox textBox = stackPanelHorizontal.Children[1] as TextBox;

            StackPanel stackPanel = stackPanelHorizontal.Parent as StackPanel;
            Grid grid = stackPanel.Parent as Grid;
            TextBox title = VisualTreeHelper.GetChild(stackPanel, 0) as TextBox;
            DependencyObject child = VisualTreeHelper.GetChild(stackPanelHorizontal, 1 );
            TextBox label = child as TextBox;

            if ( checkBox.IsChecked.HasValue )
            {
                if ( checkBox.IsChecked.Value == true )
                {

                    Keyboard.ClearFocus();
                    //Task.WaitAll();
                    Note note = noteList.FindNoteByTextBox( title );
                    
                    if (textBox.Text.Equals(txtBoxChanged))
                    {
                        Console.WriteLine("Löschen Before Focus:" + txtBoxChanged);

                        note.RemoveSingleMemo( txtBoxChanged );
                        noteList.SaveJson();
                    }
                    else
                    {
                        Console.WriteLine("Löschen von Element: " + textBox.Text);

                        note.RemoveSingleMemo(textBox.Text);
                        noteList.SaveJson();
                    }
                    
                    stackPanel.Children.Remove( stackPanelHorizontal );
                    label.Foreground = Brushes.Gray;
                    label.TextDecorations = TextDecorations.Strikethrough;
                }
                else
                {
                    label.Foreground = Brushes.Black;
                    label.TextDecorations = null;
                }
            }
        }
        private void InitializeStandardEvents( List<Note> noteList )
        {
            if( NotesList.isCreatedFirstTime )
            {
                List<String> memos = new List<String>
                {
                    "Objekte können freiwillig verschoben werden indem Sie auf einer leeren Stelle vom Objekt mit der Linken Maustaste halten",
                    "Neue Objekte können durch das anklicken vom leeren Raum erstellt werden",
                    "Damit die Aufgaben in der TODO-Liste auf der Startseite angezeigt werden muss der Name komplett übereinstimmen"
                };
                Note note = new Note("Hint", memos);
                InitializeNote(300, 300, note.title, 0, memos);
            }
            foreach (Note note in noteList)
            {
                InitializeNote(note.x, note.y, note.title, note.zIndex, note.memoList);
                
            }
        }


        private String XYToMargin(int x, int y) { 
            return x +" " + y + " 0 0";
        }

        void OnButtonClick( object sender, RoutedEventArgs e )
        {
            //Console.WriteLine("Hinzufügen von Note");
            Button button = sender as Button;
            StackPanel stackPanel = button.Parent as StackPanel;

            // Console.WriteLine("Hinzufügen von Note");
            if( stackPanel != null )
            {
                AddCheckBox( stackPanel );
            }

            createdNewMemo = true;
        }

        private void AddCheckBox( StackPanel stackPanel ) {
            
            Button button = StaticHelperClass.GetChildOfType<Button>( stackPanel );
            stackPanel.Children.RemoveAt( stackPanel.Children.Count - 1 );
            
            if ( button is null )
            {
                Console.WriteLine("Button has been not Found");
            }

            ImmediateUpdateSourceTextBox txtBox = new ImmediateUpdateSourceTextBox();
            txtBox.Style = getStyle("checkBoxText");
            txtBox.GotFocus += new RoutedEventHandler(txtBox_GotFocus);
            txtBox.LostFocus += new RoutedEventHandler(txtBox_LostFocus);
            CheckBox checkBox = new CheckBox();

            StackPanel stackPanelHorizontal = new StackPanel();
            stackPanelHorizontal.Orientation = Orientation.Horizontal;
            stackPanelHorizontal.Children.Add(checkBox);
            checkBox.Click += checkBox_CheckedChanged;
            stackPanelHorizontal.Children.Add(txtBox);
            stackPanel.Children.Add(stackPanelHorizontal);

            stackPanel.Children.Add(button);
            txtBox.Focus();
            this.UpdateLayout();
        }

        private void InitializeCheckBox(StackPanel stackPanel, String input)
        {
            Button button = StaticHelperClass.GetChildOfType<Button>( stackPanel );
            stackPanel.Children.RemoveAt( stackPanel.Children.Count - 1 );

            if (button is null)
            {
                Console.WriteLine( "Button has been not Found" );
            }

            ImmediateUpdateSourceTextBox txtBox = new ImmediateUpdateSourceTextBox();
            
            txtBox.Style = getStyle("checkBoxText");
            txtBox.GotFocus += new RoutedEventHandler(txtBox_GotFocus);
            txtBox.LostFocus += new RoutedEventHandler(txtBox_LostFocus);
            CheckBox checkBox = new CheckBox();

            StackPanel stackPanelHorizontal = new StackPanel();
            stackPanelHorizontal.Orientation = Orientation.Horizontal;
            stackPanelHorizontal.Children.Add(checkBox);
            checkBox.Click += checkBox_CheckedChanged;
            stackPanelHorizontal.Children.Add(txtBox);
            stackPanel.Children.Add(stackPanelHorizontal);

            txtBox.Text = input;
            stackPanel.Children.Add(button);
            this.UpdateLayout();
        }

        void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            if (createdGridSaved == false)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
                Keyboard.ClearFocus();
            }  

            canCreate = false;
            mouseWasDownOn = sender as Grid;

            DropShadowEffect dropShadow = new DropShadowEffect( );
            dropShadow.BlurRadius = 10;
            dropShadow.Color = Color.FromRgb(0, 0, 0);
            dropShadow.Opacity = 0.7;
            mouseWasDownOn.Effect = dropShadow;

            SetZIndexWithGrid( mouseWasDownOn );
            

            Point p = e.GetPosition( this );
            positionInElementX = p.X - mouseWasDownOn.Margin.Left;
            positionInElementY = p.Y - mouseWasDownOn.Margin.Top;

            threadTimer = new System.Threading.Timer(OnCallBack, null, 0, 10);
        }

        private void SetZIndexWithGrid( Grid grid )
        {
            int nextIndex = GetNextIndex(grid);
            Grid.SetZIndex(grid, nextIndex);
            noteList.setZIndex(grid, nextIndex);
        }

        private void setFocusOnTitle( Grid grid )
        {

            TextBox titleTxB = noteList.getTitleItemOfGrid(grid);
            titleTxB.Focus();
        }

        private void OnCallBack(object state)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {

                    Point p = Mouse.GetPosition(gridToDo);
                    p.X = p.X - positionInElementX;

                    p.Y = p.Y - positionInElementY;
                    if (p.X > 0 && p.Y > 0 && mouseWasDownOn != null)
                    {
                        mouseWasDownOn.Margin = new Thickness(p.X, p.Y, 0, 0);
                    }
                    else if (p.X < 0 && p.Y < 0 && p.X > -300 && p.X > -300)
                    {
                        mouseWasDownOn.Margin = new Thickness(2, 2, 0, 0);
                    }

                    else if (p.X < 0)
                    {
                        mouseWasDownOn.Margin = new Thickness(2, p.Y, 0, 0);
                    }
                    else if (p.Y < 0)
                    {
                        mouseWasDownOn.Margin = new Thickness(p.X, 2, 0, 0);
                    }
                    else
                    {
                        mouseWasDownOn.Margin = new Thickness(p.X + 1, p.Y + 1, 0, 0);
                        mouseWasDownOn.Effect = null;
                        threadTimer.Dispose();
                    }
                });
            }
            catch( TaskCanceledException e )
            {
                    Console.WriteLine( "Exception" );
            }
                
           
            
        }

        private int GetNextIndex( Grid grid )
        {

            if ( grid.Equals( lastClickedGrid ))
            {
                return lastZIndex;
            }
            
            lastClickedGrid = grid;

            if( lastZIndex + 2 >= int.MaxValue )
            {
                noteList.organizeZindex( );

                lastZIndex = noteList.getList().Count;
            }

            return lastZIndex++;
        }

        
        private void onClick( object sender, MouseButtonEventArgs e )
        {
            
            if ( mouseWasDownOn != null )
            {

                if ( mouseWasDownOn.Effect != null )
                {
                    mouseWasDownOn.Effect = null;
                }
            }
            
            if ( threadTimer != null )
            {
                TextBox textBox = mouseWasDownOn.GetChildOfType<TextBox>();

                if( textBox != null )
                {
                    // Console.WriteLine("Position changed on "+ textBox.Text + " X:" + mouseWasDownOn.Margin.Left + " Y:" + mouseWasDownOn.Margin.Top);
                    noteList.ChangePositionByTitle( textBox.Text, mouseWasDownOn.Margin.Left, mouseWasDownOn.Margin.Top );
                    noteList.SaveJson();
                }
                threadTimer.Dispose();
            }
            
            if( canCreate )
            {
                FrameworkElement s = sender as FrameworkElement;
                if (sender.Equals( gridToDo ) && !s.Name.Equals( "noteGrid" ))
                {
                    Point p = e.GetPosition( this );
                    
                    InitializeNote( p.X, p.Y, getFreeStandardName() );
                    if(lastCreatedGrid != null)
                    {
                        setFocusOnTitle(lastCreatedGrid);
                    }
                }

            }
            else
            {
                canCreate = true;
            }
        }

        private void ContentOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (createdGridSaved == false)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
                Keyboard.ClearFocus();
            }
        }

        private String getFreeStandardName( )
        {
            String outputString = "Standard";
            int counter = 1;
            if(noteList.getList().Count != 0)
            {
                while (!(checkIfNoteTitleIsFree(outputString)))
                {
                    if (counter == 1)
                    {
                        outputString += counter;
                    }
                    else
                    {

                        outputString = outputString.Substring(0, outputString.Length - (int)(Math.Floor(Math.Log10(counter - 1)) + 1)) + counter;
                    }
                    counter++;
                }
            }
            return outputString;
        } 

        private bool checkIfNoteTitleIsFree(String name)
        {
            return noteList.FindNoteByTitle(name) == null;
        }
    }
}
