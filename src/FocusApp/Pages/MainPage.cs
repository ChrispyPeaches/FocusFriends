namespace FocusApp.Pages;

internal sealed class MainPage : BaseContentPage<MainViewModel>
{
	readonly IDispatcher dispatcher;

	public MainPage(IDispatcher dispatcher,
					MainViewModel MainViewModel) : base(MainViewModel, "Top Stoes")
	{
		this.dispatcher = dispatcher;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}
}