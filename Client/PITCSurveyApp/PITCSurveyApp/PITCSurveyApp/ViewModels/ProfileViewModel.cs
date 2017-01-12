using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class ProfileViewModel : BaseViewModel
    {
        private const int SaveButtonMessageDelay = 3000;
        private const string DefaultSaveButtonText = "Save Profile";

        private string _saveButtonText = DefaultSaveButtonText;

        private string _currentFirstName;
        private string _currentLastName;
        private string _currentEmail;
        private string _currentMobilePhone;
        private string _currentHomePhone;
        private string _currentStreet;
        private string _currentCity;
        private string _currentState;
        private string _currentZipCode;

        public ProfileViewModel()
        {
            MicrosoftLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.MicrosoftAccount));
            GoogleLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.Google));
            LogoutCommand = new Command(Logout);
            SaveProfileCommand = new Command(SaveProfile, () => CanSaveProfile);
            UpdateCurrentInfo();
        }

        public Command MicrosoftLoginCommand { get; }

        public Command GoogleLoginCommand { get; }

        public Command LogoutCommand { get; }

        public Command SaveProfileCommand { get; }

        public bool IsAnonymous => !UserSettings.IsLoggedIn;

        public bool IsLoggedIn => UserSettings.IsLoggedIn;

        public bool HasProfileChanged =>
            FirstName != _currentFirstName ||
            LastName != _currentLastName ||
            Email != _currentEmail ||
            MobilePhone != _currentMobilePhone ||
            HomePhone != _currentHomePhone ||
            Street != _currentStreet ||
            City != _currentCity ||
            State != _currentState ||
            ZipCode != _currentZipCode;

        public bool CanSaveProfile =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(MobilePhone) &&
            HasProfileChanged;

        public string SaveButtonText
        {
            get { return _saveButtonText; }
            set { SetProperty(ref _saveButtonText, value); }
        }

        public string FirstName
        {
            get { return UserSettings.Volunteer?.FirstName; }
            set
            {
                if (UserSettings.Volunteer.FirstName != value)
                {
                    UserSettings.Volunteer.FirstName = value;
                    OnPropertyChanged();
                    SaveProfileCommand.ChangeCanExecute();
                }
            }
        }

        public string LastName
        {
            get { return UserSettings.Volunteer?.LastName; }
            set
            {
                if (UserSettings.Volunteer.LastName != value)
                {
                    UserSettings.Volunteer.LastName = value;
                    OnPropertyChanged();
                    SaveProfileCommand.ChangeCanExecute();
                }
            }
        }

        public string Email
        {
            get { return UserSettings.Volunteer?.Email; }
            set
            {
                if (UserSettings.Volunteer.Email != value)
                {
                    UserSettings.Volunteer.Email = value;
                    OnPropertyChanged();
                    SaveProfileCommand.ChangeCanExecute();
                }
            }
        }

        public string MobilePhone
        {
            get { return UserSettings.Volunteer?.MobilePhone; }
            set
            {
                if (UserSettings.Volunteer.MobilePhone != value)
                {
                    UserSettings.Volunteer.MobilePhone = value;
                    OnPropertyChanged();
                    SaveProfileCommand.ChangeCanExecute();
                }
            }
        }

        public string HomePhone
        {
            get { return UserSettings.Volunteer?.HomePhone; }
            set
            {
                if (UserSettings.Volunteer.HomePhone != value)
                {
                    UserSettings.Volunteer.HomePhone = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Street
        {
            get { return UserSettings.Volunteer?.Address.Street; }
            set
            {
                if (UserSettings.Volunteer.Address.Street != value)
                {
                    UserSettings.Volunteer.Address.Street = value;
                    OnPropertyChanged();
                }
            }
        }

        public string City
        {
            get { return UserSettings.Volunteer?.Address.City; }
            set
            {
                if (UserSettings.Volunteer.Address.City != value)
                {
                    UserSettings.Volunteer.Address.City = value;
                    OnPropertyChanged();
                }
            }
        }

        public string State
        {
            get { return UserSettings.Volunteer?.Address.State; }
            set
            {
                if (UserSettings.Volunteer.Address.State != value)
                {
                    UserSettings.Volunteer.Address.State = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ZipCode
        {
            get { return UserSettings.Volunteer?.Address.ZipCode; }
            set
            {
                if (UserSettings.Volunteer.Address.ZipCode != value)
                {
                    UserSettings.Volunteer.Address.ZipCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ChangeAllProperties()
        {
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(MobilePhone));
            OnPropertyChanged(nameof(HomePhone));
            OnPropertyChanged(nameof(Street));
            OnPropertyChanged(nameof(City));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(ZipCode));
        }

        private void UpdateCurrentInfo()
        {
            _currentFirstName = FirstName;
            _currentLastName = LastName;
            _currentEmail = Email;
            _currentMobilePhone = MobilePhone;
            _currentHomePhone = HomePhone;
            _currentStreet = Street;
            _currentCity = City;
            _currentState = State;
            _currentZipCode = ZipCode;
        }

        private async void SignIn(MobileServiceAuthenticationProvider provider)
        {
            await App.LoginAsync(provider);
            OnPropertyChanged(nameof(IsAnonymous));
            OnPropertyChanged(nameof(IsLoggedIn));
            ChangeAllProperties();
            UpdateCurrentInfo();
        }

        private async void Logout()
        {
            await App.LogoutAsync();
            OnPropertyChanged(nameof(IsAnonymous));
            OnPropertyChanged(nameof(IsLoggedIn));
            ChangeAllProperties();
            UpdateCurrentInfo();
        }

        private async void SaveProfile()
        {
            try
            {
                SaveButtonText = "Saving...";
                IsBusy = true;
                SaveProfileCommand.ChangeCanExecute();
                DependencyService.Get<IMetricsManagerService>().TrackEvent("SaveVolunteer");
                await SurveyCloudService.SaveVolunteerAsync(UserSettings.Volunteer);
                SaveButtonText = "Saved!";
                UpdateCurrentInfo();
                IsBusy = false;
                SaveProfileCommand.ChangeCanExecute();
                await Task.Delay(SaveButtonMessageDelay);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("SaveVolunteerFailed", ex);
                App.DisplayAlert("Profile Save Error", "Failed to update profile information. Please try again later.", "OK");
            }
            finally
            {
                IsBusy = false;
                SaveProfileCommand.ChangeCanExecute();
                SaveButtonText = DefaultSaveButtonText;
            }
        }
    }
}
