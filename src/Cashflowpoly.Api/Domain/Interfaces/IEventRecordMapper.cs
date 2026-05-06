using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal interface IEventRecordMapper
{
    EventRequest ToEventRequest(EventDb record);
}
