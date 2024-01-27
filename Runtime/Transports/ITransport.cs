namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Interface used to define the generic transport methods.
    /// Generally only peers should communicate with these classes.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Runs the update loop of the transport.
        /// Use this method to handle all things that should happen on your transport on a regular interval.
        /// </summary>
        void Tick();
    }
}