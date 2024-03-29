using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;
using CommunityToolkit.Maui.Converters;
using FocusApp.Client.Clients;

namespace FocusApp.Client.Views.Social
{
    internal class LeaderboardsPage : BasePage
    {
        // Row / Column structure for entire page
        enum PageRow { PageHeader, LeaderboardSelectors, TopThreeFriendsDisplay, RemainingFriendsDisplay, BottomWhiteSpace }
        enum PageColumn { DailyLeadboardButton, WeeklyLeaderboardButton }

        // Row / Column structure for top three friends grid
        enum TopThreeRow { TopProfilePicture, GoldPillar, SilverPillar, BronzePillar }
        enum TopThreeColumn { Left, Center, Right }

        // Row / Column structure for remaining friends entries
        enum RemainingFriendsColumn { Picture, Name, Score }

        Grid _topThreeFriendsGrid { get; set; }
        StackLayout _remainingFriendsContent { get; set; }

        // Top three friends image, score, and username references
        Image _firstPlacePicture { get; set; }
        Label _firstPlaceScore { get; set; }
        Label _firstPlaceUsername { get; set; }
        Image _secondPlacePicture { get; set; }
        Label _secondPlaceScore { get; set; }
        Label _secondPlaceUsername { get; set; }
        Image _thirdPlacePicture { get; set; }
        Label _thirdPlaceScore { get; set; }
        Label _thirdPlaceUsername { get; set; }

        IAPIClient _client { get; set; }
        public LeaderboardsPage(IAPIClient client)
        {
            _client = client;
            _topThreeFriendsGrid = GetTopThreeFriendsGrid();
            ScrollView remainingFriendsScrollView = GetRemainingFriendsScrollView();

            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (PageRow.PageHeader, GridRowsColumns.Stars(1)),
                    (PageRow.LeaderboardSelectors, GridRowsColumns.Stars(1)),
                    (PageRow.TopThreeFriendsDisplay, GridRowsColumns.Stars(4.5)),
                    (PageRow.RemainingFriendsDisplay, GridRowsColumns.Stars(3.5)),
                    (PageRow.BottomWhiteSpace, GridRowsColumns.Stars(1.25))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (PageColumn.DailyLeadboardButton, GridRowsColumns.Stars(1)),
                    (PageColumn.WeeklyLeaderboardButton, GridRowsColumns.Stars(1))
                    ),
                BackgroundColor = AppStyles.Palette.OrchidPink,
                Children =
                { 
                    // Back Button
				    new Button
                    {
                        Text = SolidIcons.ChevronLeft,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Row(PageRow.PageHeader)
                    .Left()
                    .CenterVertical()
				    // When clicked, go to social view
				    .Invoke(button => button.Released += (sender, eventArgs) =>
                        BackButtonClicked(sender, eventArgs)),

                    // Page Header
                    new Label
                    { 
                        Text = "Leaderboards",
                        TextColor = Colors.Black,
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Row(PageRow.PageHeader)
                    .Top()
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .CenterHorizontal()
                    .CenterVertical(),

                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        HeightRequest = 2,
                    }
                    .Row(PageRow.PageHeader)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .Bottom(),

                    // Daily Leaderboards Button
                    new Button
                    { 
                        Text = "Daily",
                        Margin = 15
                    }
                    .Row(PageRow.LeaderboardSelectors)
                    .Column(PageColumn.DailyLeadboardButton)
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                        GetDailyLeaderboards(sender, eventArgs)),

                    // Weekly Leaderboards Button
                    new Button
                    {
                        Text = "Weekly",
                        Margin = 15
                    }
                    .Row(PageRow.LeaderboardSelectors)
                    .Column(PageColumn.WeeklyLeaderboardButton)
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                        GetWeeklyLeaderboards(sender, eventArgs)),

                    // Top Three Friends Leaderboard Display
                    _topThreeFriendsGrid
                    .Row(PageRow.TopThreeFriendsDisplay)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                    // Remaining Friends Leaderboard Display
                    remainingFriendsScrollView
                    .Row(PageRow.RemainingFriendsDisplay)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                }
            };
        }

        Grid GetTopThreeFriendsGrid()
        {
            SetTopThreeDynamicElements();

            // Top Three Friends Leaderboard Display
            Grid topThreeFriendsGrid = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (TopThreeRow.TopProfilePicture, GridRowsColumns.Stars(1)),
                    (TopThreeRow.GoldPillar, GridRowsColumns.Stars(1)),
                    (TopThreeRow.SilverPillar, GridRowsColumns.Stars(1)),
                    (TopThreeRow.BronzePillar, GridRowsColumns.Stars(1))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (TopThreeColumn.Left, GridRowsColumns.Stars(1)),
                    (TopThreeColumn.Center, GridRowsColumns.Stars(1)),
                    (TopThreeColumn.Right, GridRowsColumns.Stars(1))
                    ),
                Children =
                {
                    // Bronze Pillar / Third Place Information
                    new Frame
                    {
                        BackgroundColor = Colors.RosyBrown,
                        Content = new StackLayout 
                        {
                            _thirdPlaceScore,
                            _thirdPlaceUsername
                        }
                    }
                    .Row(TopThreeRow.BronzePillar)
                    .Column(TopThreeColumn.Left),

                    // Third Place Friend Picture
                    _thirdPlacePicture
                    .Row(TopThreeRow.SilverPillar)
                    .Column(TopThreeColumn.Left),

                    // Silver Pillar / Second Place Information
                    new Frame
                    {
                        BackgroundColor = Colors.Silver,
                        Content = new StackLayout
                        { 
                            _secondPlaceScore,
                            _secondPlaceUsername
                        }
                    }
                    .Row(TopThreeRow.SilverPillar)
                    .Column(TopThreeColumn.Right)
                    .RowSpan(2),

                    // Second Place Friend Picture
                    _secondPlacePicture
                    .Row(TopThreeRow.GoldPillar)
                    .Column(TopThreeColumn.Right),

                    // Gold Pillar
                    new Frame
                    {
                        BackgroundColor = Colors.Gold,
                        Content = new StackLayout
                        { 
                            _firstPlaceScore,
                            _firstPlaceUsername
                        }
                    }
                    .Row(TopThreeRow.GoldPillar)
                    .Column(TopThreeColumn.Center)
                    .RowSpan(3),

                    // First Place Friend
                    _firstPlacePicture
                    .Row(TopThreeRow.TopProfilePicture)
                    .Column(TopThreeColumn.Center),
                }
            };

            return topThreeFriendsGrid;
        }

        ScrollView GetRemainingFriendsScrollView()
        {
            List<TestFriend> testFriends = new List<TestFriend>
            {
                new TestFriend { Name = "Richard 1", Score = 10, Rank = 4 },
                new TestFriend { Name = "Richard 2", Score = 10 ,Rank = 5},
                new TestFriend { Name = "Richard 3", Score = 10 ,Rank= 6},
                new TestFriend { Name = "Richard 4", Score = 10 ,Rank = 7},
                new TestFriend { Name = "Richard 5", Score = 10 ,Rank = 8},
                new TestFriend { Name = "Richard 6" , Score = 10,Rank = 9},
                new TestFriend { Name = "Richard 7" , Score = 10,Rank = 10},
                new TestFriend { Name = "Richard 8" , Score = 10,Rank = 11},
                new TestFriend { Name = "Richard 9" , Score = 10,Rank = 12},
                new TestFriend { Name = "Richard 10" , Score = 10,Rank = 13},
                new TestFriend { Name = "Richard 11" , Score = 10,Rank = 14},
                new TestFriend { Name = "Richard 12" , Score = 10,Rank = 15},
                new TestFriend { Name = "Richard 13" , Score = 10,Rank = 16},
                new TestFriend { Name = "Richard 14" , Score = 10,Rank = 17},
                new TestFriend { Name = "Richard 15" , Score = 10,Rank = 18}
            };

            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                /* For when data is fetched from API
                Image friendPicture = new Image
                {
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                };
                friendPicture.SetBinding(Image.SourceProperty, "Picture", converter: new ByteArrayToImageSourceConverter());
                */

                Image friendPicture = new Image
                {
                    HeightRequest = 64,
                    WidthRequest = 64,
                    VerticalOptions = LayoutOptions.Center,
                    Source = new FileImageSource
                    { 
                        File = "dotnet_bot.png"
                    }
                }
                .Column(RemainingFriendsColumn.Picture);

                Label friendName = new Label
                {
                    FontSize = 24,
                    VerticalOptions = LayoutOptions.Center,
                }
                .Column(RemainingFriendsColumn.Name);
                friendName.SetBinding(Label.TextProperty, "Name");

                Label friendScore = new Label
                {
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                };
                friendScore.SetBinding(Label.TextProperty, "Score");

                Label friendRank = new Label
                {
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
                .Margins(20,10,0,0);
                friendRank.SetBinding(Label.TextProperty, "Rank");

                StackLayout friendScoreAndRank = new StackLayout().Column(RemainingFriendsColumn.Score);
                friendScoreAndRank.Add(friendScore);
                friendScoreAndRank.Add(friendRank);

                Grid friendGrid = new Grid
                {
                    ColumnDefinitions = GridRowsColumns.Columns.Define(
                        (RemainingFriendsColumn.Picture, GridRowsColumns.Stars(1)),
                        (RemainingFriendsColumn.Name, GridRowsColumns.Stars(3)),
                        (RemainingFriendsColumn.Score, GridRowsColumns.Stars(1))),
                    BackgroundColor = Colors.LightGray,
                };
                friendGrid.Add(friendPicture);
                friendGrid.Add(friendName);
                friendGrid.Add(friendScoreAndRank);

                Frame friendContainer = new Frame
                {
                    Content = friendGrid,
                    BackgroundColor = Colors.DarkGray,
                }
                .ColumnSpan(typeof(RemainingFriendsColumn).GetEnumNames().Length);

                return friendContainer;
            });

            _remainingFriendsContent = new StackLayout();
            BindableLayout.SetItemsSource(_remainingFriendsContent, testFriends);
            BindableLayout.SetItemTemplate(_remainingFriendsContent, dataTemplate);
            
            var scrollView = new ScrollView
            { 
                Content = _remainingFriendsContent
            };
            
            return scrollView;
        }

        void SetTopThreeDynamicElements()
        {
            // Set third place dynamic elements
            _thirdPlacePicture = new Image
            {
                Source = new FileImageSource { File = "dotnet_bot.jpg" } // <-- Temp
            }
            //.Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
            .CenterHorizontal()
            .CenterVertical();

            _thirdPlaceScore = new Label
            {
                Text = "10"
            }
            .CenterHorizontal();

            _thirdPlaceUsername = new Label
            {
                Text = "Username"
            }
            .CenterHorizontal();

            // Bind third place properties to friend fields
            _thirdPlacePicture.Bind(Image.SourceProperty, "Picture", converter: new ByteArrayToImageSourceConverter());
            _thirdPlaceScore.Bind(Label.TextProperty, "Score");
            _thirdPlaceUsername.Bind(Label.TextProperty, "Name");

            // Set second place dynamic elements
            _secondPlacePicture = new Image
            {
                Source = new FileImageSource { File = "dotnet_bot.jpg" } // <-- Temp
            }
            //.Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
            .CenterHorizontal()
            .CenterVertical();

            _secondPlaceScore = new Label
            {
                Text = "20",
                TextColor = Colors.Black
            }
            .CenterHorizontal();

            _secondPlaceUsername = new Label
            {
                Text = "Username",
                TextColor = Colors.Black
            }
            .CenterHorizontal();

            // Bind second place properties to friend fields
            _secondPlacePicture.Bind(Image.SourceProperty, "Picture", converter: new ByteArrayToImageSourceConverter());
            _secondPlaceScore.Bind(Label.TextProperty, "Score");
            _secondPlaceUsername.Bind(Label.TextProperty, "Name");

            // Set first place dynamic elements
            _firstPlacePicture = new Image
            {
                Source = new FileImageSource { File = "dotnet_bot.jpg" } // <-- Temp
            }
            //.Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
            .CenterHorizontal()
            .CenterVertical();

            _firstPlaceScore = new Label
            {
                Text = "30",
                TextColor = Colors.Black
            }
            .CenterHorizontal();

            _firstPlaceUsername = new Label
            {
                Text = "Username",
                TextColor = Colors.Black
            }
            .CenterHorizontal();

            _firstPlacePicture.Bind(Image.SourceProperty, "Picture", converter: new ByteArrayToImageSourceConverter());
            _firstPlaceScore.Bind(Label.TextProperty, "Score");
            _firstPlaceUsername.Bind(Label.TextProperty, "Name");
        }

        public class TestFriend
        { 
            public string Name { get; set; }
            public byte[] Picture { get; set; }
            public int Score { get; set; }
            public int Rank { get; set; }
        }

        // Prototype: Link to data fetched from API
        void GetDailyLeaderboards(object sender, EventArgs e)
        {
            _firstPlacePicture.Source = new FileImageSource { File = "logo.png" };
            _firstPlaceScore.Text = "100";
            _secondPlacePicture.Source = new FileImageSource { File = "pet_beans.png" };
            _secondPlaceScore.Text = "80";
            _thirdPlacePicture.Source = new FileImageSource { File = "pet_bob.png" };
            _thirdPlaceScore.Text = "50";
        }

        void GetWeeklyLeaderboards(object sender, EventArgs e)
        {
            _firstPlacePicture.Source = new FileImageSource { File = "pet_wurmy.png" };
            _firstPlaceScore.Text = "600";
            _secondPlacePicture.Source = new FileImageSource { File = "pet_greg.png" };
            _secondPlaceScore.Text = "400";
            _thirdPlacePicture.Source = new FileImageSource { File = "pet_danole.png" };
            _thirdPlaceScore.Text = "200";
        }

        async void BackButtonClicked(object sender, EventArgs e)
        {
            // Test code for dynamic top three
            var q = await _client.GetAllShopItems(new FocusCore.Queries.Shop.GetAllShopItemsQuery(), default);
            var kylepic = q.First(s => s.Name == "Kyle").ImageSource;

            TestFriend test = new TestFriend { Name = "YourFriend3", Rank = 3, Score = 500, Picture = kylepic };
            _thirdPlacePicture.BindingContext = test;
            _thirdPlaceScore.BindingContext = test;
            _thirdPlaceUsername.BindingContext = test;
            _secondPlacePicture.BindingContext = test;
            _secondPlaceScore.BindingContext = test;
            _secondPlaceUsername.BindingContext = test;
            _firstPlacePicture.BindingContext = test;
            _firstPlaceScore.BindingContext = test;
            _firstPlaceUsername.BindingContext = test;

            // Back navigation reverses animation so can keep right to left
            Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
        }
    }
}
