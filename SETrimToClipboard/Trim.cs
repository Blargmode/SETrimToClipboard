using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SETrimToClipboard
{
	internal class Trim
	{
		//Spaces adjacent to special characters can be removed.
		private char[] specials = { '<', '>', '(', ')', '{', '}', '+', '-', '*', '/', '%', ':', ';', '=' };

		public Trim(string args)
		{
			string combinedFiles = "";
			string[] files;
			string path = args;

			files = Directory.GetFiles(path, "*.cs");

			int count = 0;
			int nrOfFiles = files.Length;
			int digits = (int)Math.Floor(Math.Log10(nrOfFiles) + 1);

			foreach (string file in files)
			{
				count++;
				Console.WriteLine(count.ToString().PadLeft(digits, ' ') + "/" + nrOfFiles + ": " + Path.GetFileName(file));
				combinedFiles += ReadFile(Path.Combine(path, file)) + '\n';
			}

			Clipboard.SetText(combinedFiles);
		}

		private string ReadFile(string path)
		{
			string line;
			string trimmed = "";
			bool read = false;
			bool read2 = false;
			int depth = 0;
			bool inQuotes = false;

			//Line length limiter
			int lastLength = 0;
			int limit = 200;

			StreamReader file = new StreamReader(path);
			while ((line = file.ReadLine()) != null)
			{
				if (!read && !read2 && line.Contains("#region in-game")) read = true;
				else if (!read && !read2 && line.Contains("#region untouched")) read2 = true;
				else if ((read || read2) && line.Contains("#region")) depth++;
				else if ((read || read2) && line.Contains("#endregion"))
				{
					if (depth <= 0)
					{
						break;
					}
					else depth--;
				}
				else if (read)
				{
					line = RemoveComments(line);
					line = line.Trim(); //+ "\n"
					for (int i = 0; i < line.Length; i++)
					{
						if (line[i] == '"')
						{
							inQuotes = !inQuotes;
						}
						else if (line[i] == ' ' && !inQuotes)
						{
							if (i > 0 && IsSpecialCharacter(line[i - 1]))
							{
								//Prev is special, remove space
								continue;
							}
							else if (i < line.Length - 1 && IsSpecialCharacter(line[i + 1]))
							{
								//Next is special, remove space
								continue;
							}
						}
						trimmed += line[i];
					}
				}
				else if (read2)
				{
					trimmed += line + "\n";
				}

				//Split into several lines. Some computers can't handle rendering very long
				//lines. This rough line length limiter prevents that.
				if (trimmed.Length > lastLength + limit)
				{
					trimmed += "\n";
					lastLength = trimmed.Length;
				}
			}
			file.Close();
			file.Dispose();
			if (!read2) return RemoveBlockComments(trimmed);
			return trimmed;
		}

		private bool IsSpecialCharacter(char c)
		{
			foreach (char s in specials)
			{
				if (c == s) return true;
			}
			return false;
		}

		static string StripComments(string code)
		{
			var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
			return Regex.Replace(code, re, "$1");
		}

		private string RemoveComments(string line)
		{
			bool inQuotes = false;
			bool slash = false;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '"') inQuotes = !inQuotes;
				else if (!inQuotes && !slash && line[i] == '/') slash = true;
				else if (!inQuotes && slash && line[i] == '/')
				{
					return line.Substring(0, i - 1);
				}
			}
			return line;
		}

		private string RemoveBlockComments(string file)
		{
			int start;
			int end;
			while (file.Contains("/*"))
			{
				start = file.IndexOf("/*");
				end = file.IndexOf("*/") + 2;
				file = file.Remove(start, end - start);
			}
			return file;
		}
	}
}