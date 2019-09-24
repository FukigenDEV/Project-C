using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using Configurator;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Logging {
	public class Logger {
		public LogLevel Level;

		private TextWriter stdout;
		private TextWriter stderr;
		private readonly bool WriteToConsole;

		private DateTime LastMsg = DateTime.Now;

		public static void Init() {
			Config.AddConfig(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Logging.DefaultConfig.json")));
		}

		/// <summary>
		/// Creates a new logger object.
		/// </summary>
		/// <param name="Level">The log level. Only messages of the same level or above will be sent to an output stream.</param>
		/// <param name="Filename"></param>
		/// <param name="WriteToConsole"></param>
		public Logger(LogLevel Level = LogLevel.Info, string Filename = "latest", bool WriteToConsole = true) {
			this.Level = Level;
			this.WriteToConsole = WriteToConsole;

			if(File.Exists("Logs\\latest.log") || File.Exists("Logs\\"+Filename + "_err.log")) {
				AdvanceFile();
			} else {
				if (!Directory.Exists("Logs")) {
					Directory.CreateDirectory("Logs");
				}
				stdout = File.CreateText("Logs\\latest.log");
				stderr = File.CreateText("Logs\\latest_err.log");
			}
		}

		/// <summary>
		/// Log a debug message. This should be used for development purposes.
		/// </summary>
		/// <param name="Message"></param>
		public void Debug(string Message) => Log(LogLevel.Debug, Message);
		/// <summary>
		/// Log an informative message.
		/// </summary>
		/// <param name="Message"></param>
		public void Info(string Message) => Log(LogLevel.Info, Message);
		/// <summary>
		/// Log a warning. Warnings indicate that something can or has gone wrong.
		/// </summary>
		/// <param name="Message"></param>
		public void Warning(string Message) => Log(LogLevel.Warning, Message);
		/// <summary>
		/// Log an error message. These indicate that a bug prevented the program from completing a task.
		/// If the program can't safely continue, use a Fatal message instead.
		/// </summary>
		/// <see cref="Fatal(string)"/>
		/// <param name="Message"></param>
		public void Error(string Message) => Log(LogLevel.Error, Message);
		/// <summary>
		/// Logs a fatal error message. These indicate that the program has stopped working due to a bug or other issue.
		/// </summary>
		/// <param name="Message"></param>
		public void Fatal(string Message) => Log(LogLevel.Error, Message);

		/// <summary>
		/// Write a message to an output stream.
		/// If the last message was written yesterday, advance the log files first.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Message"></param>
		private void Log(LogLevel Level, string Message) {
			if (this.Level <= Level) {
				if (LastMsg.Day < DateTime.Now.Day || LastMsg.Month < DateTime.Now.Month) {
					AdvanceFile();
				}
				LastMsg = DateTime.Now;
				string Msg = LastMsg.ToShortTimeString() +" ["+Level.ToString().ToUpper()+"]: "+Message;

				//Write to streams
				stdout.WriteLine(Msg);
				stdout.Flush();

				if (WriteToConsole) {
					Console.WriteLine(Msg);
				}

				if(Level >= LogLevel.Warning) {
					stderr.WriteLine(Msg);
					stderr.Flush();
				}
			}
		}

		/// <summary>
		/// Advances the logs to a new file, compressing the old files into an archive.
		/// The archive is named "Log_yyyy-MM-dd_HH-mm_#.zip"
		/// </summary>
		private void AdvanceFile() {
			DateTime CreationDate = File.GetCreationTime("Logs\\latest.log");

			//Close stdout and stderr streams if they exist.
			stderr?.Close();
			stdout?.Close();

			//Create a temporary directory, then move the log files there.
			Directory.CreateDirectory("Logs\\temp");
			File.Move("Logs\\latest.log", "Logs\\temp\\latest.log");
			File.Move("Logs\\latest_err.log", "Logs\\temp\\latest_err.log");

			//Add a number to the filename to prevent duplicate archives
			string Timestamp = CreationDate.ToString("yyyy-MM-dd_HH-mm");
			int FileCount = Directory.GetFiles("Logs", "Log_"+Timestamp+"_*.zip").Length;

			//Compress the files
			ZipFile.CreateFromDirectory("Logs\\temp", "Logs\\Log_"+Timestamp + "_"+FileCount+".zip");

			//Delete temp folder
			Directory.Delete("Logs\\temp", true);

			//Create new log files
			stdout = File.CreateText("Logs\\latest.log");
			stderr = File.CreateText("Logs\\latest_err.log");
		}
 
	}

	/// <summary>
	/// Log level enum. Loggers will ignore all logs with a severity above what they've got configured.
	/// </summary>
	public enum LogLevel {
		Debug,
		Info,
		Warning,
		Error,
		Critical
	}
}
