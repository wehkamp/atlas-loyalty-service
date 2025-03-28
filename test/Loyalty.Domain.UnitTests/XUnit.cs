using Loyalty.Api;
using Loyalty.Domain.UnitTests.Mapper;
using Loyalty.Domain.UnitTests.VerifyTests;
using System.Globalization;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Loyalty.Domain.UnitTests.XUnit", "Loyalty.Domain.UnitTests")]

namespace Loyalty.Domain.UnitTests;

public class XUnit : XunitTestFramework
{
    public XUnit(IMessageSink messageSink) : base(messageSink)
    {
        var culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // Initialize the mapping configuration
        TestMappingConfiguration.Scan<Program>();

        // Verify tests settings
        VerifyTestsDefaultVerifierSettings.Init();
    }
}