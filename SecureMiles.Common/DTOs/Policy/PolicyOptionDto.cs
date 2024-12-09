namespace SecureMiles.Common.DTOs.Policy
{
    public class PolicyOptionDto
    {
        public int VehicleID { get; set; }
        public string? PolicyType { get; set; }

        public string? PolicyName { get; set; }

        public string? PolicyDescription { get; set; }

        public string? PolicyTerms { get; set; }

        public string? PolicyCoverage { get; set; }


        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public List<string>? AddOns { get; set; }
    }

}