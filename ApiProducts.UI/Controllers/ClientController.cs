using Messenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.RegularExpressions;

namespace WebHook.Controllers
{
    public class ClientController : Controller
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Subscribe([FromBody] string url)
        {
            try
            {
                string result = Regex.Replace(url, @"[\\/](?!.*[\\/])", string.Empty);

                RabbitMQPublish rabbit = new();
                rabbit.Add(new Notification {
                        Type = "Create",
                        Client= result,
                        Description = string.Format("Novo cliente inscrito."),
                        Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    });
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return new ObjectResult("Inscribed") { StatusCode = (int)HttpStatusCode.OK };
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult UnSubscribe([FromBody] string url)
        {
            try
            {
                string result = Regex.Replace(url, @"[\\/](?!.*[\\/])", string.Empty);

                RabbitMQPublish rabbit = new();
                rabbit.Add(new Notification
                {
                    Type = "Remove",
                    Client= result,
                    Description = string.Format("O cliente cancelou a inscrito."),
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return new ObjectResult("UnSubscribe") { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}
