﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSignalR.BLL.DTO
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CreatorId { get; set; }
    }
}
