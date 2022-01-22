
namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// This class has been added to make up for the missing nullability in this .NET 4.7.2.
	/// Some classes such as the KnowledgeBase are implementing an interface that is tagged by nullability attributes
	/// so we need these attributes to exist to be able to write the signature of implementations.
	/// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }
    }
}
