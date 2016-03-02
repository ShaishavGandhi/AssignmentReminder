using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Assignment_Reminder.Resources;
using Assignment_Reminder.ViewModels;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Tasks;
using GoogleAds;


namespace Assignment_Reminder
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        private InterstitialAd interstitialAd;
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.ViewModel;
            //interstitialAd = new InterstitialAd("ca-app-pub-7421031935026273/9146095743");
            //AdRequest adRequest = new AdRequest();
            
            //interstitialAd.ReceivedAd += OnAdReceived;
            //interstitialAd.LoadAd(adRequest);
            IsolatedStorageSettings settings1 = IsolatedStorageSettings.ApplicationSettings;

            if (settings1.Contains("ratecount") && !settings1.Contains("reviewed"))
            {
                int count = Convert.ToInt32(settings1["ratecount"]);
                if (count != -1)
                    count++;
                if (count % 5 == 0 && count != -1)
                {
                    //Add Dialog Code Here
                    // MessageBoxButton btn = new MessageBoxButton();

                    MessageBoxResult result = MessageBox.Show("Please take a moment to review this application. It means a lot to us :)", "Would you like to rate this application?", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

                        marketplaceReviewTask.Show();
                        settings1.Add("reviewed", true);
                        settings1.Save();
                    }


                }
                if (count == 5)
                    count = 0;

                settings1["ratecount"] = count;
                settings1.Save();
            }
            else
            {
                if (!settings1.Contains("ratecount"))
                    settings1.Add("ratecount", 0);
                settings1.Save();
            }
            //MainLongListSelector.ItemsSource = App.ViewModel.Items;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items

        private void OnAdReceived(object sender, AdEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Ad received successfully");
            interstitialAd.ShowAd();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            else {
               //InitializeComponent();                
                App.ViewModel.Items.Clear();
                App.ViewModel.LoadData();
               

                //MainLongListSelector.ItemsSource = null;
                //MainLongListSelector.ItemsSource = App.ViewModel.Items;
                
               
               
            }
            if (App.ViewModel.Items.Count > 6)
            {
                ApplicationBar.Opacity = 1;
            }
            
        }

        // Handle selection changed on LongListSelector
        //private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // If selected item is null (no selection) do nothing
        //    //if (MainLongListSelector.SelectedItem == null)
        //    //    return;

        //    //// Navigate to the new page
        //    //NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));

        //    //// Reset selected item to null (no selection)
        //    //MainLongListSelector.SelectedItem = null;
        //}

        

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            var control = (sender as TextBlock);
            string name=control.Text;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem="+name, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddAssign.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Support1.xaml", UriKind.Relative));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool isCompleted;
            var checkbox = (sender as CheckBox);
            if (checkbox.IsChecked == true)
                isCompleted = true;
            else
                isCompleted = false;

            var something = checkbox.Parent;
            TextBlock tp = VisualTreeHelper.GetChild(something, 1) as TextBlock;
            string name = tp.Text;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(name))
            {
                var currentObject = settings[name] as ItemViewModel;
                settings[name] = new ItemViewModel() { ID = "8", Title = currentObject.Title, Completed = isCompleted, DueDate = currentObject.DueDate };
                settings.Save();
                if (isCompleted == true)
                {
                    if (ScheduledActionService.Find(currentObject.Title) != null)
                        ScheduledActionService.Remove(currentObject.Title);
                }
                else {
                    if (ScheduledActionService.Find(currentObject.Title) != null)
                        ScheduledActionService.Remove(currentObject.Title);

                    if((Convert.ToDateTime(currentObject.DueDate))>DateTime.Now){
                    Microsoft.Phone.Scheduler.Reminder reminder = new Microsoft.Phone.Scheduler.Reminder(currentObject.Title);
                    reminder.Title = currentObject.Title;
                    reminder.Content = "You have your " + currentObject.Title + " assignment due tomorrow";
                    reminder.BeginTime = Convert.ToDateTime(currentObject.DueDate).AddDays(-1).AddHours(18);
                    //reminder.ExpirationTime = new DateTime(1814, 08, 30, 11, 50, 30);
                    //reminder.RecurrenceType = recurrence;
                    reminder.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                    ScheduledActionService.Add(reminder);
                        }
                }
            }
            //ShellToast toast = new ShellToast();
            //toast.Title = "Something";
            //toast.Content = "Relevant";
            //toast.Show();
            
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}

