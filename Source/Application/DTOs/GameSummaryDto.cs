using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class GameSummaryDto
    {
        public string Code { get; set; } = string.Empty;
        public string CreatorUsername { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public string State { get; set; } = string.Empty;
    }
}
