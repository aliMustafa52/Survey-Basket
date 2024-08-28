namespace SurveyBasketV3.Api.Entities
{
	public class ApplicationRole : IdentityRole
	{
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
