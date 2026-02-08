using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class GameDto
    {
        public string Code { get; set; } 
        public string CreatorUsername { get; set; } 
        public int PlayerCount { get; set; }
        public string State { get; set; } 
        public string? CityName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public List<string> PlayerUsernames { get; set; } 
    }
}
