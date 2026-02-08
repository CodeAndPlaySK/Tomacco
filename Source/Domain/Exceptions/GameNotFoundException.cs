using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public string GameCode { get; }

        public GameNotFoundException(string gameCode)
            : base($"Game with code '{gameCode}' not found")
        {
            GameCode = gameCode;
        }
    }
}
