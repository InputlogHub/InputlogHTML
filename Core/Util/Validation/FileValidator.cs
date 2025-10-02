using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Util.Progress;
using System.Collections.Concurrent;
using System.Threading;

namespace InputLog.Core.Util.Validation
{
	/// <summary>
	/// Validates files using a certain validator class. This class keeps track of which files
	/// have been validated, and which have not.
	/// </summary>
	/// <typeparam name="TV">Type of the validator to be used.</typeparam>
	public class FileValidator<TV>: ProcessTask
		where TV: IValidator<string>
	{
		#region protected_fields
		/// <summary>
		/// The validator used to validate the files.
		/// </summary>
		private readonly TV Validator;

		/// <summary>
		/// Multipe-value dictionary of files and issues associated with 
		/// those files.
		/// </summary>
		private readonly Dictionary<string, List<FileIssue>> Issues;
		#endregion 

		#region public_fields
		/// <summary>
		/// Dictionary that keeps track of which files have already been validated and
		/// which have been found to be valid or invalid. <br />
		/// True means the file is valid. <br />
		/// False means the file is invalid. <br />
		/// Null means the file hasn't been checked yet. <br />
		/// </summary>
		private ConcurrentDictionary<string, bool?> Validated { get; set; }
		//public Dictionary
		#endregion

		//
		// File Issue class
		//
		#region Internal_class
		/// <summary>
		/// Issues with any of the files validated
		/// </summary>
		public class FileIssue
		{
			public string File { get; private set; }
			public string Description { get; private set; }
			public Exception Exc { get; private set; }

			public FileIssue(string file, string description)
			{
				File = file;
				Description = description;
			}

			public FileIssue(string file, string description, Exception e)
			{
				File = file;
				Description = description;
				Exc = e;
			}
		}
		#endregion

		/// <summary>
		/// Construct a new File validator.
		/// </summary>
		/// <param name="validator">The validator to be used to validate the files.</param>
		public FileValidator(TV validator)
		{
			if (validator == null)
			{
				throw new ArgumentNullException("validator", "Validator parameter can not be null!");
			}
			Validator = validator;
			Validated = new ConcurrentDictionary<string, bool?>();
			Issues = new Dictionary<string, List<FileIssue>>();
		}

		/// <summary>
		/// Add files to be validated, to the fileValidator.
		/// If these files were already in the file validator they will be 
		/// ignored.
		/// </summary>
		/// <param name="files">An enumeration of files to validate</param>
		public void AddFiles(IEnumerable<string> files)
		{
			lock (Validated)
			{
				foreach (string file in files)
				{
					if (!Validated.ContainsKey(file))
					{
						Validated.TryAdd(file, null);
					}
				}
			}
		}

		/// <summary>
		/// Removes the enumeration of files from the file validator.
		/// They will no longer be validated.
		/// </summary>
		/// <param name="files">Enumeration of files to be removed from the file validators' validation list.</param>
		public void RemoveFiles(IEnumerable<string> files)
		{
			foreach (string file in files)
			{
				bool? output;
				Validated.TryRemove(file, out output);
			}
		}

		/// <summary>
		/// Run the validation for the different files.
		/// </summary>
		public override void Run()
		{
			try
			{
				ReportProgress(this, new ProgressEventArgs("Started validation of files.", ProgressEventArgs.ProgressCode.STARTED));
				lock (Issues)
				{
				    Issues.Clear();
				}
				NumberOfSteps = 0;
				while (Validated.Values.Contains(null))
				{
					var unvalidated = Validated.Keys.Where(key => Validated[key] == null);
				    var enumerable = unvalidated as string[] ?? unvalidated.ToArray();
				    NumberOfSteps += enumerable.Length;
					foreach (string file in enumerable)
					{
						bool valid = false;
						Exception e = null;
						try
						{
							valid = Validator.Validate(file);
						}
						catch (Exception exc)
						{
							e = exc;
						}
						finally
						{
							ReportProgress(this, new ProgressEventArgs("File \"" + file + "\" completed.",
								ProgressEventArgs.ProgressCode.STEP_COMPLETED));
						}
						bool? currentValue;
						Validated.TryGetValue(file, out currentValue);
						if (!Validated.TryUpdate(file, valid, currentValue))
						{
							continue;
						}

					    if (valid) continue;
					    AddIssue(file, e != null
					            ? new FileIssue(file, "Could not validate file: \"" + Validator.Remark() + "\"", e)
					            : new FileIssue(file, "Invalid file: \"" + Validator.Remark() + "\""));
					}
				}
				ReportProgress(this, new ProgressEventArgs("Validation completed.", ProgressEventArgs.ProgressCode.DONE));
			}
			catch (ThreadAbortException)
			{
				ReportProgress(this, new ProgressEventArgs("File validation thread aborted", ProgressEventArgs.ProgressCode.FAILED));
			}
			catch (Exception e)
			{
				ReportProgress(this, new ProgressEventArgs("Exception caught - Processing aborted", ProgressEventArgs.ProgressCode.FAILED));
				MessageLogger.CatchException(this, e, Severity.ERROR);
			}
		}

		/// <summary>
		/// Get all the issues that have been found.
		/// </summary>
		/// <returns>Returns a dictionary with all the issues that have been found for a certain file.</returns>
		public Dictionary<string, List<FileIssue>> GetIssues()
		{
			return Issues;
		}

		/// <summary>
		/// Add an issue to a file. If the file already has an issue on its name, it is overwritten.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="issue"></param>
		private void AddIssue(string file, FileIssue issue)
		{
			lock (Issues)
			{
				List<FileIssue> issueList;
				Issues.TryGetValue(file, out issueList);
				if (issueList == null)
				{
					issueList = new List<FileIssue>();
					Issues.Add(file, issueList);
				}
				issueList.Add(issue);
			}
		}
	}
}
