using System;
using System.IO;
using System.Text;
using Siren.Infrastructure.Mermaid;

namespace Siren.Infrastructure.Io
{
	public static class FileWriter
	{
		public static void Perform(
			string filePath,
			StringBuilder result,
			string markdownAnchor = null
		)
		{
			var exists =
				File
					.Exists(filePath);

			if (!exists)
			{
				Console
					.WriteLine($"Existing file \"{filePath}\" not found, creating...");
				
				OverwriteFileContents(
					filePath,
					result.ToString()
				);
				return;
			}
			
			if (string.IsNullOrEmpty(markdownAnchor))
			{
				Console
					.WriteLine($"Markdown anchor \"{markdownAnchor}\"not specified, writing directly...");
				
				OverwriteFileContents(
					filePath,
					result.ToString()
				);
				return;
			}

			var fileString =
				File
					.ReadAllText(filePath);

			// First occurrence of markdown anchor
			var markdownPosition =
				fileString
					.IndexOf(
						markdownAnchor,
						StringComparison.InvariantCultureIgnoreCase
					) + markdownAnchor.Length;

			if (markdownPosition < 0)
			{
				Console
					.WriteLine($"Markdown anchor \"{markdownAnchor}\"not found in file; overwriting...");
				
				OverwriteFileContents(
					filePath,
					result.ToString()
				);
				return;
			}

			// First occurrence of siren start header, otherwise straight after anchor
			var sirenAnchorStartPosition =
				fileString
					.IndexOf(
						MermaidConstants.SirenAnchorStart,
						markdownPosition,
						StringComparison.InvariantCultureIgnoreCase
					);

			if (sirenAnchorStartPosition < 0)
				sirenAnchorStartPosition = markdownPosition;
			
			// First occurrence of siren end header, otherwise straight after anchor
			var sirenAnchorEndPosition =
				fileString
					.IndexOf(
						MermaidConstants.SirenAnchorEnd,
						sirenAnchorStartPosition,
						StringComparison.InvariantCultureIgnoreCase
					);

			// +2 for length of carriage return (otherwise appends line feed each re-write)
			var lengthToTrim = MermaidConstants.SirenAnchorEnd.Length + 2;
			
			if (sirenAnchorEndPosition < 0)
			{
				sirenAnchorEndPosition = sirenAnchorStartPosition;
				lengthToTrim = 0;
			}

			fileString =
				fileString
					.Remove(
						sirenAnchorStartPosition,
						sirenAnchorEndPosition - sirenAnchorStartPosition + lengthToTrim 
					);

			fileString =
				fileString
					.Insert(
						sirenAnchorStartPosition,
						result.ToString()
					);
			
			OverwriteFileContents(
				filePath,
				fileString
			);
		}

		private static void OverwriteFileContents(string filePath, string result) =>
			File
				.WriteAllText(
					filePath,
					result
				);
	}
}