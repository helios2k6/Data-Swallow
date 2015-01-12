namespace DataSwallow.Utilities
{
    /// <summary>
    /// Represents an object that can determine whether or not an input
    /// passes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICriterion<in T>
    {
        bool ApplyCriterion(T t);
    }
}
