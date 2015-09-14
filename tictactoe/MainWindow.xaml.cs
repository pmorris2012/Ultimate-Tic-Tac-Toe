using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace tictactoe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Color filled = Color.FromArgb(0xFF, 0x89, 0xF8, 0xD8);
        public Color filledOrange = Color.FromArgb(0xFF, 0xFF, 0xCD, 0x70);
        private List<Grid> smallboards;
        private List<List<Image>> boardImages;
        private List<List<Rectangle>> boardRects;
        private int turnToDisplay;
        private Game game;
        private SmartAI AI;
        private Move lastMove;

        public MainWindow()
        {
            SplashScreen screen = new SplashScreen(
                new Random().Next(1,3) == 1 ? 
                "/Resources/blueo.png":"/Resources/redx.png");
            screen.Show(false);
            InitializeComponent();
            HashTables.Initialize();
            ResetButton1_Click(null,null);
            screen.Close(TimeSpan.MinValue);
        }

        private void InfoGridUpdate(bool turn)
        {
            string turnID = turn ? "O" : "X";
            InfoImage.Source = TryFindResource(turnID) as BitmapImage;
            if (AI != null)
                InfoBox.Text = turnID == "X" ? 
                    "X's Turn to Move..." : "Computer is thinking...";
            else
                InfoBox.Text = turnID + "'s Turn to Move...";
        }

        private void Board_Init()
        {
            smallboards = new List<Grid>(9);
            boardImages = new List<List<Image>>(10);
            boardRects = new List<List<Rectangle>>(9);
            for (int i = 0; i < 9; ++i)
            {
                smallboards.Add(new Grid());
                Grid.SetRow(smallboards[i], (int)Math.Floor((decimal)i / 3));
                Grid.SetColumn(smallboards[i], i % 3);

                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition c2 = new ColumnDefinition();
                c2.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition c3 = new ColumnDefinition();
                c3.Width = new GridLength(1, GridUnitType.Star);
                RowDefinition r1 = new RowDefinition();
                r1.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition r2 = new RowDefinition();
                r2.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition r3 = new RowDefinition();
                r3.Height = new GridLength(1, GridUnitType.Star);
                smallboards[i].ColumnDefinitions.Add(c1);
                smallboards[i].ColumnDefinitions.Add(c2);
                smallboards[i].ColumnDefinitions.Add(c3);
                smallboards[i].RowDefinitions.Add(r1);
                smallboards[i].RowDefinitions.Add(r2);
                smallboards[i].RowDefinitions.Add(r3);
                smallboards[i].ShowGridLines = true;
                
                Board.Children.Add(smallboards[i]);

                boardImages.Add(new List<Image>(9));
                boardRects.Add(new List<Rectangle>(9));
                for(int j = 0; j < 9; ++j)
                {
                    boardImages[i].Add(new Image());
                    boardRects[i].Add(new Rectangle());
                    boardImages[i][j].Stretch = Stretch.Uniform;
                    boardRects[i][j].Fill = new SolidColorBrush(
                        i % 2 == 0 ? filled:Colors.White);
                    boardRects[i][j].MouseLeftButtonDown += EmptyTile_MouseDown;
                    boardRects[i][j].Name = "t" + i.ToString() + j.ToString();
                    boardImages[i][j].Name = "t" + i.ToString() + j.ToString();
                    boardRects[i][j].Stretch = Stretch.Uniform;
                    Canvas.SetZIndex(boardRects[i][j], 0);
                    Canvas.SetZIndex(boardImages[i][j], 1);
                    Grid.SetRow(boardRects[i][j], (int)Math.Floor((decimal)j / 3));
                    Grid.SetColumn(boardRects[i][j], j % 3);
                    Grid.SetRow(boardImages[i][j], (int)Math.Floor((decimal)j / 3));
                    Grid.SetColumn(boardImages[i][j], j % 3);
                    smallboards[i].Children.Add(boardImages[i][j]);
                    smallboards[i].Children.Add(boardRects[i][j]);
                }
            }
            boardImages.Add(new List<Image>(9));
            for (int i = 0; i < 9; ++i )
            {
                boardImages[9].Add(new Image());
                boardImages[9][i].Stretch = Stretch.Uniform;
                Grid.SetRowSpan(boardImages[9][i], 3);
                Grid.SetColumnSpan(boardImages[9][i], 3);
                Canvas.SetZIndex(boardImages[9][i], 2);
                smallboards[i].Children.Add(boardImages[9][i]);
            }
            UpdateTiles(game.GetBoards(game.currentTurn));
            UpdateTurnSelectButtons();
        }

        private void UpdateTiles(Boards boards)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (boards.lastmove == i)
                        boardRects[i][j].Fill = new SolidColorBrush(Colors.Yellow);
                    else
                        boardRects[i][j].Fill = new SolidColorBrush(
                            i % 2 == 0 ? filled : Colors.White);
                    boardImages[i][j].Opacity = 1;
                    boardImages[i][j].Source = TryFindResource
                        (boards.GetTile_String(new Move(i, j))) as BitmapImage;
                }
                string result = boards.GetWinner_String(i);
                if(result == "X" || result == "O")
                {
                    for (int j = 0; j < 9; ++j)
                        boardImages[i][j].Opacity = .33;
                    boardImages[9][i].Source = TryFindResource(result) as BitmapImage;
                }
                else { boardImages[9][i].Source = null; }
            }
            string winner = boards.GetWinner_String(9);
            if (winner == " ") { InfoGridUpdate(boards.turn); }
            else {  for (int i = 0; i < 9; ++i) { for (int j = 0; j < 9; ++j) { boardRects[i][j].Fill = new SolidColorBrush(i % 2 == 0 ? filled : Colors.White); } }
                    if (winner == "D") { InfoImage.Source = null; InfoBox.Text = "Game Over: It's a Draw"; }
                    else { InfoImage.Source = TryFindResource(winner) as BitmapImage; InfoBox.Text = "Game Over: " + winner + " Wins"; }
            }
            if(lastMove != null && turnToDisplay == game.currentTurn)
            {
                boardRects[lastMove.board][lastMove.tile].Fill = new SolidColorBrush(filledOrange);
            }
        }

        private void UpdateTurnSelectButtons()
        {
            if (turnToDisplay == game.currentTurn) { TurnForward.Opacity = .3; }
            else { TurnForward.Opacity = 1; }
            if (turnToDisplay == 0) { TurnBack.Opacity = .3; }
            else { TurnBack.Opacity = 1; }
        }

        private void EmptyTile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int i = int.Parse((sender as Rectangle).Name.Substring(1, 1));
            int j = int.Parse((sender as Rectangle).Name.Substring(2, 1));
            Move move = new Move(i, j);
            if (!game.IsValidMove(move, turnToDisplay)) { return; }
            if (AI != null)
            {
                if((turnToDisplay % 2) == 0)
                {
                    if (turnToDisplay != game.currentTurn) { game.RevertToTurn(turnToDisplay); }
                    game.MakeMove(move);
                    Boards boards = game.GetBoards(++turnToDisplay);
                    lastMove = move;
                    UpdateTiles(boards);
                    UpdateTurnSelectButtons();
                    AllowUIToUpdate();
                    try 
                    {
                        move = AI.GetMove(boards, 5);
                        game.MakeMove(move);
                    }
                    catch { return; }
                    lastMove = move;
                    UpdateTiles(game.GetBoards(++turnToDisplay));
                    UpdateTurnSelectButtons();
                }
            }
            else
            {
                if (turnToDisplay++ != game.currentTurn) { game.RevertToTurn(turnToDisplay - 1); }
                game.MakeMove(move);
                lastMove = move;
                UpdateTiles(game.GetBoards(turnToDisplay));
                UpdateTurnSelectButtons();
            }
        }

        private void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate(object parameter){frame.Continue = false;return null;}), null);
            Dispatcher.PushFrame(frame);
        }


        private void ResetButton1_Click(object sender, RoutedEventArgs e)
        {
            game = new Game();
            turnToDisplay = game.currentTurn;
            lastMove = null;
            Board_Init();
            AI = new SmartAI();
        }

        private void ResetButton2_Click(object sender, RoutedEventArgs e)
        {
            game = new Game();
            turnToDisplay = game.currentTurn;
            lastMove = null;
            Board_Init();
            AI = null;
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TurnBack_Click(object sender, RoutedEventArgs e)
        {
            if(turnToDisplay > 0)
            { --turnToDisplay; UpdateTiles(game.GetBoards(turnToDisplay)); }
            UpdateTurnSelectButtons();
        }

        private void TurnForward_Click(object sender, RoutedEventArgs e)
        {
            if (turnToDisplay < game.currentTurn)
            { ++turnToDisplay; UpdateTiles(game.GetBoards(turnToDisplay)); }
            UpdateTurnSelectButtons();
        }
    }
}
