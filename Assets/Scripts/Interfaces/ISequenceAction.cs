using Data;
namespace Interfaces
{
    public interface ISequenceAction
    {
        public void Invoke();
        public void SetEventsData(EventsData data);
    }
}