using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using VerifyTests;
using VerifyXunit;

namespace Loyalty.Domain.UnitTests.VerifyTests;

public static class VerifyExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        }
    };

    private static VerifySettings SetDefaultVerifySettings(VerifySettings? settings, string fileName)
    {
        // Ensure settings is not null
        settings ??= new VerifySettings();

        // Get the directory of the file because the default is where this extension method is
        var directory = Path.GetDirectoryName(fileName);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new ArgumentException("The directory is invalid.", nameof(directory));
        }

        // Use the directory of the file that we run
        settings.UseDirectory(directory);

        return settings;
    }

    private static SettingsTask VerifyJsonAsync(this ObjectAssertions objectAssertions, VerifySettings? settings = null, [CallerFilePath] string fileName = "")
    {
        return Verifier.VerifyJson(target: JsonSerializer.Serialize(objectAssertions.Subject, _jsonSerializerOptions), settings: SetDefaultVerifySettings(settings, fileName));
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Theory test case
    /// It's mandatory to give a unique name per testcase
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="testCaseName">testCase.ToString()</param>
    /// <param name="fileName">Don't fill this.</param>
    public static SettingsTask TheoryVerifyJsonAsync(this ObjectAssertions objectAssertions, string testCaseName, [CallerFilePath] string fileName = "")
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(testCaseName);

        return VerifyJsonAsync(objectAssertions: objectAssertions, settings: settings, fileName: fileName);
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Fact test
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask FactVerifyJsonAsync(this ObjectAssertions objectAssertions, [CallerFilePath] string fileName = "")
    {
        return VerifyJsonAsync(objectAssertions: objectAssertions, settings: null, fileName: fileName);
    }

    private static SettingsTask VerifyJsonAsync<T>(this GenericCollectionAssertions<T> objectAssertions, VerifySettings? settings = null, [CallerFilePath] string fileName = "")
    {
        return Verifier.VerifyJson(target: JsonSerializer.Serialize(objectAssertions.Subject, _jsonSerializerOptions), settings: SetDefaultVerifySettings(settings, fileName));
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Theory test case
    /// It's mandatory to give a unique name per testcase
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="testCaseName">testCase.ToString()</param>
    /// <param name="fileName">Don't fill this.</param>
    public static SettingsTask TheoryVerifyJsonAsync<T>(this GenericCollectionAssertions<T> objectAssertions, string testCaseName, [CallerFilePath] string fileName = "")
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(testCaseName);

        return VerifyJsonAsync<T>(objectAssertions: objectAssertions, settings: settings, fileName: fileName);
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Fact test
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask FactVerifyJsonAsync<T>(this GenericCollectionAssertions<T> objectAssertions, [CallerFilePath] string fileName = "")
    {
        return VerifyJsonAsync<T>(objectAssertions: objectAssertions, settings: null, fileName: fileName);
    }

    private static SettingsTask VerifyJsonAsync(this StringAssertions stringAssertions, VerifySettings? settings = null, [CallerFilePath] string fileName = "")
    {
        return Verifier.VerifyJson(stringAssertions.Subject, settings: SetDefaultVerifySettings(settings, fileName));
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Theory test case
    /// It's mandatory to give a unique name per testcase
    /// </summary>
    /// <param name="stringAssertions">The kind of assertion we extend</param>
    /// <param name="testCaseName">testCase.ToString()</param>
    /// <param name="fileName">Don't fill this.</param>
    public static SettingsTask TheoryVerifyJsonAsync(this StringAssertions stringAssertions, string testCaseName, [CallerFilePath] string fileName = "")
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(testCaseName);

        return VerifyJsonAsync(stringAssertions: stringAssertions, settings: settings, fileName: fileName);
    }

    /// <summary>
    /// Verify the JSON file of a XUnit Fact test
    /// </summary>
    /// <param name="stringAssertions">The kind of assertion we extend</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask FactVerifyJsonAsync(this StringAssertions stringAssertions, [CallerFilePath] string fileName = "")
    {
        return VerifyJsonAsync(stringAssertions: stringAssertions, settings: null, fileName: fileName);
    }

    private static SettingsTask VerifyXmlAsync(this StringAssertions objectAssertions, VerifySettings? settings = null, [CallerFilePath] string fileName = "")
    {
        return Verifier.VerifyXml(target: objectAssertions.Subject, settings: SetDefaultVerifySettings(settings, fileName));
    }

    /// <summary>
    /// Verify the XML file of a XUnit Theory test case
    /// It's mandatory to give a unique name per testcase
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="testCaseName">testCase.ToString()</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask TheoryVerifyXmlAsync(this StringAssertions objectAssertions, string testCaseName, [CallerFilePath] string fileName = "")
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(testCaseName);

        return VerifyXmlAsync(objectAssertions: objectAssertions, settings: settings, fileName: fileName);
    }

    /// <summary>
    /// Verify the XML file of a XUnit Fact test
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask FactVerifyXmlAsync(this StringAssertions objectAssertions, [CallerFilePath] string fileName = "")
    {
        return VerifyXmlAsync(objectAssertions: objectAssertions, settings: null, fileName: fileName);
    }

    private static SettingsTask VerifyXmlAsync(this ObjectAssertions objectAssertions, VerifySettings? settings = null, [CallerFilePath] string fileName = "")
    {
        var serializer = new XmlSerializer(objectAssertions.Subject.GetType());
        using var writer = new StringWriter();
        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        });
        serializer.Serialize(xmlWriter, objectAssertions.Subject);

        return writer.ToString().Should().VerifyXmlAsync(settings, fileName);
    }

    /// <summary>
    /// Verify the XML file of a XUnit Theory test case
    /// It's mandatory to give a unique name per testcase
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="testCaseName">testCase.ToString()</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask TheoryVerifyXmlAsync(this ObjectAssertions objectAssertions, string testCaseName, [CallerFilePath] string fileName = "")
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(testCaseName);

        return VerifyXmlAsync(objectAssertions: objectAssertions, settings: settings, fileName: fileName);
    }

    /// <summary>
    /// Verify the XML file of a XUnit Fact test
    /// </summary>
    /// <param name="objectAssertions">The kind of assertion we extend</param>
    /// <param name="fileName">Don't fill this.</param>
    /// <returns></returns>
    public static SettingsTask FactVerifyXmlAsync(this ObjectAssertions objectAssertions, [CallerFilePath] string fileName = "")
    {
        return VerifyXmlAsync(objectAssertions: objectAssertions, settings: null, fileName: fileName);
    }
}