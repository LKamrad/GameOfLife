using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOFLifeInWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static Button[,] btnArray = new Button[40, 70];
        static bool IsEditingMode = false;
        static Random rnd = new Random();
        static int[,] array_old = new int[42, 72];
        static int[,] array_new = new int[42, 72];
        static int[,] array = new int[42, 72];
        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;

        static CancellationTokenSource source2 = new CancellationTokenSource();
        static CancellationToken token2 = source2.Token;
        static bool isGrowing = true;
        static int interval = -1;

        public MainWindow()
        {
            InitializeComponent();

            Thickness myThickness = new Thickness();
            myThickness.Bottom = 0;
            myThickness.Left = 0;
            myThickness.Right = 0;
            myThickness.Top = 0;

            InitializeButtons();
        }


        private void InitializeButtons()
        {
            Thickness myThickness = new Thickness();
            myThickness.Bottom = 0;
            myThickness.Left = 0;
            myThickness.Right = 0;
            myThickness.Top = 0;
            for (int i = 0; i < btnArray.GetLength(0); i++)
            {
                for (int j = 0; j < btnArray.GetLength(1); j++)
                {
                    btnArray[i, j] = new Button();
                    btnArray[i, j].Width = 10;
                    btnArray[i, j].Height = 10;
                    btnArray[i, j].Background = Brushes.White;
                    btnArray[i, j].BorderThickness = myThickness;
                    btnArray[i, j].Click += new RoutedEventHandler(ButtonStop_Click);
                    myWrapPanel.Children.Add(btnArray[i, j]);
                }
            }
        }

        private void ActivateEditing()
        {
            IsEditingMode = true;
            ButtonManualMode.Background = Brushes.LightCoral;
        }

        private void DeactivateEditing()
        {
            IsEditingMode = false;
            ButtonManualMode.ClearValue(BackgroundProperty);
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {

            if (IsEditingMode)
            {
                if (((Button)sender).Background == Brushes.Green)
                {
                    ((Button)sender).Background = Brushes.White;
                }
                else
                {
                    ((Button)sender).Background = Brushes.Green;
                }
            }


        }
        private async void NextGeneration()
        {
            int temp;

            isGrowing = false;
            for (int i = 1; i < array.GetLength(0) - 1; i++)
            {

                for (int j = 1; j < array.GetLength(1) - 1; j++)
                {
                    array_old[i, j] = array[i, j];


                    temp = array[i - 1, j - 1] + array[i - 1, j] + array[i - 1, j + 1] + array[i, j - 1] + array[i, j + 1] + array[i + 1, j - 1] + array[i + 1, j] + array[i + 1, j + 1];


                    if (temp == 3 || (temp == 2 && array[i, j] == 1))
                    {
                        array_new[i, j] = 1;
                    }
                    if (temp > 3 || temp < 2)
                    {
                        array_new[i, j] = 0;
                    }
                }

            }
            for (int i = 1; i < array.GetLength(0) - 1; i++)
            {

                for (int j = 1; j < array.GetLength(1) - 1; j++)
                {
                    array[i, j] = array_new[i, j];
                    if (array_new[i, j] != array_old[i, j])
                    {
                        isGrowing = true;
                    }
                }
            }
        }
        private void FillArray()
        {

            for (int i = 0; i < btnArray.GetLength(0); i++)
            {
                for (int j = 0; j < btnArray.GetLength(1); j++)
                {
                    if (btnArray[i, j].Background == Brushes.White)
                    {
                        array[i + 1, j + 1] = 0;
                    }
                    if (btnArray[i, j].Background == Brushes.Green)
                    {
                        array[i + 1, j + 1] = 1;
                    }
                }
            }

        }
        private void UnlockButtons()
        {
            CheckboxIntervals.IsEnabled = true;
            if (CheckboxIntervals.IsChecked == true)
            {
                TextboxIntervals.IsEnabled = true;
            }
            ButtonStart.IsEnabled = true;
            ButtonLoad.IsEnabled = true;
            ButtonSave.IsEnabled = true;
            ButtonRandom.IsEnabled = true;
            ButtonManualMode.IsEnabled = true;
            ButtonClear.IsEnabled = true;
            ButtonStepOver.IsEnabled = true;

            ButtonPause.IsEnabled = false;
        }

        private async void StartGame()
        {
            source = new CancellationTokenSource();
            token = source.Token;
            isGrowing = true;


            while (isGrowing)
            {
               
                try
                {

                    Task.Delay(200).Wait();
                    token.ThrowIfCancellationRequested();
                    NextGeneration();
                    await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                    {
                        token.ThrowIfCancellationRequested();
                        ShowArray();
                    }
                    )), token);
                }
                catch
                {
                    isGrowing = false;
                }
            }

            source = new CancellationTokenSource();
            token = source.Token;
            try
            {
                await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                {
                    UnlockButtons();
                })), token);
            }
            catch
            {

            }


        }

        private async void StartGameInterval()
        {
            source2 = new CancellationTokenSource();
            token2 = source2.Token;
            isGrowing = true;

            int iteration = interval;
            for(iteration = interval; iteration > 0; iteration--)
            {
                
                try
                {

                    Task.Delay(200).Wait();
                    token2.ThrowIfCancellationRequested();
                    NextGeneration();
                    await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                    {
                        TextboxIntervals.Text = $"{iteration}";
                        token2.ThrowIfCancellationRequested();
                        
                        ShowArray();
                    }
                    )), token2);
                }
                catch
                {
                    break;
                }


            }
            source2 = new CancellationTokenSource();
            token2 = source2.Token;
            try
            {
                await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                {
                    TextboxIntervals.Text = $"{iteration}";
                    UnlockButtons();
                })), token2);
            }
            catch
            {

            }


        }
        private void ShowArray()
        {

            for (int i = 1; i < array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < array.GetLength(1) - 1; j++)
                {
                    if (array_new[i, j] == 1)
                    {
                        btnArray[i - 1, j - 1].Background = Brushes.Green;
                    }
                    if (array_new[i, j] == 0)
                    {
                        btnArray[i - 1, j - 1].Background = Brushes.White;
                    }
                }
            }
        }
        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {

            FillArray();

            if (IsEditingMode)
            {
                DeactivateEditing();
            }
            TextboxIntervals.IsEnabled = false;
            CheckboxIntervals.IsEnabled = false;
            ButtonPause.IsEnabled = true;
            ButtonStart.IsEnabled = false;
            ButtonLoad.IsEnabled = false;
            ButtonSave.IsEnabled = false;
            ButtonRandom.IsEnabled = false;
            ButtonManualMode.IsEnabled = false;
            ButtonClear.IsEnabled = false;
            ButtonStepOver.IsEnabled = false;

            if(CheckboxIntervals.IsChecked == false)
            {

                try
                {
                    await Task.Run(() => StartGame());
                }
                catch
                {
                    try
                    {
                        await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                        {
                            UnlockButtons();
                        })), token);
                    }
                    catch
                    {

                    }


                }
            }
            else
            {

                try
                {
                    await Task.Run(() => StartGameInterval());
                }
                catch
                {
                    try
                    {
                        await Task.Factory.StartNew(() => Dispatcher.BeginInvoke(new Action(delegate
                        {
                            UnlockButtons();
                        })), token2);
                    }
                    catch
                    {

                    }


                }
            }


        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditingMode)
            {
                DeactivateEditing();
            }
            source.Cancel();
            source2.Cancel();
            UnlockButtons();

        }

        private void ButtonStepOver_Click(object sender, RoutedEventArgs e)
        {
            FillArray();
            NextGeneration();
            ShowArray();
            if (IsEditingMode)
            {
                DeactivateEditing();
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < btnArray.GetLength(0); i++)
            {
                for (int j = 0; j < btnArray.GetLength(1); j++)
                {
                    btnArray[i, j].Background = Brushes.White;

                }
            }


        }

        private void ButtonManualMode_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditingMode == false)
            {

                ActivateEditing();
            }
            else
            {
                DeactivateEditing();

            }


        }

        private void ButtonRandom_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < btnArray.GetLength(0); i++)
            {
                for (int j = 0; j < btnArray.GetLength(1); j++)
                {
                    int rndValue = rnd.Next(0, 2);
                    if (rndValue == 0)
                    {
                        btnArray[i, j].Background = Brushes.White;
                    }
                    else
                    {
                        btnArray[i, j].Background = Brushes.Green;
                    }

                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditingMode)
            {
                DeactivateEditing();
            }
            FillArray();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultDirectory = @"c:\Filestream";
            dlg.FileName = "GameOfLife"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV files (*.csv)|*.csv";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string path = dlg.FileName;
                StreamWriter streamWriter = new StreamWriter(path);

                string temp = "";
                for (int i = 1; i < array.GetLength(0) - 1; i++)
                {
                    temp = "";
                    for (int j = 1; j < array.GetLength(1) - 1; j++)
                    {
                        temp += array[i, j];
                        temp += ',';
                    }

                    streamWriter.WriteLine(temp.TrimEnd(','));
                }
                streamWriter.Close();
            }

        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditingMode)
            {
                DeactivateEditing();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultDirectory = @"c:\Filestream";
            openFileDialog.FileName = "GameOfLife.csv"; // Default file name
            openFileDialog.DefaultExt = ".csv"; // Default file extension
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";

            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string path = openFileDialog.FileName;
                StreamReader streamReader = new StreamReader(path);

                List<string> temp = new List<string>();


                while(streamReader.EndOfStream == false)
                {
                    temp.Add(streamReader.ReadLine());
                }
                streamReader.Close();

                int height = 0;
                foreach (string line in temp)
                {
                    string[] read = line.Split(",");
                    for(int i = 0; i< read.Length; i++)
                    {
                        array_new[height+1, i + 1] = int.Parse(read[i]);
                    }
                    

                    height++;
                }

                
            }
            ShowArray();
        }

        private void CheckboxIntervals_Checked(object sender, RoutedEventArgs e)
        {
            TextboxIntervals.IsEnabled = true;
            TextboxIntervals.Text = "";
        }

        private void CheckboxIntervals_Unchecked(object sender, RoutedEventArgs e)
        {
            TextboxIntervals.IsEnabled = false;
            TextboxIntervals.Text = "Infinity";
        }

        private void TextboxIntervals_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TextboxIntervals.Text == "Infinity")
            {
                TextboxIntervals.Background = Brushes.LightBlue;
            }

            if (int.TryParse(TextboxIntervals.Text, out interval))
            {

                TextboxIntervals.Background = Brushes.LightGreen;
            }
            else
            {
                interval = -1;
                if (TextboxIntervals.Text == "Infinity")
                {
                    TextboxIntervals.Background = Brushes.LightBlue;
                }
                else
                {
                    if (string.IsNullOrEmpty(TextboxIntervals.Text))
                    {
                        TextboxIntervals.Background = Brushes.White;
                    }
                    else 
                    {
                        TextboxIntervals.Background = Brushes.Red;
                    }
                    
                }
                
            }
        }
    }
}