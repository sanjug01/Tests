
namespace RdClient.Shared.Models
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public bool HaveBeenPersisted { get; set; }
    }
}
