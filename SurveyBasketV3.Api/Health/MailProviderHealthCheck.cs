using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasketV3.Api.Settings;

namespace SurveyBasketV3.Api.Health
{
	public class MailProviderHealthCheck(IOptions<MailSettings> mailsettings) : IHealthCheck
	{
		private readonly MailSettings _mailsettings = mailsettings.Value;

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				using var smtp = new SmtpClient();
				smtp.Connect(_mailsettings.Host, _mailsettings.Port, SecureSocketOptions.StartTls, cancellationToken);
				smtp.Authenticate(_mailsettings.Mail, _mailsettings.Password, cancellationToken);

				return await Task.FromResult(HealthCheckResult.Healthy());
			}
			catch (Exception exception)
			{

				return await Task.FromResult(HealthCheckResult.Unhealthy(exception: exception));
			}
		}
	}
}
