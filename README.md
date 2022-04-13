# OneSchedule

OneNote-based task scheduler.

## What is this?

This project provides a way to trigger actions at pre-determined times from OneNote.
It's like [cron][1], but for note-taking. You can use this to fire
[Telegram notifications](#OneTelegram), [launch processes](#OneExec)
(though you probably shouldn't), or anything else you want!

## Usage

First, launch the OneSchedule application and give it the command line
to launch for each event:

```text
OneSchedule [OPTIONS]+ program [ARGS]+

  -s, --silent               do not display a console window
  -f, --full-scan=VALUE      interval for full scans, in minutes (default: 30)
  -h, --help                 this cruft
```

In OneNote, you create a trigger by typing a timestamp of the form
`//2021-05-01T12:00+03:00//`. Then, when the specified time arrives (and OneSchedule
is running), the specified process will be launched and passed the event information
over `stdin`. The information is passed as a UTF-8-encoded JSON of the form:

```json
{
    "date": "2021-05-01T12:00:00+03:00",
    "comment": "Some comment"
}
```

Where the `comment` field contains the text on the same line in OneNote, except
the timestamp itself. For instance, if you have a line in OneNote that says:

```text
Shave the yak //2021-05-01T12:00+03:00//
```

Then the JSON will be:

```json
{
    "date": "2021-05-01T12:00:00+03:00",
    "comment": "Shave the yak"
}
```

For more concrete examples, [see](#OneTelegram) [below](#OneExec).

**Note**: Currently OneSchedule scans *all* open notebooks looking for timestamps
when it launches. This may lead to slow response times in OneNote while the scan
is in progress. A full scan is also performed according to the interval specified
using the `-f` option.

**Note 2**: OneNote need *not* be open for OneSchedule to work. In this case,
OneSchedule will use the notebooks that were last open.

### OneTelegram

This application sends a message to a Telegram chat of your choice when triggered by
OneSchedule. The message contains the timestamp and the comment.

```text
OneTelegram [OPTIONS]+
The TELEGRAM_TOKEN environment variable must be set to the bot's token.
The TELEGRAM_CHAT_ID must be set for sending notifications.

  -d, --display-chat-ids     only display chat IDs of received messages
  -h, --help                 this cruft
```

To use, set the aforementioned environment variables and launch OneSchedule with:

```text
OneSchedule.exe OneTelegram.exe
```

To simplify matters, you can use the attached
[`OneTelegram.bat`](Scripts/OneTelegram.bat) script. The chat ID can be determined
by setting the `TELEGRAM_TOKEN` environment variable and launching OneTelegram
with the `-d` flag.

### OneExec

**Note**: This is a really terrible idea.

This application launches a process of your choice when triggered by OneSchedule.
For instance, writing:

```text
//2021-05-01T12:00+03:00// calc.exe
```

Will pop a calc at the specified time.

To use, launch OneSchedule with:

```text
OneSchedule.exe OneExec.exe
```

[1]: https://en.wikipedia.org/wiki/Cron
     "cron - Wikipedia"
