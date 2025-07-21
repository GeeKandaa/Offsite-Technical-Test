using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_TechTest_Backend.Models
{
    public class RowData
    {
        public static readonly string[] RequiredHeaders = ["MPAN", "MeterSerial", "DateOfInstallation", "AddressLine1", "PostCode"];
        [Required]
        [Column(TypeName = "numeric(13,0)")]
        public long MPAN { get; set; }
        [Required]
        [MaxLength(10)]
        public string MeterSerial { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfInstallation { get; set; }
        [MaxLength(40)]
        public string? AddressLine1 { get; set; } = string.Empty;
        [MaxLength(10)]
        public string? Postcode { get; set; } = string.Empty;
    }
    public class InvalidRowData
    {
        public enum FieldFlags
        {
            MPAN = 1,
            MeterSerial = 2,
            DateOfInstallation = 4,
            AddressLine1 = 8,
            Postcode = 16
        }
        public string MPAN { get; set; } = string.Empty;
        public string MeterSerial { get; set; } = string.Empty;
        public string DateOfInstallation { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public int FailedField { get; set; } = 0;
    }
}
