using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventRecordMapper
{
    EventRequest ToEventRequest(EventDb record);
}
