using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Assignment_Reminder.Resources;
using System.IO.IsolatedStorage;

namespace Assignment_Reminder.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            //this.Items.Add(new ItemViewModel() { ID = "0", Title = "Maths", Completed = true, DueDate = "2/3/2014" });
            //this.Items.Add(new ItemViewModel() { ID = "1", Title = "History", Completed = false, DueDate = "2/12/2014" });

            this.IsDataLoaded = true;

            foreach (Object o in IsolatedStorageSettings.ApplicationSettings.Values){
                int i;
                bool b;
                
                if (int.TryParse(o.ToString(), out i))
                    continue;
                if (bool.TryParse(o.ToString(),out b))
                    continue;
                try { 
                this.Items.Add((ItemViewModel)o);
                    }
                catch
                {
                    continue;
                }
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}