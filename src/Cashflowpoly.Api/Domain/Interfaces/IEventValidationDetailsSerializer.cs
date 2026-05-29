using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventValidationDetailsSerializer
{
    string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error);
}
