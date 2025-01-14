using System;

namespace dummy_CRUD.Models
{
    public class ClientInfo
    {
        public int id { get; set; }          // Client ID, stored as a string for flexibility
        public string name { get; set; }        // Name of the client
        public string email { get; set; }       // Client's email address
        public string phone { get; set; }       // Client's phone number
        public string address { get; set; }     // Client's address
        public DateTime created_at { get; set; } // Timestamp when the client was created
    }
}
