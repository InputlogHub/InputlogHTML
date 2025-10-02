using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InputLog.Core.Util.Validation;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
	/// <summary>
	/// Class that knows how to validate idfx and .tsv files for purposes
	/// of merging the Tobii generated file with idfx files.
	/// </summary>
	public class DragonMergeFileValidator: IValidator<string>
	{
		#region private_fields
		private const string T_EXT = ".dat";
		private const string IL_EXT = ".idfx";

		private const char T_SEPARATOR = '\t';

		private string CurrentRemark = "";

		/// <summary>
		/// The set of required information to be present in a tobii file in order 
		/// to successfully merge a tobii file with an idfx file.
		/// </summary>
		private HashSet<string> RequiredInformation;
		#endregion

        public DragonMergeFileValidator()
		{
			RequiredInformation = new HashSet<string>();
			// Add all required tokens to the set of required tokens.
			/*TAGS.General.Keys.Where(token => TAGS.General[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.EyeTracking.Keys.Where(token => TAGS.EyeTracking[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.GazeEvent.Keys.Where(token => TAGS.GazeEvent[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.GeneralData.Keys.Where(token => TAGS.GeneralData[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.Media.Keys.Where(token => TAGS.Media[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.RecordingEvent.Keys.Where(token => TAGS.RecordingEvent[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.SegmentScene.Keys.Where(token => TAGS.SegmentScene[token]).ToList().ForEach(item => RequiredInformation.Add(item));
			TAGS.Timestamp.Keys.Where(token => TAGS.Timestamp[token]).ToList().ForEach(item => RequiredInformation.Add(item));*/
		}

		/// <summary>
		/// Validates the idfx and tobii files to see whether they are valid for merging.
		/// This is just a high level check which mostly checks to see if the tobii files have
		/// the correct information in them.
		/// </summary>
		/// <param name="input">File to be validated</param>
		/// <returns>True if the file is valid, false if it is not.</returns>
		public bool Validate(string input)
		{
			string extension = Path.GetExtension(input);
			CurrentRemark = "";
			if(extension == IL_EXT)
			{
				CurrentRemark = "Inputlog files are not explicitely checked for validity beforehand.";
				return true;
			}
			else if (extension == T_EXT)
			{
                return true;
				/*try
				{
					using (StreamReader reader = new StreamReader(File.OpenRead(input)))
					{
						string line = reader.ReadLine();
						HashSet<string> tokens = new HashSet<string>(line.Split(T_SEPARATOR));

						bool valid = true;
						foreach (string requiredToken in RequiredInformation)
						{
							if (!tokens.Contains(requiredToken))
							{
								if (!requiredToken.Contains("[*]"))
								{
									valid = false;
									CurrentRemark += requiredToken + "; ";
								}
								else
								{
									string[] tokenParts = requiredToken.Split(new string[] { "[*]" }, StringSplitOptions.None);
									if (!tokens.Any(token => token.StartsWith(tokenParts[0]) && token.EndsWith(tokenParts[tokenParts.Length - 1])))
									{
										valid = false;
										CurrentRemark += requiredToken + "; ";
									}
								}
							}
						}
						if (!valid)
						{
							CurrentRemark = "Following required tokens are missing: " + CurrentRemark;
						}
						return valid;
					}
				}
				catch (IOException ioexc)
				{
					CurrentRemark = "Error reading file";
					throw ioexc;
				}
				catch (Exception exc)
				{
					CurrentRemark = "Unknown Error Occurred";
					throw exc;
				}*/
			}
			else
			{
				CurrentRemark = "Unknown File Extension";
				return false;
			} 
		}

		/// <summary>
		/// Gets the remark for the last validated file. Validating a new file resets the remark of 
		/// the previous file.
		/// </summary>
		/// <returns>A string remark for the file that has been last validated.</returns>
		public string Remark()
		{
			return CurrentRemark;
		}
	}
}
