using MementoContainer.Factories;

namespace MementoContainer
{
    /// <summary>
    /// Represents a Memento container with custom memento factory, which saves the state of objects' properties
    /// so that they can be later restored to their initial state.
    /// </summary>
    public class Memento<TFactory> : Memento where TFactory : IMementoFactory, new()
    {
        /// <summary>
        /// Creates memento factory via generic type TFactory
        /// </summary>
        protected override void CreateMementoFactory()
        {
            this.Factory = New<TFactory>.Instance();
        }
    }
}
