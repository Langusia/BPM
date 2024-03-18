using Marten.Events;

namespace Core.BPM.MediatR.Mediator;

public class CountableAggregate : Aggregate
{
    public Dictionary<string, int> EventCounters { get; set; } = new();

    public void Apply(IEvent<ICountableEvent> @event)
    {
        if (EventCounters.ContainsKey(@event.EventTypeName))
            EventCounters.Add(@event.EventTypeName, 0);
        else
            EventCounters[@event.EventTypeName] += 1;
    }
}