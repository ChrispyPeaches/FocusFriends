using FocusApp.Client.Clients;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
using System.Net.Security;

namespace FocusApp.Client.DevHttp
{
    public static class DevHttpClientHelperExtensions
    {
        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="HttpClient"/> to use localhost or 10.0.2.2 and bypass certificate checking on Android.
        /// </summary>
        /// <param name="sslPort">Development server port</param>
        /// <returns>The IServiceCollection</returns>
        public static IServiceCollection AddDevHttpClient(this IServiceCollection services, int sslPort)
        {
            var devServerRootUrl = new UriBuilder("https", DevServerName, sslPort).Uri.ToString();

#if WINDOWS
            services.AddRefitClient<IAPIClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new UriBuilder("https", DevServerName, sslPort).Uri;
                }); 
            
            return services;
#endif

#if ANDROID
            services.AddRefitClient<IAPIClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new UriBuilder("https", DevServerName, sslPort).Uri;
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new CustomAndroidMessageHandler();
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (cert != null && cert.Issuer.Equals("CN=localhost"))
                            return true;
                        return errors == SslPolicyErrors.None;
                    };
                    return handler;
                });

            return services;

#else
        throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif
        }

        // Configure the host name (Android always use 10.0.2.2, which is an alias to the host's loopback interface aka 127.0.0.1 or localhost)
        public static string DevServerName =>
#if WINDOWS
        "localhost";
#elif ANDROID
        "10.0.2.2";
#else
        throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif

#if ANDROID
        internal sealed class CustomAndroidMessageHandler : Xamarin.Android.Net.AndroidMessageHandler
        {
            protected override Javax.Net.Ssl.IHostnameVerifier GetSSLHostnameVerifier(Javax.Net.Ssl.HttpsURLConnection connection)
                => new CustomHostnameVerifier();

            private sealed class CustomHostnameVerifier : Java.Lang.Object, Javax.Net.Ssl.IHostnameVerifier
            {
                public bool Verify(string hostname, Javax.Net.Ssl.ISSLSession session)
                {
                    return
                        Javax.Net.Ssl.HttpsURLConnection.DefaultHostnameVerifier.Verify(hostname, session)
                        || hostname == "10.0.2.2" && session.PeerPrincipal?.Name == "CN=localhost";
                }
            }
        }
#endif
    }
}
