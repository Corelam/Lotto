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

using System.Net.Http;
using HtmlAgilityPack;          // NuGet -> Install-Package HtmlAgilityPack -Version 1.4.9.5

namespace Lotto
{
    public sealed partial class MainPage : Page
    {
        private Random randomGenerator = new Random();
        private int[] numbers;

        List<string> onlineResults = new List<string>();

        public MainPage()
        {
            this.InitializeComponent();

            if (displayGameType.Text.Equals("Lotto"))
            {
                BallsVisibility(0);
                playerInstruction.Text = "Losowanie 6 liczb - od 1 do 46";
            }
        }

        #region Interface

        #region Buttons
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            MySplitView.SetValue(Canvas.ZIndexProperty, 1);
        }

        private void MySplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            MySplitView.SetValue(Canvas.ZIndexProperty, 0);
        }

        private void lottoButton_Click(object sender, RoutedEventArgs e)
        {
            displayGameType.Text = "Lotto";
            playerInstruction.Text = "Losowanie 6 liczb - od 1 do 46";
            BallsVisible(false);
        }

        private void multimultiButton_Click(object sender, RoutedEventArgs e)
        {
            displayGameType.Text = "Multi Multi";
            playerInstruction.Text = "Losowanie 10 liczb - od 1 do 80";
            BallsVisible(true);
        }

        private void getHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            displayGameType.Text = "Najświeższe wyniki losowania";
            playerInstruction.Text = "Lotto";
            BallsVisible(false);
            GetOnlineResults_Lotto();
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
            else
            {
                SaveToFile("_WYNIKI");
            }
        }

        private void readFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            ReadFromFile();
            LoadedBallsVisible(true);
        }
        
        private void clearReadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadedBallsVisible(false);
        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayGameType.Text.Equals("Lotto"))
            {
                RandomNumbers("Lotto");
            }
            else if (displayGameType.Text.Equals("Multi Multi"))
            {
                RandomNumbers("Multi Multi");
            }
        }
        #endregion

        /// <summary> Metoda do ukrywania kul 7-10. </summary>
        private void BallsVisibility(int value=100)
        {
            ball7shape.Opacity = value;
            ball8shape.Opacity = value;
            ball9shape.Opacity = value;
            ball10shape.Opacity = value;
        }
        
        /// <summary> Metoda czyszcząca wylosowane liczby z kul. </summary>
        private void BallsClear()
        {
            TextBlock[] balls = { ball1, ball2, ball3, ball4, ball5, ball6, ball7, ball8, ball9, ball10 };

            foreach (TextBlock ball in balls)
            {
                ball.Text = "";
            }
        }

        /// <summary> Metoda do ukrywania lub pokazywania kul 7-10. </summary>
        private void BallsVisible(bool visible)
        {
            if (visible)
            {
                BallsClear();
                BallsVisibility(100);
            }
            else
            {
                BallsClear();
                BallsVisibility(0);
            }
        }
        
        /// <summary> Metoda czyszcząca wczytane liczby z kul. </summary>
        private void LoadedBallsClear()
        {
            TextBlock[] balls = { loadedBall1, loadedBall2, loadedBall3, loadedBall4, loadedBall5, loadedBall6, loadedBall7, loadedBall8, loadedBall9, loadedBall10 };

            foreach (TextBlock ball in balls)
            {
                ball.Text = "";
            }
        }

        /// <summary> Metoda do ukrywania kul z wczytanymi liczbami. </summary>
        private void LoadedBallsVisibility(int value = 100)
        {
            loadedBall1shape.Opacity = value;
            loadedBall2shape.Opacity = value;
            loadedBall3shape.Opacity = value;
            loadedBall4shape.Opacity = value;
            loadedBall5shape.Opacity = value;
            loadedBall6shape.Opacity = value;
            loadedBall7shape.Opacity = value;
            loadedBall8shape.Opacity = value;
            loadedBall9shape.Opacity = value;
            loadedBall10shape.Opacity = value;
        }

        /// <summary> Metoda do ukrywania lub pokazywania kul z wczytanymi liczbami. </summary>
        private void LoadedBallsVisible(bool visible)
        {
            if (visible)
            {
                LoadedBallsVisibility(100);
            }
            else
            {
                LoadedBallsVisibility(0);
                LoadedBallsClear();
            }
        }
        #endregion

        /// <summary> Metoda generująca i przypisująca losowe liczby do kul. </summary>
        private void RandomNumbers(string drawType)
        {
            TextBlock[] balls;
            List<int> possible;

            if (drawType.Equals("Lotto"))
            {
                numbers = new int[6];
                balls = new TextBlock[] { ball1, ball2, ball3, ball4, ball5, ball6 };

                possible = Enumerable.Range(1, 46).ToList();
            }
            else if (drawType.Equals("Multi Multi"))
            {
                numbers = new int[10];
                balls = new TextBlock[] { ball1, ball2, ball3, ball4, ball5, ball6, ball7, ball8, ball9, ball10 };

                possible = Enumerable.Range(1, 80).ToList();
            }
            else
            {
                throw new Exception("Nie wybrano rodzaju losowania");
            }

            for (int i = 0; i < numbers.Length; i++)
            {
                int index = randomGenerator.Next(0, possible.Count);
                numbers[i] = possible[index];
                possible.RemoveAt(index);
            }

            int j = 0;
            foreach (TextBlock ball in balls)
            {
                ball.Text = numbers[j].ToString();
                j++;
            }
        }

        /// <summary> Metoda służąca do zapisywania wylosowanych liczb do pliku. </summary>
        private async void SaveToFile(string drawType)
        {
            string numbersToSave = "";

            try
            {
                foreach (int number in numbers)
                {
                    numbersToSave += number.ToString() + ", ";
                }

                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("Plik tekstowy (.txt)", new List<string>() { ".txt" });
                savePicker.SuggestedFileName = "wylosowane_liczby" + drawType;

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    await Windows.Storage.FileIO.WriteTextAsync(file, numbersToSave);   // write to file
                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        ShowToastNotification("Zapisano w: " + file.Path, file.Name);
                    }
                    else
                    {
                        ShowToastNotification("Błąd zapisu", file.Name);
                    }
                }
                else
                {
                    ShowToastNotification("Operacja anulowana.", "");
                }
            }
            catch
            {
                ShowToastNotification("Błąd zapisu", "Nie wylosowano żadnych liczb!");
            }
        }

        /// <summary> Metoda służąca do wczytawania istniejącego pliku. </summary>
        private async void ReadFromFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                using (StreamReader reader = new StreamReader(stream.AsStream()))
                {
                    string filetext = reader.ReadToEnd();
                    UpdateBallsFromFile(filetext);
                }
            }
            else
            {
                ShowToastNotification("Operacja anulowana.", "");
            }
        }

        /// <summary> Metoda służąca do wyświetlania powiadomień o podanym tytule i zawartości. </summary>
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

        /// <summary> Metoda pobiera ze strony 6 wylosowanych oficjalnie liczb. </summary>
        private async void GetOnlineResults_Lotto()
        {
            var uri = new Uri("http://www.lotto.pl/lotto/wyniki-i-wygrane/ostatnie-wyniki");
            var httpClient = new HttpClient();
            
            try
            {
                var result = await httpClient.GetStringAsync(uri);
                var doc = new HtmlDocument();
                doc.LoadHtml(result);

                onlineResults = new List<string>();
                var findclasses = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("wynik_lotto"));

                numbers = new int[6];
                int i = 0;
                foreach (HtmlNode selectNode in findclasses)
                {
                    if (i == 6) { break; }
                    onlineResults.Add(selectNode.InnerText);
                    numbers[i] = Int32.Parse(selectNode.InnerText);
                    i++;
                }

                UpdateBallsOnline(onlineResults);
            }
            catch
            {
                ShowToastNotification("Błąd sieci", "Nie można połączyć się z żądaną stroną.");
            }

            httpClient.Dispose();
        }

        /// <summary> Metoda zapisuje na kulach 6 wylosowanych oficjalnie liczb. </summary>
        private void UpdateBallsOnline(List<string> numbers)
        {
            TextBlock[] balls = { ball1, ball2, ball3, ball4, ball5, ball6 };

            int i = 0;
            foreach (TextBlock ball in balls)
            {
                ball.Text = numbers[i];
                i++;
            }
        }

        /// <summary> Metoda zapisuje na kulach 6 lub 10 wczytanych z pliku liczb. </summary>
        private void UpdateBallsFromFile(string text)
        {
            try {
                string newText = "";

                foreach (char letter in text)
                {
                    if (letter.Equals(','))
                    {
                        continue;
                    }
                    newText += letter;
                }
                
                string[] numbers = newText.Split(' ');

                try
                {
                    for (int check = 0; check < numbers.Length - 1; check++)
                    {
                        int test = Int32.Parse(numbers[check]);
                    }
                }
                catch
                {
                    ShowToastNotification("Błąd odczytu pliku.", "Podany plik zawiera znaki inne niż liczby.");
                    return;
                }

                TextBlock[] balls = { };

                if (numbers.Length.Equals(7))
                {
                    balls = new TextBlock[] { loadedBall1, loadedBall2, loadedBall3, loadedBall4, loadedBall5, loadedBall6 };
                }
                else if (numbers.Length.Equals(11))
                {
                    balls = new TextBlock[] { loadedBall1, loadedBall2, loadedBall3, loadedBall4, loadedBall5, loadedBall6, loadedBall7, loadedBall8, loadedBall9, loadedBall10 };
                }
                else
                {
                    ShowToastNotification("Błąd odczytu pliku.", "UpdateBallsFromFile failed.");
                }

                int i = 0;
                foreach (TextBlock ball in balls)
                {
                    ball.Text = numbers[i];
                    i++;
                }
            }
            catch
            {
                ShowToastNotification("Błąd odczytu pliku", "");
            }
        }
    }
}