#NoTrayIcon
#ErrorStdOut
#SingleInstance Ignore

try {
    x := A_Args[1]
    y := A_Args[2]
    if !x || !y
        ExitApp
    MouseMove x, y
} catch {
    ; Silently exit on any error
    ExitApp
}