using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class ContentButton : ContentView
    {
        public ContentButton()
        {
            InitializeComponent();
        }

        public event EventHandler Tapped;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ContentButton), null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(this, new EventArgs());
        }
    }
}
