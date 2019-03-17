namespace Vision.Web.Core
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CiCdDto
    {
        [Required]
        public Guid CiCdId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Text)]        
        public string Endpoint { get; set; }

        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsGuestEnabled { get; set; }

        [Required]
        [EnumDataType(typeof(CiCdKind))]
        public CiCdKind Kind { get; set; }
    }
}
