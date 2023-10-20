using DevControlsMAUI;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UIDevControlsMAUI
{
    [ObservableObject]
    partial class Person
    {
        [ObservableProperty]
        string nombre;
    }

    public partial class MainPage : ContentPage
    {
        int count = 0;
        readonly Person VM = new Person();
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = VM;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            VM.Nombre = Guid.NewGuid().ToString("n");
            

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}