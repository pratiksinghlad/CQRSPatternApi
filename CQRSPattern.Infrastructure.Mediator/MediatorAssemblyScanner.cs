using System.Collections;
using System.Reflection;
using FluentValidation;
using MediatR.Pipeline;

namespace CQRSPattern.Infrastructure.Mediator
{
    /// <summary>
    /// Assembly scanner for mediator types.
    /// </summary>
    public class MediatorAssemblyScanner : IEnumerable<MediatorAssemblyScannerResult>
    {
        readonly IEnumerable<Type> _types;
        readonly Type _typeToFind;

        /// <summary>
        /// Creates a scanner that works on a sequence of types.
        /// </summary>
        public MediatorAssemblyScanner(IEnumerable<Type> types, Type typeToFind)
        {
            _types = types;
            _typeToFind = typeToFind;
        }

        /// <summary>
        /// Method used to automatically register preprocessors.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static MediatorAssemblyScanner FindPreProcessorsInAssembly(Assembly assembly)
        {
            return new MediatorAssemblyScanner(assembly.GetTypes(), typeof(IRequestPreProcessor<>));
        }

        /// <summary>
        /// Performs the specified action to all of the assembly scan results.
        /// </summary>
        public void ForEach(Action<MediatorAssemblyScannerResult> action)
        {
            foreach (var result in this)
            {
                action(result);
            }
        }

        private IEnumerable<MediatorAssemblyScannerResult> Execute()
        {
            var query =
                from type in _types
                where !type.IsAbstract && !type.IsGenericTypeDefinition
                let interfaces = type.GetInterfaces()
                let genericInterfaces = interfaces.Where(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == _typeToFind
                )
                let matchingInterface = genericInterfaces.FirstOrDefault()
                where matchingInterface != null
                select new MediatorAssemblyScannerResult(matchingInterface, type);

            return query;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<MediatorAssemblyScannerResult> GetEnumerator()
        {
            return Execute().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
