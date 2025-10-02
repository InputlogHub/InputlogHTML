namespace InputLog.Core.Util.Validation
{
	/// <summary>
	/// Interface for a validator. A validator takes a 
	/// certain type as Input and validates the input.
	/// </summary>
	/// <typeparam name="T">Type of input to validate</typeparam>
	public interface IValidator<in T>
	{
		/// <summary>
		/// Validate the input parameter.
		/// </summary>
		/// <param name="input">Input to be validated.</param>
		/// <returns>True if the input is valid, false if not.</returns>
		bool Validate(T input);

		/// <summary>
		/// Gets the remark for the last validated file. Validating a new file resets the remark of 
		/// the previous file.
		/// </summary>
		/// <returns>A string remark for the file that has been last validated.</returns>
		string Remark();
	}
}
