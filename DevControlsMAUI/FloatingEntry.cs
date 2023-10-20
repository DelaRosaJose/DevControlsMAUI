namespace DevControlsMAUI;

public class FloatingEntry : ContentView
{
    private const int _placeholderFontSize = 18;
    private const int _titleFontSize = 12;
    private const int _topMargin = -24;
    readonly Entry Field = new Entry();
    readonly Label LabelTitle = new Label();
    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();


    public FloatingEntry()
    {
        Field.Focused += Handle_Focused;
        Field.Unfocused += Handle_Unfocused;
        tapGestureRecognizer.Tapped += (s, e) => Field.Focus();
        LabelTitle.GestureRecognizers.Add(tapGestureRecognizer);

        Field.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        LabelTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), BindingMode.TwoWay, source: this));

        Content = new Grid
        {
            Children = { Field, LabelTitle }
        };
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingEntry), string.Empty, BindingMode.TwoWay, null, HandleBindingPropertyChangedDelegate);
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(string), "SetTheTitle", BindingMode.TwoWay, null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    async void Handle_Focused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
        {
            await TransitionToTitle(true);
        }
    }
    async void Handle_Unfocused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
        {
            await TransitionToPlaceholder(true);
        }
    }

    static async void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as FloatingEntry;
        if (!control.Field.IsFocused)
        {
            if (!string.IsNullOrEmpty((string)newValue))
            {
                await control.TransitionToTitle(false);
            }
            else
            {
                await control.TransitionToPlaceholder(false);
            }
        }

    }
    async Task TransitionToTitle(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(0, _topMargin, 100);
            var t2 = SizeTo(_titleFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 0;
            LabelTitle.TranslationY = -30;
            LabelTitle.FontSize = 14;
        }
    }
    async Task TransitionToPlaceholder(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(10, 0, 100);
            var t2 = SizeTo(_placeholderFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 10;
            LabelTitle.TranslationY = 0;
            LabelTitle.FontSize = _placeholderFontSize;
        }
    }
    Task SizeTo(int fontSize)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        var startingHeight = LabelTitle.FontSize;
        var endingHeight = fontSize;
        var rate = 5u;
        var length = 100u;

        LabelTitle.Animate("size", callback, startingHeight, endingHeight, rate, length, Easing.Linear, (v, c) => taskCompletionSource.SetResult(c));

        return taskCompletionSource.Task;

        void callback(double input)
        {
            LabelTitle.FontSize = input;
        }
    }
}