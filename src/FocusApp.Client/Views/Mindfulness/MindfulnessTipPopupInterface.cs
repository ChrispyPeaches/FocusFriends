using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Methods.MindfulnessTip;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Models;
using FocusCore.Extensions;
using MediatR;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FocusApp.Client.Views.Mindfulness;

internal class MindfulnessTipPopupInterface : BasePopup, INotifyPropertyChanged
{
    private Helpers.PopupService _popupService;
    Grid _popupContent;
    private readonly IMediator _mediator;

    private HtmlWebViewSource _tipHtmlSource;
    public HtmlWebViewSource TipHtmlSource
    {
        get => _tipHtmlSource;
        private set => SetProperty(ref _tipHtmlSource, value);
    }

    /// <summary>
    /// Determines whether to show the loading spinner or not.
    /// </summary>
    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    enum Row { TopBar, TipDisplay}

    public MindfulnessTipPopupInterface(Helpers.PopupService popupService, IMediator mediator)
    {
        _popupService = popupService;
        _mediator = mediator;

        // Set popup location
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;

        Color = Colors.Transparent;

        TipHtmlSource = new HtmlWebViewSource() { Html = "" };
        IsBusy = true;

        CanBeDismissedByTappingOutsideOfPopup = false;

        Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = Colors.White,
                WidthRequest = 360,
                HeightRequest = 460,
                Content = new Grid()
                {
                    RowDefinitions = GridRowsColumns.Rows.Define(
                        (Row.TopBar, 50),
                        (Row.TipDisplay, GridRowsColumns.Stars(1))
                    ),
                    Children =
                    {
                        // Dismiss Popup Button
                        new Button
                            {
                                Text = SolidIcons.x,
                                TextColor = Colors.Black,
                                FontFamily = nameof(SolidIcons),
                                FontSize = 20,
                                BackgroundColor = Colors.Transparent
                            }
                            .ZIndex(1)
                            .Right()
                            .CenterVertical()
                            .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                            .Row(Row.TopBar)
                            // When clicked, close the popup
                            .Invoke(button => button.Released += OnDismissPopup),
                        new WebView()
                            .ZIndex(1)
                            .Row(Row.TipDisplay)
                            .Bind(WebView.SourceProperty,
                                getter: (MindfulnessTipPopupInterface popup) => popup.TipHtmlSource, source: this),
                        new ActivityIndicator()
                            {
                                Color = AppStyles.Palette.OrchidPink
                            }
                            .ZIndex(2)
                            .RowSpan(typeof(Row).GetEnumNames().Length)
                            .Center()
                            .Bind(ActivityIndicator.IsRunningProperty,
                                getter: (MindfulnessTipPopupInterface popup) => popup.IsBusy, source: this)
                    }
                }
            }
            .Padding(horizontalSize: 10, verticalSize: 0);
    }

    public async Task PopulatePopup(
        MindfulnessTipExtensions.FocusSessionRating sessionRating,
        CancellationToken cancellationToken)
    {
        // Get tip
        MindfulnessTip? tip = null;
        try
        {
            tip = await _mediator.Send(
                new GetMindfulnessTipByRatingLevel.Query() { RatingLevel = sessionRating },
                cancellationToken);
        }
        catch (Exception e)
        {
            // Handle exception
        }
            
        // Display tip and hide loading spinner or close popup if tip retrieval failed
        if (tip != null)
        {
            IsBusy = false;
            TipHtmlSource = new HtmlWebViewSource() { Html = tip.Content };
        }
        else
        {
            _popupService.HidePopup();
        }
    }

    // Navigate to page according to button
    private async void OnDismissPopup(object? sender, EventArgs e)
    {
        _popupService.HidePopup();
    }

    #region Property Changed Notification Logic

    private void SetProperty<T>(ref T backingStore, in T value, [CallerMemberName] in string propertyname = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return;
        }

        backingStore = value;

        OnPropertyChanged(propertyname);
    }

    void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion

}