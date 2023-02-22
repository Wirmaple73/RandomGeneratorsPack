using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RandomGeneratorsPack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Number Generator

        int rndMin, rndMax;
        double genNum;

        readonly Random rnd = new Random();


        private void NumGen_ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetRandNum();  // Generate a random number

                NumGen_LBox.Items.Add(genNum);  // Add the generated number to the Listbox
                WriteNumberToFile(); // Write the generated number to 'GeneratedNumbers.txt'
            }
            catch { ShowMbError("Invalid Min/Max value(s), please enter integers only."); }
        }

        private void GetRandNum()
        {
            rndMin = int.Parse(NumGen_Tb1.Text);
            rndMax = int.Parse(NumGen_Tb2.Text);

            genNum = rnd.Next(rndMin, rndMax + 1);


            if (NumGen_RBtnDec.IsChecked == true)
            {
                genNum += Math.Round(rnd.NextDouble(), 5);
            }
        }

        private void WriteNumberToFile()
        {
            if (NumGen_CBoxWrite.IsChecked == true)
            {
                WriteToFile("GeneratedNumbers.txt", genNum.ToString());
            }
        }

        private void NumGen_ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(NumGen_LBox.SelectedValue.ToString());
            }
            catch { MBoxSelectItemFirst(); }
        }

        private void NumGen_ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            NumGen_LBox.Items.Clear();
        }

        #endregion

        #region Name Generator

        string[] maleNames, femaleNames, lastNames;
        string genName;
        bool hasInitialized = false, succeeded;


        private void GetNames()
        {
            if (hasInitialized == false)
            {
                try
                {
                    ReadNamesFromFile();
                }
                catch
                {
                    MessageBox.Show("There was an error trying to access 'Names.txt' in the program's directory, using the default name list.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    try
                    {
                        File.WriteAllText("Names.txt", Properties.Resources.Names);
                        ReadNamesFromFile();
                    }
                    catch
                    {
                        MessageBox.Show("Could not read from the file, the file or program directory might be read-only.\nPlease resolve the issue and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        hasInitialized = false;
                        succeeded = false;
                    }
                }
            }
        }

        private void ReadNamesFromFile()
        {
            string[] allNames = File.ReadAllLines("Names.txt");

            maleNames = allNames[0].Split(',');  // Split the lines into their appropriate arrays
            femaleNames = allNames[1].Split(',');
            lastNames = allNames[2].Split(',');

            hasInitialized = true;
            succeeded = true;
        }

        private void NameGen_BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GetNames();  // Read and store the names used for name generation

            if (succeeded)
            {
                GenerateName();
                WriteNameToFile();

                NameGen_LBoxNameList.Items.Add(genName);
            }
        }

        private void GenerateName()
        {
            // Check for the Radiobuttons and generate a male/female name
            genName = NameGen_RBtnMale.IsChecked == true ? maleNames[rnd.Next(0, maleNames.Length)] : femaleNames[rnd.Next(0, femaleNames.Length)];

            // Generate a last name
            genName += " " + lastNames[rnd.Next(0, lastNames.Length)];
        }

        private void WriteNameToFile()
        {
            if (NameGen_CBoxWriteFile.IsChecked == true)
            {
                WriteToFile("GeneratedNames.txt", genName);
            }
        }

        private void NameGen_BtnClear_Click(object sender, RoutedEventArgs e)
        {
            NameGen_LBoxNameList.Items.Clear();
        }

        private void NameGen_BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(NameGen_LBoxNameList.SelectedValue.ToString());
            }
            catch { MBoxSelectItemFirst(); }
        }

        #endregion

        #region Password Generator

        string passChars;

        private void PassGen_BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GetPassChars();
            GeneratePassword();
        }

        private void GetPassChars()
        {
            switch (PassGen_ComBoxPassType.SelectedIndex)
            {
                case 0:  // Characters only
                    passChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; break;

                case 1:  // Digits only
                    passChars = "0123456789"; break;

                case 2:  // Both
                    passChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; break;

                case 3:  // Custom
                    passChars = PassGen_TbCustomChars.Text; break;
            }
        }

        private void GeneratePassword()
        {
            try
            {
                short passLength = short.Parse(PassGen_Length.Text);
                ValidatePassLength(passLength);  // Make sure the 'length' value is greater than zero

                var sb = new StringBuilder(passLength);

                for (short i = 0; i < passLength; i++)
                {
                    sb.Append(passChars[rnd.Next(0, passChars.Length)]);
                }

                PassGen_LBoxPassList.Items.Add(sb.ToString());
                WritePassToFile(sb);
            }
            catch (IndexOutOfRangeException) { ShowMbError("Invalid or empty custom password characters."); }
            catch { ShowMbError("Invalid password length,\nPlease enter a positive non-zero integer."); }
        }

        private void ValidatePassLength(short passLength)
        {
            if (passLength <= 0)
                throw new ArgumentOutOfRangeException();
        }

        private void WritePassToFile(StringBuilder sb)
        {
            if (PassGen_CBoxWriteFile.IsChecked == true)
            {
                WriteToFile("GeneratedPasswords.txt", sb.ToString());
            }
        }

        private void PassGen_BtnClear_Click(object sender, RoutedEventArgs e)
        {
            PassGen_LBoxPassList.Items.Clear();
        }

        private void PassGen_BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(PassGen_LBoxPassList.SelectedValue.ToString());
            }
            catch { MBoxSelectItemFirst(); }
        }

        #endregion

        #region Miscellaneous

        private void MBoxSelectItemFirst()
        {
            MessageBox.Show("Please select an item from the Listbox first.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ShowMbError(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Menu_NumGenHistory_Click(object sender, RoutedEventArgs e)
        {
            OpenFile("GeneratedNumbers.txt");
        }

        private void Menu_NameGenHistory_Click(object sender, RoutedEventArgs e)
        {
            OpenFile("GeneratedNames.txt");
        }

        private void Menu_PassGenHistory_Click(object sender, RoutedEventArgs e)
        {
            OpenFile("GeneratedPasswords.txt");
        }

        private void OpenFile(string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch { ShowMbError(string.Format("There was an error trying to open '{0}',\nThe file might not exist.", fileName)); }
        }

        private void WriteToFile(string fileName, string content)
        {
            try
            {
                File.AppendAllText(fileName, content + "\r\n");
            }
            catch { ShowMbError(string.Format("There was an error trying to write to '{0}',\nThe file might be read-only.", fileName)); }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void About_Link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/Wirmaple73");
        }

        #endregion
    }
}