using Microsoft.AspNetCore.Mvc;
using SocketServer.Services;
using System.Net.WebSockets;
using System.Text;

namespace SocketServer.Controllers
{
    [ApiController]
    [Route("api")]
    public class ServiceController : Controller
    {
        private readonly DataService _dataService;
       

        public ServiceController(DataService dataservice)
        {
            _dataService = dataservice; 
        }

        [HttpGet("/Data")]
        public async Task GetData()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using (var socket = await HttpContext.WebSockets.AcceptWebSocketAsync())
                {
                    var buffer = new byte[1024 * 4];
                    var receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!receiveResult.CloseStatus.HasValue)
                    {
                        var Message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count); 
                        await _dataService.ProcessRequest(socket, Message.Trim());
                        receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }

                    await socket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);

                };
            }
           }
    }
}
