using FocusApp.Clients;
using FocusApp.Resources;

namespace FocusApp
{
    class App : Application
    {
        /// <summary>
        /// Create a new instance of the application with the styling resources and 
        /// a <see cref="Shell"/> that only acts as a container for the <see cref="MainPage"/>.
        /// </summary>
        public App()
        {
            //var user = apiClient.GetUser(new FocusCore.Queries.User.GetUserQuery { Id = Guid.NewGuid() });
            Resources = new AppStyles();

            MainPage = new Shell() { CurrentItem = new MainPage() };
        }
    }
}
