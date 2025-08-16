#NoTrayIcon
#ErrorStdOut
DetectHiddenWindows True
#SingleInstance Ignore

hwnd := A_Args[1]
x := A_Args[2]
y := A_Args[3]

if !hwnd || hwnd = "0"
    ExitApp

ControlClick("x" x " y" y, "ahk_id " hwnd, , , , "NA")