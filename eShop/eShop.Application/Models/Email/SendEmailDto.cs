using Microsoft.AspNetCore.Http;

namespace eShop.Application.Models
{
    public class SendEmailDto
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public IList<IFormFile> Attachments { get; set; } = new List<IFormFile>();
        public IEnumerable<string> ToEmails { get; set; } = new List<string>();
        public IEnumerable<string> EmailCC { get; set; } = new List<string>();
        public IEnumerable<string> EmailBCC { get; set; } = new List<string>();
        public string Priority { get; set; }

    }
}
