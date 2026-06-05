using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventValidationDetailsSerializer
{
    string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error);
}
