// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventValidationDetailsSerializer.
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventValidationDetailsSerializer
{
    string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error);
}
