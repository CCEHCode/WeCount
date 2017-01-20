using System;
using System.Threading.Tasks;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
using PITCSurveyApp.Views;
using PITCSurveyLib.Models;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class SurveyLocationViewModel : BaseViewModel
    {
        private const int LocationTimeoutMilliseconds = 5000;

        private static string s_lastStreet;
        private static string s_lastCity;
        private static string s_lastState;
        private static string s_lastZipCode;
        private static string s_lastLocationNotes;

        private readonly UploadedItem<SurveyResponseModel> _response;

        private string _position = "Loading...";

        public SurveyLocationViewModel()
            : this(new UploadedItem<SurveyResponseModel>(SurveyResponseModelExtensions.CreateNew()), false)
        {
        }

        public SurveyLocationViewModel(UploadedItem<SurveyResponseModel> response, bool updateLocation)
        {
            _response = response;
            UseLastLocationCommand = new Command(UseLastLocation, () => CanUseLastLocation);
            StartSurveyCommand = new Command(StartSurvey, () => CanGoForward);
            UpdateLocationCommand = new Command(UseCurrentLocation);
            IsUpdateLocation = updateLocation;

            if (!updateLocation)
            {
                IsBusy = true;
                InitializeLocation();
            }
            else
            {
                Position = PrettyPrintPosition();
            }
        }

        public Command UseLastLocationCommand { get; }
        
        public Command StartSurveyCommand { get; }

        public Command UpdateLocationCommand { get; }

        public bool IsUpdateLocation { get; }

        public bool IsInitialLocation => !IsUpdateLocation;

        public string Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }   
        }

        public string Street
        {
            get { return _response.Item.NearestAddress.Street; }
            set
            {
                if (_response.Item.NearestAddress.Street != value)
                {
                    _response.Item.NearestAddress.Street = value;
                    StartSurveyCommand.ChangeCanExecute();
                    OnPropertyChanged();
                }
            }
        }

        public string City
        {
            get { return _response.Item.NearestAddress.City; }
            set
            {
                if (_response.Item.NearestAddress.City != value)
                {
                    _response.Item.NearestAddress.City = value;
                    StartSurveyCommand.ChangeCanExecute();
                    OnPropertyChanged();
                }
            }
        }

        public string State
        {
            get { return _response.Item.NearestAddress.State; }
            set
            {
                if (_response.Item.NearestAddress.State != value)
                {
                    _response.Item.NearestAddress.State = value;
                    StartSurveyCommand.ChangeCanExecute();
                    OnPropertyChanged();
                }
            }
        }

        public string ZipCode
        {
            get { return _response.Item.NearestAddress.ZipCode; }
            set
            {
                if (_response.Item.NearestAddress.ZipCode != value)
                {
                    _response.Item.NearestAddress.ZipCode = value;
                    StartSurveyCommand.ChangeCanExecute();
                    OnPropertyChanged();
                }
            }
        }

        public string LocationNotes
        {
            get { return _response.Item.LocationNotes; }
            set
            {
                if (_response.Item.LocationNotes != value)
                {
                    _response.Item.LocationNotes = value;
                    OnPropertyChanged();
                }
            }
        }

        public Task SaveAsync()
        {
            UpdateLastLocation();
            return _response.SaveAsync();
        }

        private bool CanGoForward =>
            !string.IsNullOrEmpty(Street) &&
            !string.IsNullOrEmpty(City) &&
            !string.IsNullOrEmpty(State) &&
            !string.IsNullOrEmpty(ZipCode);

        private bool CanUseLastLocation =>
            !string.IsNullOrEmpty(s_lastStreet) ||
            !string.IsNullOrEmpty(s_lastCity) ||
            !string.IsNullOrEmpty(s_lastState) ||
            !string.IsNullOrEmpty(s_lastZipCode) ||
            !string.IsNullOrEmpty(s_lastLocationNotes);

        private void UseLastLocation()
        {
            Street = s_lastStreet;
            City = s_lastCity;
            State = s_lastState;
            ZipCode = s_lastZipCode;
            LocationNotes = s_lastLocationNotes;
        }

        private async void StartSurvey()
        {
            UpdateLastLocation();

            // Save the current page instance reference for removal
            var currentPage = App.NavigationPage.CurrentPage;

            await App.NavigationPage.PushAsync(new SurveyPage(_response));

            // Since the user can always edit the location from the survey page toolbar,
            // we don't need to keep this location page in the navigation stack
            App.NavigationPage.Navigation.RemovePage(currentPage);
        }

        private void UseCurrentLocation()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("UseCurrentLocation");
            IsBusy = true;
            InitializeLocation();
        }

        private void UpdateLastLocation()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("UseLastLocation");
            s_lastStreet = Street;
            s_lastCity = City;
            s_lastState = State;
            s_lastZipCode = ZipCode;
            s_lastLocationNotes = LocationNotes;
            UseLastLocationCommand.ChangeCanExecute();
        }

        private async void InitializeLocation()
        {
            try
            {
                // Check if the geolocator is available
                var geolocator = CrossGeolocator.Current;
                if (geolocator != null && geolocator.IsGeolocationEnabled && geolocator.IsGeolocationAvailable)
                {
                    // Start a timer that queues the position request timeout
                    var timeoutTask = Task.Delay(LocationTimeoutMilliseconds);
                    // Start a task to get the current position
                    var positionTask = geolocator.GetPositionAsync();
                    // Wait for the position or timeout task to complete
                    var positionOrTimeoutTask = await Task.WhenAny(positionTask, timeoutTask);
                    // If it was the position task that completed first...
                    if (positionOrTimeoutTask != timeoutTask)
                    {
                        // Update the survey GPS location
                        var position = await positionTask;
                        _response.Item.GPSLocation.Lat = position.Latitude;
                        _response.Item.GPSLocation.Lon = position.Longitude;
                        _response.Item.GPSLocation.Accuracy = (float)position.Accuracy;
                        // Try to reverse geocode the position to a nearby address
                        var address = await Geocoder.ReverseGeocodeAsync(position.Latitude, position.Longitude);
                        if (address != null)
                        {
                            Street = address.AddressLine;
                            City = address.Locality;
                            State = address.AdminDistrict;
                            ZipCode = address.PostalCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("ReverseGeocodeFailed", ex);
            }
            finally
            {
                Position = PrettyPrintPosition();
                IsBusy = false;
            }
        }

        private string PrettyPrintPosition()
        {
            var gpsLocation = _response.Item.GPSLocation;
            return gpsLocation.Lat.HasValue
                ? $"{gpsLocation.Lat:N4}, {gpsLocation.Lon:N4}"
                : "Position not currently available";
        }
    }
}
