﻿using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0; 
        private readonly MainViewModel ViewModel = new();
        private enum Row { TextEntry }
        private enum Column { Description, Input }

        public MainPage()
        {
            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.TextEntry, 36)
                ),
                ColumnDefinitions = Columns.Define(
                    (Column.Description, Star),
                    (Column.Input, Stars(2))
                ),

                Children =
                {
                    new Label()
                        .Text("Customer Name:")
                        .Row(Row.TextEntry).Column(Column.Description),

                    new Entry()
                    {
                        Keyboard = Keyboard.Numeric,
                        BackgroundColor = Colors.AliceBlue,
                    }.Row(Row.TextEntry).Column(Column.Input)
                        .FontSize(15)
                        .Placeholder("Enter Name")
                        .TextColor(Colors.Black)
                        .Height(44)
                        .Margin(6, 6)
                        .Bind(Entry.TextProperty, nameof(ViewModel.Name), BindingMode.TwoWay)
                }
            };
        }
    }

}
