using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV3.Api.ValidationAttributes
{
	[AttributeUsage(AttributeTargets.All)]
	public class MinAgeAttribute(int minAge) : ValidationAttribute
	{
		private readonly int _minAge = minAge;

		public override bool IsValid(object? value)
		{
			if(value is not null)
			{
				var date = (DateTime)value;
				if(date.AddYears(_minAge) > DateTime.Now) 
					return false;
			}

			return true;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value is not null)
			{
				var date = (DateTime)value;
				if (date.AddYears(_minAge) > DateTime.Now)
					return new ValidationResult($"{validationContext.DisplayName} must be more than {_minAge}");
			}

			return ValidationResult.Success;
		}
	}
}
