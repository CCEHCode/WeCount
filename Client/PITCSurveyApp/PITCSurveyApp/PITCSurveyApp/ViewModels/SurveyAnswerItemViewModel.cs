using PITCSurveyLib;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    public class SurveyAnswerItemViewModel : BaseViewModel
    {
        private readonly SurveyQuestionAnswerChoiceModel _item;
        private readonly SurveyQuestionAnswerChoiceResponseModel _answer;

        private bool _isSelected;
        private bool _isSpecifiable;
        private Color _backgroundColor;

        public SurveyAnswerItemViewModel(
            SurveyQuestionAnswerChoiceModel item,
            SurveyQuestionAnswerChoiceResponseModel answer)
            : this(item, answer, false)
        {
        }

        public SurveyAnswerItemViewModel(
            SurveyQuestionAnswerChoiceModel item,
            SurveyQuestionAnswerChoiceResponseModel answer,
            bool isSelected)
        {
            _item = item;
            _answer = answer;
            _isSelected = isSelected;
            AnswerSelectedCommand = new Command(AnswerSelected);
            UpdateProperties();
        }

        public Command AnswerSelectedCommand { get; }

        public SurveyQuestionAnswerChoiceModel Item => _item;

        public SurveyQuestionAnswerChoiceResponseModel Answer => _answer;

        public string Name => !string.IsNullOrWhiteSpace(_item.AnswerChoiceText) ? _item.AnswerChoiceText : "Specify:";

        public Keyboard Keyboard =>
            Item.AdditionalAnswerDataFormat == AnswerFormat.Int
                ? Keyboard.Numeric
                : Keyboard.Default;

        public string Placeholder =>
            Item.AdditionalAnswerDataFormat == AnswerFormat.Date
                ? "MM/DD/YYYY"
                : string.Empty;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
                UpdateProperties();
            }
        }

        public bool IsSpecifiable
        {
            get { return _isSpecifiable; }
            set { SetProperty(ref _isSpecifiable, value); }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        public string SpecifiedText
        {
            get { return _answer.AdditionalAnswerData; }
            set { _answer.AdditionalAnswerData = value; }
        }

        private void UpdateProperties()
        {
            IsSpecifiable = _isSelected && Item.AdditionalAnswerDataFormat != AnswerFormat.None;
            BackgroundColor = _isSelected
                ? (Color)Application.Current.Resources["DarkBackgroundColor"]
                : (Color)Application.Current.Resources["LightBackgroundColor"];
        }

        private void AnswerSelected()
        {
            IsSelected = !_isSelected;
        }
    }
}
