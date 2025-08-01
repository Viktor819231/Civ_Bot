#NoTrayIcon
#SingleInstance Force

; Suppress error dialogs
OnError(ErrorHandler)

ErrorHandler(exception, mode) {
    ; Log error silently instead of showing dialog
    FileAppend("Error in HitBackspace: " . exception.message . "`n", "errors.log")
    return true  ; Continue execution
}

; Main functionality
try {
    Send("{Backspace}")
} catch Error as e {
    ; Silent error handling - don't show popup
}

ExitApp