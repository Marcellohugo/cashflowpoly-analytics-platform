using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal interface IEventValidationDetailsSerializer
{
    string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error);
}
