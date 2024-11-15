namespace SatelliteService.EventProcessing.Abstract;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}