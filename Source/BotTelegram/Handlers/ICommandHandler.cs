using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.Handlers
{
    public interface ICommandHandler
    {
        string CommandName { get; }
        Task<string> HandleAsync(CommandContext context);
    }
}
