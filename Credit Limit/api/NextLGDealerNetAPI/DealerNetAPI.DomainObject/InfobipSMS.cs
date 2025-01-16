using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class InfobipSMS
    {
        public InfobipSMS()
        {
            Messages = new List<Messages>();
        }
        [JsonProperty("bulkId")]
        public string BulkId { get; set; }

        [JsonProperty("messages")]
        public List<Messages> Messages { get; set; }

    }

    public class Messages
    {
        public Messages()
        {
            Destination = new List<Destinations>();
            Regional = new Regional();
        }
        [JsonProperty("from")]
        public string From { get; set; } // The sender ID which can be alphanumeric or numeric (e.g., CompanyName).

        [JsonProperty("destinations")]
        public List<Destinations> Destination { get; set; }

        [JsonProperty("regional")]
        public Regional Regional { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } // Content of the message being sent.

        [JsonProperty("entityId")]
        public string EntityId { get; set; } // Required for entity use in a send request for outbound traffic. Returned in notification events.
    }

    public class Destinations
    {
        [JsonProperty("messageId")]
        public string MessageId { get; set; } // The ID that uniquely identifies the message sent.
        [JsonProperty("to")]
        public string To { get; set; } // Message destination address. Addresses must be in international format (Example: 41793026727).

    }

    public class Regional
    {
        public Regional()
        {
            indiaDlt = new IndiaDlt();
        }
        [JsonProperty("indiaDlt")]
        public IndiaDlt indiaDlt { get; set; }
    }


    public class IndiaDlt
    {
        [JsonProperty("contentTemplateId")]
        public string ContentTemplateId { get; set; } // Registered DLT content template ID which matches message you are sending
        [JsonProperty("principalEntityId")]
        public string PrincipalEntityId { get; set; }  // Your assigned DLT principal entity ID
    }

}
