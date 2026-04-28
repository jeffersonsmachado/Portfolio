using System.Runtime.CompilerServices;
using FluentAssertions;

namespace Portfolio.Integration.Tests.Fixtures;

public static class LicenseSetup
{
	[ModuleInitializer]
	public static void Init()
	{
		// Enable unlicensed usage of FluentAssertions in test projects
		License.Accepted = true;
	}
}