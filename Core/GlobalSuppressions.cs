// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, LinearAnalysisType, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

// Suppressed because we want to have this fields public as they are part of the information we want to pass on
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", Scope = "member", Target = "InputLog.Core.Hooks.Keyboard.KBDLLHOOKSTRUCT.#dwExtraInfo")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", Scope = "member", Target = "InputLog.Core.Hooks.Mouse.MSLLHOOKSTRUCT.#dwExtraInfo")]

// Suppressed because we do not want to instantiate a new object derived from EventArgs as this would be less efficient
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope = "member", Target = "InputLog.Core.SystemLogger.#FocusEvent")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope = "member", Target = "InputLog.Core.SystemLogger.#KeyboardEvent")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope = "member", Target = "InputLog.Core.SystemLogger.#MouseEvent")]

// Suppressed because opened streams are owned and closed by the EventLogWriter/EventLogReader
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.IO.Basic.EventLogFactory.#CreateFileEventLogReader(System.String,InputLog.Core.IO.Basic.LogFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.IO.Basic.EventLogFactory.#CreateFileEventLogWriter(System.String,InputLog.Core.IO.Basic.LogFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Scope = "member", Target = "InputLog.Core.Hooks.SystemMonitor.#MouseEvent")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Scope = "member", Target = "InputLog.Core.Hooks.SystemMonitor.#KeyboardEvent")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type", Target = "InputLog.Core.Analyses.PAUSELOCATION")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type", Target = "InputLog.Core.Hooks.Mouse.MouseMessages")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type", Target = "InputLog.Core.Hooks.Keyboard.KeyboardMessages")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.Analyses.Focus.SourceAnalysis.#DoAnalysis()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.Analyses.General.GeneralAnalysis.#AnalyzeEvents(System.Collections.Generic.IEnumerable`1<InputLog.Core.Util.Pair`2<InputLog.Core.Events.Event,InputLog.Core.Analyses.PauseLocation>>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.Analyses.Linear.LinearAnalysis.#DoAnalysis()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.Analyses.Pause.PauseAnalysis.#CreateSummary()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "InputLog.Core.Analyses.Summary.SummaryAnalysis.#CreateSummary()")]
