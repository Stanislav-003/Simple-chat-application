using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSignalR.BLL.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
    }
}
