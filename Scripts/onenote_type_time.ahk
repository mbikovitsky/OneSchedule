#IfWinActive ahk_exe onenote.exe
^!+t::SendTimestamp()


SendTimestamp()
{
    VarSetCapacity(TimeZoneInfo, 172, 0)
    Retval := DllCall("kernel32\GetTimeZoneInformation", "Ptr", &TimeZoneInfo)
    Bias := NumGet(TimeZoneInfo, Offset := 0, Type := "Int")
    Switch Retval
    {
    Case -1: ; TIME_ZONE_ID_INVALID
        MsgBox, 0x30,, % "Error retrieving timezone info"
        Return

    Case 0: ; TIME_ZONE_ID_UNKNOWN

    Case 1: ; TIME_ZONE_ID_STANDARD
        Bias += NumGet(TimeZoneInfo, Offset := 84, Type := "Int")

    case 2: ; TIME_ZONE_ID_DAYLIGHT
        Bias += NumGet(TimeZoneInfo, Offset := 168, Type := "Int")
    }

    HoursDiff := -Bias // 60
    MinutesDiff := Mod(Abs(Bias), 60)

    FormatTime, TimeString,, yyyy-MM-ddTHH:mm

    TimestampString := Format("//{:s}{:+03d}:{:02u}//", TimeString, HoursDiff, MinutesDiff)

    Send {Text}%TimestampString%
}
