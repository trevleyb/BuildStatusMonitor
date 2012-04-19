using BuildStatusMonitor.Configuration;

namespace BuildStatusMonitor.Components
{
    public interface IComponent {
        void Initialise(string name, Settings settings);
        void Finalise();
        string Name { get; }
    }
}
