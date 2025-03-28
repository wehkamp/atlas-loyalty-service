using Loyalty.Api;
using Loyalty.Domain.UnitTests.Mapper;
using Loyalty.Domain.UnitTests.VerifyTests;
using System.Globalization;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Loyalty.Application.UnitTests.XUnit", "Loyalty.Application.UnitTests")]

namespace Loyalty.Application.UnitTests;

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