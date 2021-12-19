using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Freelance.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
