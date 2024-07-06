using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SocketServer.Services
{
    public class DataService
    {
        public Dictionary<string, List<Dictionary<string, int>>> data { get; set; }
        public FolderService _folder { get; set; }
        private readonly CryptoService _cryptoService;
        public DataService(FolderService folder,CryptoService cryptoService)
        {
            _folder = folder;
            _cryptoService = cryptoService;
            string jsonContent =string.Empty ;
            using (var filestream=new FileStream(_folder.FolderPath,FileMode.Open,FileAccess.Read))
            {
                using (var stream=new StreamReader(filestream))
                {
                    jsonContent=stream.ReadToEnd();
                }
            }
            
            data = JsonSerializer.Deserialize<Dictionary<string,List<Dictionary<string,int>>>>(jsonContent);
            

        }
        public async Task ProcessRequest(WebSocket webSocket, string Message)
        {
            //var decryptedmsg = _cryptoService.Decrypt(Message); 
            //Some error with Padding
            string[] pair = Message.Split('-');
            if (pair.Length != 2)
            {
                await SendResponse(webSocket, "EMPTY");
            }

            string setName = pair[0];
            string key = pair[1];

            if (data.TryGetValue(setName,out List<Dictionary<string,int>> sets))
            {
                var SetValue = sets.Find(x => x.ContainsKey(key));
                if (SetValue != null && SetValue.TryGetValue(key, out int value))
                {
                    for (int i = 0; i < value; i++)
                    {
                        await SendResponse(webSocket,DateTime.Now.ToString());
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    await SendResponse(webSocket, "EMPTY"); 
                }
            }
            else
            {
                await SendResponse(webSocket, "EMPTY");
            }
        }
        public async Task SendResponse(WebSocket webSocket, string message)
        {
            //var Encryptmsg=_cryptoService.Encrypt(message); // Some error with padding
            var bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
