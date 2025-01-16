using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class EMail
    {
        public EMail()
        {
            Attachments = new List<string>();
        }
        public string To { get; set; }
        public string CC { get; set; }
        public string From { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string AttachmentFileName { get; set; }
        public List<string> Attachments { get; set; }
        public bool isAttachment { get; set; }
        public string EmailSupport { get; set; }

    }
}
