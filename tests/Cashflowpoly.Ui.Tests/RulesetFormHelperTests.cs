// Fungsi file: Menguji helper formulir ruleset agar default config dan JSON formatting tidak tertahan di controller.
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetFormHelperTests
{
    [Fact]
    public void BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig()
    {
        var model = RulesetFormHelper.BuildDefaultCreateViewModel();

        Assert.False(model.IsEditMode);
        using var document = JsonDocument.Parse(model.ConfigJson);
        Assert.Equal("PEMULA", document.RootElement.GetProperty("mode").GetString());
        Assert.Equal(20, document.RootElement.GetProperty("starting_cash").GetInt32());
    }

    [Fact]
    public void TryResolveMode_ReturnsUppercaseModeForValidConfig()
    {
        var config = JsonNode.Parse("""{"mode":"mahir"}""")!.AsObject();

        var ok = RulesetFormHelper.TryResolveMode(config, out var mode);

        Assert.True(ok);
        Assert.Equal("MAHIR", mode);
    }

    [Fact]
    public void SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull()
    {
        using var document = JsonDocument.Parse("""{"mode":"PEMULA","starting_cash":20}""");

        var formatted = RulesetFormHelper.SerializeIndentedJson(document.RootElement);

        Assert.Contains(Environment.NewLine, formatted);
        Assert.Equal("{}", RulesetFormHelper.SerializeIndentedJson(null));
    }

    [Fact]
    public async Task BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent("""{"message":"Config tidak valid"}""", Encoding.UTF8, "application/json")
        };

        var message = await RulesetFormHelper.BuildRulesetApiErrorMessage(response, "Fallback", CancellationToken.None);

        Assert.Equal("Config tidak valid", message);
    }
}
