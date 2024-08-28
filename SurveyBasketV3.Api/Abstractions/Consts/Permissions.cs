namespace SurveyBasketV3.Api.Abstractions.Consts
{
	public static class Permissions
	{
		public static string Type { get; } = "permissions";

		public const string GetPolls = "Polls:Get";
		public const string AddPolls = "Polls:Add";
		public const string UpdatePolls = "Polls:Update";
		public const string DeletePolls = "Polls:Delete";

		public const string GetQuestions = "Questions:Get";
		public const string AddQuestions = "Questions:Add";
		public const string UpdateQuestions = "Questions:Update";

		public const string GetResults = "Results:Get";

		//public const string GetVotes = "Results:Get";
		//public const string AddVotes = "Results:Add";

		public const string GetUsers = "Users:Get";
		public const string AddUsers = "Users:Add";
		public const string UpdateUsers = "Users:Update";

		public const string GetRoles = "Roles:Get";
		public const string AddRoles = "Roles:Add";
		public const string UpdateRoles = "Roles:Update";

		public static IList<string?> GetAllPermissions() =>
			typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();

	}
}
