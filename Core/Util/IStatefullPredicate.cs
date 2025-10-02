
namespace InputLog.Core.Util
{
    /// <summary>
    /// Interface representing a predicate that can maintain state between different calls to the Check() method.
    /// A predicate is a function that asserts a certain condition on an object and returns true when this condition is 
    /// met and false otherwise.
    /// 
    /// This interface basically represents such a function by providing a functor with a single method that returns true. 
    /// The reason we need this is because the Predicate T class provided by C# is a delegate which cannot 
    /// maintain state between different invocations.
    /// </summary>
    /// <typeparam name="T">Type of the element that can be verified by this predicate.</typeparam>
    public interface IStatefullPredicate<in T>
    {

        /// <summary>
        /// Method that actually verifies whether a given object fulfills a certain condition (condition itself is 
        /// obviously implementation specific). 
        /// 
        /// The outcome of this method can be dependent on previous invocations as this class 
        /// can maintain state between calls to Check().
        /// </summary>
        /// <param name="obj">Object on which to verify a condition.</param>
        /// <returns>true if the condition is met, false otherwise</returns>
        bool Check(T obj);
    }
}
