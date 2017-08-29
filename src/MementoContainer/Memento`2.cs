using MementoContainer.Analysis;
using MementoContainer.Factories;

namespace MementoContainer
{
    /// <summary>
    /// Represents a Memento container with custom property analyzer and collection analyzer, which saves the state of objects' properties
    /// so that they can be later restored to their initial state.
    /// </summary>
    public class Memento<TPropAnalyzer, TColAnalyzer> : Memento
        where TPropAnalyzer : IPropertyAnalyzer, new()
        where TColAnalyzer : ICollectionAnalyzer, new()
    {
        /// <summary>
        /// Creates memento factory via generic types TPropAnalyzer and TColAnalyzer using default MementoFactory implementation
        /// </summary>
        protected override void CreateMementoFactory()
        {
            this.Factory = new MementoFactory(New<TPropAnalyzer>.Instance(), New<TColAnalyzer>.Instance());
        }
    }
}
