using System.Xml;
using FocusApp.Clients;
using FocusCore.Queries.User;

namespace FocusApp.Views.Shop
{
    internal class MainPage : ContentPage
    {
        IAPIClient _client;
        public MainPage(IAPIClient client)
        {
            _client = client;

            List<ImageCell> shopItemImages = new List<ImageCell>
            {
                new ImageCell
                {
                    Text = "ShopItem1",
                    ImageSource = new FileImageSource
                    {
                        File = "dotnet_bot.png"
                    },
                    BindingContext = this
                },
                new ImageCell
                {
                    Text = "ShopItem2",
                    ImageSource = new FileImageSource
                    {
                        File = "dotnet_bot.png"
                    },
                    BindingContext = this
                }
            };

            DataTemplate dataTemplate = new DataTemplate(typeof(ImageCell));
            dataTemplate.SetBinding(ImageCell.TextColorProperty, "Text");
            dataTemplate.SetBinding(ImageCell.ImageSourceProperty, "ImageSource");

            CarouselView carouselView = new CarouselView();
            carouselView.ItemTemplate = new DataTemplate(() =>
            {
                Label nameLabel = new Label { Text = "Item" };
                nameLabel.SetBinding(Label.TextProperty, "Name");

                Image image = new Image();
                image.SetBinding(Image.SourceProperty, "ImageUrl");
            });

            Content = new StackLayout
            {
                BackgroundColor = Colors.LightYellow,
                Children =
                {
                    new Label
                    {
                        Text = "Shop",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new CarouselView
                    { 
                        ItemsSource = shopItemImages,
                        ItemTemplate = dataTemplate
                    }
                }
            };
        }

        protected override async void OnAppearing()
        {
            var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
            base.OnAppearing();
        }
    }
}

/*
public class ShopItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ShopItemTemplate { get; set; }
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ShopItemTemplate;
    }
}
*/

public class ShopItem
{ 
    public string Name { get; set; }
}