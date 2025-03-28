using DiffEngine;
using VerifyTests;

namespace Loyalty.Domain.UnitTests.VerifyTests;

public static class VerifyTestsDefaultVerifierSettings
{
    /// <summary>
    /// VerifierSettings for the project but it needs to be initialized in a ModuleInitializer for all projects that use Verify.
    /// </summary>
    public static void Init()
    {
        // Turn off default scrubbers
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DontScrubGuids();
        VerifierSettings.DontScrubProjectDirectory();
        VerifierSettings.DontScrubSolutionDirectory();
        VerifierSettings.DontScrubUserProfile();

        // Turn off default sorters and ignores
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifierSettings.DontSortDictionaries();

        // Set the default DateTime formats
        VerifierSettings.ScrubInlineDateTimes("o");
        VerifierSettings.ScrubInlineDateTimeOffsets("o");

        // Use actual JSON files
        VerifierSettings.UseStrictJson();

        // Prioritize VSCode (with a few fallbacks)
        DiffTools.UseOrder(
            DiffTool.VisualStudioCode,
            DiffTool.VisualStudio,
            DiffTool.Rider,
            DiffTool.Neovim,
            DiffTool.Vim
        );
    }
}