using Newtonsoft.Json;
using RestSharp;

namespace Messenger
{
    public class HttpClientRequest
    {
        public void RequestClients(Notification notification) 
        {
            List<Notification>  notifications = new();

            notifications.Add(notification);

            string body = JsonConvert.SerializeObject(notifications);
            
            Uri uri = new Uri($"{notification.Client}/Notification/Receive");

            RestResponse response = new();

            using (RestClient client = new(uri))
            {
                RestRequest request = new(uri, Method.Post);

                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                response = client.Execute(request);
            }
        }
    }
}
