using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xarajat.Bot.Context;
using Xarajat.Bot.Entities;
using Xarajat.Bot.Services;
using User = Xarajat.Bot.Entities.User;

namespace XarajatBot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BotController : ControllerBase
{
    private readonly TelegramBotService _botService;
    private readonly XarajatDbContext _context;

    public BotController(TelegramBotService botService, XarajatDbContext context)
    {
        _botService = botService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Get() => Ok("working..");
    
    [HttpPost]
    public async Task PostUpdate(Update update)
    {
        if (update.Type != UpdateType.Message)
            return;

        var (chatId, message, username) = GetValues(update);
        
        var user = await FilterUser(chatId, username);

        if (user.Step == 0)
        {
            if (message == "Create room")
            {
                user.Step = 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _botService.SendMessage(user.ChatId, "Enter room name?");
            }
            else if (message == "Join room")
            {
                user.Step = 2;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _botService.SendMessage(user.ChatId, "Enter room key?");
            }
            else
            {
                var menu = new List<string> { "Create room", "Join room" };
                await _botService.SendMessage(user.ChatId, "Menu", reply: _botService.GetKeyboard(menu));
            }
        }
        else if (user.Step == 1)
        {
            var room = new Room
            {
                Name = message,
                Key = Guid.NewGuid().ToString("N")[..10],
                Status = RoomStatus.Created
            };

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            user.RoomId = room.Id;
            user.IsAdmin = true;
            user.Step = 3;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var menu = new List<string> { "Add outlay", "Calculate" };

            await _botService.SendMessage(user.ChatId, "Menu", reply: _botService.GetKeyboard(menu));
        }
        else if (user.Step == 3)
        {
            if (message == "Add outlay")
            {
                user.Step = 4;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _botService.SendMessage(user.ChatId, "Enter outlay details?");
            }
            else if (message == "Calculate")
            {
                var room = await _context.Rooms.Include(r=>r.Users).Include(r=>r.Outlays).FirstOrDefaultAsync(r => r.Id == user.RoomId);
                var msg = $"Room: {room.Name}\nUsers: {room.Users.Count}\nTotal: {room.Outlays.Sum(o=>o.Cost)}\nPerUser: {room.Outlays.Sum(o=>o.Cost)/room.Users.Count}";
                await _botService.SendMessage(user.ChatId, msg);
            }
            else
            {
                var menu = new List<string> { "Add outlay", "Calculate" };

                await _botService.SendMessage(user.ChatId, "Menu", reply: _botService.GetKeyboard(menu));
            }
        }
        else if (user.Step == 4)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == user.RoomId);
            if (room != null)
            {
                int.TryParse(message, out var cost);

                var outlay = new Outlay()
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Cost = cost,
                };
                await _context.Outlays.AddAsync(outlay);
                await _context.SaveChangesAsync();
            }

            user.Step = 3;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var menu = new List<string> { "Add outlay", "Calculate" };

            await _botService.SendMessage(user.ChatId, "Menu", reply: _botService.GetKeyboard(menu));
        }
    }

    private Tuple<long, string, string> GetValues(Update update)
    {
        var chatId = update.Message!.From!.Id;
        var message = update.Message!.Text!;
        var name = update.Message.From.Username ?? update.Message.From.FirstName;

        return new(chatId, message, name);
    }

    public async Task<User> FilterUser(long chatId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
        if (user is null)
        {
            user = new User
            {
                ChatId = chatId,
                Name = username,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }
}