using Cashflowpoly.Api.Controllers;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class ApiErrorHelperTests
{
    [Fact]
    public void BuildError_MapsCoreFields_AndTraceId()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-001"
        };
        var detail = new ErrorDetail("username", "wajib diisi");

        var error = ApiErrorHelper.BuildError(context, "VALIDATION_ERROR", "Input tidak valid", detail);

        Assert.Equal("VALIDATION_ERROR", error.ErrorCode);
        Assert.Equal("Input tidak valid", error.Message);
        Assert.Equal("trace-001", error.TraceId);
        Assert.Single(error.Details);
        Assert.Equal("username", error.Details[0].Field);
        Assert.Equal("wajib diisi", error.Details[0].Issue);
    }

    [Fact]
    public void BuildError_ReturnsEmptyDetails_WhenNoDetailProvided()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-002"
        };

        var error = ApiErrorHelper.BuildError(context, "NOT_FOUND", "Data tidak ditemukan");

        Assert.Equal("NOT_FOUND", error.ErrorCode);
        Assert.Equal("trace-002", error.TraceId);
        Assert.Empty(error.Details);
    }
}
