namespace EcoPlatesMobile.Views.User.Components;

public partial class TabSwitcherView : ContentView
{
    public event EventHandler<string>? TabChanged;

    public static readonly BindableProperty Tab1_TitleProperty =
       BindableProperty.Create(nameof(Tab1_Title), typeof(string), typeof(TabSwitcherView), default(string), propertyChanged: Tab1_TitleChanged);

    public static readonly BindableProperty Tab2_TitleProperty =
       BindableProperty.Create(nameof(Tab2_Title), typeof(string), typeof(TabSwitcherView), default(string), propertyChanged: Tab2_TitleChanged);

    public string Tab1_Title
    {
        get => (string)GetValue(Tab1_TitleProperty);
        set => SetValue(Tab1_TitleProperty, value);
    }

    public string Tab2_Title
    {
        get => (string)GetValue(Tab2_TitleProperty);
        set => SetValue(Tab2_TitleProperty, value);
    }

    public TabSwitcherView()
    {
        InitializeComponent();
    }

    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is Button clickedButton)
        {
            if (clickedButton == button1)
            {
                SetActiveTab(button1);
                TabChanged?.Invoke(this, Tab1_Title);
            }
            else if (clickedButton == button2)
            {
                SetActiveTab(button2);
                TabChanged?.Invoke(this, Tab2_Title);
            }
        }
    }

    private static void Tab1_TitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TabSwitcherView)bindable;
        control.button1.Text = (string)newValue;
    }

    private static void Tab2_TitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TabSwitcherView)bindable;
        control.button2.Text = (string)newValue;
    }

    private void SetActiveTab(Button activeButton)
    {
        button1.BackgroundColor = activeButton == button1 ? Colors.White : Colors.Transparent;
        button1.TextColor = activeButton == button1 ? Colors.Black : Colors.Gray;

        button2.BackgroundColor = activeButton == button2 ? Colors.White : Colors.Transparent;
        button2.TextColor = activeButton == button2 ? Colors.Black : Colors.Gray;
    }
}