using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PITCSurveyApp.ViewModels;
using PITCSurveyLib;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class SurveyPage : ContentPage
    {
        private readonly SurveyViewModel _viewModel;

        public SurveyPage()
        {
            InitializeComponent();

            _viewModel = new SurveyViewModel();
            _viewModel.QuestionChanged += (sender, e) => UpdateQuestion();
            BindingContext = _viewModel;
            UpdateQuestion();
        }

	    private void UpdateQuestion()
	    {
            AnswerOptionsStackLayout.Children.Clear();
            var q = _viewModel.CurrentQuestion;
            try
            {
                Title = $"Survey Question {q.QuestionNum} of {_viewModel.SurveyQuestionsCount}";
                QuestionLabel.Text = q.QuestionText;
                HelpTextLabel.Text = q.QuestionHelpText;

                var listView = new ListView();
                listView.HasUnevenRows = true;
                listView.ItemTemplate = new DataTemplate(typeof(WrappedItemSelectionTemplate));
                listView.ItemsSource = CreateChoices(q, _viewModel.CurrentAnswers);
                listView.ItemSelected += (sender, e) =>
                {
                    var answer = (WrappedAnswerChoice)e.SelectedItem;
                    answer.IsSelected = !answer.IsSelected;
                };

                AnswerOptionsStackLayout.Children.Add(listView);
            }
            catch (Exception e)
            {
                // TODO: provide better details, log in HockeyApp, etc.
                DisplayAlert("Error", "Something went wrong when loading this question.", "OK");
            }
        }

        private IList<WrappedAnswerChoice> CreateChoices(
            SurveyQuestionModel q, 
            IList<SurveyQuestionAnswerChoiceResponseModel> previousAnswers)
        {
            var choices = new List<WrappedAnswerChoice>(q.AnswerChoices.Count);
            foreach (var choice in q.AnswerChoices)
            {
                var previousAnswer = previousAnswers.FirstOrDefault(a => a.AnswerChoiceID == choice.AnswerChoiceID);
                choices.Add(previousAnswer != null
                    ? new WrappedAnswerChoice(choice, previousAnswer, true)
                    : new WrappedAnswerChoice(
                        choice,
                        new SurveyQuestionAnswerChoiceResponseModel
                        {
                            QuestionID = q.QuestionID,
                            AnswerChoiceID = choice.AnswerChoiceID,
                        }));
            }

            foreach (var choice in choices)
            {
                choice.PropertyChanged += (sender, e) =>
                {
                    if (!q.AllowMultipleAnswers && e.PropertyName == nameof(WrappedAnswerChoice.IsSelected) && choice.IsSelected)
                    {
                        foreach (var otherChoice in choices)
                        {
                            if (otherChoice != choice)
                            {
                                otherChoice.IsSelected = false;
                            }
                        }
                    }

                    if (choice.IsSelected)
                    {
                        _viewModel.AddAnswer(choice.Answer);
                    }
                    else
                    {
                        _viewModel.RemoveAnswer(choice.Answer);
                    }

                    _viewModel.UpdateCommands();
                };
            }

            return choices;
        }

        class WrappedAnswerChoice : INotifyPropertyChanged
        {
            private readonly SurveyQuestionAnswerChoiceModel _item;
            private readonly SurveyQuestionAnswerChoiceResponseModel _answer;

            private bool _isSelected = false;
            private bool _isSpecifiable = false;
            private Keyboard _keyboard = Keyboard.Default;

            public WrappedAnswerChoice(
                SurveyQuestionAnswerChoiceModel item,
                SurveyQuestionAnswerChoiceResponseModel answer)
                : this(item, answer, false)
            {
            }

            public WrappedAnswerChoice(
                SurveyQuestionAnswerChoiceModel item, 
                SurveyQuestionAnswerChoiceResponseModel answer, 
                bool isSelected)
            {
                _item = item;
                _answer = answer;
                _isSelected = isSelected;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public SurveyQuestionAnswerChoiceModel Item => _item;

            public SurveyQuestionAnswerChoiceResponseModel Answer => _answer;

            public string Name => _item.AnswerChoiceText;

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
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        PropertyChanged (this, new PropertyChangedEventArgs(nameof(IsSelected))); // C# 6
                        UpdateSpecifiable();
                    }
                }
            }

            public bool IsSpecifiable
            {
                get { return _isSpecifiable; }
                set
                {
                    if (_isSpecifiable != value)
                    {
                        _isSpecifiable = value;
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSpecifiable)));
                    }
                }
            }

            public string Text
            {
                get { return _answer.AdditionalAnswerData; }
                set { _answer.AdditionalAnswerData = value; }
            }

            private void UpdateSpecifiable()
            {
                IsSpecifiable = _isSelected && Item.AdditionalAnswerDataFormat != AnswerFormat.None;
            }
        }

        class WrappedItemSelectionTemplate : ViewCell
        {
            public WrappedItemSelectionTemplate()
            {
                var name = new Label
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                name.SetBinding(Label.TextProperty, new Binding(nameof(WrappedAnswerChoice.Name)));

                var mainSwitch = new Switch
                {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                mainSwitch.SetBinding(Switch.IsToggledProperty, new Binding(nameof(WrappedAnswerChoice.IsSelected)));

                var answerLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                };

                answerLayout.Children.Add(name);
                answerLayout.Children.Add(mainSwitch);
                var entry = new Entry
                {
                    HorizontalOptions = LayoutOptions.Fill,
                };

                entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(WrappedAnswerChoice.Placeholder)));
                entry.SetBinding(InputView.KeyboardProperty, new Binding(nameof(WrappedAnswerChoice.Keyboard)));
                entry.SetBinding(IsVisibleProperty, new Binding(nameof(WrappedAnswerChoice.IsSpecifiable)));
                entry.SetBinding(Entry.TextProperty, new Binding(nameof(WrappedAnswerChoice.Text), BindingMode.TwoWay));

                var stackLayout = new StackLayout();
                stackLayout.Children.Add(answerLayout);
                stackLayout.Children.Add(entry);
                View = stackLayout;
            }
        }
    }
}
