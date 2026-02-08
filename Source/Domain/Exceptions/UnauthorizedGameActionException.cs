using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UnauthorizedGameActionException : Exception
    {
        public string GameCode { get; }
        public string PlayerId { get; }
        public string Action { get; }

        public UnauthorizedGameActionException(string gameCode, string playerId, string action)
            : base($"Player '{playerId}' is not authorized to '{action}' game '{gameCode}'")
        {
            GameCode = gameCode;
            PlayerId = playerId;
            Action = action;
        }
    }
}
