using System;
using System.ComponentModel.DataAnnotations;

namespace Vision.Web.Core
{
    public class VersionControlDto
    {
        [Required]
        public Guid VersionControlId { get; set; }

        [Required]
        [EnumDataType(typeof(VersionControlKind))]
        public VersionControlKind Kind { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Url)]
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public int Repositories { get; set; }
        public bool IsEnabled { get; set; }
    }
}
