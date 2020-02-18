using System;
using System.ComponentModel.DataAnnotations;

namespace Vision.Core
{
    public class VersionControlDto
    {
        [Required]
        public Guid VcsId { get; set; }

        [Required]
        [EnumDataType(typeof(VcsKind))]
        public VcsKind Kind { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Url)]
        public string Endpoint { get; set; }

        public string ApiKey { get; set; }

        public int Repositories { get; set; }

        public bool IsEnabled { get; set; }
    }
}
