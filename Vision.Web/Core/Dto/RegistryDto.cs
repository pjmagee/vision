namespace Vision.Web.Core
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RegistryDto
    {
        [Required]
        public Guid RegistryId { get; set;  }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Url)]
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEnabled { get; set; }
        
        [EnumDataType(typeof(DependencyKind))]
        public DependencyKind Kind { get; set; }
    }
}
