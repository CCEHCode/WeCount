using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Views;
using PITCSurveyLib.Models;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class SurveyLocationViewModel : BaseViewModel
    {
        private static string s_lastStreet;
        private static string s_lastCity;
        private static string s_lastState;
        private static string s_lastZipCode;
        private static string s_lastLocationNotes;

        private readonly UploadedItem<SurveyResponseModel> _response;

        private string _position = "Loading...";

        public SurveyLocationViewModel()
            : this(new UploadedItem<SurveyResponseModel>(SurveyResponseModelExtensions.CreateNew()))
        {
        }

        public SurveyLocationViewModel(UploadedItem<SurveyResponseModel> response)
        {
            _response = response;
            UseLastLocationCommand = new Command(UseLastLocation, () => CanUseLastLocation);
            StartSurveyCommand = new Command(StartSurvey, () => CanGoForward);
            IsBusy = true;
            Init();
        }

        public Command UseLastLocationCommand { get; }
        
        public Command StartSurveyCommand { get; }

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
            await App.NavigationPage.PushAsync(new SurveyPage(_response));
        }

        private void UpdateLastLocation()
        {
            s_lastStreet = Street;
            s_lastCity = City;
            s_lastState = State;
            s_lastZipCode = ZipCode;
            s_lastLocationNotes = LocationNotes;
            UseLastLocationCommand.ChangeCanExecute();
        }

        private async void Init()
        {
            try
            {
                var geolocator = CrossGeolocator.Current;
                if (geolocator != null && geolocator.IsGeolocationEnabled && geolocator.IsGeolocationAvailable)
                {
                    var position = await geolocator.GetPositionAsync();
                    Position = $"{position.Latitude:N4}, {position.Longitude:N4}";
                    _response.Item.GPSLocation.Lat = position.Latitude;
                    _response.Item.GPSLocation.Lon = position.Longitude;
                    _response.Item.GPSLocation.Accuracy = (float) position.Accuracy;
                    var address = await Geocoder.ReverseGeocode(position.Latitude, position.Longitude);
                    Street = address.AddressLine;
                    City = address.AdminDistrict;
                    State = address.Locality;
                    ZipCode = address.PostalCode;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
