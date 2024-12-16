

namespace SecureMiles.Common.DTOs.Admin
{
    public class DashboardDataResponseDto
    {
        // Overall counts
        public int TotalUsers { get; set; }
        public int TotalPolicies { get; set; }
        public int TotalClaims { get; set; }
        public int TotalProposals { get; set; }
        public int TotalVehicles { get; set; }

        // Active counts
        public int ActivePolicies { get; set; }
        public int ActiveClaims { get; set; }
        public int ActiveProposals { get; set; }
        public int ActiveVehicles { get; set; }

        // Trends for the last month
        public int ClaimsLastMonth { get; set; }
        public int ProposalsLastMonth { get; set; }
        public int PoliciesLastMonth { get; set; }
        public int VehiclesLastMonth { get; set; }

        // Trends for the last year
        public int ClaimsLastYear { get; set; }
        public int ProposalsLastYear { get; set; }
        public int PoliciesLastYear { get; set; }
    }
}
