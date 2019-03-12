namespace Vision.Web.Core
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CiCdDto
    {
        public Guid CiCdId { get; set; }

        [Required]
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsGuestEnabled { get; set; }

        [Required]
        public CiCdKind Kind { get; set; }
    }
}
