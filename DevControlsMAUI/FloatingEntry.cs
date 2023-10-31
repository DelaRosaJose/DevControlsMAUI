
using Microsoft.Maui.Converters;
using System.ComponentModel;

namespace DevControlsMAUI;

public class FloatingEntry : ContentView
{
    private const int _placeholderFontSize = 18;
    private const int _titleFontSize = 15;
    private const int _topMargin = -24;
    readonly Entry Field = new Entry();
    readonly Label LabelTitle = new Label() { VerticalOptions = LayoutOptions.Center };
    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();


    public FloatingEntry()
    {
        Field.Focused += Handle_Focused;
        Field.Unfocused += Handle_Unfocused;
        tapGestureRecognizer.Tapped += (s, e) => Field.Focus();
        LabelTitle.GestureRecognizers.Add(tapGestureRecognizer);

        #region SetBindings
        Field.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        Field.SetBinding(Entry.IsPasswordProperty, new Binding(nameof(IsPassword), BindingMode.TwoWay, source: this));
        Field.SetBinding(Entry.KeyboardProperty, new Binding(nameof(KeyBoard), BindingMode.TwoWay, source: this));

        LabelTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), BindingMode.TwoWay, source: this));
        #endregion

        Content = new Grid
        {
            Children = { Field, LabelTitle }
        };
    }

    #region BindableProperties

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingEntry), string.Empty, BindingMode.TwoWay, null, HandleBindingPropertyChangedDelegate);
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(FloatingEntry), "SetTheTitle", BindingMode.TwoWay, null);
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FloatingEntry), default, BindingMode.TwoWay, null);
    public static readonly BindableProperty KeyBoardProperty = BindableProperty.Create(nameof(KeyBoard), typeof(Keyboard), typeof(FloatingEntry), default, BindingMode.TwoWay, null);

    #endregion
    #region Properties

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
    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    //[TypeConverter(typeof(KeyboardTypeConverter))]
    public Keyboard KeyBoard
    {
        get => (Keyboard)GetValue(KeyBoardProperty);
        set => SetValue(KeyBoardProperty, value);
    }

    #endregion
    #region EventHandlers

    async void Handle_Focused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
            await TransitionToTitle();
    }

    async void Handle_Unfocused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
            await TransitionToPlaceholder(true);
    }

    static async void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as FloatingEntry;
        if (!control.Field.IsFocused)
        {
            if (!string.IsNullOrEmpty((string)newValue))
            {
                await control.TransitionToTitle();
            }
            else
            {
                await control.TransitionToPlaceholder(false);
            }
        }

    }
    #endregion
    #region Methods

    async Task TransitionToTitle()
    {
        var t1 = LabelTitle.TranslateTo(10, _topMargin, 100);
        var t2 = SizeTo(_titleFontSize);
        await Task.WhenAll(t1, t2);
    }
    async Task TransitionToPlaceholder(bool animated)
    {
        var t1 = LabelTitle.TranslateTo(10, 0, 100);
        var t2 = SizeTo(_placeholderFontSize);
        await Task.WhenAll(t1, t2);
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
    #endregion

}