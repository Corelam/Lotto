using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Diagnostics;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;

namespace Lotto
{
    public sealed partial class MainPage : Page
    {
        private Random randomGenerator = new Random();
        private int[] numbers;

        public MainPage()
        {
            this.InitializeComponent();

            if (displayGameType.Text.Equals("Lotto"))
            {
                BallsVisibility(0);
                playerInstruction.Text = "Losowanie 6 liczb - od 1 do 46";
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            MySplitView.SetValue(Canvas.ZIndexProperty, 1);
        }

        private void lottoButton_Click(object sender, RoutedEventArgs e)
        {
            displayGameType.Text = "Lotto";
            playerInstruction.Text = "Losowanie 6 liczb - od 1 do 46";
            BallsVisibility(0);
            ClearBalls();
        }

        /// <summary> Metoda do ukrywania kul 7-10. </summary>
        private void BallsVisibility(int value=100)
        {
            ball7shape.Opacity = value;
            ball8shape.Opacity = value;
            ball9shape.Opacity = value;
            ball10shape.Opacity = value;
        }

        private void multimultiButton_Click(object sender, RoutedEventArgs e)
        {
            displayGameType.Text = "Multi Multi";
            playerInstruction.Text = "Losowanie 10 liczb - od 1 do 80";
            BallsVisibility();
            ClearBalls();
        }

        private void MySplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            MySplitView.SetValue(Canvas.ZIndexProperty, 0);
        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayGameType.Text.Equals("Lotto"))
            {
                RandomNumbersLotto();
            }
            else if (displayGameType.Text.Equals("Multi Multi"))
            {
                RandomNumbersMultiMulti();
            }
        }

        /// <summary> Metoda LOTTO, generująca i przypisująca losowe liczby do kul. </summary>
        private void RandomNumbersLotto()
        {
            numbers = new int[6];
            TextBlock[] balls = { ball1, ball2, ball3, ball4, ball5, ball6 };

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = randomGenerator.Next(1, 47);
            }

            int j = 0;
            foreach (TextBlock ball in balls)
            {
                ball.Text = numbers[j].ToString();
                j++;
            }
        }

        /// <summary> Metoda MULTI MULTI, generująca i przypisująca losowe liczby do kul. </summary>
        private void RandomNumbersMultiMulti()
        {
            numbers = new int[10];
            TextBlock[] balls = { ball1, ball2, ball3, ball4, ball5, ball6, ball7, ball8, ball9, ball10 };

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = randomGenerator.Next(1, 81);
            }

            int j = 0;
            foreach (TextBlock ball in balls)
            {
                ball.Text = numbers[j].ToString();
                j++;
            }
        }

        /// <summary> Metoda czyszcząca wylosowane liczby z kul. </summary>
        private void ClearBalls()
        {
            TextBlock[] balls = { ball1, ball2, ball3, ball4, ball5, ball6, ball7, ball8, ball9, ball10 };

            foreach (TextBlock ball in balls)
            {
                ball.Text = "";
            }
        }

        private void saveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayGameType.Text.Equals("Lotto"))
            {
                SaveToFile("_LOTTO");
            }
            else if (displayGameType.Text.Equals("Multi Multi"))
            {
                SaveToFile("_MULTI_MULTI");
            }
        }

        /// <summary> Metoda, służąca do zapisywania wylosowanych liczb do pliku. </summary>
        private async void SaveToFile(string drawType)
        {
            string numbersToSave = "";

            foreach (int number in numbers)
            {
                numbersToSave += number.ToString() + ", ";
            }

            StorageFile resultsFile = await DownloadsFolder.CreateFileAsync("wylosowane_liczby" + drawType + ".txt", CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(resultsFile, numbersToSave);
            
            ShowToastNotification("Plik zapisany w folderze 'Pobrane\\Lotto' jako:", resultsFile.Name);
        }

        /// <summary> Metoda, służąca do wyświetlania powiadomień o podanym tytule i zawartości. </summary>
        private void ShowToastNotification(string title, string content)
        {
            ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(content));
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(300);
            toastNotifier.Show(toast);
        }
    }
}



// ZLIKWIDOWAC POWTARZANIE SIE LICZB