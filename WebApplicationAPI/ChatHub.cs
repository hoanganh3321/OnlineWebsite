using ClassLibraryDATA.Models;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebApplicationAPI.ChatHub
{
    public class ChatHubb : Hub
    {
        private readonly IMessageService _messageService;

        private static readonly ConcurrentDictionary<int, string> _userConnections = new();

        public ChatHubb(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.GetHttpContext()?.Request.Query.TryGetValue("UserID", out var userIdStr) == true
                && int.TryParse(userIdStr, out var userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"[DEBUG] Người dùng {userId} đã kết nối, ConnectionId: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("[DEBUG] Không lấy được UserID từ query");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userConnection = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (userConnection.Key != 0)
            {
                _userConnections.TryRemove(userConnection.Key, out _);
                Console.WriteLine($"[DEBUG] Người dùng {userConnection.Key} đã ngắt kết nối");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            Console.WriteLine($"[DEBUG] Bắt đầu SendMessage: senderId={senderId}, receiverId={receiverId}, message={message}");
            try
            {
                var msg = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message,
                    SentAt = DateTime.UtcNow,
                    IsReadd = false
                };

                Console.WriteLine("[DEBUG] Gọi SaveMessageAsync");
                await _messageService.SaveMessageAsync(msg);
                Console.WriteLine("[DEBUG] Lưu tin nhắn thành công");

                if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
                {
                    Console.WriteLine("[DEBUG] Gửi tin nhắn đến receiver: " + receiverConnectionId);
                    await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message, msg.SentAt.ToString("o"), msg.Id);
                }
                if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
                {
                    Console.WriteLine("[DEBUG] Gửi tin nhắn lại cho sender: " + senderConnectionId);
                    await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, message, msg.SentAt.ToString("o"), msg.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Lỗi khi xử lý SendMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
            Console.WriteLine("[DEBUG] Kết thúc SendMessage");
        }
    }
}