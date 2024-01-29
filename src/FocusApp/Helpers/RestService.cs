using FocusCore.Models.User;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FocusApp.Helpers
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<UserModel> GetUser()
        {
            Uri uri = new Uri("http://10.0.2.2:5223/User");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                { 
                    string content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserModel>(content, _serializerOptions);
                    return user;
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception: " + e.Message.ToString() + "\n" + "\n");
            }

            return new UserModel();
        }
    }
}