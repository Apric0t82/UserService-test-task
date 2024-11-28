using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using UserService_test_task.Filters;

namespace UserService_test_task.Controllers
{
    [ValidateModel]
    public class WebSocketController : ControllerBase
    {
        private readonly IUserService _userService;

        public WebSocketController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocket(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task HandleWebSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                try
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    var user = JsonSerializer.Deserialize<User>(message);
                    var existing = await _userService.GetUserByIdAsync(user!.Id) ?? null;

                    if (user != null && existing != null)
                    {
                        await _userService.UpdateUserAsync(user);

                        var successMessage = Encoding.UTF8.GetBytes($"User updated successfully for User ID: {user.Id}");
                        await webSocket.SendAsync(new ArraySegment<byte>(successMessage), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                    else
                    {
                        var errorMessage = Encoding.UTF8.GetBytes("Invalid message format");
                        await webSocket.SendAsync(new ArraySegment<byte>(errorMessage), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    var errorResponse = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
                    await webSocket.SendAsync(new ArraySegment<byte>(errorResponse), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}