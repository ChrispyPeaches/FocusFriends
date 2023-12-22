using FocusApp.Pages;
using FocusApp.ViewModels;

namespace FocusApp;

class AppShell : Shell
{
	static readonly IReadOnlyDictionary<Type, string> pageRouteMappingDictionary = new Dictionary<Type, string>(new[]
	{
		CreateRoutePageMapping<MainPage, MainViewModel>()
	});

	public AppShell(MainPage MainPage)
	{
		Items.Add(MainPage);
	}

	public static string GetRoute<TPage, TViewModel>() where TPage : BaseContentPage<TViewModel>
														where TViewModel : BaseViewModel
	{
		return GetRoute(typeof(TPage));
	}

	public static string GetRoute(Type type)
	{
		if (!pageRouteMappingDictionary.TryGetValue(type, out var route))
		{
			throw new KeyNotFoundException($"No map for ${type} was found on navigation mappings. Please register your ViewModel in {nameof(AppShell)}.{nameof(pageRouteMappingDictionary)}");
		}

		return route;
	}

	static KeyValuePair<Type, string> CreateRoutePageMapping<TPage, TViewModel>() where TPage : BaseContentPage<TViewModel>
																					where TViewModel : BaseViewModel
	{
		var route = CreateRoute();
		Routing.RegisterRoute(route, typeof(TPage));

		return new KeyValuePair<Type, string>(typeof(TPage), route);

		static string CreateRoute()
		{
			return $"//{typeof(MainPage).Name}";

			throw new NotSupportedException($"{typeof(TPage)} Not Implemented in {nameof(pageRouteMappingDictionary)}");
		}
	}
}