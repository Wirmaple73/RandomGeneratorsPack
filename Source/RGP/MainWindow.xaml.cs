using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RGP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NumberGenerator numGen = new NumberGenerator();
        NameGenerator nameGen = new NameGenerator();
        PasswordGenerator passGen = new PasswordGenerator();

        const string numGenFileName = "GeneratedNumbers.txt",
            nameGenFileName = "GeneratedNames.txt",
            passGenFileName = "GeneratedPasswords.txt";


        public MainWindow()
        {
            InitializeComponent();
        }


        // --- Number Generator ---

        private void NumGen_Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                numGen.Generate(min: Convert.ToInt32(NumGen_TbMin.Text), max: Convert.ToInt32(NumGen_TbMax.Text), isDecimal: NumGen_RBtnDec.IsChecked == true);
            }
            catch
            {
                ShowMsgboxError("Invalid min/max value(s).");
                return;
            }

            NumGenWrite();
        }

        private void NumGenWrite()
        {
            NumGen_LBox.Items.Add(numGen.Number);

            if (NumGen_CBoxWrite.IsChecked == true)
                WriteToFile(numGenFileName, numGen.Number.ToString());
        }

        private void NumGen_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            NumGen_LBox.Items.Clear();
        }

        private void NumGen_CopySelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(NumGen_LBox.SelectedValue.ToString());
            }
            catch { ShowMsgboxError("Please select an item from the listbox first."); }
        }


        // --- Name Generator ---

        private void NameGen_Generate_Click(object sender, RoutedEventArgs e)
        {
            nameGen.Generate(NameGen_RBtnMale.IsChecked == true);
            NameGenWrite();
        }

        private void NameGenWrite()
        {
            NameGen_LBoxNameList.Items.Add(nameGen.Name);

            if (NameGen_CBoxWriteFile.IsChecked == true)
                WriteToFile(nameGenFileName, nameGen.Name);
        }

        private void NameGen_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            NameGen_LBoxNameList.Items.Clear();
        }

        private void NameGen_CopySelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(NameGen_LBoxNameList.SelectedValue.ToString());
            }
            catch { ShowMsgboxError("Please select an item from the listbox first."); }
        }


        // Password Generator

        private void PassGen_Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                passGen.Generate(GetPassChars(), GetPassLength());
            }
            catch (FormatException)
            {
                ShowMsgboxError("Invalid password length.");
                return;
            }
            catch
            {
                ShowMsgboxError("Invalid or empty custom password characters.");
                return;
            }

            PassGenWrite();
        }

        private string GetPassChars()
        {
            switch (PassGen_ComBoxPassType.SelectedIndex)
            {
                case 0:  // Characters only
                    return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

                case 1:  // Digits only
                    return "0123456789";

                case 2:  // Both
                    return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                case 3:  // Custom
                    return PassGen_TbCustomChars.Text;

                default:
                    throw new Exception();
            }
        }

        private short GetPassLength()
        {
            short length = Convert.ToInt16(PassGen_Length.Text);

            if (length <= 0)
                throw new FormatException();

            return length;
        }

        private void PassGenWrite()
        {
            PassGen_LBoxPassList.Items.Add(passGen.Password);

            if (PassGen_CBoxWriteFile.IsChecked == true)
                WriteToFile(passGenFileName, passGen.Password);
        }

        private void PassGen_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            PassGen_LBoxPassList.Items.Clear();
        }

        private void PassGen_CopySelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(PassGen_LBoxPassList.SelectedValue.ToString());
            }
            catch { ShowMsgboxError("Please select an item from the listbox first."); }
        }


        // --- About Page ---

        private void About_Link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/Wirmaple73");
        }


        // --- Menu Events ---

        private void Menu_ViewNumGen(object sender, RoutedEventArgs e)
        {
            OpenFile(numGenFileName);
        }

        private void Menu_ViewNameGen(object sender, RoutedEventArgs e)
        {
            OpenFile(nameGenFileName);
        }

        private void Menu_ViewPassGen(object sender, RoutedEventArgs e)
        {
            OpenFile(passGenFileName);
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }


        // --- Miscellaneous Events ---

        private void ShowMsgboxError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void WriteToFile(string fileName, string str)
        {
            try
            {
                File.AppendAllText(fileName, (str + Environment.NewLine));
            }
            catch { ShowMsgboxError(string.Format("Could not write to \"{0}\",\nThe file might be read-only.", fileName)); }
        }

        private void OpenFile(string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch { ShowMsgboxError(string.Format("Could not open \"{0}\",\nThe file might not exist.", fileName)); }
        }
    }

    class NumberGenerator
    {
        readonly Random rnd = new Random();
        double rndNum = 0.0d;

        public double Number
        {
            get { return rndNum; }
        }

        public void Generate(int min, int max, bool isDecimal)
        {
            rndNum = rnd.Next(min, max + 1);

            if (isDecimal)
                rndNum += Math.Round(rnd.NextDouble(), 2);
        }
    }

    class NameGenerator
    {
        readonly Random rnd = new Random();
        string genName = string.Empty;

        string[] maleNames = Properties.Resources.Names.Split('\n')[0].Split(',');
        string[] femaleNames = Properties.Resources.Names.Split('\n')[1].Split(',');
        string[] lastNames = Properties.Resources.Names.Split('\n')[2].Split(',');

        public string Name
        {
            get { return genName; }
        }

        public void Generate(bool isMaleName)
        {
            genName = ((isMaleName) ? (maleNames[rnd.Next(0, maleNames.Length - 1)]) : (femaleNames[rnd.Next(0, femaleNames.Length - 1)]));
            genName += (" " + lastNames[rnd.Next(0, lastNames.Length - 1)]);  // Length - 1 for fixing the new line bug
        }
    }

    class PasswordGenerator
    {
        readonly Random rnd = new Random();
        readonly StringBuilder sb = new StringBuilder();

        public string Password
        {
            get { return sb.ToString(); }
        }

        public void Generate(string chars, short passLength)
        {
            sb.Clear();

            for (short i = 0; i < passLength; i++)
            {
                sb.Append(chars[rnd.Next(0, chars.Length)]);
            }
        }
    }
}