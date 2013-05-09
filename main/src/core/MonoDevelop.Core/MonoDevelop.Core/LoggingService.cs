// 
// LoggingService.cs
// 
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MonoDevelop.Core.Logging;

namespace MonoDevelop.Core
{
	
	public static class LoggingService
	{
		static List<ILogger> loggers = new List<ILogger> ();
		static RemoteLogger remoteLogger;
		static DateTime timestamp;
		static TextWriter defaultError;
		static TextWriter defaultOut;

		static LoggingService ()
		{
			ConsoleLogger consoleLogger = new ConsoleLogger ();
			loggers.Add (consoleLogger);
			loggers.Add (new InstrumentationLogger ());
			
			string consoleLogLevelEnv = Environment.GetEnvironmentVariable ("MONODEVELOP_CONSOLE_LOG_LEVEL");
			if (!string.IsNullOrEmpty (consoleLogLevelEnv)) {
				try {
					consoleLogger.EnabledLevel = (EnabledLoggingLevel) Enum.Parse (typeof (EnabledLoggingLevel), consoleLogLevelEnv, true);
				} catch {}
			}
			
			string consoleLogUseColourEnv = Environment.GetEnvironmentVariable ("MONODEVELOP_CONSOLE_LOG_USE_COLOUR");
			if (!string.IsNullOrEmpty (consoleLogUseColourEnv) && consoleLogUseColourEnv.ToLower () == "false") {
				consoleLogger.UseColour = false;
			} else {
				consoleLogger.UseColour = true;
			}
			
			string logFileEnv = Environment.GetEnvironmentVariable ("MONODEVELOP_LOG_FILE");
			if (!string.IsNullOrEmpty (logFileEnv)) {
				try {
					FileLogger fileLogger = new FileLogger (logFileEnv);
					loggers.Add (fileLogger);
					string logFileLevelEnv = Environment.GetEnvironmentVariable ("MONODEVELOP_FILE_LOG_LEVEL");
					fileLogger.EnabledLevel = (EnabledLoggingLevel) Enum.Parse (typeof (EnabledLoggingLevel), logFileLevelEnv, true);
				} catch (Exception e) {
					LogError (e.ToString ());
				}
			}

			timestamp = DateTime.Now;
		}

		static string GenericLogFile {
			get { return "Ide.log"; }
		}
		
		public static DateTime LogTimestamp {
			get { return timestamp; }
		}

		static string UniqueLogFile {
			get {
				return string.Format ("Ide.{0}.log", timestamp.ToString ("yyyy-MM-dd__HH-mm-ss"));
			}
		}
		
		public static void Initialize (bool redirectOutput)
		{
			PurgeOldLogs ();

			// Always redirect on windows otherwise we cannot get output at all
			if (Platform.IsWindows || redirectOutput)
				RedirectOutputToLogFile ();
		}
		
		public static void Shutdown ()
		{
			RestoreOutputRedirection ();
		}

		static void PurgeOldLogs ()
		{
			// Delete all logs older than a week
			if (!Directory.Exists (UserProfile.Current.LogDir))
				return;

			// HACK: we were using EnumerateFiles but it's broken in some Mono releases
			// https://bugzilla.xamarin.com/show_bug.cgi?id=2975
			var files = Directory.GetFiles (UserProfile.Current.LogDir)
				.Select (f => new FileInfo (f))
				.Where (f => f.CreationTimeUtc < DateTime.UtcNow.Subtract (TimeSpan.FromDays (7)));

			foreach (var v in files) {
				try {
					v.Delete ();
				} catch (Exception ex) {
					Console.Error.WriteLine (ex);
				}
			}
		}

		static void RedirectOutputToLogFile ()
		{
			FilePath logDir = UserProfile.Current.LogDir;
			if (!Directory.Exists (logDir))
				Directory.CreateDirectory (logDir);
			
			try {
				if (Platform.IsWindows) {
					//TODO: redirect the file descriptors on Windows, just plugging in a textwriter won't get everything
					RedirectOutputToFileWindows (logDir, UniqueLogFile);
				} else {
					RedirectOutputToFileUnix (logDir, UniqueLogFile);
				}
			} catch (Exception ex) {
				Console.Error.WriteLine (ex);
			}
		}

		static void RedirectOutputToFileWindows (FilePath logDirectory, string logName)
		{
			var stream = File.Open (logDirectory.Combine (logName), FileMode.Create, FileAccess.Write, FileShare.Read);
			var writer = new StreamWriter (stream) { AutoFlush = true };
			
			var stderr = new MonoDevelop.Core.ProgressMonitoring.LogTextWriter ();
			stderr.ChainWriter (Console.Error);
			stderr.ChainWriter (writer);
			defaultError = Console.Error;
			Console.SetError (stderr);

			var stdout = new MonoDevelop.Core.ProgressMonitoring.LogTextWriter ();
			stdout.ChainWriter (Console.Out);
			stdout.ChainWriter (writer);
			defaultOut = Console.Out;
			Console.SetOut (stdout);
		}
		
		static void RedirectOutputToFileUnix (FilePath logDirectory, string logName)
		{
			const int STDOUT_FILENO = 1;
			const int STDERR_FILENO = 2;
			
			Mono.Unix.Native.OpenFlags flags = Mono.Unix.Native.OpenFlags.O_WRONLY
				| Mono.Unix.Native.OpenFlags.O_CREAT | Mono.Unix.Native.OpenFlags.O_TRUNC;
			var mode = Mono.Unix.Native.FilePermissions.S_IFREG
				| Mono.Unix.Native.FilePermissions.S_IRUSR | Mono.Unix.Native.FilePermissions.S_IWUSR
				| Mono.Unix.Native.FilePermissions.S_IRGRP | Mono.Unix.Native.FilePermissions.S_IWGRP;
			
			var file = logDirectory.Combine (logName);
			int fd = Mono.Unix.Native.Syscall.open (file, flags, mode);
			if (fd < 0)
				//error
				return;
			try {
				int res = Mono.Unix.Native.Syscall.dup2 (fd, STDOUT_FILENO);
				if (res < 0)
					//error
					return;
				
				res = Mono.Unix.Native.Syscall.dup2 (fd, STDERR_FILENO);
				if (res < 0)
					//error
					return;

				var genericLog = logDirectory.Combine (GenericLogFile);
				File.Delete (genericLog);
				Mono.Unix.Native.Syscall.symlink (file, genericLog);
			} finally {
				Mono.Unix.Native.Syscall.close (fd);
			}
		}

		static void RestoreOutputRedirection ()
		{
			if (defaultError != null)
				Console.SetError (defaultError);
			if (defaultOut != null)
				Console.SetOut (defaultOut);
		}
		
		internal static RemoteLogger RemoteLogger {
			get {
				if (remoteLogger == null)
					remoteLogger = new RemoteLogger ();
				return remoteLogger;
			}
		}
		
#region the core service
		
		public static bool IsLevelEnabled (LogLevel level)
		{
			EnabledLoggingLevel l = (EnabledLoggingLevel) level;
			foreach (ILogger logger in loggers)
				if ((logger.EnabledLevel & l) == l)
					return true;
			return false;
		}
		
		public static void Log (LogLevel level, string message)
		{
			EnabledLoggingLevel l = (EnabledLoggingLevel) level;
			foreach (ILogger logger in loggers)
				if ((logger.EnabledLevel & l) == l)
					logger.Log (level, message);
		}
		
#endregion
		
#region methods to access/add/remove loggers -- this service is essentially a log message broadcaster
		
		public static ILogger GetLogger (string name)
		{
			foreach (ILogger logger in loggers)
				if (logger.Name == name)
					return logger;
			return null;
		}
		
		public static void AddLogger (ILogger logger)
		{
			if (GetLogger (logger.Name) != null)
				throw new Exception ("There is already a logger with the name '" + logger.Name + "'");
			loggers.Add (logger);
		}
		
		public static void RemoveLogger (string name)
		{
			ILogger logger = GetLogger (name);
			if (logger == null)
				throw new Exception ("There is no logger registered with the name '" + name + "'");
			loggers.Remove (logger);
		}
		
#endregion
		
#region convenience methods (string message)
		
		public static void LogDebug (string message)
		{
			Log (LogLevel.Debug, message);
		}
		
		public static void LogInfo (string message)
		{
			Log (LogLevel.Info, message);
		}
		
		public static void LogWarning (string message)
		{
			Log (LogLevel.Warn, message);
		}
		
		public static void LogError (string message)
		{
			Log (LogLevel.Error, message);
		}
		
		public static void LogFatalError (string message)
		{
			Log (LogLevel.Fatal, message);
		}
		
#endregion
		
#region convenience methods (string messageFormat, params object[] args)
		
		public static void LogDebug (string messageFormat, params object[] args)
		{
			Log (LogLevel.Debug, string.Format (messageFormat, args));
		}
		
		public static void LogInfo (string messageFormat, params object[] args)
		{
			Log (LogLevel.Info, string.Format (messageFormat, args));
		}
		
		public static void LogWarning (string messageFormat, params object[] args)
		{
			Log (LogLevel.Warn, string.Format (messageFormat, args));
		}
		
		public static void LogError (string messageFormat, params object[] args)
		{
			Log (LogLevel.Error, string.Format (messageFormat, args));
		}
		
		public static void LogFatalError (string messageFormat, params object[] args)
		{
			Log (LogLevel.Fatal, string.Format (messageFormat, args));
		}
		
#endregion
		
#region convenience methods (string message, Exception ex)
		
		public static void LogDebug (string message, Exception ex)
		{
			Log (LogLevel.Debug, message + (ex != null? System.Environment.NewLine + ex.ToString () : string.Empty));
		}
		
		public static void LogInfo (string message, Exception ex)
		{
			Log (LogLevel.Info, message + (ex != null? System.Environment.NewLine + ex.ToString () : string.Empty));
		}
		
		public static void LogWarning (string message, Exception ex)
		{
			Log (LogLevel.Warn, message + (ex != null? System.Environment.NewLine + ex.ToString () : string.Empty));
		}
		
		public static void LogError (string message, Exception ex)
		{
			Log (LogLevel.Error, message + (ex != null? System.Environment.NewLine + ex.ToString () : string.Empty));
		}
		
		public static void LogFatalError (string message, Exception ex)
		{
			Log (LogLevel.Fatal, message + (ex != null? System.Environment.NewLine + ex.ToString () : string.Empty));
		}

#endregion
	}
}
